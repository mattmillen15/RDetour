#!/usr/bin/env python3
"""Feed capture run: fresh Duo round on the RD Web feed path.

Arms CDP Fetch (auth + response-body capture), drives webfeed.aspx through
FeedLogin -> Duo -> back, prints the number-match code the instant it renders,
and dumps every response body seen on the chain to /tmp/feedcap/.

Usage: feedrun.py [wait_secs]   (default 240)
"""
import base64
import json
import os
import re
import sys
import time

from playwright.sync_api import sync_playwright

OUT = "/tmp/feedcap"
os.makedirs(OUT, exist_ok=True)

with sync_playwright() as pw:
    b = pw.chromium.connect_over_cdp("http://127.0.0.1:9222")
    ctx = b.contexts[0]
    page = ctx.pages[0]

    # fresh Duo state, keep RD Web TSWA cookies
    keep = [c for c in ctx.cookies() if c["domain"].endswith("ecorp.local")
            and c["name"].startswith("TSWA")]
    ctx.clear_cookies()
    for c in keep:
        c.pop("sameSite", None)
    ctx.add_cookies(keep)
    print("kept cookies:", [c["name"] for c in keep])

    cdp = ctx.new_cdp_session(page)
    cap = {"n": 0}

    def save_body(url, body, b64):
        if not body:
            return
        data = base64.b64decode(body) if b64 else body.encode("utf-8", "replace")
        cap["n"] += 1
        fn = f"{OUT}/resp{cap['n']:02d}_{re.sub('[^A-Za-z0-9]', '_', url.split('/')[2])}_" \
             f"{(url.split('/')[-1] or 'root')[:40]}.bin"
        open(fn, "wb").write(data)
        print(f"  [body saved] {url[:90]} -> {fn} ({len(data)}B)")

    def on_paused(p):
        rid = p["requestId"]
        if p.get("responseStatusCode"):  # Response stage
            try:
                r = cdp.send("Fetch.getResponseBody", {"requestId": rid})
                save_body(p["request"]["url"], r.get("body"), r.get("base64Encoded"))
            except Exception:
                pass
        try:
            cdp.send("Fetch.continueRequest", {"requestId": rid})
        except Exception:
            pass

    def on_auth(p):
        try:
            cdp.send("Fetch.continueWithAuth", {
                "requestId": p["requestId"],
                "authChallengeResponse": {
                    "response": "ProvideCredentials",
                    "username": "ECORP.local\\testuser",
                    "password": "testuser"}})
            print("  [ntlm answered]", p["request"]["url"][:70])
        except Exception:
            pass

    cdp.on("Fetch.requestPaused", on_paused)
    cdp.on("Fetch.authRequired", on_auth)
    cdp.send("Fetch.enable", {"handleAuthRequests": True})

    try:
        page.goto("https://ecorp-rds.ecorp.local/RDWeb/feed/webfeed.aspx",
                  wait_until="commit", timeout=45000)
    except Exception as e:
        print("goto:", str(e)[:110])

    shown = None
    deadline = time.time() + int(sys.argv[1] if len(sys.argv) > 1 else 240)
    outcome = None
    while time.time() < deadline:
        url = page.url
        try:
            body = page.inner_text("body") or ""
        except Exception:
            body = ""
        if "ecorp-rds" in url and "/RDWeb/feed" in url:
            outcome = "FEED-REACHED"
            break
        if "login expired" in body.lower():
            outcome = "EXPIRED"
            break
        if "Skip for now" in body:
            try:
                page.get_by_text("Skip for now", exact=True).click(timeout=2000)
                print(time.strftime("%H:%M:%S"), "skipped interstitial")
            except Exception:
                pass
        m = re.search(r"verification code.*?(\d{3})", body, re.S)
        if m and m.group(1) != shown:
            shown = m.group(1)
            print("*** DUO CODE:", shown, "*** enter in Duo Mobile NOW")
        if "Yes, this is my device" in body:
            try:
                page.get_by_text("Yes, this is my device", exact=True).click(timeout=2000)
                print("device question answered")
            except Exception:
                pass
        if cap["n"] and "duosecurity" not in url and url != "about:blank":
            outcome = "POST-CHAIN at " + url[:80]
            break
        time.sleep(0.7)

    print("outcome:", outcome)
    print("final url:", page.url[:120])
    print("captured bodies:", cap["n"], "in", OUT)
