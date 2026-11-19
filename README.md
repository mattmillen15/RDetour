# RDetour

RD Gateway (MS-TSGU HTTP transport) to SOCKS5. Any TCP port the gateway's RAP
allows — not just 3389 — through a local SOCKS5 listener, no RDP client, no
RDP session. Works against Server 2008R2–2025 RD Gateway deployments that
accept NTLM over the HTTP transport (default CAP).

## How it works

SOCKS5 over ONE authorized tunnel (RDG_OUT_DATA + RDG_IN_DATA TLS legs,
SPNEGO-wrapped NTLM with tls-server-end-point channel binding for EPA,
TUNNEL_CREATE/AUTH, then a CHANNEL_CREATE per proxied connection). After a
clean CLOSE_CHANNEL the same tunnel carries the next connection's channel —
so per-auth MFA (e.g. a Duo RD Gateway plugin) is paid once per tunnel
lifetime, not per connection. Connections are serialized (one channel at a
time, wire-verified Server 2019 RDG).

Protocol per [MS-TSGU] HTTP transport, cross-checked against FreeRDP
`libfreerdp/core/gateway/rdg.c`.

## Install

    pip install -r requirements.txt   # pyspnego (required), cryptography (recommended)

Python 3.8+. No other dependencies.

## Usage

SOCKS5 proxy (one approval per tunnel, sequential channels):

    ./rdetour.py -g rdg.corp.com -u CORP\jsmith -p 'pass'
    ./rdetour.py -g rdg.corp.com -u jsmith@corp.com --hashes :NT_HASH
    proxychains -q nxc smb 10.0.0.0/24 -u jsmith -p 'pass'

One-shot check (tunnel + SMB2 negotiate to prove the data plane):

    ./rdetour.py -g rdg.corp.com -u CORP\jsmith -p 'pass' --target 10.0.0.10:445 --probe smb

MFA fatigue (Duo-style push spam until approved; check mode):

    ./rdetour.py -g rdg.corp.com -u CORP\jsmith -p 'pass' --mfa-fatigue \
        --mfa-interval 90 --mfa-jitter 30 --mfa-max 20

Password via environment: `RDETOUR_PASSWORD` (avoids shell history).

## Flags

| Flag | Default | Notes |
|------|---------|-------|
| `-g` / `--gport` | required / 443 | gateway host (FQDN or IP) and port |
| `-u` / `-d` | required / — | `DOMAIN\user`, `user@domain`, or bare user + `-d` |
| `-p` / `--hashes` | — / — | password or `LM:NT` pass-the-hash |
| `--rdg-path` | `/remoteDesktopGateway/` | override only if the IIS vdir was renamed/proxied |
| `--target` / `--probe` | — / smb | one-shot check mode instead of SOCKS |
| `--socks-host` / `--socks-port` | 127.0.0.1 / 1080 | listener bind |
| `--no-gw-fallback` | off | disable the 445 gateway-loopback fallback (with it, every 445 channel is created as `[target,127.0.0.1]` so dead hosts can never deny the create and cost a push; the SMB ServerGUID tells dead from alive) |
| `--timeout` | 15 | TCP/TLS connect timeout |
| `--auth-timeout` | 60 | tunnel-auth approval window — time you have to approve an MFA push per connection |
| `--mfa-fatigue` | off | retry tunnel auth on a cadence; each attempt fires a fresh push |
| `--mfa-interval` / `--mfa-jitter` | 60 / 0 | cadence and randomization |
| `--mfa-max` | 0 (unlimited) | stop after N pushes |
| `--mfa-auth-timeout` | 65 | per-attempt approval window |
| `-v` | off | protocol trace on stderr |

## Failure taxonomy (SOCKS reply + stderr)

| Condition | SOCKS reply | Meaning |
|-----------|-------------|---------|
| leg HTTP != 200 / bad creds | 0x01 | auth failed |
| tunnel-auth denied or timed out | 0x02 | MFA not approved (retryable; use `--mfa-fatigue`) |
| channel denied (RAP) | 0x02 | target host/port not in resource policy |
| channel denied (win32 23005) | 0x02 | gateway cannot resolve/reach the target |
| no channel response in 25s | 0x03 | target likely unreachable from the gateway |
| connect refused/unreachable via gateway | 0x03 | target down or firewalled |

## Notes

- TLS certificates are not validated — gateway certs are commonly internal-CA
  or self-signed. Channel binding uses the presented cert regardless.
- **MFA behavior:** under per-auth MFA (e.g. Duo RD Gateway plugin) a new
  *tunnel* = one fresh approval prompt — one push per tunnel, verified
  against Server 2019 RDG + Duo plugin: no approval cache in the plugin (the
  push fires in the authorization callback per tunnel; channel creation is
  Duo-free), a second CHANNEL_CREATE on a *live* channel is ignored, and the
  channel-response AUTHNCOOKIE is UDP-transport-only.
- **One push → unlimited connections:** after a clean `CLOSE_CHANNEL`
  (reason 0, acked 0x11), the SAME authorized tunnel accepts a new
  `CHANNEL_CREATE` to a different host:port — wire-verified and RE-verified
  against the Duo plugin (`AuthorizeResource` never calls Duo). SOCKS builds
  on this: approve one push, then every proxied connection is a channel on
  the warm tunnel (validated live: 1 push, then SSH/SMB/HTTP channels at 0s
  each with no further prompts). Constraints: one channel at a time
  (connections queue); a denied CHANNEL_CREATE (unreachable target / RAP
  deny) makes the gateway RST the tunnel — the next connection
  re-establishes and re-pays.
- **SMB1 probe answered locally:** impacket/nxc open every 445 connection
  with a single-dialect SMB1 negotiate; SMB1-disabled targets (Server 2019
  default) RST it, which would kill the tunnel (0x800759d8). rdetour
  answers the probe locally with an RST before any channel exists — the
  client reconnects multi-dialect (same as direct) and the probe never
  touches the gateway.
- **Refused ports (the stillborn):** a CHANNEL_CREATE whose first name
  resolves to a REFUSED port (e.g. an SSH-only box on 445) is GRANTED but
  the channel never delivers and never acks a close — the tunnel is spent
  (wire matrix A1-A4). rdetour detects it within 3s of the first write,
  discards the tunnel, and answers that 445 host locally (SOCKS refused)
  from then on. Some refused hosts RST the tunnel outright before the
  stillborn window can classify them (observed live: two hosts, both
  deterministic on first hit) — those cost one tunnel each on first
  contact and cannot be caught client-side; exclude them too. Cost model
  for a /24 sweep with per-auth MFA: dead hosts free (loopback fallback +
  GUID oracle), live hosts free (probe answered locally), each distinct
  refused-445 host costs ONE tunnel — exclude known-refused hosts to hold
  a single push for the whole sweep.
- **Refused-445 kill cache (persistent):** every host classified refused on
  445 is persisted to `~/.rdetour/refused445-<gateway>.json` (one file per
  gateway as typed, `version`/`gateway`/`refused445` host→unix-ts, atomic
  tmp+rename writes) and answered locally (SOCKS refused, no tunnel, no
  channel) on every later connection for TTL 7 days — expired entries are
  dropped and the file rewritten on load; a corrupt file loads as empty
  (one stderr line, run continues). Three feed paths: STILLBORN (granted
  but silent past the 3s window), INSTA-KILL (first-contact RST — create
  granted, client's negotiate forwarded, tunnel dead with ZERO channel
  data; a host that answered anything first is never cached, so the live
  0x800759d8 multi-request abort stays out), and oracle-DEAD (fallback
  response carries the gateway's own ServerGUID — dead and refused are the
  same verdict for an SMB scan, and caching sidesteps the gateway's racy
  per-hit refused→loopback fallback). Seeding pattern for push users: run
  the sweep once with a Duo-exempt recon account (tunnels are free, every
  refused host is classified and written), then the push-user run answers
  those hosts locally — `proxychains nxc smb 192.168.15.0/24` costs exactly
  one push. `RDETOUR_CACHE_DIR` relocates the cache directory (tests only;
  the operator path is fixed). Ctrl-C summary reports
  `instakill_cached=` and `refused445_cache=` sizes.
- **Loopback primaries refused locally:** a SOCKS CONNECT whose target is
  loopback (`127.0.0.0/8`, `::1`, `localhost`) is answered locally with
  SOCKS refused — the gateway denies loopback as a client-requested
  PRIMARY (win32 23005, observed live when a client-stack artifact
  emitted one mid-sweep) and that denial costs the whole tunnel. Loopback
  as the fallback's ALTERNATE name is unaffected (that is the GUID
  oracle).
- **Channel contract (wire-verified, Server 2019 native RDG):** if the
  *target* closes or resets its socket while the channel is open — protocol
  violation, invalid probe bytes, or a second request the server rejects
  (e.g. a 2nd SMB negotiate on one connection gets RST'd by the server) —
  the gateway aborts the channel (`CLOSE_CHANNEL` reason `0x800759d8`) and
  tears down the whole tunnel → next connection re-establishes (re-push for
  push-paying accounts). Safe traffic: one request/response per connection
  (negotiate-style scans — the port-445 gateway-loopback fallback survived
  24/24 parallel probe rounds with zero rebuilds), one long-lived
  well-behaved session (SSH/RDP), or full multi-exchange SMB sessions
  (nxc auth + share enum verified through a warm channel). rdetour
  attributes every abort
  ("gateway ABORTED channel: the target closed/reset") and flags torn-down
  tunnels (`OUT leg dead`) instead of misreporting them as unreachable.
- **MFA bypass recon:** Duo RDG deployments commonly exempt service accounts
  (bypass group / per-user policy). Cheap oracle: time the tunnel-auth —
  <1s with no push on the phone = account is exempt, tunnel freely (in the
  reference lab, `svcaccount` is exempt while `testuser` pushes on every
  tunnel).
- Under **fail-closed** MFA, users with no MFA enrollment are denied outright
  (win32 23003). That is gateway policy (the plugin denies un-enrolled
  identities), not a tool failure — use an enrolled account or a gateway
  without gateway-layer MFA.
- One channel at a time per tunnel (protocol limitation); long scans queue
  on the single warm tunnel — expect gateway event-log volume (200/300/302
  per channel) but only one tunnel-auth (and one MFA prompt) per tunnel
  lifetime.
- Detected-by: RD Gateway events 200/300/302, tunnel churn rate, non-RDP
  payloads on established channels.

## Legal

Authorized testing only. You need valid credentials and permission to reach
the gateway.
