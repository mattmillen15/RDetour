#!/usr/bin/env python3
"""Warm server with a full tunnel-packet tap (every send/recv packet logged
with direction+timestamp) to catch the instant-EOF create killer."""
import sys
import threading
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
import rdetour  # noqa: E402

TAP = open("/tmp/rdg_tap.log", "a", buffering=1)
_lock = threading.Lock()


def ts():
    return time.strftime("%H:%M:%S") + ".%03d" % int((time.time() % 1) * 1000)


orig_send = rdetour.RDGTunnel.send_packet
orig_recv = rdetour.RDGTunnel.recv_packet


def tap_send(self, raw):
    pt = struct_pt = int.from_bytes(raw[0:2], "little") if len(raw) >= 2 else -1
    with _lock:
        TAP.write(f"{ts()} TX 0x{pt:02x} len={len(raw)}\n")
    return orig_send(self, raw)


def tap_recv(self, timeout=15):
    t0 = time.monotonic()
    p = orig_recv(self, timeout)
    dt = time.monotonic() - t0
    with _lock:
        if p is None:
            TAP.write(f"{ts()} RX None after {dt:.2f}s\n")
        else:
            TAP.write(f"{ts()} RX 0x{p[0]:02x} len={8+len(p[1])} "
                      f"body={p[1][:8].hex()} ({dt:.2f}s)\n")
    return p


rdetour.RDGTunnel.send_packet = tap_send
rdetour.RDGTunnel.recv_packet = tap_recv

sys.argv = ["rdetour", "-g", "ecorp-rds.ecorp.local",
            "-u", "ECORP.local\\svcaccount", "-p", "svcaccount",
            "--warm", "--socks-port", "11997"]
rdetour.main(sys.argv[1:])
