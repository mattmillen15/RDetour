#!/usr/bin/env python3
"""repro14 — fate tests with CORRECT channel lifecycle.

Bug in all prior fate probes: raw create never set t.channel_open, so
close_channel() no-op'd (channel stayed LIVE) and the probe create hit
the known '2nd create with live channel -> RST' rule. All 'tunnel died'
readings were that artifact.

Now: grant -> channel_open=True -> smb -> real close (0x10/0x11) ->
+2s probe create (also lifecycle-correct) -> banner -> close -> 3rd create.
  G1 [DC] control | G2 [127.0.0.1] | G3 [DEAD,DC] | G4 [DC,DC]
  G5 [DC] cycle then 40s idle then create (post-reap window)
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
SSH = "192.168.15.152"


def names_body(names, port):
    body = struct.pack("<BB", len(names), 0) + struct.pack("<HH", port, 3)
    for h in names:
        n = h.encode("utf-16-le") + b"\x00\x00"
        body += struct.pack("<H", len(n)) + n
    return body


def mcreate(t, names, port, tag, timeout=40):
    t.send_packet(pkt(0x8, names_body(names, port)))
    t0 = time.time()
    p = t.recv_packet(timeout=timeout)
    dt = time.time() - t0
    if p is None:
        print(f"  [{tag}] create -> None ({dt:.1f}s)", flush=True)
        return None
    hr = struct.unpack("<I", p[1][0:4])[0] if p[0] == 0x9 else None
    print(f"  [{tag}] create hr={hr and '0x%08x' % hr} ({dt:.1f}s)",
          flush=True)
    if hr == 0:
        t.channel_open = True      # make close_channel() actually close
        t._srv_closed_channel = False
    return hr


def banner(t, tag):
    t0 = time.time()
    buf = b""
    while time.time() - t0 < 6:
        d = t.read(timeout=2)
        if d is None:
            break
        buf += d
        if b"SSH" in buf[:8]:
            break
    ok = b"SSH" in buf[:8]
    print(f"  [{tag}] banner {'ok' if ok else buf[:16]!r}", flush=True)
    return ok


def smb(t, tag):
    t.write(smb2_negotiate())
    r = t.recv_packet(timeout=8)
    ok = r and r[0] == 0xA and len(r[1]) > 4
    print(f"  [{tag}] smb {'ok' if ok else 'FAIL'}", flush=True)
    return bool(ok)


def run(tag, names, idle_after=0):
    print(f"=== {tag} names={names} idle={idle_after} ===", flush=True)
    t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
                  domain="ECORP.local", verbose=False, connect_timeout=10)
    t.establish(auth_timeout=8)
    t0 = time.time()
    if mcreate(t, names, 445, f"{tag}/c1") != 0:
        t.close()
        return False
    smb(t, tag)
    t.close_channel()
    print(f"  [{tag}] closed @{time.time()-t0:.2f}s", flush=True)
    if idle_after:
        time.sleep(idle_after)
        print(f"  [{tag}] idled {idle_after}s", flush=True)
    # probe cycle 1: SSH
    if mcreate(t, [SSH], 22, f"{tag}/c2") != 0:
        print(f">>> {tag}: DEAD at 2nd create", flush=True)
        t.close()
        return False
    banner(t, tag)
    t.close_channel()
    # probe cycle 2: DC again (3rd create)
    if mcreate(t, [DC], 445, f"{tag}/c3") != 0:
        print(f">>> {tag}: DEAD at 3rd create (survived 2nd)", flush=True)
        t.close()
        return False
    smb(t, tag)
    t.close_channel()
    print(f">>> {tag}: TUNNEL FULLY HEALTHY (3 cycles)", flush=True)
    t.close()
    return True


if __name__ == "__main__":
    socket.setdefaulttimeout(20)
    run("G1-DC", [DC])
    run("G2-loopback", ["127.0.0.1"])
    run("G3-dead+dc", [DEAD, DC])
    run("G4-dcx2", [DC, DC])
    run("G5-dc-idle40", [DC], idle_after=40)
