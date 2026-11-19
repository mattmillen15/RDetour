# Duo RD Gateway Plugin (DuoTsgZ) — Authorization/Session Logic RE Report

Target: `DuoTsgPlugIn.dll` (Server 2019 RDG rig, CLSID {54BBA037-EADE-4459-B5CD-90D2670F790D}, "DuoTsgZ")
File: `/home/kali/workspace/RDetour/re/ecorp-rds/duo-security/Program Files/Duo Security/DuoTsg/DuoTsgPlugIn.dll`
SHA256: see host. Size 750,936. PE32+ x64, 7 sections, native C++ (ATL/COM + static CRT + jsoncpp), version string `DuoTsg/3.0.0.0`.
Tooling: radare2 6.0.5 (project `/tmp/duowork/duoplug`), strings, objdump. All RVAs below are `imagebase 0x180000000 + RVA` (e.g. AuthorizeConnection RVA 0x20610).

RTTI classes present: `CDuoRdAuthenticationPlugIn` (DuoTsgA), `CDuoRdAuthorizationPlugIn` (DuoTsgZ), `CDuoAuthSession`, `CDuoClientRdsso`, `CConfigurationManager`, `CDuoTsgPlugInModule`.

## 0. Interface map (ITSGPolicyEngine vtable on CComContainedObject<CDuoRdAuthorizationPlugIn>)

| vtable slot | RVA | Method (log-string confirmed) |
|---|---|---|
| +0x18 | 0x180020610 | `AuthorizeConnection` ("CDuoRdAuthorizationPlugIn::AuthorizeConnection: Called on session %1!s! ... cookie (%7!d! bytes): ><%8!s!<>") |
| +0x20 | 0x180021710 | `AuthorizeResource` |
| +0x28 | 0x180022870 | `IsQuarantineEnabled` (logs "returning TRUE") |
| +0x30 | 0x1800226e0 | `Refresh` |
| 2nd iface | 0x1800229b0 | `DoAccounting` (AA_MAIN_SESSION_CREATION / SUB_SESSION_* ) |

DuoTsgA (inactive on this rig): `AuthenticateUser` 0x180012580, `CancelAuthentication` 0x180013140.

## Q1. Session/cache state

Three distinct state mechanisms exist. Only #3 is active on this rig.

### 1a. NO username->authorized cache (the direct answer)
`AuthorizeConnection` (0x180020610) contains no per-user allow-cache:
- It converts `clientParameters` (GUID struct, ctx offsets 0x20=userName, 0x28=client IP; 0x30=cookie/opaque, 0x38=ITSGAuthorizeConnectionSink, 0x40=EnableDuo flag, 0x48=bRAPonly flag) into heap copies, runs `CleanupOldPushEntries`, logs the entry log line, then (0x18002144d) `QueueUserWorkItem(worker 0x1800292e0, ctx, WT_EXECUTEDEFAULT)`.
- Worker 0x1800292e0: optional UPN translation -> logs "Username being sent to Duo: <u>" -> calls `CDuoAuthSession::virtual_16` (0x18003c790, `AuthorizeDuoUser`) -> `PreAuthorizeDuoUser` (0x1800427e0, preauth) -> on `result=="auth"` the async push path (0x18003c8c0) -> on success logs "Successful Duo login for '<u>'" and completes sink via 0x180026f70 ("Authorizing session connection").
- No lookup of any user-keyed map occurs before the Duo call; no success is recorded for later reuse. Every connection performs a fresh preauth+auth.

### 1b. SSO ticket cache (present in code, INERT on this rig)
Strings/xrefs show a full Duo SSO cookie/ticket system:
- Cookie XML parsed at 0x180018ff0 (`/SsoCred/tkt`, `/SsoCred/vsUrl`, `/SsoCred/n`, `/SsoCred/hmac`).
- Verify request built by `CDuoClientRdsso` (fcn.18000b010 / 0x1800105f0): JSON fields `ticketToCheck, GetTicketUserInfo, checkUserSidSddl, checkGatewayMachineName, checkTargetServer, checkClientIP`, method name `VerifyTicket`, response fields `isValid, invalidReason, validationFailureFields`, response HMAC fields `hmacSignature, hmacDateTime`.
- Ticket states: `None / Not recognized / Expired / Already redeemed / Validation failure / Policy / Other`; "known invalid ticket", "Forgetting ticket %1!s!", "unused", "reconnect window" (`UnusedTicketMaxAge`), verification failures: target resource hostname mismatch, client IP mismatch, verifier already reported invalid, redemption never attempted, lack of client IP, lack of ticket info, "ticket age exceeds session duration limit".
- Consumption path lives in `AuthorizeResource` (0x180021c13 calls the VerifyTicket builder; 0x1800222c9 logs "Session %1!s! authorized for known good ticket"). Tickets are REDEEMED at resource-authorize time; validity is decided by a remote verify server (`vsUrl` from the cookie), not by a local map, plus local checks (SID, client IP, target hostname, gateway machine name — strings at 0x1800843f0..0x1800845b8).
- INERT here because: DuoTsgA (the plugin that would issue/see these cookies in `AuthenticateUser`) is not the active authentication plugin (registry: Authentication plug-ins @="native"), no SSO credential format is enabled (`CredentialFormat_Unencrypted_Enable`/`_Hmac_Enable` unset, `CredentialFormat_Hmac_Key` empty), and no verify-server component exists in the installed file set (DuoApi.dll / DuoTsgTools.dll / DuoUtilCommon.dll are .NET client/utility libraries — ilspycmd type lists show DuoApi/pinvoke helpers only).
- `W_DuoRdssoData` and `;\\DuoCookieRecognizer\\;` strings also exist; no code xref to `DuoCookieRecognizer` in this DLL (likely consumed by the SSO IIS/verify-server component that is not installed).

### 1c. Push-suppression cache (present and ACTIVE, but disabled by config)
Global containers (`.data`):
- `0x1800ac8f8` — intrusive `std::list` head (sentinel)
- `0x1800ac900` — element count, `0x1800ac908`/`0x1800ac920` — `std::unordered_map` sentinel nodes (bucket array ptr at 0x1800ac920)

Entry = 0x60 bytes (layout proven by CleanupOldPushEntries 0x1800272c0 which prints `thread`, `user`, `IP`, `last push <n> seconds ago.`):

```c
struct PushEntry {          // 0x60 bytes, heap-allocated
    LIST_ENTRY links;       // +0x00 next, +0x08 prev
    DWORD     threadId;     // +0x10  (worker thread that issued the push)
    std::string user;       // +0x18 (ptr +0x18, size +0x28, cap +0x30)
    std::string clientIP;   // +0x38 (ptr +0x38, size +0x48, cap +0x50)
    FILETIME   pushed;      // +0x58
};
```

- Key = `std::hash<std::string>(itoa(GetCurrentThreadId()))` — thread id ONLY (0x180020bb7 FNV-1a of the decimal thread-id string; map keyed at 0x1800ac8f0 map object, insert in `RecordPush` 0x180028c50 via fcn.180024570).
- `RecordPush(user, clientIP, ttl)` is called from the worker (0x180029aea) with ttl = cfg+0x140 right after the async auth request returns a txid.
- `CleanupOldPushEntries(ttl)` (0x1800272c0): `age_sec = (now - entry->pushed) / FILETIME(1s)` (magic `0x112e0be826d694b3` = 2^64/10^7 reciprocal shift `sar 0x1a`); `if (age_sec > ttl) erase`. Called at the top of every `AuthorizeConnection` (0x18002098e) and inside `RecordPush`.
- ttl source = registry `HKLM\SOFTWARE\Duo Security\DuoTsg` value `SuppressDuplicatePushDuration` (DWORD) read at 0x180005601 into cfg+0x140; if the value is absent/0 it stays 0 (no minimum clamp — contrast ADCachePollingInterval which clamps to 60 and ADCacheAutoRefreshInterval to 600). This rig does not set it -> **0 seconds -> every entry is erased immediately -> dedup effectively OFF**.
- Suppressed path (when ttl>0): AuthorizeConnection looks the current worker-thread up in the map (0x180020b02-0x180020b7b); on hit with `age_sec < ttl` it logs `Duplicate push suppressed for user: <user>, IP: <ip>, on thread: <tid>` and skips issuing a new push. Since the key is the thread id, two concurrent AuthorizeConnections (different work items = different threads) never share an entry; the dedup only catches the same worker thread re-entering within the window.
- NOTE: suppression is a *push-sending* dedup, not an authorization cache; the suppressed connection still has to succeed some Duo interaction on its own path.

Registry writes by the plugin: only EventLog registration keys at DLL register time. No persistence of auth state. Log file: `\Duo Security\DuoTsg\DuoTsg.log` (secure ACL'd, rotation via LogFileMaxSizeMB/LogFileMaxCount).

## Q2. Exempt/bypass path

Cloud bypass (the one that fires for `svcaccount`): `PreAuthorizeDuoUser` (0x1800427e0) POSTs `/rest/v1/preauth` (op enum 4; op 6 = `auth_status`, "auth" op used by the push call) with params `username`, `ipaddr` (client IP), `hostname` (gateway hostname), then:
- `result == "allow"` (len 5, 0x180043012-0x180043041) -> logs `Skipped Duo login for '<user>'`, returns success code 0 -> sink authorized, no push, no local record.
- `result == "deny"` (len 4) AND `status` contains "disable" (substr at 0x18008a928) -> "This account has been disabled..." -> deny.
- `result == "enroll"` -> enrollment message -> deny.
- `result == "auth"` -> proceed to 2FA (factor=auto, device=auto, async=1, pushinfo with username/hostname).
- else -> "ERROR: result not understood" -> deny.
So bypass is purely the Duo cloud preauth response. (Duo preauth returns `allow` for a user in a bypass policy group.)

Local bypass config EXISTS but is dead code in DuoTsgZ: `BypassStandaloneHosts`, `BypassLocalUsers`, `DuoEnforceGroupName`, `DuoBypassGroupName` are read into the config struct (offsets 0xd8, 0xb8, 0xf8, 0x118; loader 0x180005220-0x1800052ac) but have zero xrefs outside the config loader — they are consumed by `CDuoRdAuthenticationPlugIn::AuthenticateUser` (DuoTsgA, inactive). The authorization plugin performs no local group/SID/local-user/standalone checks (no group-enumeration strings, no `Lsa*`/token-group imports; the WS2_32 `GetAddrInfoW` import is used by the SSO resolver at 0x180018ff0-region, "resolving hostname" logs).

## Q3. Identity string sent to Duo

- aaedge passes `clientParameters` — plugin converts the username BSTR to a narrow string (0x1800206c2-0x180020852); that exact string is what Duo sees (worker logs `Username being sent to Duo: <u>` at 0x1800296d1).
- `UseUpnUsername` (cfg read at 0x18000575d; rig value 0): when 1, worker logs `Duo has been configured for UPN username format. Translating from: <name>` (0x180029524) and calls a DsCrackNamesA-based translator (fcn.180027ea0; strings `DsBind failed`, `Translating SAM account:`, `Translated account - username:`, `domain:`, `Username translation failed with error 0x`). When 0 (this rig) the name is passed through unmodified — i.e., the plain username as handed over by aaedge (SAM account name, not DOMAIN\user, not UPN). [aaedge's exact construction of this field not re-verified in this pass — INFERENCE from MS docs + observed pushes.]
- preauth also carries `ipaddr` = client IP and `hostname` = gateway machine name; pushinfo carries username + target hostname (ShowHostnameInPushPrompt).

## Q4. AuthorizeConnection vs AuthorizeResource granularity

- `AuthorizeConnection` (per TUNNEL_CREATE): full Duo (preauth + async auth + auth_status polling loop until allow/deny; polling in fcn.18003df30 `CheckTransactionStatus`, loop at 0x18003d048..0x18003d2ee).
- `AuthorizeResource` (per CHANNEL_CREATE): NO Duo API call at all. The only network interaction is the SSO VerifyTicket request (inert here). Then it completes via the resource sink ("authorized for known good ticket" / plain authorize). No CDuoAuthSession/MakeRequest xrefs exist in virtual_32.
- Therefore all pushes observed on TUNNEL_CREATEs come solely from AuthorizeConnection; channels are free.

## Q5. Second AuthorizeConnection for same user — short-circuit?

- No user-keyed "auth in progress" or "recently authorized" map exists. Each connection allocates its own `std::_Ref_count_obj2<CDuoAuthSession>` and its own txid; no txid sharing between connections.
- The only cross-connection state is the push-suppression list (Q1c), keyed by worker thread id, gated by `SuppressDuplicatePushDuration` (=0 here), and even when enabled it only suppresses issuing another push, it does not inherit an approval.
- Same-TLS-connection multiple TUNNEL_CREATEs: the plugin is TLS-agnostic (no socket state); each AuthorizeConnection is independent regardless of transport. (Whether aaedge invokes the policy engine per tunnel on one TLS connection is an aaedge/MS-TSGU behavior; the wire observation "push per TUNNEL_CREATE on new TLS" is consistent with per-tunnel invocation.)

## Q6. FailOpen semantics (binary truth table)

Config `FailOpen` (rig: 0). Observed code paths in `CDuoAuthSession::MakeRequest`/wrappers (strings + xrefs):
- Config load failure: "ERROR: Unable to load Duo configuration; failing open" -> allow (unconditional).
- Duo session object missing: "ERROR: Duo session not initialized; failing open" (0x18002130a) -> allow (unconditional).
- HTTP response "permanent failure": "HTTP response status is a permanent failure; failing open" -> allow per FailOpen.
- All retry attempts exhausted (timeout/network): "Timeout or network error on all attempts to connect to Duo; failing open" / "Failsafe Duo login: timeout or other network error" -> allow per FailOpen.
- Unparseable Duo response: "Failsafe Duo login: Duo response not understood" -> allow per FailOpen.
- Explicit Duo deny: "WARNING: Duo server denied access" -> deny (not failopen-able).
- Internal queueing failure: "ERROR: Unable to queue thread pool work" -> completes the sink with deny (0x18002147c, deny) regardless of FailOpen.
With FailOpen=0 the Duo-API-failure paths fail CLOSED (deny). The config/session-missing paths allow regardless.

## Q7. Client-controlled HTTP-layer inputs

- The only client-controllable input the plugin consumes is the **connection cookie** (logged as `cookie (%7!d! bytes): ><%8!s!<>`), which in the RDG protocol (MS-TSGU `packetCookie`, HTTP cookie `rdgclientcookie`) is attacker-supplied cookie text. In SSO mode it is parsed as XML with `/SsoCred/tkt|vsUrl|n|hmac` (format gated by CredentialFormat_* + HMAC key). With SSO inert (this rig) the cookie is ignored entirely.
- No other header, client cert, or NTLM artifact is read by the plugin (WinHttp usage is exclusively outbound to the Duo API; no inbound HTTP parsing in the DLL).

## Pseudo-C: AuthorizeConnection (condensed)

```c
HRESULT AuthorizeConnection(self, connectionId, userName, clientParams, soh, sink, blob, *pfRAPOnly) {
    ctx = new Ctx{ config=self->cfg, user=wide2narrow(userName), ip=narrow(clientParams.ipaddr),
                   cookie=..., sink=sink, enableDuo=cfg->EnableDuo, bRAP=self->+0x40 };
    CleanupOldPushEntries(cfg->SuppressDuplicatePushDuration);          // 0x18002098e
    if (entry = pushMap[itoa(GetCurrentThreadId())])                    // 0x180020b02
        if (age(entry) < cfg->SuppressDuplicatePushDuration)            // 0x180020b75
            Log("Duplicate push suppressed for user: %s, IP: %s, on thread: %s"); // skip push
    Log("AuthorizeConnection: Called on session %s ... cookie: >%s<");  // 0x180021223
    if (!self->duoSession && !flag) { Log("...failing open"); Complete(sink, allow=TRUE); return; } // 0x1800212fe
    if (!QueueUserWorkItem(worker_0x1800292e0, ctx)) { Log("Unable to queue thread pool work"); Complete(sink, allow=FALSE); }
}
void worker_0x1800292e0(ctx) {
    if (cfg->UseUpnUsername) ctx->user = TranslateViaDsCrackNames(ctx->user);  // fcn.180027ea0
    Log("Username being sent to Duo: %s");
    ok = duoSession->AuthorizeDuoUser(ctx->user, ctx->ip, ...);         // 0x18003c790
        // = PreAuthorizeDuoUser(preauth):
        //     allow -> "Skipped Duo login" -> ret 0
        //     auth  -> POST /auth/v2/auth async=1 -> txid -> poll auth_status
        //     deny(disabled)/enroll/error -> ret 4/3
    if (cfg->SuppressDuplicatePushDuration) RecordPush(user, ip, ttl);  // 0x180029aea (cond: cfg+0x140>0)
    Log(ok ? "Successful Duo login for '%s'" : "Connection authorization for '%s'");
    Complete(ctx->sink, allow=ok, bRAP=ctx->bRAP, redirectInfo=NULL);   // fcn.180026f70
}
```

## Verdicts

**same-TLS multi-tunnel: plugin-level cache — NO.** Expected key: none (no user/SID/connection cache). Expected TTL: n/a. The only plugin state, the push-suppression list (key = worker thread id, TTL = `SuppressDuplicatePushDuration`, rig value 0 s), does not grant authorization and cannot share an approval between two connections.

**Cheapest path to N tunnels with 1 push (plugin-level options observed in code):**
1. None within DuoTsgZ alone: every TUNNEL_CREATE = 1 preauth + 1 async auth + user approval, by design.
2. If the rig set `SuppressDuplicatePushDuration > 0`, subsequent connections on the *same worker thread* would skip sending a push — but each still requires a Duo allow, so it does not yield N tunnels from one approval.
3. The designed multi-tunnel-single-approval mechanism is the SSO ticket cookie (`/SsoCred/tkt` + verify server), which is entirely INERT on this rig (auth plugin inactive, no credential formats enabled, no verify server installed). Enabling it would require registry (CredentialFormat_Hmac_Enable + key, DuoTsgA activation) plus the Duo SSO verify infrastructure — not attacker-controllable client-side.
4. Attack-relevant corollaries from this binary: FailOpen=0 currently denies on Duo-outage; the two "failing open" unconditional paths (config load failure, session-not-initialized) are config-state dependent; the connection cookie is fully client-controlled text and is the only plugin input — it becomes security-relevant the moment SSO credential formats are enabled on any deployment of this DLL.

### Evidence vs inference
- EVIDENCE: all RVAs/strings/xrefs above (radare2 project `/tmp/duowork/duoplug`, disasm dumps `/tmp/duowork/{authconn,worker,preauth,authpush,poll,record,cleanup,cfgdis,authres,decrypt}_dis.txt`).
- INFERENCE: aaedge's exact username formatting and cookie-to-HTTP-header mapping (not re-verified in aaedge.dll this pass); identity of the SSO verify server component (absent from file set); cfg field-name guesses beyond those proven by string xrefs.
