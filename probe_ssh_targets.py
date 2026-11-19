#!/usr/bin/env python3
"""RAP reachability probe: one svcaccount (Duo-exempt) tunnel per target,
read the first bytes off the channel. SSH servers banner-first, so a
readable 'SSH-2.0-...' proves full data-plane through the gateway."""
import sys
sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import RDGTunnel

TARGETS = ["192.168.15.150:22", "192.168.15.5:22", "192.168.15.6:22",
           "192.168.15.90:22", "192.168.15.45:22", "192.168.15.152:22"]

for tgt in TARGETS:
    host, port = tgt.rsplit(":", 1)
    t = RDGTunnel(gateway="ECORP-RDS.ecorp.local", domain="ECORP.local",
                  user="svcaccount", password="svcaccount", verbose=False)
    try:
        t.establish(auth_timeout=15)
        t.open_channel(host, int(port))
        t.write(b"")  # no-op; SSH banner arrives unsolicited
        data = t.read(timeout=5)
        print("%-22s OK via RAP — banner: %r" % (tgt, (data or b"")[:40]))
    except Exception as e:
        print("%-22s FAIL — %s" % (tgt, str(e)[:90]))
    finally:
        t.close()
