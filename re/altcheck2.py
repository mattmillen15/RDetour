#!/usr/bin/env python3
"""S13 follow-ups: multi-primary name semantics + post-grant tunnel fate.

S13 showed [dead, live] primary names -> INSTANT grant + working SMB,
but tunnel dead right after close. Distinguish:
  a) loser-connect (dead name) failure at +21s tears tunnel (delayed kill)
  b) close/other immediate cause

  S13b [dead, live]: grant -> smb -> close -> alive checks at +2s/+25s/+45s
  S13c [live, dead]:  order swapped — instant grant? tunnel fate?
  S14  [dead, dead2, live]: 3 names, 2 dead — still instant? fate?
  S15  [dead, GW(.47)]: gateway-as-fallback (realistic scan config; .47:445
      should be open on the RDS box itself)
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
GWIP = "192.168.15.47"
DEAD = "192.168.15.99"
DEAD2 = "192.168.15.98"
DC = "192.168.15.40"
SSH = "192.168.15.152"


def fresh(tag):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{tag}] tunnel up", flush=True)
    return t


def name_entry(host):
    n = host.encode("utf-16-le") + b"\x00\x00"
    return struct.pack("<H", len(n)) + n


def create_multi(t, names, port, timeout=60, tag=""):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
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


def smb_os(t, tag):
    """Negotiate and print the SMB2 dialect/context summary as fingerprint."""
    try:
        t.write(smb2_negotiate())
        r = t.recv_packet(timeout=8)
        if r and r[0] == 0xA and len(r[1]) > 4:
            ln = struct.unpack("<H", r[1][:2])[0]
            data = r[1][2:2 + ln]
            # SMB2 negotiate response: struct size @2, dialect count @4...
            print(f"[{tag}] SMB2 resp {len(data)}B hdr={data[:8].hex()}",
                  flush=True)
            return True
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] data err {e!r}", flush=True)
    return False


def alive_check(t, tag):
    try:
        t.send_packet(pkt(0xD))  # keepalive probe
        p, _ = create_multi(t, [SSH], 22, timeout=8, tag=tag + "/probe")
        ok = p is not None and p[0] == 0x9 and \
            struct.unpack("<I", p[1][0:4])[0] == 0
        if ok:
            t.close_channel()
        print(f"[{tag}] tunnel alive: {ok}", flush=True)
        return ok
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] DEAD {e!r}", flush=True)
        return False


def cycle(t, names, tag):
    p, dt = create_multi(t, names, 445, tag=tag + "/create")
    if not (p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0):
        return False
    smb_os(t, tag)
    t.close_channel()
    return True


def watch(t, tag, delays=(2, 25, 45)):
    for d in delays:
        time.sleep(d if d == 2 else d - 20)  # approx schedule: +2, +25, +45
        if not alive_check(t, f"{tag}/+{d}s"):
            return False
    return True


def run(tag, names):
    print(f"=== {tag}: {names} ===", flush=True)
    t = fresh(tag)
    ok = cycle(t, names, tag)
    if not ok:
        t.close()
        return
    t0 = time.time()
    for wait in (2, 25, 45):
        time.sleep(max(0, wait - (time.time() - t0)))
        if not alive_check(t, f"{tag}/+{wait}s"):
            print(f">>> {tag}: tunnel died by +{wait}s", flush=True)
            t.close()
            return
    print(f">>> {tag}: tunnel SURVIVED 45s after multi-name cycle", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    run("S13b", [DEAD, DC])
    run("S13c", [DC, DEAD])
    run("S14", [DEAD, DEAD2, DC])
    run("S15", [DEAD, GWIP])
    run("S16", [DEAD, "127.0.0.1"])      # user's idea: loopback fallback
    run("S17", ["127.0.0.1"])            # loopback alone (baseline sanity)
