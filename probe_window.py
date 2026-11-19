"""Measure the RDG Duo plugin's approval-cache window (if any).

Establishes serialized tunnels with growing gaps and times each tunnel-auth:
  fast (<2s)  = auto-approved (cache hit, no push)
  slow        = fresh push (awaited manual approval)
One tunnel = at most one push; the FIRST needs the operator's approval.
"""
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
import rdetour

GW = "ECORP-RDS.ecorp.local"
GAPS = [0, 2, 5, 15, 45, 90]   # seconds after the previous tunnel AUTH
FAST_MS = 2000

results = []
prev_auth_done = None

for i, gap in enumerate(GAPS, 1):
    if prev_auth_done is not None and gap > 0:
        print("[gap] waiting %ds ..." % gap, file=sys.stderr, flush=True)
        time.sleep(gap)

    t = rdetour.RDGTunnel(GW, 443, "ECORP.local", "testuser", "testuser")

    # time the auth stage specifically (the push point)
    timings = {}

    def timed_auth(timeout, _orig=rdetour.RDGTunnel._tunnel_auth, _t=t,
                   _tim=timings):
        s = time.time()
        r = _orig(_t, timeout)
        _tim["auth_ms"] = (time.time() - s) * 1000
        return r

    t._tunnel_auth = timed_auth
    if i == 1:
        print("[*] tunnel 1: approve the push on the phone (45s window)",
              file=sys.stderr, flush=True)
    try:
        t.establish(auth_timeout=45)
    except Exception as e:
        print("[!] tunnel %d failed: %r" % (i, e), file=sys.stderr)
        break
    ms = timings.get("auth_ms", -1)
    verdict = "FAST (auto-approved — cache hit)" if 0 <= ms < FAST_MS \
        else "SLOW (pushed/approved)"
    results.append((i, gap, ms, verdict))
    print("[tunnel %d] gap=%3ds  auth=%7.1fms  -> %s"
          % (i, gap, ms, verdict), file=sys.stderr, flush=True)
    t.close()
    prev_auth_done = time.time()

print("\n=== summary ===", file=sys.stderr)
hits = [r for r in results if r[2] < FAST_MS]
if not hits:
    print("no cache hits — every tunnel pushed. No window.", file=sys.stderr)
elif len(hits) == len(results) - 1:
    print("only tunnel 1 pushed — window covers >= %ds (max gap tested)"
          % max(GAPS[1:]), file=sys.stderr)
else:
    last_hit_gap = max(r[1] for r in hits)
    next_miss = min(r[1] for r in results if r[2] >= FAST_MS and r[1] > 0)
    print("window is between %ds and %ds" % (last_hit_gap, next_miss),
          file=sys.stderr)
print("total pushes you should have seen on the phone: %d"
      % (len(results) - len(hits)), file=sys.stderr)
