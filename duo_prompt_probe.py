#!/usr/bin/env python3
"""Drive the Duo OIDC Universal Prompt protecting RD Web, in pure HTTP.

Chain (observed live):
  login.aspx POST (AD password)
   -> 302 default.aspx -> 302 duo:/oauth/v1/authorize?client_id=...
   -> 303 /prompt/<akey>/oidc/v1/<cid>/authorize?token=...
   -> 302 /prompt/<akey>?authkey=<authkey>   (prompt shell)
  then Prompt API on the Duo origin (needs its session cookies):
   GET  /prompt/<akey>/auth/payload?authkey=...      (bootstrap/factors)
   POST /prompt/<akey>/auth/factors/push/auth        (pkey -> push_txid)
   GET  /prompt/<akey>/auth/factors/push/status      (poll)
   GET  /prompt/<akey>/auth/finalize_auth?...        -> {url}
   GET  url  -> redirects home with OIDC code -> post-MFA RDWeb session

A push is only ever started by an explicit --push flag.
"""
import argparse
import json
import sys
import time

import requests
import urllib3

urllib3.disable_warnings()

RDS = "https://ECORP-RDS.ecorp.local"
LOGIN = RDS + "/RDWeb/Pages/en-US/login.aspx"
DEFAULT = RDS + "/RDWeb/Pages/en-US/default.aspx"
import re  # noqa: E402


def rdweb_password_login(s, user, pw, domain="ECORP"):
    r = s.get(LOGIN, timeout=15)
    hidden = {m[0]: m[1] for m in re.findall(
        r'<input[^>]*type="hidden"[^>]*name="([^"]+)"[^>]*value="([^"]*)"',
        r.text)}
    hidden.update({m[0]: m[1] for m in re.findall(
        r'<input[^>]*name="([^"]+)"[^>]*type="hidden"[^>]*value="([^"]*)"',
        r.text)})
    data = dict(hidden)
    data.update(DomainUserName="%s\\%s" % (domain, user), UserPass=pw,
                MachineType="Public")
    r = s.post(LOGIN, data=data, timeout=15, allow_redirects=False)
    if r.status_code != 302:
        raise RuntimeError("password login failed: %d" % r.status_code)
    return r.headers["Location"]


def walk_to_prompt(s):
    """Follow default.aspx -> Duo authorize -> prompt shell, one hop at a
    time, so every cookie on both origins lands in the session jar."""
    url = DEFAULT
    hops = 0
    while hops < 8:
        r = s.get(url, timeout=15, allow_redirects=False)
        if r.status_code not in (301, 302, 303):
            break
        url = r.headers.get("Location", "")
        if url.startswith("/"):  # relative to current origin
            from urllib.parse import urlparse
            o = urlparse(r.url)
            url = "%s://%s%s" % (o.scheme, o.netloc, url)
        hops += 1
    m = re.search(r"authkey=([A-Z0-9]+)", url)
    akey = re.search(r"/prompt/([A-Z0-9]+)[/?]", url)
    if not m or not akey:
        raise RuntimeError("did not land on prompt shell: %s\nbody: %s"
                           % (url, r.text[:200]))
    return url, akey.group(1), m.group(1), r


def prompt_api(s, prompt_url, path, method="GET", params=None, body=None):
    from urllib.parse import urlparse, parse_qs
    o = urlparse(prompt_url)
    url = "%s://%s/prompt/%s%s" % (o.scheme, o.netloc, AKY, path)
    trace = (parse_qs(o.query).get("req_trace_group") or [""])[0]
    headers = {"X-Duo-Req-Trace-Group": trace,
               "Referer": prompt_url,
               "Accept": "application/json"}
    if method == "GET":
        return s.get(url, params=params or {}, timeout=20, headers=headers)
    return s.post(url, params=params or {}, json=body, timeout=20,
                  headers=headers)


if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("user")
    ap.add_argument("password")
    ap.add_argument("--push", action="store_true",
                    help="actually start a Duo push (enrolled users)")
    ap.add_argument("--push-timeout", type=int, default=60)
    a = ap.parse_args()

    s = requests.Session()
    s.verify = False
    s.headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)"
    loc = rdweb_password_login(s, a.user, a.password)
    print("[+] password ok -> %s" % loc)
    prompt_url, AKY, authkey, resp = walk_to_prompt(s)
    print("[+] prompt shell: akey=%s authkey=%s" % (AKY, authkey))

    r = prompt_api(s, prompt_url, "/auth/payload",
                   params={"authkey": authkey, "browser_features": json.dumps({
                       "touch_supported": False,
                       "platform_authenticator_status": "unavailable",
                       "webauthn_supported": False,
                       "screen_resolution_height": 1080,
                       "screen_resolution_width": 1920,
                       "screen_color_depth": 24,
                       "is_uvpa_available": False,
                       "client_capabilities_uvpa": "",
                   })})
    print("[payload] %d %s" % (r.status_code, r.text[:600]))
    if r.status_code != 200:
        sys.exit(1)
    p = r.json()
    if p.get("stat") != "OK":
        sys.exit("payload not OK: %s" % p)

    factors = (p.get("response") or {})
    print("[factors] " + json.dumps(factors)[:800])

    # try finalize directly (bypass/exempt users may need no factor)
    r = prompt_api(s, prompt_url, "/auth/finalize_auth",
                   params={"authkey": authkey})
    print("[finalize/direct] %d %s" % (r.status_code, r.text[:400]))
    try:
        f = r.json()
        if f.get("stat") == "OK" and (f.get("response") or {}).get("url"):
            url = f["response"]["url"]
            print("[+] finalize url: %s" % url[:120])
            r2 = s.get(url, timeout=20, allow_redirects=True)
            print("[+] final: %d %s" % (r2.status_code, r2.url))
            for c in s.cookies:
                print("  cookie %s=%s..." % (c.name, str(c.value)[:40]))
            sys.exit(0)
    except ValueError:
        pass

    if not a.push:
        print("[-] no direct finalize; re-run with --push to send a Duo push")
        sys.exit(2)

    # find the push factor device pkey
    pkey = None
    txt = json.dumps(factors)
    m = re.findall(r'"pkey"\s*:\s*"([^"]+)"', txt)
    if m:
        pkey = m[0]
    print("[*] push factor pkey=%s" % pkey)
    r = prompt_api(s, prompt_url, "/auth/factors/push/auth", method="POST",
                   params={"authkey": authkey},
                   body={"authkey": authkey, "pkey": pkey, "otp_code": ""})
    print("[push] %d %s" % (r.status_code, r.text[:300]))
    txid = (r.json().get("response") or {}).get("push_txid") or \
        (r.json().get("response") or {}).get("txid")
    t0 = time.time()
    while time.time() - t0 < a.push_timeout:
        r = prompt_api(s, prompt_url, "/auth/factors/push/status",
                       params={"authkey": authkey, "push_txid": txid,
                               "saw_good_news": "false"})
        body = r.text[:200]
        print("[status %.0fs] %d %s" % (time.time() - t0, r.status_code, body))
        try:
            st = (r.json().get("response") or {})
        except ValueError:
            break
        if st.get("result") in ("SUCCESS", "Allow") or \
                st.get("status") in ("allow", "success"):
            break
        time.sleep(3)
    r = prompt_api(s, prompt_url, "/auth/finalize_auth",
                   params={"authkey": authkey})
    print("[finalize] %d %s" % (r.status_code, r.text[:400]))
    try:
        f = r.json()
        url = (f.get("response") or {}).get("url")
        if url:
            r2 = s.get(url, timeout=20, allow_redirects=True)
            print("[+] final: %d %s" % (r2.status_code, r2.url))
    except ValueError:
        pass
    for c in s.cookies:
        print("  cookie %s=%s..." % (c.name, str(c.value)[:40]))
