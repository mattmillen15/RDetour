#!/usr/bin/env python3
"""loserfate — does the loser-connect verdict kill the warm tunnel?

RE (connectchannel.asm/disconhandler.asm): any HandleInboundPacket error
in CAAEdgeChannel::SendToServer normalizes to 0x800759d8 -> DisconnectChannel.
Wire (rdg_tap 22:08): create([host,127.0.0.1],445) granted -> 51B data ->
unsolicited 0x10 d8590780 14ms later -> OUT leg EOF -> next create eats
instant-None -> warm pool rebuilds (re-push for push users).

THEORY: create([DEAD, 127.0.0.1], 445) leaves a loser connect() pending to
DEAD. When its verdict lands (ms w/ warm negative-ARP, ~3s cold ARP,
~21s SYN blackhole) aaedge aborts the whole channel & tunnel. Scans prime
the gateway's negative ARP entry -> ms verdicts -> kill lands mid-data.

Phases (fresh tunnel each, svcaccount = push-free):
  L4 control : 4x [DC,127.0.0.1] back-to-back (live winner, no loser)
  L2 prime   : 6x [DEAD,127.0.0.1] back-to-back, minimal gap (nxc-like)
  L1 gap     : 6x [DEAD,127.0.0.1] with 5s gaps (negative ARP expires?)
  L3 hold    : 1x [DEAD,127.0.0.1] hold open 25s watching for unsolicited
               packets (catch the verdict mid-channel), then probe creates
Verdicts: per-cycle create/smb/close results + any unsolicited packet,
OUT-EOF flagged (not confused with timeouts).
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


def ts():
    return "%7.3f" % (time.time() - T0)


def say(*a):
    print(*a, flush=True)


class TapTunnel(RDGTunnel):
    """RDGTunnel with packet tap + real EOF detection on the OUT leg."""
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
        p = super().recv_packet(timeout)
        if p is None:
            say("%s   RX -    (eof=%s)" % (ts(), self.eof))
        else:
            say("%s   RX 0x%02x len=%d body=%s" %
                (ts(), p[0], 8 + len(p[1]), p[1][:12].hex()))
        return p


def fresh(tag):
    say("=== %s new tunnel ===" % tag)
    t = TapTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    return t


def cycle(t, names, port, tag, hold=0):
    """Lifecycle-correct [names]:port cycle. Returns 'ok'/'dead' + notes."""
    notes = []
    try:
        t.open_channel(names, port)
    except RDGError as e:
        say("%s [%s] CREATE FAIL %s (eof=%s)" % (ts(), tag, e, t.eof))
        return "dead"
    say("%s [%s] granted" % (ts(), tag))
    if port == 445:
        t.write(smb2_negotiate())
        buf = b""
        t0 = time.monotonic()
        while len(buf) < 92 and time.monotonic() - t0 < 4:
            d = t.read(timeout=1)
            if d:
                buf += d
        g = smb2_server_guid(buf)
        notes.append("guid=%s" % (g.hex()[:8] if g else "none"))
    if hold:
        say("%s [%s] holding %ds..." % (ts(), tag, hold))
        t0 = time.monotonic()
        while time.monotonic() - t0 < hold and not t.closed:
            d = t.read(timeout=1)
            if d is None and not t.channel_open:
                say("%s [%s] channel closed by server while holding "
                    "(eof=%s)" % (ts(), tag, t.eof))
                break
    try:
        t.close_channel()
        say("%s [%s] closed clean (%s)" % (ts(), tag, " ".join(notes)))
        return "ok"
    except RDGError as e:
        say("%s [%s] CLOSE FAIL %s (eof=%s)" % (ts(), tag, e, t.eof))
        return "dead"


def probe(t, tag, names, port):
    """Post-cycle tunnel probe: create + app banner + close."""
    r = cycle(t, names, port, tag)
    return r


def phase(tag, fn):
    say("\n########## %s ##########" % tag)
    t = fresh(tag)
    fn(t)
    alive = not (t.closed or t.eof)
    say(">>> %s tunnel alive=%s eof=%s" % (tag, alive, t.eof))
    t.close()
    return alive


def ph_control(t):
    for i in range(4):
        r = cycle(t, [DC, "127.0.0.1"], 445, "L4/%d" % i)
        if r != "ok":
            say(">>> control died at cycle %d — theory WRONG or rig busy" % i)
            return


def ph_prime(t):
    for i in range(6):
        r = cycle(t, [DEAD, "127.0.0.1"], 445, "L2/%d" % i)
        if r != "ok":
            say(">>> primed cycle %d KILLED (create/close fail above)" % i)
            return


def ph_gap(t):
    for i in range(6):
        r = cycle(t, [DEAD, "127.0.0.1"], 445, "L1/%d" % i)
        if r != "ok":
            say(">>> gapped cycle %d KILLED" % i)
            return
        time.sleep(5)


def ph_hold(t):
    r = cycle(t, [DEAD, "127.0.0.1"], 445, "L3/hold", hold=25)
    if r != "ok":
        say(">>> hold cycle itself died")
    if not t.closed:
        probe(t, "L3/probe-ssh", [SSH], 22)
    if not t.closed:
        probe(t, "L3/probe-dc", [DC], 445)


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    phase("L4-control", ph_control)
    phase("L2-prime", ph_prime)
    phase("L1-gap", ph_gap)
    phase("L3-hold", ph_hold)
    say("\nDONE")
