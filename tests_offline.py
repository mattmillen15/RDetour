"""Offline tests: arg forms, packet builders, SOCKS front end with stub tunnel."""
import atexit
import os
import shutil
import socket
import struct
import sys
import tempfile
import threading
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
import rdetour

# kill-cache isolation at IMPORT time (pytest collects this module without
# touching __main__): every WarmSocksServer reads AND writes the refused-445
# cache, so point RDETOUR_CACHE_DIR at a throwaway dir — without it one run
# poisons the operator's ~/.rdetour and the next run answers test hosts
# from the cache (flake)
_OLD_CACHE_DIR = os.environ.get("RDETOUR_CACHE_DIR")
_CACHE_DIR = tempfile.mkdtemp(prefix="rdetour-tests-")
os.environ["RDETOUR_CACHE_DIR"] = _CACHE_DIR
atexit.register(shutil.rmtree, _CACHE_DIR, ignore_errors=True)


def test_identity_forms():
    a = rdetour.parse_args(["-g", "gw", "-u", "CORP\\jdoe", "-p", "x"])
    assert (a.domain, a.user) == ("CORP", "jdoe"), (a.domain, a.user)
    a = rdetour.parse_args(["-g", "gw", "-u", "jdoe@corp.com", "-p", "x"])
    assert (a.domain, a.user) == ("corp.com", "jdoe")
    a = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-d", "CORP", "-p", "x"])
    assert (a.domain, a.user) == ("CORP", "jdoe")
    a = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x"])
    assert a.domain is None and a.user == "jdoe"
    # env password fallback
    import os
    os.environ["RDETOUR_PASSWORD"] = "envpass"
    a = rdetour.parse_args(["-g", "gw", "-u", "jdoe"])
    assert a.password == "envpass"
    del os.environ["RDETOUR_PASSWORD"]
    print("identity forms OK")


def test_probe_bytes():
    msg = rdetour.smb2_negotiate()
    assert msg[0:4] == b"\x00\x00\x00\x6a", msg[:4].hex()  # NBSS len 106
    assert msg[4:8] == b"\xfeSMB", msg[:8].hex()
    print("smb2 probe bytes OK (len %d)" % len(msg))


def test_ext_auth_encoding():
    # handshake selector: last u16 of the 14-byte packet
    hs = rdetour.pkt(0x1, b"\x01\x00" + rdetour.u16(0) +
                     rdetour.u16(rdetour.RDGTunnel.EXT_AUTH_SSPI_NTLM))
    assert hs[12:14] == b"\x04\x00" and len(hs) == 14, hs.hex()

    # PAA TUNNEL_CREATE: fieldsPresent=1, cookie as UTF-16 with byte length
    cookie = rdetour.utf16z("abc")
    body = rdetour.u32(0xD) + rdetour.u16(1) + rdetour.u16(0) + \
        rdetour.u16(len(cookie)) + cookie
    p = rdetour.pkt(0x4, body)
    assert p[8:12] == rdetour.u32(0xD)
    assert p[12:14] == b"\x01\x00"                     # fieldsPresent PAA
    assert p[16:18] == rdetour.u16(len(cookie))        # 8 bytes for "abc\0"
    assert p[18:].decode("utf-16-le") == "abc\x00"

    # RDG-Auth-Scheme replaces Authorization in ext-auth modes
    t = rdetour.RDGTunnel("gw", user="u", password="p", ext_auth="paa",
                          paa_cookie="abc")
    req = t._request("RDG_OUT_DATA").decode()
    assert "RDG-Auth-Scheme: PAA" in req and "Authorization:" not in req
    t2 = rdetour.RDGTunnel("gw", user="u", password="p")
    assert "RDG-Auth-Scheme" not in t2._request("RDG_OUT_DATA").decode()
    print("ext-auth encoding OK (handshake/PAA/Scheme header)")





class StubTunnel:
    """Echo tunnel: returns whatever was written, in one read()."""

    def __init__(self, *a, **kw):
        self.buf = b""
        self.closed = False
        self.channel_open = True   # models a tunnel whose channel is live
        self.event = threading.Event()

    def write(self, d):
        self.buf += d
        self.event.set()

    def read(self, timeout=15):
        if not self.event.wait(timeout):
            return None
        out, self.buf = self.buf, b""
        self.event.clear()
        if out:
            return out
        return None

    def close(self):
        self.closed = True


def test_socks_frontend():
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11987"])
    made = []

    def factory(host, port, auth_timeout=15):
        made.append((host, port))
        return StubTunnel()

    srv = rdetour.SocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    # IPv4 CONNECT + echo round trip
    c = socket.create_connection(("127.0.0.1", 11987), timeout=5)
    c.sendall(b"\x05\x01\x00")
    assert c.recv(2) == b"\x05\x00"
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.1.2.3") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(b"hello-tunnel")
    echoed = b""
    while len(echoed) < 12:
        echoed += c.recv(64)
    assert echoed == b"hello-tunnel"
    assert made == [("10.1.2.3", 445)]
    c.close()

    # domain CONNECT
    c = socket.create_connection(("127.0.0.1", 11987), timeout=5)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    host = b"dc01.corp.local"
    c.sendall(b"\x05\x01\x00\x03" + bytes([len(host)]) + host +
              struct.pack(">H", 88))
    r = c.recv(10)
    assert r[1] == 0x00
    c.sendall(b"k")
    time.sleep(0.2)
    c.recv(64)
    assert made[-1] == ("dc01.corp.local", 88)
    c.close()

    # IPv6 CONNECT
    c = socket.create_connection(("127.0.0.1", 11987), timeout=5)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x04" + socket.inet_pton(socket.AF_INET6,
              "dead::1") + struct.pack(">H", 3389))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    assert made[-1] == ("dead::1", 3389)
    c.close()

    # auth failure maps to reply 0x01
    def bad_factory(host, port, auth_timeout=15):
        raise rdetour.RDGAuthError("leg failed: HTTP 401")

    srv2 = rdetour.SocksServer(args, bad_factory)
    args2 = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                                "--socks-port", "11988"])
    srv2.args = args2
    threading.Thread(target=srv2.serve, daemon=True).start()
    time.sleep(0.3)
    c = socket.create_connection(("127.0.0.1", 11988), timeout=5)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.9.9.9") +
              struct.pack(">H", 80))
    r = c.recv(10)
    assert r[1] == 0x01, r.hex()
    c.close()

    # MFA not approved maps to reply 0x02
    def mfa_factory(host, port, auth_timeout=15):
        raise rdetour.MfaNotApproved("tunnel auth denied: CAP_DENIED (win32 23003)")

    args3 = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                                "--socks-port", "11989"])
    srv3 = rdetour.SocksServer(args3, mfa_factory)
    threading.Thread(target=srv3.serve, daemon=True).start()
    time.sleep(0.3)
    c = socket.create_connection(("127.0.0.1", 11989), timeout=5)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.9.9.9") +
              struct.pack(">H", 80))
    r = c.recv(10)
    assert r[1] == 0x02, r.hex()
    c.close()

    # gateway-side unreachable maps to reply 0x03
    def unreach_factory(host, port, auth_timeout=15):
        raise rdetour.RDGUnreachableError(
            "no channel response in 25s — target %s:%d likely unreachable "
            "from the gateway" % (host, port))

    args4 = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                                "--socks-port", "11993"])
    srv4 = rdetour.SocksServer(args4, unreach_factory)
    threading.Thread(target=srv4.serve, daemon=True).start()
    time.sleep(0.3)
    c = socket.create_connection(("127.0.0.1", 11993), timeout=5)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.9.9.9") +
              struct.pack(">H", 80))
    r = c.recv(10)
    assert r[1] == 0x03, r.hex()
    c.close()

    print("SOCKS front end OK (v4/domain/v6 CONNECT, error mapping)")


def test_fatigue_flow():
    """Two MfaNotApproved then success; verify pushes counted and gate set."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11990", "--mfa-fatigue",
                               "--mfa-interval", "1", "--mfa-jitter", "0"])
    n = {"i": 0}

    def factory(host, port, auth_timeout=15):
        n["i"] += 1
        if n["i"] <= 2:
            raise rdetour.MfaNotApproved("no response in 1s (MFA pending?)")
        return StubTunnel()

    srv = rdetour.SocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)
    c = socket.create_connection(("127.0.0.1", 11990), timeout=10)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.5") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    assert n["i"] == 3 and srv.stats["pushes"] == 3 and srv.mfa_gate.is_set()
    c.close()

    # post-gate connection goes straight through (attempt 4)
    c = socket.create_connection(("127.0.0.1", 11990), timeout=10)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.6") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00 and n["i"] == 4
    c.close()
    print("fatigue flow OK (2 denials -> approve -> gate open)")


def test_fatigue_max():
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11991", "--mfa-fatigue",
                               "--mfa-interval", "1", "--mfa-max", "2"])

    def factory(host, port, auth_timeout=15):
        raise rdetour.MfaNotApproved("denied")

    srv = rdetour.SocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)
    c = socket.create_connection(("127.0.0.1", 11991), timeout=15)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.5") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x01, r.hex()  # exhausted -> general failure
    assert srv.mfa_dead
    c.close()

    # subsequent connection fails fast without pushing
    pushes = srv.stats["pushes"]
    c = socket.create_connection(("127.0.0.1", 11991), timeout=10)
    c.sendall(b"\x05\x01\x00")
    c.recv(2)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.5") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x01 and srv.stats["pushes"] == pushes
    c.close()
    print("fatigue max/dead-flag OK")


def test_establishment_serialized():
    """Regression: parallel SOCKS connects must NOT run tunnel setups in
    parallel (parallel setups = parallel MFA pushes = approval races)."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11992", "--auth-timeout", "5"])
    state = {"inflight": 0, "max": 0, "done": 0}
    lock = threading.Lock()

    def factory(host, port, auth_timeout=15):
        with lock:
            state["inflight"] += 1
            state["max"] = max(state["max"], state["inflight"])
        time.sleep(0.3)  # simulate slow establish (MFA wait)
        with lock:
            state["inflight"] -= 1
            state["done"] += 1
        return StubTunnel()

    srv = rdetour.SocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    socks = []
    for _ in range(3):
        c = socket.create_connection(("127.0.0.1", 11992), timeout=10)
        c.sendall(b"\x05\x01\x00")
        c.recv(2)
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.7.7.7") +
                  struct.pack(">H", 445))
        socks.append(c)
    for c in socks:
        r = c.recv(10)
        assert r[1] == 0x00, r.hex()
        c.close()
    assert state["max"] == 1, "parallel establishments observed: %d" % state["max"]
    assert state["done"] == 3
    print("establishment serialization OK (3 parallel connects, no overlap)")


def test_channel_create_body():
    """Single name must be byte-identical to the legacy layout; a name LIST
    rides the pick-live semantics (numResources=N, numAlt=0)."""
    nm = rdetour.utf16z("192.168.15.40")
    legacy = b"\x01\x00" + rdetour.u16(445) + rdetour.u16(3) + \
        rdetour.u16(len(nm)) + nm
    assert rdetour.channel_create_body("192.168.15.40", 445, 3) == legacy
    body = rdetour.channel_create_body(["192.168.15.99", "127.0.0.1"], 445, 3)
    assert body[:2] == b"\x02\x00"                    # 2 primary, 0 alt
    assert body[2:6] == rdetour.u16(445) + rdetour.u16(3)
    n1 = rdetour.utf16z("192.168.15.99")
    n2 = rdetour.utf16z("127.0.0.1")
    assert body[6:8] == rdetour.u16(len(n1)) and body[8:8 + len(n1)] == n1
    off = 8 + len(n1)
    assert body[off:off + 2] == rdetour.u16(len(n2)) and \
        body[off + 2:] == n2
    print("channel_create_body OK (legacy single + pick-live list)")


def test_smb2_server_guid():
    """GUID at [76:92] of an NBSS+SMB2 negotiate response; None otherwise."""

    def resp(guid):
        hdr = b"\xfeSMB" + b"\x00" * 60
        assert len(hdr) == 64
        body = rdetour.u16(65) + rdetour.u16(1) + rdetour.u16(0x0302) + \
            rdetour.u16(0) + guid + rdetour.u32(0) + rdetour.u16(0) * 2
        msg = hdr + body
        return b"\x00" + struct.pack(">I", len(msg))[1:] + msg

    g = bytes(range(0x10, 0x20))
    full = resp(g)
    assert len(full) >= 92
    assert rdetour.smb2_server_guid(full) == g
    assert rdetour.smb2_server_guid(full[:91]) is None      # short buffer
    assert rdetour.smb2_server_guid(b"SSH-2.0-x\r\n" * 20) is None  # not SMB
    print("smb2_server_guid OK (parse + short/non-SMB guards)")


def test_gw_fallback():
    """Fallback channels: dead host's answer carries the GATEWAY'S GUID ->
    SOCKS client gets cut; live host's GUID differs -> bytes relayed."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11995"])

    def smb2_resp(guid):
        hdr = b"\xfeSMB" + b"\x00" * 60
        body = rdetour.u16(65) + rdetour.u16(1) + rdetour.u16(0x0302) + \
            rdetour.u16(0) + guid + rdetour.u32(0) + rdetour.u16(0) * 2
        msg = hdr + body
        return b"\x00" + struct.pack(">I", len(msg))[1:] + msg

    gw_guid = b"G" * 16
    other_guid = b"H" * 16

    class FbStub(StubTunnel):
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []
            self.out_q = []
            self.script = {
                ("127.0.0.1", 445): smb2_resp(gw_guid),
                ("10.9.9.9", 445): smb2_resp(gw_guid),      # dead -> gw ans
                ("10.2.2.2", 445): smb2_resp(other_guid),   # live target
            }

        def open_channel(self, hosts, port, protocol=3):
            first = hosts if isinstance(hosts, str) else hosts[0]
            self.channels.append((hosts, port))
            r = self.script.get((first, port))
            if r:
                self.out_q.append(r)
                self.event.set()
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def close_channel(self, timeout=8):
            assert self.channel_open
            self.channel_open = False

        def _close_channel_xchg(self, timeout=8):
            self.channel_open = False

        def read(self, timeout=15):
            if self.out_q:
                return self.out_q.pop(0)
            return StubTunnel.read(self, timeout)

    made = []

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return FbStub()

    srv = rdetour.WarmSocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    def socks_connect(host, port):
        c = socket.create_connection(("127.0.0.1", 11995), timeout=10)
        c.sendall(b"\x05\x01\x00")
        assert c.recv(2) == b"\x05\x00"
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton(host) +
                  struct.pack(">H", port))
        r = c.recv(10)
        assert r[1] == 0x00, r.hex()
        return c

    # dead host: gateway answers, client must be cut off with NO smb bytes
    c = socks_connect("10.9.9.9", 445)
    c.settimeout(5)
    c.sendall(b"negotiate-request")
    got = c.recv(64)          # EOF (or reset) — never the gw's GUID bytes
    assert got == b"", "dead host must not relay gateway bytes: %r" % got
    c.close()
    time.sleep(0.3)
    assert srv.stats["gw_dead"] == 1, srv.stats

    # live host: response bytes flow to the client
    c = socks_connect("10.2.2.2", 445)
    c.settimeout(5)
    c.sendall(b"negotiate-request")
    got = b""
    while len(got) < 92:
        d = c.recv(128)
        if not d:
            break
        got += d
    assert rdetour.smb2_server_guid(got) == other_guid, got[:16]
    c.close()
    time.sleep(0.3)

    assert len(made) == 1, "one tunnel for all of it, got %d" % len(made)
    names = [ch[0] for ch in srv.tunnel.channels]
    assert ["127.0.0.1"] in names                 # baseline cycle
    assert ["10.9.9.9", "127.0.0.1"] in names     # dead rode the fallback
    assert ["10.2.2.2", "127.0.0.1"] in names
    assert srv.stats["pushes"] == 1
    print("gw fallback OK (dead cut locally, live relayed, 1 tunnel)")


def test_warm_mode():
    """One tunnel, many sequential channels: 3 SOCKS connects -> 1 factory
    call (one MFA push), channels opened/closed per connection."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11994",
                               "--no-gw-fallback"])

    class WarmStub(StubTunnel):
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def close_channel(self, timeout=8):
            assert self.channel_open, "close on dead channel"
            self.channel_open = False

        def _close_channel_xchg(self, timeout=8):
            self.channel_open = False

    made = []

    def factory(host=None, port=None, auth_timeout=15):
        assert host is None, "warm factory must not carry a target"
        made.append(1)
        return WarmStub()

    srv = rdetour.WarmSocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    for i, tgt in enumerate([("10.1.1.1", 445), ("10.2.2.2", 22),
                             ("10.3.3.3", 88)]):
        c = socket.create_connection(("127.0.0.1", 11994), timeout=10)
        c.sendall(b"\x05\x01\x00")
        assert c.recv(2) == b"\x05\x00"
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton(tgt[0]) +
                  struct.pack(">H", tgt[1]))
        r = c.recv(10)
        assert r[1] == 0x00, "conn %d: %s" % (i, r.hex())
        c.sendall(b"ping%d" % i)
        echoed = b""
        while len(echoed) < 5:
            echoed += c.recv(64)
        assert echoed == b"ping%d" % i
        c.close()
        time.sleep(0.2)  # let the server cycle the channel

    assert len(made) == 1, "expected ONE tunnel, got %d" % len(made)
    assert srv.tunnel.channels == [("10.1.1.1", 445), ("10.2.2.2", 22),
                                   ("10.3.3.3", 88)], srv.tunnel.channels
    assert srv.stats["pushes"] == 1
    print("warm mode OK (3 connections, 1 tunnel, %d channels)" %
          len(srv.tunnel.channels))


def test_out_dead_attribution():
    """OUT-leg death (FIN/RST/EOF) is flagged and closes the tunnel, and
    _channel_create blames the teardown instead of 'target unreachable'.
    A mere timeout must set neither flag (wire-verified contract behind
    the warm-pool corpse detection)."""
    t = rdetour.RDGTunnel("gw", user="u", password="p")

    def dead_fill(n, timeout=10):
        raise rdetour.RDGProtocolError("EOF on OUT stream")
    t._out_fill = dead_fill
    assert t.recv_packet(timeout=1) is None
    assert t.out_dead and t.closed, "EOF must flag the tunnel dead"
    t.send_packet = lambda raw: None
    try:
        t._channel_create(["h"], 445, 3)
        raise AssertionError("expected RDGUnreachableError")
    except rdetour.RDGUnreachableError as e:
        assert "torn down" in str(e), str(e)

    t2 = rdetour.RDGTunnel("gw", user="u", password="p")

    def slow_fill(n, timeout=10):
        raise socket.timeout()
    t2._out_fill = slow_fill
    t2.send_packet = lambda raw: None
    try:
        t2._channel_create(["h"], 445, 3)
        raise AssertionError("expected RDGUnreachableError")
    except rdetour.RDGUnreachableError as e:
        assert "torn down" not in str(e), str(e)
        assert not t2.out_dead and not t2.closed
    print("out-dead attribution OK")


def test_srv_close_reason():
    """A gateway CLOSE_CHANNEL (0x10) is surfaced with its reason so the
    warm log can attribute 0x800759d8 to the target closing/resetting."""
    t = rdetour.RDGTunnel("gw", user="u", password="p", verbose=False)
    t.recv_packet = lambda timeout=15: (0x10, rdetour.u32(0x800759D8))
    t.channel_open = True
    assert t.read(timeout=1) is None
    assert t._srv_closed_channel and not t.channel_open
    assert t.last_srv_close_reason == 0x800759D8
    assert rdetour.hresult_name(0x800759D8).startswith("CHANNEL_IO_ABORT")
    print("server-close reason attribution OK")


def test_close_channel_drains_inflight():
    """_close_channel_xchg drains up to 2 pending packets before sending
    CLOSE: a response landing after the CLOSE aborts the whole tunnel
    (0x800759d8 — wire-observed delayed post-close kill)."""
    t = rdetour.RDGTunnel("gw", user="u", password="p")
    seq = [(0xA, rdetour.u16(3) + b"abc"), None, (0x11, b"\x00\x00\x00\x00")]
    t.recv_packet = lambda timeout=15: seq.pop(0) if seq else None
    sent = []
    t.send_packet = lambda raw: sent.append(raw)
    t._close_channel_xchg()
    assert seq == [], "drain consumed data+None, ack consumed the 0x11"
    assert sent and sent[0][0:2] == rdetour.u16(0x10)
    print("close-channel drain OK")


def _smb1_neg(dialects):
    hdr = (b"\xffSMB" + bytes([0x72]) + rdetour.u32(0) + b"\x00" +
           rdetour.u16(0) + rdetour.u16(0) + b"\x00" * 10 +
           rdetour.u16(0) * 4)
    assert len(hdr) == 32
    body = b"\x00" + rdetour.u16(len(dialects)) + dialects
    msg = hdr + body
    return b"\x00" + len(msg).to_bytes(3, "big") + msg


def test_smb1_probe_matcher():
    """The matcher catches impacket's 51-byte single-dialect SMB1 negotiate
    and nothing else (multi-dialect reconnects carry SMB 2.x dialects)."""
    probe = _smb1_neg(b"\x02NT LM 0.12\x00")
    assert len(probe) == 51, len(probe)
    assert rdetour.smb1_probe(probe)
    multi = _smb1_neg(b"\x02PC NETWORK PROGRAM 1.0\x00\x02SMB 2.002\x00"
                      b"\x02SMB 2.???\x00")
    assert not rdetour.smb1_probe(multi)
    assert not rdetour.smb1_probe(rdetour.smb2_negotiate())   # \xfeSMB
    assert not rdetour.smb1_probe(probe[:40])                 # partial frame
    assert not rdetour.smb1_probe(b"hello-tunnel")
    print("smb1 probe matcher OK (probe=%dB multi=%dB)"
          % (len(probe), len(multi)))


def test_smb1_probe_answered_locally():
    """A 445 client sending the SMB1 probe gets an RST and NO channel and
    NO tunnel: the probe must never touch the gateway."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11996", "--no-gw-fallback"])
    made = []

    class WarmStub(StubTunnel):
        def open_channel(self, hosts, port, protocol=3):
            self.channel_open = True
            self._srv_closed_channel = False

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return WarmStub()

    srv = rdetour.WarmSocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    probe = _smb1_neg(b"\x02NT LM 0.12\x00")
    c = socket.create_connection(("127.0.0.1", 11996), timeout=5)
    c.sendall(b"\x05\x01\x00")
    assert c.recv(2) == b"\x05\x00"
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.4.4.4") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(probe)
    try:
        after = c.recv(64)          # RST -> ECONNRESET, or EOF
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.2)
    assert made == [], "probe must not establish a tunnel: %r" % made
    assert srv.stats["smb1_local"] == 1, srv.stats
    print("smb1 probe local answer OK (no tunnel, no channel, client RST)")


def test_stillborn_discard_and_cache():
    """A granted-but-silent 445 channel (refused port) is detected after
    STILLBORN_S: client RST, tunnel discarded, and the host answered
    locally (SOCKS refused) on every later connection — no second tunnel."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11985", "--no-gw-fallback"])
    made = []

    class SilentStub(StubTunnel):
        """Grants channels that never answer (the stillborn wire shape)."""
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []

        def write(self, d):
            pass                             # swallow: no echo, no event

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.channel_open = True
            self._srv_closed_channel = False
            return self                      # NOTE: no event set — silent

        def close_channel(self, timeout=8):
            self.channel_open = False

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return SilentStub()

    old = rdetour.STILLBORN_S
    rdetour.STILLBORN_S = 0.4
    try:
        srv = rdetour.WarmSocksServer(args, factory)
        threading.Thread(target=srv.serve, daemon=True).start()
        time.sleep(0.3)

        c = socket.create_connection(("127.0.0.1", 11985), timeout=10)
        c.sendall(b"\x05\x01\x00")
        assert c.recv(2) == b"\x05\x00"
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.7.7.7") +
                  struct.pack(">H", 445))
        r = c.recv(10)
        assert r[1] == 0x00, r.hex()
        c.sendall(b"multi-dialect-negotiate")     # non-probe -> channel
        try:
            after = c.recv(64)                    # RST after the window
        except ConnectionError:
            after = b""
        assert after == b"", after
        c.close()
        time.sleep(0.3)

        assert made == [1], made
        assert srv.stats["stillborn"] == 1, srv.stats
        assert "10.7.7.7" in srv.stillborn_hosts
        assert srv.tunnel is None or srv.tunnel.closed, "tunnel discarded"

        # second connection to the same host: refused locally, no tunnel
        c = socket.create_connection(("127.0.0.1", 11985), timeout=5)
        c.sendall(b"\x05\x01\x00")
        assert c.recv(2) == b"\x05\x00"
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.7.7.7") +
                  struct.pack(">H", 445))
        r = c.recv(10)
        assert r[1] == 0x05, r.hex()              # connection refused
        c.close()
        assert made == [1], "cached refuse must not rebuild: %r" % made
        assert srv.stats["stillborn"] == 2, srv.stats
    finally:
        rdetour.STILLBORN_S = old
    print("stillborn discard+cache OK (1 tunnel, 2 local refuses)")


def test_loopback_primary_refused_locally():
    """A SOCKS CONNECT to a loopback primary (client-stack artifact, seen
    live: nxc drain emitted one and the gateway's 23005 deny cost a tunnel)
    must be refused locally: no channel, no tunnel, SOCKS reply 0x05."""
    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11984", "--no-gw-fallback"])
    made = []

    class WarmStub(StubTunnel):
        def open_channel(self, hosts, port, protocol=3):
            self.channel_open = True
            self._srv_closed_channel = False

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return WarmStub()

    srv = rdetour.WarmSocksServer(args, factory)
    threading.Thread(target=srv.serve, daemon=True).start()
    time.sleep(0.3)

    for host_bytes, why in ((socket.inet_aton("127.0.0.1"), "v4"),
                            (b"\x00" * 15 + b"\x01", "v6")):
        c = socket.create_connection(("127.0.0.1", 11984), timeout=5)
        c.sendall(b"\x05\x01\x00")
        assert c.recv(2) == b"\x05\x00"
        c.sendall(b"\x05\x01\x00" + (b"\x01" if why == "v4" else b"\x04") +
                  host_bytes + struct.pack(">H", 445))
        r = c.recv(10)
        assert r[1] == 0x05, (why, r.hex())   # connection refused, locally
        c.close()
    time.sleep(0.2)
    assert made == [], "loopback primary must not establish a tunnel: %r" % made
    assert srv.stats["loopback_local"] == 2, srv.stats
    print("loopback primary local refuse OK (no tunnel, no channel, 2x 0x05)")


def test_warm_flag_removed():
    """Warm IS the socks mode: no --warm flag, no cold dispatch."""
    a = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x"])
    assert not hasattr(a, "warm"), "flag must be gone"
    try:
        rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x", "--warm"])
        raise AssertionError("--warm must be rejected")
    except SystemExit:
        pass
    print("warm flag removal OK")


def _warm_session(srv, port):
    """One SOCKS client through srv.handle() on a throwaway listener bound
    to `port` (freed on return): the serve()-based tests above hold their
    ports for the process lifetime and 11986 is the only one left in the
    mandated 11984-11996 window, so the cache tests drive handle() directly
    and share it."""
    l = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    l.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    l.bind(("127.0.0.1", port))
    l.listen(1)

    def run():
        conn, _ = l.accept()
        l.close()
        srv.handle(conn)

    threading.Thread(target=run, daemon=True).start()
    c = socket.create_connection(("127.0.0.1", port), timeout=10)
    c.sendall(b"\x05\x01\x00")
    assert c.recv(2) == b"\x05\x00"
    return c


def _smb2_resp(guid):
    hdr = b"\xfeSMB" + b"\x00" * 60
    body = rdetour.u16(65) + rdetour.u16(1) + rdetour.u16(0x0302) + \
        rdetour.u16(0) + guid + rdetour.u32(0) + rdetour.u16(0) * 2
    msg = hdr + body
    return b"\x00" + struct.pack(">I", len(msg))[1:] + msg


def test_refused445_cache_roundtrip():
    """Kill-cache round trip: instance A classifies a refused-445 host
    (stillborn wire shape) and persists it; instance B — a fresh
    WarmSocksServer on the same gateway — answers CONNECT from the cache
    alone: SOCKS 0x05, no factory call, no channel."""
    import json
    import os
    import shutil
    import tempfile

    old_cd = os.environ.get("RDETOUR_CACHE_DIR")
    cdir = tempfile.mkdtemp(prefix="rdetour-rt-")
    os.environ["RDETOUR_CACHE_DIR"] = cdir
    try:
        args = rdetour.parse_args(["-g", "rtgw", "-u", "jdoe", "-p", "x",
                                   "--socks-port", "11986",
                                   "--no-gw-fallback"])
        made = []

        class SilentStub(StubTunnel):
            """Grants channels that never answer (the stillborn wire shape)."""
            def __init__(self, *a, **kw):
                StubTunnel.__init__(self, *a, **kw)
                self.channels = []

            def write(self, d):
                pass                             # swallow: no echo, no event

            def open_channel(self, hosts, port, protocol=3):
                self.channels.append((hosts, port))
                self.channel_open = True
                self._srv_closed_channel = False
                return self                      # NOTE: no event set — silent

            def close_channel(self, timeout=8):
                self.channel_open = False

        def factory(host=None, port=None, auth_timeout=15):
            made.append(1)
            return SilentStub()

        old = rdetour.STILLBORN_S
        rdetour.STILLBORN_S = 0.4
        try:
            srv = rdetour.WarmSocksServer(args, factory)
            c = _warm_session(srv, 11986)
            c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.8.8.8") +
                      struct.pack(">H", 445))
            r = c.recv(10)
            assert r[1] == 0x00, r.hex()
            c.sendall(b"multi-dialect-negotiate")     # non-probe -> channel
            try:
                after = c.recv(64)                    # cut after the window
            except ConnectionError:
                after = b""
            assert after == b"", after
            c.close()
            time.sleep(0.3)

            assert made == [1], made
            assert "10.8.8.8" in srv.stillborn_hosts, srv.stillborn_hosts
            path = rdetour.refused445_cache_path("rtgw")
            assert os.path.exists(path), path
            with open(path) as f:
                doc = json.load(f)
            assert doc["version"] == 1 and doc["gateway"] == "rtgw", doc
            assert "10.8.8.8" in doc["refused445"], doc

            # instance B: fresh server, same gateway — cache hit, no channel
            made2 = []

            def factory2(host=None, port=None, auth_timeout=15):
                made2.append(1)
                return SilentStub()

            srv2 = rdetour.WarmSocksServer(args, factory2)
            assert srv2.stillborn_hosts == {"10.8.8.8"}, srv2.stillborn_hosts
            c = _warm_session(srv2, 11986)
            c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.8.8.8") +
                      struct.pack(">H", 445))
            r = c.recv(10)
            assert r[1] == 0x05, r.hex()              # refused, locally
            c.close()
            assert made2 == [], "cache hit must not open a tunnel: %r" % made2
            assert srv2.stats["stillborn"] == 1, srv2.stats
        finally:
            rdetour.STILLBORN_S = old
    finally:
        if old_cd is None:
            del os.environ["RDETOUR_CACHE_DIR"]
        else:
            os.environ["RDETOUR_CACHE_DIR"] = old_cd
        shutil.rmtree(cdir, ignore_errors=True)
    print("refused-445 cache round-trip OK (A persists, B refuses locally)")


def test_instakill_cached():
    """First-contact RST, the REAL wire shape (money log 192.168.15.1 /
    192.168.15.152): clean tunnel, create granted, negotiate forwarded,
    tunnel dies silently with ZERO channel data and NO close reason ever
    delivered -> host cached + persisted. Pins the rejected audit "fix":
    gating on last_srv_close_reason would suppress classification exactly
    on the real killers — the classifier must stay reason-agnostic."""
    import json

    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11986",
                               "--no-gw-fallback"])
    made = []
    stubs = []

    class RstStub(StubTunnel):
        """Grants the channel, then the target's RST kills channel AND
        tunnel the moment the first write lands — no data, no 0x10, no
        reason: the OUT leg just dies (the money-log shape)."""
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []

        def write(self, d):
            self._srv_closed_channel = False  # silent: no close seen at all
            self.channel_open = False
            self.closed = True                 # tunnel dies with it
            # NOTE: last_srv_close_reason stays unset — none arrives

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def close_channel(self, timeout=8):
            self.channel_open = False

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        s = RstStub()
        stubs.append(s)
        return s

    srv = rdetour.WarmSocksServer(args, factory)
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.3.3.3") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())       # multi-dialect -> forwarded
    try:
        after = c.recv(64)                    # tunnel death cuts the client
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.3)

    assert made == [1], made
    assert "10.3.3.3" in srv.stillborn_hosts, srv.stillborn_hosts
    assert srv.stats["instakill_cached"] == 1, srv.stats
    assert not stubs[0].suspect, "silent RST on a clean tunnel is not dirty"
    with open(rdetour.refused445_cache_path("gw")) as f:
        assert "10.3.3.3" in json.load(f)["refused445"]
    print("insta-kill classification OK (no-reason silent RST cached)")


def test_delayed_kill_suppressed():
    """Dirty-tunnel suppression: conn 1 ends with the gateway closing the
    channel itself (0x10 seen, no clean ack handshake) — the tunnel stays
    warm but SUSPECT; conn 2's channel on the SAME tunnel dies silently
    zero-data (the previous close's delayed kill) -> NOT cached."""
    import json

    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11986",
                               "--no-gw-fallback"])
    stubs = []

    class DelayedKillStub(StubTunnel):
        """Session 1: gateway 0x10 close seen by the reader (abrupt, no
        clean ack). Session 2: the delayed kill RSTs the new channel the
        moment its first write lands — silent, zero data, no reason."""
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []
            self.sessions = 0

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.sessions += 1
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def write(self, d):
            if self.sessions >= 2:
                self._srv_closed_channel = False   # silent: no close seen
                self.channel_open = False
                self.closed = True

        def read(self, timeout=15):
            if self.sessions == 1:
                # gateway closes the channel itself: reader sees the 0x10
                self._srv_closed_channel = True
                self.last_srv_close_reason = 0
                self.channel_open = False
            return None

        def close_channel(self, timeout=8):
            self.channel_open = False

    def factory(host=None, port=None, auth_timeout=15):
        s = DelayedKillStub()
        stubs.append(s)
        return s

    srv = rdetour.WarmSocksServer(args, factory)

    # conn 1: channel granted, negotiate forwarded, gateway 0x10 ends it
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.5") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())
    try:
        after = c.recv(64)
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.3)
    assert stubs[0].suspect, "abrupt server close must mark the tunnel"
    assert len(stubs) == 1, "tunnel must survive (warm), got %d" % len(stubs)

    # conn 2, SAME tunnel: silent zero-data death -> suppressed, not cached
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.5.5.5") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())
    try:
        after = c.recv(64)
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.3)

    assert len(stubs) == 1            # same tunnel object the whole time
    assert "10.5.5.5" not in srv.stillborn_hosts, srv.stillborn_hosts
    assert srv.stats["instakill_cached"] == 0, srv.stats
    with open(rdetour.refused445_cache_path("gw")) as f:
        assert "10.5.5.5" not in json.load(f)["refused445"]
    print("delayed-kill suppression OK (dirty tunnel, no cache entry)")


def test_drain_seen_data_not_cached():
    """data_seen suppression: the client hangs up before the slow answer
    lands; the close-drain consumes the pending SMB2 response (invisible
    to the reader) and the no-ack teardown turns the tunnel suspect and
    dead -> zero READER data, but the target DID answer -> NOT cached."""
    import json

    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11986",
                               "--no-gw-fallback"])
    live_guid = b"y" * 16
    stubs = []

    class SlowAnswerStub(StubTunnel):
        """Pending SMB2 response that only surfaces inside the close drain,
        then the close gets no ack (the real delayed post-close kill)."""
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []
            self.pending = _smb2_resp(live_guid)

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def write(self, d):
            pass                       # answer stays pending — slow target

        def read(self, timeout=15):
            return None                # nothing reaches the reader in time

        def _close_channel_xchg(self, timeout=8):
            # the real drain semantics: pending DATA consumed here where
            # the reader can never see it; then no ack -> suspect discard
            if self.pending:
                self.pending = None
                self.drain_saw_data = True
            raise rdetour.RDGError("no close-channel ack — tunnel suspect")

        def close_channel(self, timeout=8):
            self.channel_open = False

    def factory(host=None, port=None, auth_timeout=15):
        s = SlowAnswerStub()
        stubs.append(s)
        return s

    srv = rdetour.WarmSocksServer(args, factory)
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.13.13.13") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())
    c.close()                          # client gone before the answer lands
    time.sleep(0.5)                    # session teardown + drain

    assert len(stubs) == 1
    assert stubs[0].drain_saw_data, "drain must flag the consumed answer"
    assert stubs[0].suspect, "no-ack close must mark the tunnel"
    assert "10.13.13.13" not in srv.stillborn_hosts, srv.stillborn_hosts
    assert srv.stats["instakill_cached"] == 0, srv.stats
    with open(rdetour.refused445_cache_path("gw")) as f:
        assert "10.13.13.13" not in json.load(f)["refused445"]
    print("drain-seen-data suppression OK (answer existed, not cached)")


def test_tunnel_data_seen_wiring():
    """The Tunnel itself must flag (a) DATA consumed by the close drain,
    (b) a mid-body DATA death (header eaten, body fill dies), and (c) the
    no-ack close as suspect — the classifier's suppression inputs."""
    t = rdetour.RDGTunnel("gw", user="u", password="p")
    seq = [(0xA, rdetour.u16(3) + b"abc"), None, (0x11, b"\x00\x00\x00\x00")]
    t.recv_packet = lambda timeout=15: seq.pop(0) if seq else None
    t.send_packet = lambda raw: None
    t._close_channel_xchg()
    assert t.drain_saw_data, "drained DATA must set the flag"
    assert not t.suspect and not t.closed, "clean ack keeps tunnel clean"

    t2 = rdetour.RDGTunnel("gw", user="u", password="p")
    t2.recv_packet = lambda timeout=15: None
    t2.send_packet = lambda raw: None
    try:
        t2._close_channel_xchg()
        raise AssertionError("expected RDGProtocolError (no ack)")
    except rdetour.RDGProtocolError:
        pass
    assert t2.closed and t2.suspect, "no-ack close is a dirty discard"
    assert not t2.drain_saw_data

    import types
    t3 = rdetour.RDGTunnel("gw", user="u", password="p")
    t3.out_tls = types.SimpleNamespace(buf=b"")
    full = rdetour.pkt(0xA, rdetour.u16(3) + b"abc")

    def fill(n, timeout=10):
        if n == 8:
            t3.out_tls.buf = full       # header lands...
            return
        raise ConnectionError("RST mid-body")   # ...body never does

    t3._out_fill = fill
    assert t3.recv_packet(timeout=1) is None
    assert t3.drain_saw_data, "mid-body DATA death must set the flag"
    assert t3.closed and t3.out_dead
    print("tunnel data_seen/suspect wiring OK (drain, no-ack, mid-body)")


def test_live_then_abort_not_cached():
    """A LIVE host that answers first and only then dies (the 0x800759d8
    multi-request abort) must NOT enter the kill cache."""
    import json

    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11986",
                               "--no-gw-fallback"])
    made = []
    live_guid = b"z" * 16

    class LiveThenAbortStub(StubTunnel):
        """Answers the negotiate once (live target GUID), then the gateway
        aborts channel+tunnel on the NEXT poll — the loud kill of a live
        host, not a refused port."""
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []
            self.pending = None
            self.answered = False

        def open_channel(self, hosts, port, protocol=3):
            self.channels.append((hosts, port))
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def write(self, d):
            self.pending = _smb2_resp(live_guid)     # live target answers

        def read(self, timeout=15):
            if self.pending:
                out, self.pending = self.pending, None
                self.answered = True
                return out
            if self.answered:                        # answer relayed -> abort
                self.last_srv_close_reason = 0x800759D8
                self._srv_closed_channel = True
                self.channel_open = False
                self.closed = True
            return None

        def close_channel(self, timeout=8):
            self.channel_open = False

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return LiveThenAbortStub()

    srv = rdetour.WarmSocksServer(args, factory)
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.12.12.12") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())
    got = b""
    while len(got) < 92:
        d = c.recv(128)
        if not d:
            break
        got += d
    assert rdetour.smb2_server_guid(got) == live_guid, got[:16]
    try:
        after = c.recv(64)                    # the abort cuts the client
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.3)

    assert made == [1], made
    assert "10.12.12.12" not in srv.stillborn_hosts, srv.stillborn_hosts
    assert srv.stats["instakill_cached"] == 0, srv.stats
    with open(rdetour.refused445_cache_path("gw")) as f:
        doc = json.load(f)
    assert "10.12.12.12" not in doc["refused445"], doc
    print("live-then-abort NOT cached OK (answered host survives the filter)")


def test_oracle_dead_cached():
    """Fallback channel whose first response carries the GATEWAY'S GUID
    (oracle-DEAD) feeds the kill cache: later contacts are answered locally
    AND immune to the refused->loopback race."""
    import json

    args = rdetour.parse_args(["-g", "gw", "-u", "jdoe", "-p", "x",
                               "--socks-port", "11986"])   # fallback armed
    made = []
    gw_guid = b"G" * 16

    class FbDeadStub(StubTunnel):
        def __init__(self, *a, **kw):
            StubTunnel.__init__(self, *a, **kw)
            self.channels = []
            self.out_q = []

        def open_channel(self, hosts, port, protocol=3):
            first = hosts if isinstance(hosts, str) else hosts[0]
            self.channels.append((hosts, port))
            if (first, port) in (("127.0.0.1", 445), ("10.6.6.6", 445)):
                self.out_q.append(_smb2_resp(gw_guid))  # gw loopback answers
                self.event.set()
            self.channel_open = True
            self._srv_closed_channel = False
            return self

        def close_channel(self, timeout=8):
            assert self.channel_open
            self.channel_open = False

        def _close_channel_xchg(self, timeout=8):
            self.channel_open = False

        def read(self, timeout=15):
            if self.out_q:
                return self.out_q.pop(0)
            return StubTunnel.read(self, timeout)

    def factory(host=None, port=None, auth_timeout=15):
        made.append(1)
        return FbDeadStub()

    srv = rdetour.WarmSocksServer(args, factory)
    c = _warm_session(srv, 11986)
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton("10.6.6.6") +
              struct.pack(">H", 445))
    r = c.recv(10)
    assert r[1] == 0x00, r.hex()
    c.sendall(rdetour.smb2_negotiate())
    try:
        after = c.recv(64)          # EOF (or reset) — never the gw GUID bytes
    except ConnectionError:
        after = b""
    assert after == b"", after
    c.close()
    time.sleep(0.3)

    assert made == [1], made
    assert srv.stats["gw_dead"] == 1, srv.stats
    assert "10.6.6.6" in srv.stillborn_hosts, srv.stillborn_hosts
    with open(rdetour.refused445_cache_path("gw")) as f:
        assert "10.6.6.6" in json.load(f)["refused445"]
    print("oracle-DEAD cached OK (gw-loopback verdict persists)")


def test_refused445_ttl():
    """Cache entries older than REFUSED_CACHE_TTL_S are dropped on load and
    the file rewritten without them; fresh entries survive."""
    import json
    import os
    import shutil
    import tempfile

    old_cd = os.environ.get("RDETOUR_CACHE_DIR")
    cdir = tempfile.mkdtemp(prefix="rdetour-ttl-")
    os.environ["RDETOUR_CACHE_DIR"] = cdir
    try:
        gw = "ttl-gw"
        path = rdetour.refused445_cache_path(gw)
        with open(path, "w") as f:
            json.dump({"version": 1, "gateway": gw,
                       "refused445": {
                           "10.1.1.1": time.time() -
                           rdetour.REFUSED_CACHE_TTL_S - 3600}}, f)
        assert rdetour.load_refused445(gw) == set()
        with open(path) as f:
            assert json.load(f)["refused445"] == {}   # rewritten w/o expired

        with open(path, "w") as f:
            json.dump({"version": 1, "gateway": gw,
                       "refused445": {"10.1.1.2": time.time()}}, f)
        assert rdetour.load_refused445(gw) == {"10.1.1.2"}
    finally:
        if old_cd is None:
            del os.environ["RDETOUR_CACHE_DIR"]
        else:
            os.environ["RDETOUR_CACHE_DIR"] = old_cd
        shutil.rmtree(cdir, ignore_errors=True)
    print("refused-445 TTL OK (expired dropped+rewritten, fresh kept)")


def test_refused445_corrupt():
    """A corrupt cache file loads as an empty set — one say line, no
    exception, run continues uncached."""
    import os
    import shutil
    import tempfile

    old_cd = os.environ.get("RDETOUR_CACHE_DIR")
    cdir = tempfile.mkdtemp(prefix="rdetour-bad-")
    os.environ["RDETOUR_CACHE_DIR"] = cdir
    try:
        gw = "bad-gw"
        with open(rdetour.refused445_cache_path(gw), "wb") as f:
            f.write(b"\x00\x01\xff{not json at all")
        assert rdetour.load_refused445(gw) == set()
    finally:
        if old_cd is None:
            del os.environ["RDETOUR_CACHE_DIR"]
        else:
            os.environ["RDETOUR_CACHE_DIR"] = old_cd
        shutil.rmtree(cdir, ignore_errors=True)
    print("refused-445 corrupt file OK (empty set, no exception)")


if __name__ == "__main__":
    test_identity_forms()
    test_probe_bytes()
    test_ext_auth_encoding()
    test_socks_frontend()
    test_fatigue_flow()
    test_fatigue_max()
    test_establishment_serialized()
    test_warm_mode()
    test_channel_create_body()
    test_smb2_server_guid()
    test_gw_fallback()
    test_out_dead_attribution()
    test_srv_close_reason()
    test_close_channel_drains_inflight()
    test_smb1_probe_matcher()
    test_smb1_probe_answered_locally()
    test_stillborn_discard_and_cache()
    test_loopback_primary_refused_locally()
    test_warm_flag_removed()
    test_refused445_cache_roundtrip()
    test_instakill_cached()
    test_delayed_kill_suppressed()
    test_drain_seen_data_not_cached()
    test_tunnel_data_seen_wiring()
    test_live_then_abort_not_cached()
    test_oracle_dead_cached()
    test_refused445_ttl()
    test_refused445_corrupt()
    print("ALL OFFLINE TESTS PASSED")
