#!/usr/bin/env python3
"""Session-5 Q1: how long does ONE approved tunnel actually live? (lab, svcaccount)

Two parallel tunnels from one process:
  A  keepalive(30s, automatic) + channel cycle every 120s   — "active use"
  B  keepalive only; single probe channel every 600s        — "idle + KA"
Cycle = open_channel(SSH .152:22) -> banner -> close_channel(0) -> 0x11.
Logs to re/lifetime.log; stops at first death or 65 min.
"""
import socket
import struct
import sys
import threading
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel  # noqa: E402

GW = "ecorp-rds.ecorp.local"
SSH = ("192.168.15.152", 22)
MAX_S = 65 * 60
LOG = "/home/kali/workspace/RDetour/re/lifetime.log"

f = open(LOG, "a", buffering=1)


def ts():
    return time.strftime("%H:%M:%S")


def cycle(t, tag):
    """One channel open->banner->close round trip. True = healthy."""
    t.open_channel(*SSH)
    t0 = time.time()
    buf = b""
    while time.time() - t0 < 8:
        d = t.read(timeout=2)
        if d is None:
            break
        buf += d
        if b"SSH" in buf[:8]:
            break
    ok = b"SSH" in buf[:8]
    t.close_channel()
    f.write(f"[{ts()}] {tag} cycle ok={ok} banner={buf[:20]!r}\n")
    return ok


def run(tag, cycle_every_s, duration_s):
    try:
        t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                      domain="ECORP.local", verbose=False, connect_timeout=10)
        t.establish(auth_timeout=8)
    except Exception as e:
        f.write(f"[{ts()}] {tag} ESTABLISH FAILED {e!r}\n")
        return
    t0 = time.time()
    f.write(f"[{ts()}] {tag} tunnel up (cycle every {cycle_every_s}s)\n")
    n = 0
    while time.time() - t0 < duration_s:
        time.sleep(cycle_every_s)
        if t.closed:
            f.write(f"[{ts()}] {tag} DEAD flag after {time.time()-t0:.0f}s "
                    f"({n} cycles)\n")
            return
        n += 1
        try:
            if not cycle(t, f"{tag}#{n}"):
                f.write(f"[{ts()}] {tag} cycle {n} failed after "
                        f"{time.time()-t0:.0f}s\n")
                return
        except Exception as e:
            f.write(f"[{ts()}] {tag} DIED after {time.time()-t0:.0f}s "
                    f"at cycle {n}: {e!r}\n")
            return
    f.write(f"[{ts()}] {tag} SURVIVED {duration_s}s, {n} cycles — closing\n")
    try:
        t.close()
    except Exception:
        pass


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    f.write(f"\n[{ts()}] === lifetime probe start ===\n")
    ta = threading.Thread(target=run, args=("A/120s", 120, MAX_S))
    tb = threading.Thread(target=run, args=("B/600s", 600, MAX_S))
    ta.start()
    tb.start()
    ta.join()
    tb.join()
    f.write(f"[{ts()}] === lifetime probe done ===\n")
