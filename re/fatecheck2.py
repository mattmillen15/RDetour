#!/usr/bin/env python3
"""fatecheck v2 — framing-safe watcher.

Watch = recv_packet loop (parses WHOLE packets, keeps TLS framing intact).
Every unsolicited packet is logged with type+len+time. Death = select
readable on a leg with no parseable packet + EOF/RST on raw recv.

  F1 [DC] control | F2 [.47] | F3 [127.0.0.1] | F4 [DEAD,DC] close-now
  F5 [DEAD,DC] hold 25s then close
"""
import select
import socket
import ssl
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
DC = "192.168.15.40"
GWIP = "192.168.15.47"
SSH = "192.168.15.152"


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def leg_dead(tls):
    r, _, _ = select.select([tls], [], [], 0)
    if not r:
        return False
    try:
        tls.settimeout(0.25)
        b = tls.recv(1)
    except (ConnectionResetError, ssl.SSLError, OSError):
        return True
    except socket.timeout:
        return False
    return b == b""


def fate(tag, names, hold_s):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    t._ka_stop.set()
    t0 = time.time()
    print(f"[{tag}] up", flush=True)

    t.send_packet(pkt(0x8, names_body(names, 445)))
    p = t.recv_packet(timeout=40)
    tg = time.time() - t0
    hr = struct.unpack("<I", p[1][0:4])[0] if p and p[0] == 0x9 else None
    print(f"[{tag}] create hr={hr and '0x%08x' % hr} @{tg:.2f}s", flush=True)
    if hr != 0:
        t.close()
        return
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    print(f"[{tag}] smb {'ok' if r and r[0] == 0xA else 'FAIL'}", flush=True)

    tc = None
    if hold_s:
        print(f"[{tag}] holding {hold_s}s", flush=True)
    else:
        t.close_channel()
        tc = time.time() - t0
        print(f"[{tag}] closed @{tc:.2f}s", flush=True)

    t_dead = None
    while time.time() - t0 < 35:
        if hold_s and time.time() - t0 > tg + hold_s + 1:
            t.close_channel()
            tc = time.time() - t0
            hold_s = 0
            print(f"[{tag}] closed @{tc:.2f}s (after hold)", flush=True)
        p = t.recv_packet(timeout=0.3)
        if p is not None:
            print(f"[{tag}] unsolicited pkt 0x{p[0]:x} len={len(p[1])} "
                  f"@{time.time()-t0:.2f}s "
                  f"body={p[1][:16].hex()}", flush=True)
            if p[0] in (0x10, 0x11, 0x3, 0x4):
                print(f"[{tag}] ^ close/teardown-family packet", flush=True)
            continue
        # None: idle or dead?
        if leg_dead(t.out_tls.tls):
            t_dead = ("OUT", time.time() - t0)
            break
        if leg_dead(t.in_tls.tls):
            t_dead = ("IN", time.time() - t0)
            break

    if t_dead:
        leg, td = t_dead
        s = f">>> [{tag}] {leg} leg died @{td:.2f}s (grant+{td-tg:.2f}s"
        if tc:
            s += f", close+{td-tc:.2f}s"
        print(s + ")", flush=True)
        t.close()
        return
    # alive: final create probe (skip 0xD)
    try:
        t.send_packet(pkt(0x8, names_body([SSH], 22)))
        p = None
        for _ in range(4):
            p = t.recv_packet(timeout=8)
            if p is None or p[0] != 0xD:
                break
        ok = p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0
        if ok:
            t.close_channel()
        print(f">>> [{tag}] legs alive 35s; probe create "
              f"{'OK — TUNNEL GOOD' if ok else 'no response (ignored?)'}",
              flush=True)
    except (ConnectionError, OSError, ssl.SSLError) as e:
        print(f">>> [{tag}] probe died: {e!r}", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    fate("F1-DC-control", [DC], 0)
    fate("F2-GWIP", [GWIP], 0)
    fate("F3-loopback", ["127.0.0.1"], 0)
    fate("F4-dead+dc-close", [DEAD, DC], 0)
    fate("F5-dead+dc-hold25", [DEAD, DC], 25)
