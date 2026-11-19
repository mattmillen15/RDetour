# RD Web Access web-layer audit — ECORP-RDS (Server 2019)

Session 4. Agent: code-auditor. Tooling: all Duo binaries are .NET
(dotfuscated); decompiled with ilspycmd. Dotfuscator string decryptor
(per-assembly XOR keystream) at `re/tools/deobf.py`; decoded sources at
`re/decomp/duo/*_dec.cs` and `re/decomp/tsportal/tsportalwebpart.cs`.
tsportalwebpart.dll = Microsoft stock, v10.0.10011.16384 (2019),
unobfuscated. Evidence (E) vs inference (I) marked.

## Q1. Web.config auth/session config

`re/ecorp-rds/rdweb-pages/Windows/Web/RDWeb/Pages/Web.config`

**E — config values:**
- `<forms loginUrl="default.aspx" name="TSWAAuthHttpOnlyCookie" protection="All" requireSSL="true" />` — no timeout, no slidingExpiration, no path/domain attributes.
- **No `machineKey`** → AutoGenerate/IsolateApps default (per-app key). Machine-level machine.config/root web.config were not pulled, so a pinned key can't be ruled out.
- Modules: stock `FormsAuthentication` **removed**, replaced by `RDWAFormsAuthenticationModule` = `Microsoft.TerminalServices.Publishing.Portal.FormAuthentication.TSDomainFormsAuthentication`; `runAllManagedModulesForAllRequests="true"`. Handlers: `PagesWebFeedHandler` (WebFeed.aspx), `ResourceFileHandler` (path `rdp`, with `<deny users="?"/>`).
- AppSettings: `PublicModeSessionTimeoutInMinutes=20`, `PrivateModeSessionTimeoutInMinutes=240`, `PasswordChangeEnabled=false`, `GatewayCredentialsSource=4` ("Ask me later"), `DefaultTSGateway=""`, `AuthenticationContext=pages`.
- **Zero Duo registrations** (no module, handler, or configSections).
- **Diff vs `Web.config.bak-tfw`: byte-identical** (`diff` exit 0). The Duo installer's backup action produces ZIPs, not `.bak-*` files (E: `webconfigbackup` implementation at `InstallUtilIisLinked_dec.cs:4077-4131`), so the `.bak-tfw` came from another tool; no config drift between them.

**E — cookie behavior from `TSDomainFormsAuthentication` (tsportalwebpart.cs:6024-6776):**
- Login = POST to login.aspx with `DomainUserName`/`UserPass` (or a `ClaimsToken` field); AD logon via `LogonUser(..., LOGON32_LOGON_NETWORK, ...)` (line 7106) + AccessCheck on the RDWeb folder SD; `UserIdentity` = SID string.
- Ticket: `new FormsAuthenticationTicket(2, SID, now, now+20/240min, isPersistent:false, UserData=DataContract dict{DomainUserName})` (line 6558), encrypted with machineKey, set as `TSWAAuthHttpOnlyCookie` (HttpOnly; Secure if TLS). **Session cookie, 20 min public / 240 min private**, and it is **re-minted on every authenticated request** (SetupHttpContextUserAsync runs on each request with a valid cookie → sliding window) (lines 6537-6581, 6635-6663).
- `TSWAAuthClientSideCookie` — NOT HttpOnly, holds only Name/MachineType/WorkSpaceID; **persistent 1000 days in private mode** (lines 6300-6331). Never used for auth (only parsed for display/workspace ID); accepted as fallback for `DomainUserName` display only (line 6521).
- `TSWAFeatureCheckCookie` — cookie-support test.
- Feed apps: `/RDWeb/Feed` (`FeedAuthModule`) authenticates off a `.ASPXAUTH` forms cookie; `/RDWeb/FeedLogin` is NTLM/Negotiate at IIS (applicationHost.config:937-961) and `WebFeedLoginHandler` mints `FormsAuthenticationTicket(version=1, name, UtcNow, **DateTime.MaxValue**, isPersistent:true, ...)` — returned both as cookie **and as the response body** (content-type `application/x-msts-webfeed-login`) (lines 7444-7505). Infinite-lifetime artifact, but gated by NTLM, not by the portal.

## Q2. Duo web-side binaries

All five DuoRDWeb files are .NET 4.7.1. **None of them contains an IHttpModule or hooks any IIS event; none mints or accepts any cookie.**

- `DuoIisBase.dll` — only `DuoSecretProtection` + `DuoConstants`. Constants: `SOFTWARE\Duo Security\DuoRdweb`, `Client_Secret`, failOpen. It only reads/writes the DPAPI-protected client secret.
- `DuoApi.dll` — classic Duo Auth API v2 client: HMAC-SHA1(skey) canonical signing, HTTP Basic, cert pinning (`CertificatePinnerFactory`); used for `/auth/v2/ping` credential tests. No OIDC strings at all.
- `DuoUtilCommon.dll` — utilities: DPAPI helpers (`ProtectRegistryValue` uses `DataProtectionScope.LocalMachine` + entropy), `ValidateKeys` (ping), `CompareSignatures` (Authenticode thumbprint pinning of caller vs DuoUtilCommon).
- `InstallUtilIisLinked.exe` — MSI custom-action helper. Actions: `webconfigbackup`/`webconfigrestore` (ZIP), SSL-binding detection, `authtest`, `IniKey_*`, `copyself`. Its **only** web-layer mutation is registering `Duo.Iis.IpSecurityModule.HttpModule` + a `<DuoIpSecurity>` configSection (allowDefinition=Everywhere) — the Duo Trusted-IP allowlisting feature, a secondary control, **not** the MFA gate. That section is absent from the live Web.config.
- Registry (`registry/HKLM_SOFTWARE_Duo_Security.reg`): DuoRdweb has OIDC Client_Id `DIBM2J2O7T99MQKO0FP3`, Host, FailOpen=0, DPAPI Client_Secret, **and a plaintext `AKey` `9iZIFWf/GvRZ1oPBjVy8muoeGSYDXQBCrJw1hnf3`** (Duo Web SDK v2-style app key).

**E — critical artifact gap:** the pulled set contains **no MFA enforcement module**. No IHttpModule exists in any pulled Duo binary; the Pages Web.config, all 9 applicationHost.config history snapshots (`grep -ri duo` over `iis-history/` = zero hits), the ASPX pages, and `Bin/` are all stock. Since prior testing behaviorally confirmed Duo OIDC SSO fronts the portal, the gate must be wired at surfaces **not captured in this pull**: the machine-level .NET root web.config/machine.config (`%windir%\Microsoft.NET\Framework64\v4.0.30319\Config\`), a GAC'd module, or app-level config of the other RDWeb apps. The pull dirs `gac-listing/`, `iis-apppools/`, `rdweb-appdata/` are empty, and `C:\Windows\Web\RDWeb\{Feed,FeedLogin}\Web.config` were not pulled. **To close Q2 you need those five items plus the module DLL itself.**
- Could any Duo output satisfy the gateway? **I:** No. The pulled Duo web components emit nothing at all; the gateway consumes only its own COM plugins — registry shows gateway auth default = `native` with DuoTsgA registered, and **authz default = `DuoTsgZ`** (the Duo authorization plugin replaces native authz), fed by NTLM at tunnel setup. Nothing crosses from the web app process to the gateway process. Prior conclusion stands.

## Q3. .rdp generation (tsportalwebpart.dll)

- `.rdp` content = Central PublishedApps WMI `RDPFileContents` + `Workspace Id:s:` + `GetCommonRdpProperties` (lines 1631-1681): `full address`, `gatewayusagemethod`, `gatewayhostname` (from ConnectionSettings/WMI), `gatewayprofileusagemethod`, `gatewaycredentialssource:i:0|1` (password|smartcard only — never 4; "Ask me later" is client-side UI), `authentication level`, optional `connection type:i:6`.
- **`gatewayaccesstoken` appears nowhere in the assembly** (grep over full decompile). No password blob, no cookie material, no per-user secret is embedded. `RdpSigningCertificate` publishes only a **public** cert hash (`wsHash`) in feed XML for client-side signature verification.
- Served by `ResourceFileHandler` at `/RDWeb/Pages/rdp/...` behind `<deny users="?">`; identity = the forms ticket (`TSFormAuthTicketInfo`).
- **Confirms the prior conclusion:** portal .rdp files carry no reusable gateway credential; gateway auth happens at tunnel NTLM where DuoTsg fires.

## Q4. One Duo approval → multiple artifacts?

- In the pulled config there is no Duo gate wired at the web layer at all (see Q2 gap), so strictly: one portal login yields only the stock artifacts.
- Under stock behavior, one authenticated portal session yields: the `TSWAAuthHttpOnlyCookie` ticket (20/240 min, sliding — effectively continuous while in use), unlimited `.rdp` downloads (no secrets inside), the WebFeed.aspx feed for the same SID, and the cosmetic client-side cookie. All encrypted under the Pages app's machineKey, all same-lifetime regime.
- The one independent-lifetime artifact (FeedLogin's `DateTime.MaxValue` ticket, Q1) sits behind NTLM, **not** behind a Duo portal approval — a portal approval cannot mint it.
- **Verdict:** no; and nothing the web layer mints is honored by the gateway regardless.

## Additional findings

1. **Host-header injection into redirects** — `RequestHelper.GetOriginalRequestUri` builds URIs from `request.Headers["Host"]` verbatim (tsportalwebpart.cs:5822-5826); used for `RedirectBackWithErrorValue` (6293-6298) and the WebFeed login redirect (5971-5973). Severity: Low (open redirect on error paths; not an auth bypass). Stock 2019 behavior.
2. **DuoRdweb Client_Secret is recoverable by any local process** — DPAPI `LocalMachine` scope with static entropy fully derivable from the DLL: pre-entropy string `UGXLjpyETqN8HuQnPqLhDpFFCeiukkAYiDKfJ7MN` XOR `LwtdbfYssbgJMRghvfTpUPUUi8AMDj`, base64 (DuoIisBase_dec.cs `GetEntropyString`; DuoUtilCommon_dec.cs:22017-22091). Location: registry `HKLM\SOFTWARE\Duo Security\DuoRdweb\Client_Secret`. Severity: Medium, on-box code exec required (any user, not admin).
3. **Plaintext `AKey` under DuoRdweb** — if the (missing) OIDC module uses it to validate Duo Web SDK-style responses or sign a local Duo session cookie, on-box read enables forgery of the app-side signature (the auth-side signature needs the SKey, which for the web half is not present in the registry). **I only** — needs the module binary to confirm.
4. Non-findings verified: UserData XML parsing sets `DtdProcessing.Prohibit` (no XXE); ticket name = SID (no username tampering via cookie); password change endpoint disabled.

**Web-layer artifacts that the GATEWAY will honor: none.**

Notable cross-check for the gateway-plugin RE: `HKLM\SOFTWARE\Microsoft\Terminal Server Gateway\Authorization plug-ins` default is `DuoTsgZ` (Duo replaces native authz), while Authentication plug-ins default remains `native` with DuoTsgA registered — consistent with the push firing at tunnel authorization rather than authentication.

Key files: `re/ecorp-rds/rdweb-pages/Windows/Web/RDWeb/Pages/Web.config`, `re/ecorp-rds/registry/HKLM_SOFTWARE_Duo_Security.reg`, `re/decomp/duo/{DuoIisBase,DuoUtilCommon,InstallUtilIisLinked}_dec.cs`, `re/decomp/tsportal/tsportalwebpart.cs`, `re/tools/deobf.py`.
