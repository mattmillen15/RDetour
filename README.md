# RDetour

Turn an RD Gateway into a general-purpose SOCKS5 proxy into its internal network.
One MFA approval, then unlimited proxied connections — `proxychains nxc smb
10.0.0.0/24` through the gateway with no RDP client and no RDP session.

## What this is

RDetour exploits a **Resource Authorization Policy (RAP) misconfiguration**, not
a vulnerability in the RD Gateway software itself. When a gateway's RAP allows
connections to hosts and ports beyond what's needed for RDP (3389), any
authenticated user can tunnel arbitrary TCP through it. This is by design in the
MS-TSGU protocol: after tunnel authentication, each CHANNEL_CREATE is checked
only against the RAP — no additional MFA, no per-channel auth. RDetour turns
that into a SOCKS5 listener.

**This is not a CVE.** The gateway software works as intended. The issue is the
RAP being configured too broadly — allowing `*` hosts, all ports, or wide
subnets instead of locking channels to specific RDP targets on 3389.

### Why it matters

- One MFA push (Duo, etc.) → tunnel lives indefinitely (keepalive every 30s)
- Every subsequent connection is a free channel on the warm tunnel
- SMB, Kerberos, LDAP, HTTP — any TCP port the RAP allows
- Full subnet scans through one approval: `proxychains nxc smb 10.0.0.0/24`

## How it works

MS-TSGU HTTP transport: two TLS legs (RDG_OUT_DATA + RDG_IN_DATA) with
SPNEGO-wrapped NTLM and tls-server-end-point channel binding (EPA). The tunnel
is authenticated once (TUNNEL_CREATE + TUNNEL_AUTH — this is where MFA fires),
then carries sequential CHANNEL_CREATE/CLOSE_CHANNEL cycles to different
host:port targets. Each SOCKS5 CONNECT becomes one channel on the warm tunnel.

Wire-verified on Server 2019 RDG:
- CHANNEL_CREATE is RAP-checked but never MFA-checked (Duo's
  `AuthorizeResource` callback does not call Duo)
- After CLOSE_CHANNEL(reason=0) acked with 0x11, the same tunnel accepts a new
  CHANNEL_CREATE to a different target
- One channel at a time (connections are serialized)
- Keepalive (packet type 0xD every 30s) keeps the tunnel alive indefinitely

### Tunnel survival mechanisms

**SMB1 probe interception:** impacket/nxc opens every 445 connection with a
single-dialect SMB1 negotiate. SMB1-disabled servers (Server 2019 default) RST
it, which kills the tunnel. RDetour answers the probe locally — the client
reconnects multi-dialect and proceeds without touching the gateway.

**First-contact gate:** For hosts not yet proven live, SMB2 commands beyond
NEGOTIATE (cmd >= 2) are dropped. This prevents TREE_CONNECT from reaching
hosts that would RST and kill the tunnel.

**Gateway-loopback fallback:** Port 445 channels are created as
`[target, 127.0.0.1]` — the gateway's pick-live behavior means the create can
never be denied (the gateway's own SMB always answers). The SMB ServerGUID in
the response distinguishes dead hosts (gateway loopback) from live ones.

**Blackhole drain:** Hosts that accept TCP but never respond to SMB get a
30-second drain window matching the Windows TCP retransmission timeout, so the
tunnel survives.

**Kill cache:** Hosts classified as tunnel-killers (stillborn channels, RST
aborts, blackhole hosts, gateway-loopback dead) are persisted to
`~/.rdetour/killcache-<gateway>.json` (TTL 7 days) and answered locally on
future connections — no tunnel, no channel, no cost.

**Scout tunnel:** Unknown host:port pairs are probed on a disposable scout
tunnel. If the probe survives, the host is promoted to known_good and served on
the warm tunnel for all ports.

## Install

    pip install -r requirements.txt   # pyspnego (required), cryptography (recommended)

Python 3.8+. No other dependencies.

## Usage

SOCKS5 proxy (one approval, sequential channels):

    ./rdetour.py -g rdg.corp.com -u CORP\\jsmith -p 'pass'
    proxychains -q nxc smb 10.0.0.0/24 -u jsmith -p 'pass'

One-shot check (tunnel + SMB2 negotiate to prove the data plane):

    ./rdetour.py -g rdg.corp.com -u CORP\\jsmith -p 'pass' --target 10.0.0.10:445 --probe smb

Bridge mode (one tunnel, one channel, bridged to a local port for SSH mux):

    ./rdetour.py -g rdg.corp.com -u CORP\\jsmith -p 'pass' --target 10.0.0.5:22 --bridge 2222
    ssh -D 1080 -p 2222 user@127.0.0.1

MFA fatigue (push loop until approved):

    ./rdetour.py -g rdg.corp.com -u CORP\\jsmith -p 'pass' --mfa-fatigue \
        --mfa-interval 90 --mfa-jitter 30 --mfa-max 20

Password via environment: `RDETOUR_PASSWORD` (avoids shell history).

## Flags

| Flag | Default | Notes |
|------|---------|-------|
| `-g` / `--gport` | required / 443 | Gateway host and port |
| `-u` / `-d` / `-p` | required | Credentials: `DOMAIN\user` or `user@domain`, password or `--hashes LM:NT` |
| `--target` / `--probe` | — / smb | One-shot check mode |
| `--bridge PORT` | — | Single-tunnel bridge to local port |
| `--socks-host` / `--socks-port` | 127.0.0.1 / 1080 | Listener bind |
| `--timeout` | 15 | TCP/TLS connect timeout |
| `--auth-timeout` | 60 | Tunnel-auth MFA approval window |
| `--mfa-fatigue` | off | Push loop until approved |
| `--mfa-interval` / `--mfa-jitter` | 60 / 0 | Push cadence |
| `--mfa-max` / `--mfa-auth-timeout` | 0 / 65 | Push limit and per-attempt window |
| `--no-gw-fallback` | off | Disable 445 gateway-loopback fallback |
| `--no-scout` | off | Disable scout tunnel (single-tunnel mode) |
| `--no-session-reuse` | off | Disable cookie/session reuse across rebuilds |
| `--rdg-path` | `/remoteDesktopGateway/` | IIS virtual directory override |
| `--ext-auth` | none | `sspi_ntlm` or `paa` for non-HTTP auth modes |
| `--client-name` | hostname | Tunnel client name |
| `-v` | off | Protocol trace on stderr |

## Detecting this

- RD Gateway event log: events 200/300/302 per channel, non-RDP payloads
- Tunnel churn rate and channel cycling patterns
- Channels to non-3389 ports (SMB, Kerberos, LDAP)
- Anomalous tunnel lifetime (hours/days with keepalive)

## Remediation

Lock down the RAP:
- Restrict allowed ports to 3389 only
- Restrict allowed hosts to specific RDP targets, not `*` or broad subnets
- Audit RAP regularly — the default "allow connections to any host" is the root cause

## Legal

Authorized testing only. You need valid credentials and permission to reach
the gateway.
