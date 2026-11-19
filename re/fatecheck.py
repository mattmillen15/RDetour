#!/usr/bin/env python3
"""When exactly does the gateway kill the TLS legs after a 'lethal' channel?

Watch both raw TCP legs (MSG_PEEK|DONTWAIT) from grant until death/+35s:
  F1 [DC]              control — should survive (denialcheck history)
  F2 [GWIP .47]        gateway by its real IP
  F3 [127.0.0.1]       loopback
  F4 [DEAD, DC]        multi-name, close immediately
  F5 [DEAD, DC]        multi-name, HOLD channel 25s before close
Timeline logs: grant/close/death deltas; alive-confirm at end if no death.
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


def peek_dead(sock):
    """True if TLS EOF (close_notify) or RST observed on the leg.
    Consumes 1 byte if real (TLS-record) traffic arrives instead."""
    r, _, _ = select.select([sock], [], [], 0)
    if not r:
        return False
    try:
        sock.settimeout(0.25)
        b = sock.recv(1)
    except (ConnectionResetError, ssl.SSLError, OSError):
        return True
    except socket.timeout:
        return False
    return b == b""


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def fate(tag, names, hold_s):
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    t._ka_stop.set()   # no keepalive replies during the watch window
    t0 = time.time()
    out, inl = t.out_tls.tls, t.in_tls.tls
    print(f"[{tag}] up @{time.time()-t0:.2f}s", flush=True)

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
    print(f"[{tag}] smb {'ok' if r and r[0] == 0xA else 'FAIL'} "
          f"@{time.time()-t0:.2f}s", flush=True)

    if hold_s:
        print(f"[{tag}] holding channel {hold_s}s...", flush=True)
        tc = None
    else:
        t.close_channel()
        tc = time.time() - t0
        print(f"[{tag}] closed @{tc:.2f}s", flush=True)

    t_dead = None
    while time.time() - t0 < 35:
        if peek_dead(out):
            t_dead = ("OUT", time.time() - t0)
            break
        if peek_dead(inl):
            t_dead = ("IN", time.time() - t0)
            break
        if hold_s and time.time() - t0 > tg + hold_s + 1:
            t.close_channel()
            tc = time.time() - t0
            hold_s = 0
            print(f"[{tag}] closed @{tc:.2f}s (after hold)", flush=True)
        time.sleep(0.1)

    if t_dead:
        leg, td = t_dead
        base = tg if td - tg < 30 else tg  # report vs grant
        print(f">>> [{tag}] {leg} leg died @{td:.2f}s "
              f"(grant+{td-tg:.2f}s"
              + (f", close+{td-tc:.2f}s)" if tc else ")"), flush=True)
    else:
        # confirm alive with a plain SSH cycle
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
            print(f">>> [{tag}] SURVIVED 35s; post-probe create "
                  f"{'ok' if ok else 'DEAD'}", flush=True)
        except (ConnectionError, OSError) as e:
            print(f">>> [{tag}] probe err {e!r}", flush=True)
    t.close()


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    fate("F1-DC-control", [DC], 0)
    fate("F2-GWIP", [GWIP], 0)
    fate("F3-loopback", ["127.0.0.1"], 0)
    fate("F4-dead+dc-close", [DEAD, DC], 0)
    fate("F5-dead+dc-hold25", [DEAD, DC], 25)
