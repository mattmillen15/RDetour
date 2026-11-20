"""TLS material for the relay listener.

Resolution order:
  1. --cert path.pem  → operator-provided (ADCS, Let's Encrypt manual, etc.)
  2. ACME HTTP-01     → auto-provision from Let's Encrypt (needs port 80 + DNS)
  3. self-signed      → fallback for lab testing (browser warning)
"""
import datetime
import ipaddress
import os

from .acme import ACMEClient, ACMEError, LETSENCRYPT_PROD, LETSENCRYPT_STAGING


def log(*a):
    print("[tls]", *a, flush=True)


def ensure_cert(cfg):
    if cfg.cert:
        if not os.path.isfile(cfg.cert):
            raise SystemExit("cert file not found: %s" % cfg.cert)
        log("using operator-provided cert: %s" % cfg.cert)
        return cfg.cert, cfg.cert

    cert_dir = os.path.join(cfg.out_dir, "certs", cfg.phish_domain)
    os.makedirs(cert_dir, exist_ok=True)

    # Try ACME auto-provisioning
    try:
        directory = LETSENCRYPT_STAGING if cfg.acme_staging else LETSENCRYPT_PROD
        client = ACMEClient(directory_url=directory)
        cert_path, key_path = client.provision(cfg.phish_domain, cert_dir)
        env = "staging" if cfg.acme_staging else "production"
        log("ACME (%s) cert ready: %s" % (env, cert_path))
        return cert_path, key_path
    except ACMEError as e:
        log("ACME failed: %s" % e)
        log("falling back to self-signed cert (browser will warn)")
    except OSError as e:
        log("ACME unavailable (port 80 blocked?): %s" % e)
        log("falling back to self-signed cert")

    # Self-signed fallback
    cert_p = os.path.join(cert_dir, "selfsigned.pem")
    if not os.path.exists(cert_p):
        gen_selfsigned(cfg.phish_domain, cert_p)
        log("generated self-signed cert: %s" % cert_p)
    return cert_p, cert_p


def gen_selfsigned(hostname, path, days=825):
    from cryptography import x509
    from cryptography.hazmat.primitives import hashes, serialization
    from cryptography.hazmat.primitives.asymmetric import rsa
    from cryptography.x509.oid import NameOID
    key = rsa.generate_private_key(public_exponent=65537, key_size=2048)
    name = x509.Name([x509.NameAttribute(NameOID.COMMON_NAME, hostname)])
    san = [x509.DNSName(hostname)]
    try:
        ipaddress.ip_address(hostname)
        san = [x509.IPAddress(ipaddress.ip_address(hostname))]
    except ValueError:
        pass
    now = datetime.datetime.now(datetime.timezone.utc)
    cert = (x509.CertificateBuilder()
            .subject_name(name).issuer_name(name)
            .public_key(key.public_key())
            .serial_number(x509.random_serial_number())
            .not_valid_before(now - datetime.timedelta(days=1))
            .not_valid_after(now + datetime.timedelta(days=days))
            .add_extension(x509.SubjectAlternativeName(san), critical=False)
            .add_extension(x509.BasicConstraints(ca=False, path_length=None),
                           critical=True)
            .sign(key, hashes.SHA256()))
    pem = cert.public_bytes(serialization.Encoding.PEM) + \
        key.private_bytes(serialization.Encoding.PEM,
                          serialization.PrivateFormat.TraditionalOpenSSL,
                          serialization.NoEncryption())
    with open(path, "wb") as f:
        f.write(pem)
    os.chmod(path, 0o600)
    return path
