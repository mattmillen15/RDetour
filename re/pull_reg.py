#!/usr/bin/env python3
"""Admin-side research pull #2: registry config for Duo RDG plugin + TSGateway service."""
import os
from impacket.smbconnection import SMBConnection

HOST = "192.168.15.47"
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "ecorp-rds", "registry")

QUERIES = [
    ("HKLM\\SOFTWARE\\Duo Security", "duo-security"),
    ("HKLM\\SOFTWARE\\Microsoft\\Terminal Server Gateway", "tsg"),
    ("HKLM\\SYSTEM\\CurrentControlSet\\Services\\TSGateway", "svc-tsgateway"),
    ("HKLM\\SYSTEM\\CurrentControlSet\\Services\\SessionEnv", "svc-sessionenv"),
    ("HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Terminal Server Gateway", "tsg-nt"),
]

smb = SMBConnection(HOST, HOST, sess_port=445)
smb.login("veeam-admin", "B@ckupP@ssw0rd", "ecorp.local")
os.makedirs(OUT, exist_ok=True)

import base64

def rpc(svc, op):
    from impacket.dcerpc.v5 import transport, rrps
    stringbinding = r"ncacn_np:445[\pipe\winreg]"
    rpctransport = transport.DCERPCTransportFactory(stringbinding)
    rpctransport.set_smb_connection(smb)
    dce = rpctransport.get_dce_rpc()
    dce.connect()
    dce.bind(rrps.MSRPC_UUID_W32REG)
    resp = getattr(rrps, svc)(dce, **op)
    dce.disconnect()
    return resp

for path, name in QUERIES:
    out = [f"### {path}"]
    try:
        resp = rpc("hOpenLocalMachine", {}) if False else None
    except Exception:
        pass
    # simpler: use winreg via reg.exe over wmi? no — use RemoteRegistry-free approach below.
print("use reg.exe via SMB task scheduled? no — switch to impacket wmiexec approach in shell")
smb.logoff()
