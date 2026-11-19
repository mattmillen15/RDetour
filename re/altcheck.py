#!/usr/bin/env python3
"""S10+: alternative-resource CHANNEL_CREATE — survivable blind-scan oracle?

MS-TSGU HTTP_CHANNEL_PACKET: numResources(1-50), numAltResources(0-3),
port(2, shared), protocol(2, =3), then name entries (UINT16 byte-len +
UTF-16). Hypothesis: gateway tries primary names, then alt names; only
ALL-failed => 23005 + tunnel RST (the multiple-MFA-push killer).

  S10 dead .99:445 primary + LIVE .40:445 alt -> grant @ ~21s? tunnel alive?
  S11 live .40:445 primary + dead alt        -> instant grant?
  S12 dead primary + dead alt (.98)          -> 2x timeout, 23005, RST?
  S13 two PRIMARY names (dead + live)        -> is the primary list also a
      fallback list (name2 tried after name1 fails)?
Oracle: grant latency 0s = primary live; ~21s = primary dead, alt used.
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
DEAD2 = "192.168.15.98"
DC = "192.168.15.40"      # 445 open
SSH = "192.168.15.152"    # 22 open


def fresh(tag):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{tag}] tunnel up", flush=True)
    return t


def name_entry(host):
    n = host.encode("utf-16-le") + b"\x00\x00"
    return struct.pack("<H", len(n)) + n


def create_multi(t, primaries, alts, port, timeout=60, tag=""):
    body = struct.pack("<BB", len(primaries), len(alts)) + \
        struct.pack("<HH", port, 3)
    for h in primaries + alts:
        body += name_entry(h)
    t.send_packet(pkt(0x8, body))
    t0 = time.time()
    p = t.recv_packet(timeout=timeout)
    dt = time.time() - t0
    if p is None:
        print(f"[{tag}] NO RESPONSE ({dt:.1f}s)", flush=True)
        return None, dt
    hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
    print(f"[{tag}] pkt 0x{p[0]:x} hr={hr and '0x%08x' % hr} ({dt:.1f}s)",
          flush=True)
    return p, dt


def smb_probe(t, tag):
    try:
        t.write(smb2_negotiate())
        r = t.recv_packet(timeout=8)
        if r and r[0] == 0xA and len(r[1]) > 4:
            ln = struct.unpack("<H", r[1][:2])[0]
            data = r[1][2:2 + ln]
            print(f"[{tag}] SMB2 reply: {data[:4].hex()} ({len(data)}B)",
                  flush=True)
            return True
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] data err {e!r}", flush=True)
    return False


def alive(t, tag):
    try:
        p, _ = create_multi(t, [SSH], [], 22, timeout=10, tag=tag + "/alive")
        ok = p is not None and p[0] == 0x9 and \
            struct.unpack("<I", p[1][0:4])[0] == 0
        if ok:
            t.close_channel()
        return ok
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] DEAD {e!r}", flush=True)
        return False


def s10():
    print("=== S10: dead primary + live alt ===", flush=True)
    t = fresh("S10")
    p, dt = create_multi(t, [DEAD], [DC], 445, tag="S10/create")
    if p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0:
        print(f">>> S10 GRANTED in {dt:.1f}s "
              f"({'dead-primary, alt used' if dt > 15 else 'PRIMARY LIVE?!'})",
              flush=True)
        smb_probe(t, "S10")
        t.close_channel()
        print(f">>> S10 tunnel alive after alt-grant: {alive(t, 'S10')}",
              flush=True)
    t.close()


def s11():
    print("=== S11: live primary + dead alt ===", flush=True)
    t = fresh("S11")
    p, dt = create_multi(t, [DC], [DEAD], 445, tag="S11/create")
    if p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0:
        print(f">>> S11 instant grant ({dt:.1f}s)", flush=True)
        smb_probe(t, "S11")
        t.close_channel()
    t.close()


def s12():
    print("=== S12: dead primary + dead alt ===", flush=True)
    t = fresh("S12")
    p, dt = create_multi(t, [DEAD], [DEAD2], 445, timeout=70, tag="S12/create")
    if p is None:
        print(">>> S12 tunnel RST after total failure (as designed)", flush=True)
    t.close()


def s13():
    print("=== S13: two primary names (dead, live) ===", flush=True)
    t = fresh("S13")
    p, dt = create_multi(t, [DEAD, DC], [], 445, timeout=60, tag="S13/create")
    if p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0:
        print(f">>> S13 GRANTED ({dt:.1f}s) — primary list IS a fallback list"
              if dt > 15 else f">>> S13 instant ({dt:.1f}s) — first name used "
              "only?", flush=True)
        smb_probe(t, "S13")
        t.close_channel()
        print(f">>> S13 tunnel alive: {alive(t, 'S13')}", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    for fn in (s10, s11, s12, s13):
        try:
            fn()
        except Exception as e:
            print(f"!!! {fn.__name__} aborted: {e!r}", flush=True)
