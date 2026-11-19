#!/usr/bin/env python3
"""Isolate: same channel cycle as datacheck but with a reader THREAD like the
warm SOCKS server uses (reader polls t.read(1)), to find why channel #2's
data response never reaches the client."""
import socket
import struct
import sys
import threading
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel, smb2_negotiate  # noqa: E402

GW = "ecorp-rds.ecorp.local"

t = RDGTunnel(GW, user="svcaccount", password="svcaccount",
              domain="ECORP.local", verbose=True, connect_timeout=10)
t.establish(auth_timeout=8)

# --- cycle 1: SSH banner via reader thread ---
t.open_channel("192.168.15.152", 22)
got1 = []


def rd1():
    got1.append(t.read(timeout=10))


th = threading.Thread(target=rd1)
th.start()
time.sleep(2)
th.join()
print("cycle1 banner:", got1)

# quiesce + close like warm server does
t.channel_open = False
t._close_channel_xchg()
print("closed1 ok")

# --- cycle 2: SMB via reader thread ---
t.open_channel("192.168.15.40", 445)
got2 = []


def rd2():
    got2.append(t.read(timeout=10))


th2 = threading.Thread(target=rd2)
th2.start()
time.sleep(0.3)
t.write(smb2_negotiate())
print("negotiate written")
th2.join()
print("cycle2 resp:", got2 and (got2[0] and got2[0][:16].hex()))

# --- cycle 3: same but WITHOUT the 0.3s delay after starting reader ---
t.channel_open = False
t._close_channel_xchg()
t.open_channel("192.168.15.152", 22)
got3 = []


def rd3():
    got3.append(t.read(timeout=10))


th3 = threading.Thread(target=rd3)
th3.start()
print("cycle3 banner:", end=" ")
th3.join()
print(got3)
t.close()
