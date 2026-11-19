#!/usr/bin/env python3
"""Admin-side research pull: copy Duo/RDG/RDWeb binaries + configs off ECORP-RDS."""
import os
import sys
from impacket.smbconnection import SMBConnection

HOST = "192.168.15.47"
USER = "veeam-admin"
DOMAIN = "ecorp.local"
PW = "B@ckupP@ssw0rd"
OUT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "ecorp-rds")

# (remote path under C$, recurse?, name)
TARGETS = [
    (r"Program Files\Duo Security", True, "duo-security"),
    (r"Windows\System32\tsgateway", True, "tsgateway"),
    (r"Windows\Web\RDWeb\Pages", True, "rdweb-pages"),
    (r"Windows\Web\RDWeb\App_Data", True, "rdweb-appdata"),
    (r"inetpub\history", True, "iis-history"),
    (r"inetpub\temp\appPools", False, "iis-apppools"),
    (r"ProgramData\Duo Security", True, "duo-programdata"),
    (r"Windows\Microsoft.NET\assembly\GAC_MSIL", False, "gac-listing"),
]
# single files
FILES = [
    r"Windows\System32\aahttp.dll",
    r"Windows\System32\aahttpconfig.xml" if False else r"Windows\System32\tsgateway.xml",
    r"inetpub\config\applicationHost.config",
    r"inetpub\config\redirect.config",
    r"inetpub\config\administration.config",
    r"Windows\Microsoft.NET\Framework64\v4.0.30319\Config\machine.config",
    r"cfgrdg.ps1",
    r"config_rdg.ps1",
    r"config_rdg_output.txt",
]


def walk(smb, share, path, local_root, recurse):
    n = 0
    try:
        for f in smb.listPath(share, path + "\\*"):
            name = f.get_longname()
            if name in (".", ".."):
                continue
            remote = path + "\\" + name
            local = os.path.join(local_root, *remote.split("\\"))
            if f.is_directory():
                os.makedirs(local, exist_ok=True)
                if recurse:
                    n += walk(smb, share, remote, local_root, recurse)
            else:
                os.makedirs(os.path.dirname(local), exist_ok=True)
                with open(local, "wb") as fh:
                    smb.getFile(share, remote, fh.write)
                n += 1
                print(f"  {remote} ({f.get_filesize()}b)")
    except Exception as e:
        print(f"  [!] {path}: {e}")
    return n


def main():
    smb = SMBConnection(HOST, HOST, sess_port=445)
    smb.login(USER, PW, DOMAIN)
    share = "C$"
    total = 0
    for path, recurse, dirname in TARGETS:
        print(f"[*] {path} (recurse={recurse})")
        local_root = os.path.join(OUT, dirname)
        os.makedirs(local_root, exist_ok=True)
        if recurse:
            total += walk(smb, share, path, local_root, True)
        else:
            # listing only
            try:
                for f in smb.listPath(share, path + "\\*duo*"):
                    print(f"  [gac] {f.get_longname()}")
            except Exception:
                pass
            try:
                for f in smb.listPath(share, path + "\\*"):
                    print(f"  [ls] {f.get_longname()}")
            except Exception as e:
                print(f"  [!] {e}")
    for f in FILES:
        local = os.path.join(OUT, "files", *f.split("\\"))
        os.makedirs(os.path.dirname(local), exist_ok=True)
        try:
            with open(local, "wb") as fh:
                smb.getFile(share, f, fh.write)
            print(f"  [file] {f}")
            total += 1
        except Exception as e:
            print(f"  [!] {f}: {e}")
    smb.logoff()
    print(f"[*] pulled {total} files into {OUT}")


if __name__ == "__main__":
    main()
