#!/usr/bin/env python3
"""Live client for the warm+fallback validation: SOCKS5 -> SMB negotiate ->
report whose GUID answered (target vs gateway) or EOF (dead, cut locally)."""
import socket
import struct
import sys

sys.path.insert(0, "/home/kali/workspace/RDetour")
from rdetour import smb2_negotiate, smb2_server_guid  # noqa: E402


def probe(host, port, socks_port=11997):
    c = socket.create_connection(("127.0.0.1", socks_port), timeout=15)
    c.sendall(b"\x05\x01\x00")
    assert c.recv(2) == b"\x05\x00"
    c.sendall(b"\x05\x01\x00\x01" + socket.inet_aton(host) +
              struct.pack(">H", port))
    r = c.recv(10)
    assert r[1] == 0x00, "socks reply %s" % r.hex()
    c.sendall(smb2_negotiate())
    c.settimeout(20)
    buf = b""
    try:
        while len(buf) < 132:
            d = c.recv(4096)
            if not d:
                break
            buf += d
            g = smb2_server_guid(buf)
            if g is not None:
                break
    except ConnectionResetError:
        print(f"{host}:{port} -> RESET (cut)")
        c.close()
        return None
    c.close()
    if not buf:
        print(f"{host}:{port} -> EOF (no smb answer — classified DEAD)")
        return None
    g = smb2_server_guid(buf)
    print(f"{host}:{port} -> answered, guid={g and g.hex()}")
    return g


if __name__ == "__main__":
    probe(sys.argv[1], int(sys.argv[2]))
