"""Lightweight ACME HTTP-01 client for auto-provisioning Let's Encrypt certs.

No external dependencies beyond cryptography (already required for TSGU CBT).
DNS must point at this box and port 80 must be reachable.
"""
import base64
import datetime
import hashlib
import json
import os
import threading
import time
import urllib.error
import urllib.request
from http.server import BaseHTTPRequestHandler, HTTPServer

from cryptography import x509
from cryptography.hazmat.primitives import hashes, serialization
from cryptography.hazmat.primitives.asymmetric import padding, rsa
from cryptography.x509.oid import NameOID

LETSENCRYPT_PROD = "https://acme-v02.api.letsencrypt.org/directory"
LETSENCRYPT_STAGING = "https://acme-staging-v02.api.letsencrypt.org/directory"


def log(*a):
    print("[acme]", *a, flush=True)


def _b64url(data):
    if isinstance(data, str):
        data = data.encode()
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode()


def _int_b64url(n):
    length = (n.bit_length() + 7) // 8
    return _b64url(n.to_bytes(length, "big"))


class ACMEError(Exception):
    pass


def cert_still_valid(cert_path, min_days=30):
    if not os.path.exists(cert_path):
        return False
    try:
        with open(cert_path, "rb") as f:
            cert = x509.load_pem_x509_certificate(f.read())
        remaining = cert.not_valid_after_utc - \
            datetime.datetime.now(datetime.timezone.utc)
        return remaining.days >= min_days
    except Exception:
        return False


class ACMEClient:
    def __init__(self, directory_url=LETSENCRYPT_PROD):
        self.directory_url = directory_url
        self.directory = None
        self.nonce = None
        self.key = None
        self.kid = None

    def _http(self, url, data=None, method=None):
        req = urllib.request.Request(url, data=data, method=method)
        if data:
            req.add_header("Content-Type", "application/jose+json")
        try:
            resp = urllib.request.urlopen(req, timeout=30)
            status = resp.status
            headers = resp.headers
            body = resp.read()
        except urllib.error.HTTPError as e:
            status = e.code
            headers = e.headers
            body = e.read()
        self.nonce = headers.get("Replay-Nonce", self.nonce)
        try:
            return json.loads(body), headers, status
        except (json.JSONDecodeError, ValueError):
            return body, headers, status

    def _signed(self, url, payload):
        if self.nonce is None:
            self._http(self.directory["newNonce"], method="HEAD")
        pub = self.key.public_key().public_numbers()
        header = {"alg": "RS256", "nonce": self.nonce, "url": url}
        if self.kid:
            header["kid"] = self.kid
        else:
            header["jwk"] = {
                "kty": "RSA",
                "n": _int_b64url(pub.n),
                "e": _int_b64url(pub.e),
            }
        protected = _b64url(json.dumps(header))
        payload_b64 = "" if payload is None else _b64url(json.dumps(payload))
        signing_input = ("%s.%s" % (protected, payload_b64)).encode()
        sig = self.key.sign(signing_input, padding.PKCS1v15(), hashes.SHA256())
        body = json.dumps({
            "protected": protected,
            "payload": payload_b64,
            "signature": _b64url(sig),
        }).encode()
        return self._http(url, data=body)

    def _thumbprint(self):
        pub = self.key.public_key().public_numbers()
        jwk = {"e": _int_b64url(pub.e), "kty": "RSA", "n": _int_b64url(pub.n)}
        canonical = json.dumps(jwk, sort_keys=True, separators=(",", ":"))
        return _b64url(hashlib.sha256(canonical.encode()).digest())

    def provision(self, hostname, cert_dir):
        os.makedirs(cert_dir, exist_ok=True)
        cert_path = os.path.join(cert_dir, "fullchain.pem")
        key_path = os.path.join(cert_dir, "privkey.pem")

        if cert_still_valid(cert_path):
            log("existing cert still valid:", cert_path)
            return cert_path, key_path

        acct_key_path = os.path.join(cert_dir, "account.key")
        if os.path.exists(acct_key_path):
            with open(acct_key_path, "rb") as f:
                self.key = serialization.load_pem_private_key(
                    f.read(), password=None)
            log("loaded account key")
        else:
            self.key = rsa.generate_private_key(65537, 2048)
            with open(acct_key_path, "wb") as f:
                f.write(self.key.private_bytes(
                    serialization.Encoding.PEM,
                    serialization.PrivateFormat.PKCS8,
                    serialization.NoEncryption()))
            os.chmod(acct_key_path, 0o600)
            log("generated account key")

        self.directory, _, _ = self._http(self.directory_url)
        log("directory:", self.directory_url)

        body, hdrs, status = self._signed(
            self.directory["newAccount"],
            {"termsOfServiceAgreed": True})
        if status not in (200, 201):
            raise ACMEError("account registration failed: HTTP %d %s"
                            % (status, body))
        self.kid = hdrs.get("Location")
        log("account:", self.kid)

        body, hdrs, status = self._signed(
            self.directory["newOrder"],
            {"identifiers": [{"type": "dns", "value": hostname}]})
        if status not in (200, 201):
            raise ACMEError("order creation failed: HTTP %d %s"
                            % (status, body))
        order_url = hdrs.get("Location")
        finalize_url = body["finalize"]
        authz_urls = body["authorizations"]
        log("order created for %s" % hostname)

        authz_body, _, status = self._signed(authz_urls[0], None)
        if status >= 400:
            raise ACMEError("authorization fetch failed: HTTP %d" % status)
        challenge = None
        for c in authz_body.get("challenges", []):
            if c["type"] == "http-01":
                challenge = c
                break
        if not challenge:
            raise ACMEError("no http-01 challenge offered")

        token = challenge["token"]
        key_authz = "%s.%s" % (token, self._thumbprint())
        log("serving http-01 challenge on port 80...")

        srv = _ChallengeServer(token, key_authz)
        srv.start()
        try:
            _, _, status = self._signed(challenge["url"], {})
            if status >= 400:
                raise ACMEError("challenge trigger failed: HTTP %d" % status)

            for attempt in range(30):
                time.sleep(2)
                body, _, _ = self._signed(authz_urls[0], None)
                st = body.get("status")
                if st == "valid":
                    log("challenge validated")
                    break
                if st == "invalid":
                    err = ""
                    for ch in body.get("challenges", []):
                        if ch.get("error"):
                            err = ch["error"].get("detail", str(ch["error"]))
                            break
                    raise ACMEError("challenge failed: %s" % err)
            else:
                raise ACMEError("challenge timed out after 60s")
        finally:
            srv.stop()

        domain_key = rsa.generate_private_key(65537, 2048)
        csr = (x509.CertificateSigningRequestBuilder()
               .subject_name(x509.Name([
                   x509.NameAttribute(NameOID.COMMON_NAME, hostname)]))
               .add_extension(
                   x509.SubjectAlternativeName([x509.DNSName(hostname)]),
                   critical=False)
               .sign(domain_key, hashes.SHA256()))
        csr_der = csr.public_bytes(serialization.Encoding.DER)

        body, _, status = self._signed(finalize_url, {"csr": _b64url(csr_der)})
        if status >= 400:
            raise ACMEError("finalize failed: HTTP %d %s" % (status, body))

        cert_url = body.get("certificate")
        if not cert_url:
            for _ in range(15):
                time.sleep(2)
                body, _, _ = self._signed(order_url, None)
                if body.get("status") == "valid" and body.get("certificate"):
                    cert_url = body["certificate"]
                    break
            else:
                raise ACMEError("certificate not issued in time")

        cert_pem, _, status = self._signed(cert_url, None)
        if status >= 400:
            raise ACMEError("cert download failed: HTTP %d" % status)
        if isinstance(cert_pem, bytes):
            pass
        elif isinstance(cert_pem, str):
            cert_pem = cert_pem.encode()
        elif isinstance(cert_pem, dict):
            raise ACMEError("unexpected cert response: %s" % cert_pem)

        with open(cert_path, "wb") as f:
            f.write(cert_pem)
        with open(key_path, "wb") as f:
            f.write(domain_key.private_bytes(
                serialization.Encoding.PEM,
                serialization.PrivateFormat.PKCS8,
                serialization.NoEncryption()))
        os.chmod(key_path, 0o600)
        log("cert saved: %s" % cert_path)
        return cert_path, key_path


class _ChallengeHandler(BaseHTTPRequestHandler):
    _token = None
    _key_authz = None

    def do_GET(self):
        expected = "/.well-known/acme-challenge/%s" % self._token
        if self.path == expected:
            self.send_response(200)
            self.send_header("Content-Type", "text/plain")
            self.end_headers()
            self.wfile.write(self._key_authz.encode())
        else:
            self.send_response(404)
            self.end_headers()

    def log_message(self, fmt, *args):
        pass


class _ChallengeServer:
    def __init__(self, token, key_authz, port=80):
        handler = type("H", (_ChallengeHandler,),
                       {"_token": token, "_key_authz": key_authz})
        self.httpd = HTTPServer(("0.0.0.0", port), handler)
        self.thread = None

    def start(self):
        self.thread = threading.Thread(
            target=self.httpd.serve_forever, daemon=True)
        self.thread.start()

    def stop(self):
        self.httpd.shutdown()
        if self.thread:
            self.thread.join(timeout=5)
