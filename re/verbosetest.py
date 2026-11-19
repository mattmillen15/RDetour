#!/usr/bin/env python3
"""Isolate the post-close tunnel death: is it the manual 0xD keepalive
burst, or does a 127.0.0.1 / multi-name channel genuinely kill the tunnel?

V1  127.0.0.1 channel -> smb -> close -> 2s -> plain create SSH (no 0xD)
V2  same, but send lone 0xD, WAIT 1s, then create (not back-to-back)
V3  same as V1 with dead+live multi-name list
Each variant prints packet-level logs (verbose) incl. close acks.
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
DC = "192.168.15.40"
SSH = "192.168.15.152"


def fresh(tag):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=True, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{tag}] === tunnel up ===", flush=True)
    return t


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def create(t, names, port, tag, timeout=30):
    t.send_packet(pkt(0x8, names_body(names, port)))
    p = t.recv_packet(timeout=timeout)
    if p is None:
        print(f"[{tag}] create -> DEAD/None", flush=True)
        return None
    hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
    print(f"[{tag}] create -> 0x{p[0]:x} hr={hr and '0x%08x' % hr}", flush=True)
    return p


def banner(t, tag):
    try:
        t0 = time.time()
        buf = b""
        while time.time() - t0 < 6:
            d = t.read(timeout=2)
            if d is None:
                break
            buf += d
            if b"SSH" in buf[:8]:
                break
        print(f"[{tag}] banner {buf[:20]!r}", flush=True)
        return b"SSH" in buf[:8]
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] banner err {e!r}", flush=True)
        return False


def smb(t, tag):
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    ok = r and r[0] == 0xA and len(r[1]) > 4
    print(f"[{tag}] smb {'ok' if ok else 'FAIL'}", flush=True)
    return bool(ok)


def variant(tag, first_names, keepalive_mode):
    t = fresh(tag)
    p = create(t, first_names, 445, tag + "/c1")
    if not (p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0):
        t.close()
        return
    smb(t, tag)
    t.close_channel()
    time.sleep(2)
    if keepalive_mode == "burst":
        t.send_packet(pkt(0xD))          # immediately followed by create
    elif keepalive_mode == "wait":
        t.send_packet(pkt(0xD))
        time.sleep(1)
    p2 = create(t, [SSH] if keepalive_mode != "none" else [SSH], 22,
                tag + "/c2")
    if p2 and p2[0] == 0x9 and struct.unpack("<I", p2[1][0:4])[0] == 0:
        banner(t, tag + "/c2")
        t.close_channel()
        p3 = create(t, [DC], 445, tag + "/c3")
        if p3 and p3[0] == 0x9:
            smb(t, tag + "/c3")
            t.close_channel()
            print(f">>> {tag}: TUNNEL SURVIVED MULTIPLE CYCLES", flush=True)
    else:
        print(f">>> {tag}: tunnel dead at second create "
              f"(mode={keepalive_mode})", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    variant("V1-loopback-noKA", ["127.0.0.1"], "none")
    variant("V2-loopback-KAwait", ["127.0.0.1"], "wait")
    variant("V3-multiname-noKA", [DEAD, DC], "none")
