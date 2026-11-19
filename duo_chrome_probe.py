#!/usr/bin/env python3
"""Complete the Duo OIDC Universal Prompt in pure HTTP with a Chrome TLS
fingerprint (curl_cffi). Hypothesis: the earlier 400 FAILs on /prompt/<akey>
API calls were H2/JA3 bot filtering, not missing params.

Stages: password login -> prompt shell -> pre_authn -> [push/auth ->
status poll (USER TAPS PHONE)] -> finalize -> back to RD Web post-MFA.
Then: pull the feed, download one .rdp payload, grep for gateway SSO
tokens (gatewayaccesstoken / gatewaycredentialssource).
"""
import json
import re
import sys
import time

from curl_cffi import requests as creq

RDS = "https://ECORP-RDS.ecorp.local"
LOGIN = RDS + "/RDWeb/Pages/en-US/login.aspx"
FEED = RDS + "/RDWeb/feed/webfeed.aspx"
IMPERSONATE = "chrome131"

user, pw = (sys.argv[1], sys.argv[2]) if len(sys.argv) > 2 else \
    ("testuser", "testuser")


def main():
    s = creq.Session(impersonate=IMPERSONATE, verify=False)

    r = s.get(LOGIN, timeout=15)
    hidden = {m[0]: m[1] for m in re.findall(
        r'<input[^>]*type="hidden"[^>]*name="([^"]+)"[^>]*value="([^"]*)"',
        r.text)}
    hidden.update({m[0]: m[1] for m in re.findall(
        r'<input[^>]*name="([^"]+)"[^>]*type="hidden"[^>]*value="([^"]*)"',
        r.text)})
    data = dict(hidden)
    data.update(DomainUserName="ECORP\\" + user, UserPass=pw,
                MachineType="Public")
    r = s.post(LOGIN, data=data, timeout=15, allow_redirects=False)
    print("[1] password login -> %d %s" % (r.status_code,
                                           r.headers.get("Location", "")))
    if r.status_code != 302:
        print(r.text[:300])
        return 1

    # walk redirects to the Duo prompt shell
    url = RDS + "/RDWeb/Pages/en-US/default.aspx"
    for _ in range(8):
        r = s.get(url, timeout=15, allow_redirects=False)
        if r.status_code not in (301, 302, 303):
            break
        url = r.headers.get("Location", "")
        if url.startswith("/"):
            from urllib.parse import urlparse
            o = urlparse(r.url)
            url = "%s://%s%s" % (o.scheme, o.netloc, url)
    m_authkey = re.search(r"authkey=([A-Z0-9]+)", url)
    m_akey = re.search(r"/prompt/([A-Z0-9]+)[/?]", url)
    if not (m_authkey and m_akey):
        print("[-] no prompt shell (url=%s)" % url)
        return 1
    akey, authkey, prompt_url = m_akey.group(1), m_authkey.group(1), url
    print("[2] prompt shell akey=%s authkey=%s" % (akey, authkey))

    def api(path, params=None, method="GET", body=None):
        u = "https://api-d933f7f3.duosecurity.com/prompt/%s%s" % (akey, path)
        from urllib.parse import urlparse, parse_qs
        trace = (parse_qs(urlparse(prompt_url).query)
                 .get("req_trace_group") or [""])[0]
        headers = {"X-Duo-Req-Trace-Group": trace,
                   "Referer": prompt_url,
                   "Accept": "application/json"}
        if method == "GET":
            return s.get(u, params=params or {}, timeout=20, headers=headers)
        return s.post(u, params=params or {}, json=body, timeout=20,
                      headers=headers)

    bf = json.dumps({"touch_supported": False,
                     "platform_authenticator_status": "unavailable",
                     "webauthn_supported": False,
                     "screen_resolution_height": 1080,
                     "screen_resolution_width": 1920,
                     "screen_color_depth": 24,
                     "is_uvpa_available": False,
                     "client_capabilities_uvpa": ""})
    r = api("/pre_authn/evaluation", {"authkey": authkey,
                                      "browser_features": bf,
                                      "local_trust_choice": "unknown"})
    print("[3] pre_authn -> %d %s" % (r.status_code, r.text[:500]))
    if r.status_code != 200:
        print("[-] chrome fingerprint did not unlock the prompt API")
        return 1
    ev = r.json().get("response", {})
    print("    action=%s" % ev.get("action"))
    return 0


if __name__ == "__main__":
    sys.exit(main())
