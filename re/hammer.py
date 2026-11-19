#!/usr/bin/env python3
"""Hammer the warm SOCKS with nxc-style PARALLEL connects to find the
silent-create (25s no response -> needless tunnel rebuild) trigger.
Rounds of 4 concurrent SOCKS connects (mix dead/live), SMB negotiate on
each, like netexec's thread pool."""
import socket
import struct
import sys
import threading
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import smb2_negotiate, smb2_server_guid  # noqa: E402

HOSTS = ["192.168.15.99", "192.168.15.40", "192.168.15.98",
         "192.168.15.97"]
PORT = 11997
results = []


def one(host, rid, idx):
    t0 = time.monotonic()
    try:
        c = socket.create_connection(("127.0.0.1", PORT), timeout=40)
        c.sendall(b"\x05\x01\x00")
        c.recv(2)
        c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton(host) +
                  struct.pack(">H", 445))
        r = c.recv(10)
        if r[1] != 0x00:
            results.append((rid, idx, host, "socks-fail %s" % r.hex(),
                            time.monotonic() - t0))
            c.close()
            return
        c.sendall(smb2_negotiate())
        c.settimeout(35)
        buf = b""
        while len(buf) < 92:
            d = c.recv(4096)
            if not d:
                break
            buf += d
            if smb2_server_guid(buf) is not None:
                break
        c.close()
        g = smb2_server_guid(buf)
        verdict = ("dead-cut" if not buf else
                   "live" if g else "answered-noguid")
        results.append((rid, idx, host, verdict, time.monotonic() - t0))
    except Exception as e:
        results.append((rid, idx, host, "ERR %r" % e,
                        time.monotonic() - t0))


def main():
    for rid in range(6):
        ths = [threading.Thread(target=one, args=(h, rid, i))
               for i, h in enumerate(HOSTS)]
        t0 = time.monotonic()
        for t in ths:
            t.start()
        for t in ths:
            t.join()
        print(f"--- round {rid} took {time.monotonic()-t0:.1f}s", flush=True)
        for rid_, idx, host, verdict, dt in sorted(
                [r for r in results if r[0] == rid], key=lambda x: x[4]):
            print(f"  {host:>15} {verdict:<28} {dt:6.1f}s", flush=True)
        time.sleep(1)


if __name__ == "__main__":
    main()
