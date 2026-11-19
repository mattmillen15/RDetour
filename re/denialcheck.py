#!/usr/bin/env python3
"""Does a DENIED CHANNEL_CREATE really kill the tunnel? (authorized lab)

Every earlier denial hit the GATEWAY ITSELF (.47:9999 — self-connect) and the
tool always discarded the tunnel itself, so 'denial RSTs the tunnel' was one
confounded observation. Systematic matrix, fresh tunnel per scenario:

  S1  dead-IP create (.99)          -> then .152:22 banner: survived?
  S2  live-host closed port (.40:9999) -> then .152:22: survived?
  S3  self closed port (.47:9999)   -> then .152:22: survived? (old evidence)
  S4  multiple denials then success -> how many denials per tunnel?
  S5  keepalive after denial then create
  S6  PIPELINED: create to dead IP, immediately create .152:22 without
      waiting — does the gateway serve the second while the first pends?
      (fast blind-scan primitive)
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, u32, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
SSH = ("192.168.15.152", 22)
SMB = ("192.168.15.40", 445)


def fresh(name):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{name}] tunnel up", flush=True)
    return t


def create(t, host, port, timeout=30, tag=""):
    name = host.encode("utf-16-le") + b"\x00\x00"
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", port, 3,
                                                     len(name)) + name))
    t0 = time.time()
    p = t.recv_packet(timeout=timeout)
    dt = time.time() - t0
    if p is None:
        print(f"[{tag}] create {host}:{port} -> NO RESPONSE in {dt:.1f}s",
              flush=True)
        return None, dt
    hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
    print(f"[{tag}] create {host}:{port} -> pkt 0x{p[0]:x} hr=0x{hr:08x} "
          f"({dt:.1f}s)", flush=True)
    return p, dt


def banner(t, tag):
    p = t.recv_packet(timeout=8)
    if p and p[0] == 0xA and len(p[1]) > 4:
        s = p[1][2:2 + struct.unpack("<H", p[1][:2])[0]]
        print(f"[{tag}] banner: {s[:40]}", flush=True)
        return b"SSH" in s[:8]
    print(f"[{tag}] no banner ({p and hex(p[0])})", flush=True)
    return False


def try_reuse(t, name):
    """After a denial, try a fresh create to the SSH host and read banner."""
    try:
        p, dt = create(t, SSH[0], SSH[1], tag=name + "/reuse")
        if p is None:
            print(f">>> {name}: TUNNEL DEAD after denial", flush=True)
            return False
        hr = struct.unpack("<I", p[1][0:4])[0]
        if hr != 0:
            print(f">>> {name}: reuse create denied hr=0x{hr:08x}", flush=True)
            return False
        ok = banner(t, name + "/reuse")
        print(f">>> {name}: {'SURVIVED — tunnel reusable' if ok else 'degraded'}",
              flush=True)
        return ok
    except (ConnectionError, OSError) as e:
        print(f">>> {name}: DEAD ({e!r})", flush=True)
        return False


def cycle_ok(t, name):
    p, _ = create(t, SMB[0], SMB[1], tag=name + "/sanity")
    assert p and struct.unpack("<I", p[1][0:4])[0] == 0
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    assert r and r[0] == 0xA, "sanity channel broken"
    t.send_packet(pkt(0x10, u32(0)))
    t.recv_packet(timeout=8)


def run():
    print("=== S1: dead-IP denial ===", flush=True)
    t = fresh("S1")
    cycle_ok(t, "S1")
    create(t, DEAD, 445, timeout=35, tag="S1")
    try_reuse(t, "S1")
    t.close()

    print("=== S2: live-host closed port ===", flush=True)
    t = fresh("S2")
    cycle_ok(t, "S2")
    create(t, SMB[0], 9999, timeout=35, tag="S2")
    try_reuse(t, "S2")
    t.close()

    print("=== S3: self closed port (old evidence) ===", flush=True)
    t = fresh("S3")
    cycle_ok(t, "S3")
    create(t, GW, 9999, timeout=35, tag="S3")
    try_reuse(t, "S3")
    t.close()

    print("=== S4: 3 denials then success ===", flush=True)
    t = fresh("S4")
    cycle_ok(t, "S4")
    create(t, DEAD, 445, timeout=35, tag="S4")
    create(t, DEAD, 445, timeout=35, tag="S4")
    create(t, SMB[0], 9999, timeout=35, tag="S4")
    try_reuse(t, "S4")
    t.close()

    print("=== S5: keepalive after denial ===", flush=True)
    t = fresh("S5")
    cycle_ok(t, "S5")
    create(t, DEAD, 445, timeout=35, tag="S5")
    t.send_packet(pkt(0xD))            # keepalive
    p = t.recv_packet(timeout=5)
    print(f"[S5] keepalive reply: {p and hex(p[0])}", flush=True)
    try_reuse(t, "S5")
    t.close()

    print("=== S6: pipelined create (dead + live back-to-back) ===", flush=True)
    t = fresh("S6")
    name_d = DEAD.encode("utf-16-le") + b"\x00\x00"
    name_s = SSH[0].encode("utf-16-le") + b"\x00\x00"
    t0 = time.time()
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", 445, 3,
                                                     len(name_d)) + name_d))
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", SSH[1], 3,
                                                     len(name_s)) + name_s))
    print(f"[S6] two creates pipelined ({time.time()-t0:.1f}s)", flush=True)
    p1 = t.recv_packet(timeout=40)
    d1 = time.time() - t0
    p2 = t.recv_packet(timeout=40)
    d2 = time.time() - t0
    for i, (p, d) in enumerate(((p1, d1), (p2, d2))):
        if p is None:
            print(f"[S6] resp{i+1}: NONE ({d:.1f}s)", flush=True)
        else:
            hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
            print(f"[S6] resp{i+1}: pkt 0x{p[0]:x} hr=0x{hr and hr:08x} "
                  f"({d:.1f}s)", flush=True)
    if p2 and p2[0] == 0x9 and struct.unpack("<I", p2[1][0:4])[0] == 0:
        banner(t, "S6")
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    run()
