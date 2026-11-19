#!/usr/bin/env python3
"""CDP driver for the RD Web post-MFA capture session.

Controls the Chromium instance launched with --remote-debugging-port=9222
(persistent profile /tmp/rdweb-profile). Session cookies survive across
invocations because the browser process stays up.

Usage: rdcap.py <command> [args]
  status                 - list open pages (id, url, title)
  goto <url>             - navigate page 0
  login                  - fill RD Web login.aspx (testuser) and submit
  waitpostmfa [secs]     - poll until URL is back on /RDWeb/ post-MFA
  cookies                - dump ALL cookies (incl. HttpOnly) as JSON
  fetch <url>            - GET url with page session (cookies), print body
  body                   - print current page text (first 3000 chars)
  shot <path>            - screenshot page 0
"""
import json
import sys
import time

from playwright.sync_api import sync_playwright

BASE = "https://ECORP-RDS.ecorp.local"
LOGIN = BASE + "/RDWeb/Pages/en-US/login.aspx"
CDP = "http://127.0.0.1:9222"


def connect(pw):
    browser = pw.chromium.connect_over_cdp(CDP)
    ctx = browser.contexts[0]
    page = ctx.pages[0] if ctx.pages else ctx.new_page()
    return browser, ctx, page


def main():
    cmd = sys.argv[1] if len(sys.argv) > 1 else "status"
    with sync_playwright() as pw:
        browser, ctx, page = connect(pw)
        if cmd == "status":
            for p in ctx.pages:
                print(p.url)
        elif cmd == "goto":
            page.goto(sys.argv[2], wait_until="domcontentloaded", timeout=30000)
            print(page.url)
        elif cmd == "login":
            page.goto(LOGIN, wait_until="domcontentloaded", timeout=30000)
            page.wait_for_selector('input[name="DomainUserName"]', timeout=10000)
            page.fill('input[name="DomainUserName"]', "ECORP.local\\testuser")
            page.fill('input[name="UserPass"]', "testuser")
            page.click('input[type="submit"]')
            page.wait_for_load_state("domcontentloaded", timeout=30000)
            print("after submit:", page.url)
        elif cmd == "waitpostmfa":
            deadline = time.time() + int(sys.argv[2] if len(sys.argv) > 2 else 300)
            last = ""
            while time.time() < deadline:
                url = page.url
                if url != last:
                    print(time.strftime("%H:%M:%S"), url)
                    last = url
                if "/RDWeb/" in url and "duosecurity" not in url:
                    print("POST-MFA (back on RDWeb)")
                    break
                time.sleep(2)
            else:
                print("TIMEOUT still at", page.url)
                sys.exit(2)
        elif cmd == "cookies":
            print(json.dumps(ctx.cookies(), indent=1, sort_keys=True))
        elif cmd == "fetch":
            r = page.evaluate(
                "async (u) => { const r = await fetch(u, {credentials:'include'});"
                " return {status: r.status, body: await r.text()}; }", sys.argv[2])
            print("status:", r["status"])
            print(r["body"])
        elif cmd == "click":
            page.get_by_text(sys.argv[2], exact=True).click(timeout=5000)
            page.wait_for_load_state("domcontentloaded", timeout=15000)
            print("clicked; now at", page.url)
        elif cmd == "flow":
            # full auto: login -> skip interstitial -> surface number code
            # -> click device question -> post-MFA default.aspx
            page.goto(LOGIN, wait_until="domcontentloaded", timeout=30000)
            page.wait_for_selector('input[name="DomainUserName"]', timeout=10000)
            page.fill('input[name="DomainUserName"]', "ECORP.local\\testuser")
            page.fill('input[name="UserPass"]', "testuser")
            page.click('input[type="submit"]')
            page.wait_for_load_state("domcontentloaded", timeout=30000)
            import re
            shown_code = None
            deadline = time.time() + int(sys.argv[2] if len(sys.argv) > 2 else 240)
            while time.time() < deadline:
                url = page.url
                if "/RDWeb/Pages" in url and "login" not in url.lower():
                    print("POST-MFA OK:", url[:120])
                    break
                if "Runtime Error" in (page.inner_text("body") or "")[:2000]:
                    print("SERVER 500:", url[:160])
                    sys.exit(3)
                try:
                    body = page.inner_text("body")
                except Exception:
                    body = ""
                if "Skip for now" in body:
                    try:
                        page.get_by_text("Skip for now", exact=True).click(timeout=2000)
                        print(time.strftime("%H:%M:%S"), "skipped interstitial")
                    except Exception:
                        pass
                m = re.search(r"verification code.*?(\d{3})", body, re.S)
                if m and m.group(1) != shown_code:
                    shown_code = m.group(1)
                    print("*** DUO CODE:", shown_code, "***  enter it in Duo Mobile now")
                if "Yes, this is my device" in body:
                    try:
                        page.get_by_text("Yes, this is my device", exact=True).click(timeout=2000)
                        print(time.strftime("%H:%M:%S"), "device remembered")
                    except Exception:
                        pass
                if shown_code and shown_code not in (body or "") and "device" not in (body or "")[:400] and "duosecurity" in url:
                    pass  # code screen gone; approval in flight
                time.sleep(0.5)
            else:
                print("FLOW TIMEOUT at", page.url)
                sys.exit(2)
        elif cmd == "links":
            data = page.evaluate(
                "() => [...document.querySelectorAll('a')].map(a =>"
                " ({text: a.innerText.trim(), href: a.href}))"
                ".concat([...document.querySelectorAll('form')].map(f =>"
                " ({text: 'FORM ' + f.name, href: f.action})))")
            for d in data:
                print(repr(d["text"])[:60], "->", d["href"])
        elif cmd == "body":
            print(page.inner_text("body")[:3000])
        elif cmd == "shot":
            page.screenshot(path=sys.argv[2])
            print("saved", sys.argv[2])
        else:
            print(__doc__)


if __name__ == "__main__":
    main()
