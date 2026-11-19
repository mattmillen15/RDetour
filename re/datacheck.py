#!/usr/bin/env python3
"""Verify the re-channeled tunnel's data plane is REAL: distinct protocols must
return their own banners (SSH vs SMB2), and dead ports must fail at channel
create. Uses the A-variant sequence: one tunnel, close, re-channel."""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, u32, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"


def dump(tag, p):
    if p is None:
        print(f"[{tag}] no reply")
        return None
    print(f"[{tag}] pkt=0x{p[0]:x} body[:48]={p[1][:48].hex()}")
    return p


def channel_to(t, host, port):
    name = host.encode("utf-16-le") + b"\x00\x00"
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", port, 3, len(name)) + name))
    p = t.recv_packet(timeout=25)
    if p is None or p[0] != 0x9:
        print(f"[chan] {host}:{port} -> pkt={p and hex(p[0])} body={p and p[1].hex()}")
        return False
    hr = struct.unpack("<I", p[1][0:4])[0]
    print(f"[chan] {host}:{port} -> CHANNEL_RESPONSE hresult=0x{hr:08x}")
    return hr == 0


def exchange(t, payload, tag, timeout=10):
    t.write(payload)
    p = t.recv_packet(timeout=timeout)
    dump(tag, p)
    return p


def run():
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=True, connect_timeout=10)
    t.establish(auth_timeout=8)
    print("[*] tunnel authorized")

    # 1: SSH banner from NAS (known reachable through this gateway)
    assert channel_to(t, "192.168.15.152", 22)
    p = exchange(t, b"", "ssh-152-wait")  # server greets first
    # 2: close, re-channel to DC SMB
    t.send_packet(pkt(0x10, u32(0)))
    dump("close1", t.recv_packet(timeout=8))
    assert channel_to(t, "192.168.15.40", 445)
    p = exchange(t, smb2_negotiate(), "smb-40")
    # 3: close, re-channel to gateway itself SMB (different host -> different GUID)
    t.send_packet(pkt(0x10, u32(0)))
    dump("close2", t.recv_packet(timeout=8))
    assert channel_to(t, "192.168.15.47", 445)
    p = exchange(t, smb2_negotiate(), "smb-47")
    # 4: close, re-channel to closed port -> expect channel deny
    t.send_packet(pkt(0x10, u32(0)))
    dump("close3", t.recv_packet(timeout=8))
    channel_to(t, "192.168.15.47", 9999)
    # 5: re-channel back to SSH after all that — still alive?
    channel_to(t, "192.168.15.152", 22)
    p = exchange(t, b"", "ssh-152-again")
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(15)
    run()
