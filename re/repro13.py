#!/usr/bin/env python3
"""Reproduce altcheck-S13's instant post-close death 3x with event log.
Every send/recv logged with time + exception errno to catch the killer."""
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
SSH = "192.168.15.152"


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def ev(t0, msg):
    print(f"  +{time.time()-t0:6.2f}s {msg}", flush=True)


def leg_state(tls):
    r, _, _ = select.select([tls], [], [], 0)
    if not r:
        return "idle"
    try:
        tls.settimeout(0.2)
        b = tls.recv(1)
        return "EOF" if b == b"" else f"DATA({b.hex()})"
    except ConnectionResetError:
        return "RST"
    except (socket.timeout, ssl.SSLError, OSError) as e:
        return f"ERR({e!r:.60})"


def attempt(n, names=None, hold=0):
    print(f"=== attempt {n} names={names} hold={hold} ===", flush=True)
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    t._ka_stop.set()
    t0 = time.time()
    ev(t0, "tunnel up")

    if hold:
        time.sleep(hold)  # hold BEFORE create? no: hold AFTER grant
    t.send_packet(pkt(0x8, names_body(names or [DEAD, DC], 445)))
    p = t.recv_packet(timeout=40)
    hr = struct.unpack("<I", p[1][0:4])[0] if p and p[0] == 0x9 else None
    ev(t0, f"create1 hr={hr and hex(hr)}")
    if hr != 0:
        t.close()
        return
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    ev(t0, f"smb {'ok' if r and r[0] == 0xA else 'FAIL'}")
    if hold:
        ev(t0, f"holding {hold}s with channel open")
        time.sleep(hold)
        ev(t0, "hold done")
    t.close_channel()
    ev(t0, "closed (0x11 ok)")

    for step in range(3):
        time.sleep(2)
        ev(t0, f"legs: out={leg_state(t.out_tls.tls)} "
               f"in={leg_state(t.in_tls.tls)}")
        try:
            t.send_packet(pkt(0x8, names_body([SSH], 22)))
            ev(t0, f"probe create sent (step {step})")
        except (ConnectionError, OSError, ssl.SSLError) as e:
            ev(t0, f"SEND FAILED: {e!r}")
            break
        ts = time.time()
        p = t.recv_packet(timeout=5)
        ev(t0, f"probe resp: {p and hex(p[0]) or 'None'} "
               f"({time.time()-ts:.1f}s)")
        if p is None:
            ev(t0, f"legs after None: out={leg_state(t.out_tls.tls)} "
                   f"in={leg_state(t.in_tls.tls)}")
            break
        if p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0:
            ev(t0, "probe GRANTED — tunnel good")
            t.close_channel()
            break
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    cases = [("G1-DC", [DC], 0), ("G2-loopback", ["127.0.0.1"], 0),
             ("G3-hold25", [DEAD, DC], 25), ("G4-DCx2", [DC, DC], 0)]
    for nm, names, hold in cases:
        try:
            attempt(nm, names, hold)
        except Exception as e:
            print(f"  {nm} aborted: {e!r}", flush=True)
