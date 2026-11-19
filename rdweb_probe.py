#!/usr/bin/env python3
"""RD Web Access forms login + feed/PAA-cookie recon.

Logs in at /RDWeb/Pages/en-US/login.aspx (WebForms: DomainUserName/Password),
dumps all session cookies (candidates for the RDG PAA cookie path), then
pulls the published-app feed and any .rdp payloads for gateway SSO markers
(gatewaycredentialssource, gatewayhostname).
"""
import re
import sys
import requests
import urllib3

urllib3.disable_warnings()

BASE = "https://ECORP-RDS.ecorp.local"
LOGIN = BASE + "/RDWeb/Pages/en-US/login.aspx"
FEED = BASE + "/RDWeb/feed/webfeed.aspx"


def login(user, password, domain="ECORP"):
    s = requests.Session()
    s.verify = False
    s.headers["User-Agent"] = "Mozilla/5.0"
    r = s.get(LOGIN, timeout=15)
    hidden = dict(re.findall(
        r'<input[^>]*type="hidden"[^>]*name="([^"]+)"[^>]*value="([^"]*)"',
        r.text))
    hidden.update(dict(re.findall(
        r'<input[^>]*name="([^"]+)"[^>]*type="hidden"[^>]*value="([^"]*)"',
        r.text)))
    data = dict(hidden)
    data["DomainUserName"] = "%s\\%s" % (domain, user)
    data["UserPass"] = password
    data["MachineType"] = "Public"
    r = s.post(LOGIN, data=data, timeout=15, allow_redirects=False)
    print("login POST -> %d  Location: %s" % (r.status_code,
                                              r.headers.get("Location", "")))
    if r.status_code not in (301, 302):
        m = re.search(r'class="errorMessage"[^>]*>([^<]+)', r.text)
        print("  login body error hint: %s" % (m and m.group(1)))
        return None
    print("cookies after login:")
    for c in s.cookies:
        print("  %s = %s" % (c.name, c.value))
    return s


def feed(s):
    r = s.get(FEED, timeout=15)
    print("feed -> %d (%d bytes)" % (r.status_code, len(r.content)))
    body = r.content.decode("utf-8", "replace")
    for m in re.findall(r'<a[^>]*href="([^"]+\.rdp[^"]*)"', body):
        print("  rdp link: %s" % m)
    return body


if __name__ == "__main__":
    user = sys.argv[1] if len(sys.argv) > 1 else "svcaccount"
    pw = sys.argv[2] if len(sys.argv) > 2 else "svcaccount"
    s = login(user, pw)
    if s:
        feed(s)
