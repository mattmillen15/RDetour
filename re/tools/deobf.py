import re, sys

CONST_RE = re.compile(r'int num = \(int\)\(\(nint\)\((\d+) \+ A_1\)((?: \+ \(nint\)\d+| \+ \d+)+)\);')
CALL_RE = re.compile(r'(?:global::)?<Module>\.b\(|[A-Za-z_][A-Za-z0-9_]*\.b\(|(?<![A-Za-z0-9_.])b\(')
ARG_RE = re.compile(r',\s*(\d+|[A-Za-z_][A-Za-z0-9_]*)\s*,')
WS = ' \t\r\n'

def unescape_cs(s):
    out = []; i = 0
    while i < len(s):
        c = s[i]
        if c == '\\' and i + 1 < len(s):
            n = s[i+1]
            if n == 'u' and i + 5 < len(s):
                out.append(chr(int(s[i+2:i+6], 16))); i += 6; continue
            if n == 'U' and i + 9 < len(s):
                out.append(chr(int(s[i+2:i+10], 16))); i += 10; continue
            mp = {'n':'\n','r':'\r','t':'\t','\\':'\\','"':'"',"'":"'",'0':'\0','a':'\a','b':'\b','f':'\f','v':'\v'}
            out.append(mp.get(n, n)); i += 2; continue
        out.append(c); i += 1
    return ''.join(out)

def decode(s, base):
    num = base; out = []
    for ch in s:
        lo = (ord(ch) & 0xFF) ^ (num & 0xFF); num = (num + 1) & 0xFFFFFFFF
        hi = ((ord(ch) >> 8) & 0xFF) ^ (num & 0xFF); num = (num + 1) & 0xFFFFFFFF
        out.append(chr(((lo << 8) | hi) & 0xFFFF))
    return ''.join(out)

def scan_quoted(src, q):
    j = q + 1
    while j < len(src):
        if src[j] == '\\': j += 2; continue
        if src[j] == '"': return j
        j += 1
    return -1

def find_call(src, open_paren, call_start):
    i = open_paren + 1
    if i >= len(src) or src[i] != '"': return None
    j = scan_quoted(src, i)
    if j < 0: return None
    raw = src[i+1:j]
    m = ARG_RE.match(src, j + 1)
    if not m: return None
    arg = m.group(1)
    if arg.isdigit():
        n = int(arg)
    else:
        am = None
        for am in re.finditer('int %s = (\\d+);' % re.escape(arg), src[:call_start]):
            pass
        if am is None: return None
        n = int(am.group(1))
    q = m.end()
    while q < len(src) and src[q] in WS:
        q += 1
    if q < len(src) and src[q] == '"':
        j2 = scan_quoted(src, q)
        if j2 < 0: return None
        q = j2 + 1
    else:
        mq = re.compile(r'[A-Za-z_][A-Za-z0-9_]*').match(src, q)
        if not mq: return None
        q = mq.end()
    while q < len(src) and src[q] in WS:
        q += 1
    if q < len(src) and src[q] == ')':
        return (raw, n, q + 1)
    return None

def process(path):
    src = open(path, encoding='utf-8', errors='replace').read()
    mc = CONST_RE.search(src)
    if not mc:
        print(path + ': no constant'); return
    base = int(mc.group(1)) + sum(int(x) for x in re.findall(r'\d+', mc.group(2)))
    out = []; pos = 0; count = 0
    for m in CALL_RE.finditer(src):
        if m.start() < pos: continue
        res = find_call(src, m.end() - 1, m.start())
        if not res: continue
        raw, n, end = res
        dec = decode(unescape_cs(raw), base + n)
        if all(32 <= ord(c) < 127 or c in '\r\n\t' for c in dec):
            out.append(src[pos:m.start()])
            out.append('"' + dec.replace('\\', '\\\\').replace('"', '\\"').replace('\r', '\\r').replace('\n', '\\n') + '"')
            pos = end; count += 1
    out.append(src[pos:])
    open(path.replace('.cs', '_dec.cs'), 'w', encoding='utf-8').write(''.join(out))
    print('%s: base=%d decoded=%d' % (path, base, count))

for p in sys.argv[1:]:
    process(p)
