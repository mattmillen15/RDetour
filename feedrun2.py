#!/usr/bin/env python3
"""Feed capture v2: browser Duo approval -> immediate token mint + use.

1. Drives the feed Duo round in the controlled browser (surfaces the
   number-match code instantly).
2. Seconds after approval: mints a fresh x-msts-webfeed-login token via
   requests (NTLM + jar), then consumes it (.ASPXAUTH) at webfeed.aspx and
   at every .rdp resource URL from the feed XML.
Saves everything to /tmp/feedcap2/.
"""
import base64
import json
import os
import re
import sys
import time
import warnings

import requests
from requests_ntlm import HttpNtlmAuth
from playwright.sync_api import sync_playwright

warnings.filterwarnings("ignore")
OUT = "/tmp/feedcap2"
os.makedirs(OUT, exist_ok=True)
BASE = "https://ecorp-rds.ecorp.local"


def log(m):
    print(time.strftime("%H:%M:%S"), m, flush=True)


def browser_round(pw):
    """Drive feed chain through Duo; return (ctx, cookies dict) on success."""
    b = pw.chromium.connect_over_cdp("http://127.0.0.1:9222")
    ctx = b.contexts[0]
    page = ctx.pages[0]

    keep = [c for c in ctx.cookies() if c["domain"].endswith("ecorp.local")
            and c["name"].startswith("TSWA")]
    ctx.clear_cookies()
    for c in keep:
        c.pop("sameSite", None)
    ctx.add_cookies(keep)

    cdp = ctx.new_cdp_session(page)

    def on_paused(p):
        try:
            cdp.send("Fetch.continueRequest", {"requestId": p["requestId"]})
        except Exception:
            pass

    def on_auth(p):
        try:
            cdp.send("Fetch.continueWithAuth", {
                "requestId": p["requestId"],
                "authChallengeResponse": {"response": "ProvideCredentials",
                    "username": "ECORP.local\\testuser", "password": "testuser"}})
        except Exception:
            pass

    cdp.on("Fetch.requestPaused", on_paused)
    cdp.on("Fetch.authRequired", on_auth)
    cdp.send("Fetch.enable", {"handleAuthRequests": True})

    try:
        page.goto(BASE + "/RDWeb/feed/webfeed.aspx", wait_until="commit",
                  timeout=45000)
    except Exception:
        pass  # expected MIME abort later; this goto lands on Duo prompt

    shown = None
    deadline = time.time() + 300
    while time.time() < deadline:
        url = page.url
        try:
            body = page.inner_text("body") or ""
        except Exception:
            body = ""
        if "success" in body.lower() or ("ecorp-rds" in url and "duosecurity" not in url):
            log("Duo round complete (or chain moved on)")
            break
        if "login expired" in body.lower():
            log("EXPIRED - abort")
            return None
        if "Skip for now" in body:
            try:
                page.get_by_text("Skip for now", exact=True).click(timeout=2000)
                log("skipped interstitial")
            except Exception:
                pass
        m = re.search(r"verification code.*?(\d{3})", body, re.S)
        if m and m.group(1) != shown:
            shown = m.group(1)
            print("*** DUO CODE:", shown, "*** enter in Duo Mobile NOW", flush=True)
        if "Yes, this is my device" in body:
            try:
                page.get_by_text("Yes, this is my device", exact=True).click(timeout=2000)
                log("device question answered")
            except Exception:
                pass
        time.sleep(0.6)
    else:
        log("TIMEOUT waiting for Duo")
        return None

    time.sleep(3)  # let finalize + FeedLogin 200 happen server-side
    jar = {}
    for c in ctx.cookies():
        if "ecorp" in c["domain"] or "duosecurity" in c["domain"]:
            jar[c["name"]] = c["value"]
    return jar


def mint_token(jar):
    s = requests.Session()
    s.auth = HttpNtlmAuth("ECORP.local\\testuser", "testuser")
    s.verify = False
    s.headers["User-Agent"] = "MS Terminal Services/10.0"
    r = s.get(BASE + "/RDWeb/feed/webfeed.aspx", cookies=jar, timeout=30,
              allow_redirects=True)
    ct = r.headers.get("Content-Type", "")
    log(f"mint: {r.status_code} {ct[:45]} {len(r.content)}B")
    if "x-msts-webfeed-login" in ct:
        return r.content.decode().strip()
    return None


def use_token(jar, token):
    full = dict(jar)
    full[".ASPXAUTH"] = token
    for auth in (None, HttpNtlmAuth("ECORP.local\\testuser", "testuser")):
        s = requests.Session()
        if auth:
            s.auth = auth
        s.verify = False
        s.headers["User-Agent"] = "MS Terminal Services/10.0"
        r = s.get(BASE + "/RDWeb/feed/webfeed.aspx", cookies=full, timeout=30,
                  allow_redirects=False)
        ct = r.headers.get("Content-Type", "")
        log(f"use({'ntlm' if auth else 'plain'}): {r.status_code} {ct[:45]} {len(r.content)}B")
        if r.status_code == 200 and ("xml" in ct.lower() or "radc" in ct.lower()):
            return r.content
    return None


def fetch_rdp(jar, token, url):
    full = dict(jar)
    full[".ASPXAUTH"] = token
    s = requests.Session()
    s.auth = HttpNtlmAuth("ECORP.local\\testuser", "testuser")
    s.verify = False
    s.headers["User-Agent"] = "MS Terminal Services/10.0"
    r = s.get(url, cookies=full, timeout=30, allow_redirects=False)
    log(f"rdp {url.split('/')[-1]}: {r.status_code} {len(r.content)}B")
    return r.content if r.status_code == 200 else None


with sync_playwright() as pw:
    jar = browser_round(pw)
    if not jar:
        sys.exit(2)
    json.dump(jar, open(f"{OUT}/jar.json", "w"))
    log(f"jar cookies: {sorted(jar)}")

    token = mint_token(jar)
    if not token:
        log("MINT FAILED")
        sys.exit(3)
    open(f"{OUT}/token.hex", "w").write(token)
    log(f"token minted: {len(token)} chars")

    xml = use_token(jar, token)
    if not xml:
        log("USE FAILED (feed XML not returned)")
        sys.exit(4)
    open(f"{OUT}/webfeed.xml", "wb").write(xml)
    log("FEED XML CAPTURED:")
    print(xml.decode("utf-8", "replace")[:1500])

    urls = re.findall(rb"<RSS>[^<]*<URL>([^<]+)</URL>", xml)
    for u in urls:
        url = u.decode()
        if not url.startswith("http"):
            url = BASE + url
        if ".rdp" in url:
            data = fetch_rdp(jar, token, url)
            if data:
                fn = f"{OUT}/" + url.split("/")[-1]
                open(fn, "wb").write(data)
                body = data.decode("utf-8", "replace")
                hits = [l for l in body.splitlines()
                        if "gateway" in l.lower() or "token" in l.lower()]
                log(f"saved {fn}; gateway/token lines:")
                for h in hits:
                    print("   *", h)
