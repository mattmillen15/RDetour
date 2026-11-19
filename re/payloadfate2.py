#!/usr/bin/env python3
"""payloadfate2 — isolate the 0x800759d8 abort: fresh tunnel per variant.

KNOWN KILLER (payloadfate P1): [DC,127.0.0.1]:445 + 51 zero bytes ->
close_channel -> gateway replies 0x10 d8590780 (no 0x11) -> tunnel dead.
Also seen in tap at 22:08: abort 14ms after garbage data, no close sent.

Each variant: fresh tunnel (svcaccount = push-free), one cycle, SSH probe,
report. Exception-isolated.

  A1  51 zero bytes, [DC,127.0.0.1], close after 2s   (confirm P1)
  A2  1 zero byte,   [DC,127.0.0.1], close after 2s
  A3  real NEGOTIATE split 51B/+10ms rest, [DC,127]
  A4  real NEGOTIATE, close immediately, no read
  A5  valid SMB1 negotiate prefix 51B then rest        (nxc-like fragment)
  A6  51 zero bytes, [DC] single name (no fallback)
  A7  51 zero bytes, [DC,127], NO close — hold 10s (abort without close?)
  A8  51 zero bytes to [SSH]:22 channel (garbage to non-SMB port)
  A9  51 zero bytes, then read 3s, then negotiate (recovery attempt?)
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import (RDGTunnel, RDGError, smb2_negotiate,  # noqa: E402
                     smb2_server_guid)

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
DC = "192.168.15.40"
SSH = "192.168.15.152"

T0 = time.time()
NEG = smb2_negotiate()


def say(*a):
    print("%7.3f" % (time.time() - T0), *a, flush=True)


def smb1_negotiate():
    """nxc/impacket-style SMB1 negotiate (valid, self-contained)."""
    dialects = b"PC NETWORK PROGRAM 1.0\x00LANMAN1.0\x00NT LM 0.12\x00"
    bcc = len(dialects)
    body = struct.pack("<B", 0) + struct.pack("<H", bcc) + dialects
    hdr = b"\xffSMB\x72" + b"\x00" * 27
    nbss = struct.pack(">I", 32 + len(body))[:1] + \
        struct.pack(">I", 32 + len(body))[1:]
    return nbss + hdr + body


class TapTunnel(RDGTunnel):
    eof = False

    def _out_fill(self, n, timeout=10):
        try:
            super()._out_fill(n, timeout)
        except RDGError as e:
            if "EOF" in str(e):
                self.eof = True
            raise

    def recv_packet(self, timeout=15):
        p = super().recv_packet(timeout)
        if p is None:
            say("    RX - (eof=%s)" % self.eof)
        elif p[0] not in (0xA, 0xD):
            say("    RX 0x%02x len=%d body=%s" %
                (p[0], 8 + len(p[1]), p[1][:12].hex()))
        return p


def fresh():
    t = TapTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    return t


def ssh_probe(t, tag):
    if t.closed or t.eof:
        return False
    try:
        t.open_channel([SSH], 22)
    except RDGError as e:
        say("  [%s] probe create FAIL %s eof=%s" % (tag, e, t.eof))
        return False
    buf = b""
    t0 = time.monotonic()
    while time.monotonic() - t0 < 6:
        d = t.read(timeout=2)
        if d is None:
            break
        buf += d
        if b"SSH" in buf[:8]:
            break
    try:
        t.close_channel()
    except RDGError:
        return False
    return b"SSH" in buf[:8]


def run_variant(name, payload_fn):
    say("=== %s ===" % name)
    t = None
    verdict = "SURVIVED"
    try:
        t = fresh()
        payload_fn(t)
    except RDGError as e:
        say("  variant raised: %s (eof=%s)" % (e, t and t.eof))
        verdict = "RAISED"
    except Exception as e:
        say("  variant crashed: %r" % e)
        verdict = "CRASHED"
    if t is not None:
        try:
            ok = ssh_probe(t, name)
            say(">>> %s: %s" % (name, "TUNNEL DEAD" if not ok else "ok"))
            verdict = "DEAD" if not ok else verdict
        finally:
            t.close()
    say("")
    return verdict


def v(names, port, data, wait=2.0, close=True, post=None):
    """Write `data` (bytes or list-of-bytes), wait, optionally close."""
    def fn(t):
        t.open_channel(names, port)
        say("  granted")
        if isinstance(data, list):
            for i, part in enumerate(data):
                t.write(part)
                say("  wrote %dB (part %d)" % (len(part), i))
                if post and i == 0:
                    time.sleep(post)
        else:
            t.write(data)
            say("  wrote %dB" % len(data))
        if wait:
            time.sleep(wait)
        if close:
            t.close_channel()
            say("  closed")
    return fn


SMB1 = smb1_negotiate()
VARIANTS = [
    ("A1-51zero-dc-fb-close", v([DC, "127.0.0.1"], 445, b"\x00" * 51)),
    ("A2-1zero-dc-fb", v([DC, "127.0.0.1"], 445, b"\x00")),
    ("A3-neg-split-51", v([DC, "127.0.0.1"], 445,
                          [NEG[:51], NEG[51:]], post=0.01)),
    ("A4-neg-blindclose", v([DC, "127.0.0.1"], 445, NEG, wait=0)),
    ("A5-smb1-split-51", v([DC, "127.0.0.1"], 445,
                           [SMB1[:51], SMB1[51:]], post=0.01)),
    ("A6-51zero-dc-single", v([DC], 445, b"\x00" * 51)),
    ("A7-51zero-hold10", v([DC, "127.0.0.1"], 445, b"\x00" * 51,
                           wait=10, close=False)),
    ("A8-51zero-ssh22", v([SSH], 22, b"\x00" * 51)),
    ("A9-51zero-then-neg", v([DC, "127.0.0.1"], 445,
                             [b"\x00" * 51, NEG], post=1.0)),
]

if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    for name, fn in VARIANTS:
        run_variant(name, fn)
    say("DONE")
