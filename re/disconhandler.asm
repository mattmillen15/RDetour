            ; CALL XREF from fcn.180050c58 @ +0xda(x)
┌ 632: fcn.180052988 (int64_t arg1, int64_t arg2);
│ `- args(rcx, rdx) vars(9:sp[0x8..0x290])
│           0x180052988      48895c2418     mov qword [var_18h], rbx
│           0x18005298d      48896c2420     mov qword [var_20h], rbp
│           0x180052992      56             push rsi
│           0x180052993      57             push rdi
│           0x180052994      4157           push r15
│           0x180052996      4881ec7002..   sub rsp, 0x270
│           0x18005299d      488b05a45a..   mov rax, qword [0x1800c8448] ; [0x1800c8448:8]=0x2b992ddfa232
│           0x1800529a4      4833c4         xor rax, rsp
│           0x1800529a7      4889842460..   mov qword [var_260h], rax
│           0x1800529af      488b02         mov rax, qword [rdx]       ; arg2
│           0x1800529b2      488bf9         mov rdi, rcx               ; arg1
│           0x1800529b5      488364244000   and qword [var_40h], 0
│           0x1800529bb      488bca         mov rcx, rdx               ; arg2
│           0x1800529be      488bda         mov rbx, rdx               ; arg2
│           0x1800529c1      488b4018       mov rax, qword [rax + 0x18]
│           0x1800529c5      ff15d5bd0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│           0x1800529cb      4c8d3d3656..   lea r15, [0x1800c8008]
│           0x1800529d2      85c0           test eax, eax
│       ┌─< 0x1800529d4      0f8408010000   je 0x180052ae2
│       │   0x1800529da      488b03         mov rax, qword [rbx]
│       │   0x1800529dd      488bcb         mov rcx, rbx
│       │   0x1800529e0      488b4010       mov rax, qword [rax + 0x10]
│       │   0x1800529e4      ff15b6bd0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│       │   0x1800529ea      4885c0         test rax, rax
│      ┌──< 0x1800529ed      0f84ef000000   je 0x180052ae2
│      ││   0x1800529f3      488b03         mov rax, qword [rbx]
│      ││   0x1800529f6      488bcb         mov rcx, rbx
│      ││   0x1800529f9      488b4010       mov rax, qword [rax + 0x10]
│      ││   0x1800529fd      ff159dbd0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│      ││   0x180052a03      833800         cmp dword [rax], 0
│     ┌───< 0x180052a06      0f84d6000000   je 0x180052ae2
│     │││   0x180052a0c      4c8bc3         mov r8, rbx                ; int64_t arg3
│     │││   0x180052a0f      488d542440     lea rdx, [var_40h]         ; int64_t arg2
│     │││   0x180052a14      488bcf         mov rcx, rdi               ; uint32_t arg1
│     │││   0x180052a17      e848feffff     call fcn.180052864
│     │││   0x180052a1c      8bd8           mov ebx, eax
│     │││   0x180052a1e      85c0           test eax, eax
│    ┌────< 0x180052a20      7949           jns 0x180052a6b
│    ││││   0x180052a22      488b0ddf55..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│    ││││   0x180052a29      493bcf         cmp rcx, r15
│   ┌─────< 0x180052a2c      0f8467010000   je 0x180052b99
│   │││││   0x180052a32      f6411c08       test byte [rcx + 0x1c], 8
│  ┌──────< 0x180052a36      0f845d010000   je 0x180052b99
│  ││││││   0x180052a3c      80791902       cmp byte [rcx + 0x19], 2
│ ┌───────< 0x180052a40      0f8253010000   jb 0x180052b99
│ │││││││   0x180052a46      ba13000000     mov edx, setsockopt        ; 0x13 ; 19
│ │││││││   0x180052a4b      4c8d0d3ea0..   lea r9, str.GetIoWriteToClientPacket ; 0x1800aca90 ; u"GetIoWriteToClientPacket"
│ │││││││   ; CODE XREF from fcn.180052988 @ 0x180052add(x)
│ ────────> 0x180052a52      488b4910       mov rcx, qword [rcx + 0x10]
│ │││││││   0x180052a56      4c8d0543a1..   lea r8, [0x1800acba0]
│ │││││││   0x180052a5d      89442420       mov dword [var_20h_2], eax
│ │││││││   0x180052a61      e8761afbff     call fcn.1800044dc
│ ────────< 0x180052a66      e927010000     jmp 0x180052b92
│ │││││││   ; CODE XREF from fcn.180052988 @ 0x180052a20(x)
│ │││└────> 0x180052a6b      8b4760         mov eax, dword [rdi + 0x60]
│ │││ │││   0x180052a6e      85c0           test eax, eax
│ │││┌────< 0x180052a70      740c           je 0x180052a7e
│ │││││││   0x180052a72      488b4f58       mov rcx, qword [rdi + 0x58]
│ │││││││   0x180052a76      e89d19fbff     call fcn.180004418
│ │││││││   0x180052a7b      8b4760         mov eax, dword [rdi + 0x60]
│ │││││││   ; CODE XREF from fcn.180052988 @ 0x180052a70(x)
│ │││└────> 0x180052a7e      488d7738       lea rsi, [rdi + 0x38]
│ │││ │││   0x180052a82      488b2e         mov rbp, qword [rsi]
│ │││ │││   0x180052a85      85c0           test eax, eax
│ │││┌────< 0x180052a87      7409           je 0x180052a92
│ │││││││   0x180052a89      488b4f58       mov rcx, qword [rdi + 0x58]
│ │││││││   0x180052a8d      e8c219fbff     call fcn.180004454
│ │││││││   ; CODE XREF from fcn.180052988 @ 0x180052a87(x)
│ │││└────> 0x180052a92      483bee         cmp rbp, rsi
│ │││┌────< 0x180052a95      0f85f7000000   jne 0x180052b92
│ │││││││   0x180052a9b      488bcf         mov rcx, rdi               ; uint32_t arg1
│ │││││││   0x180052a9e      e841030000     call fcn.180052de4
│ │││││││   0x180052aa3      8bd8           mov ebx, eax
│ │││││││   0x180052aa5      85c0           test eax, eax
│ ────────< 0x180052aa7      0f8929010000   jns 0x180052bd6
│ │││││││   0x180052aad      488b0d5455..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│ │││││││   0x180052ab4      493bcf         cmp rcx, r15
│ ────────< 0x180052ab7      0f84dc000000   je 0x180052b99
│ │││││││   0x180052abd      f6411c08       test byte [rcx + 0x1c], 8
│ ────────< 0x180052ac1      0f84d2000000   je 0x180052b99
│ │││││││   0x180052ac7      80791902       cmp byte [rcx + 0x19], 2
│ ────────< 0x180052acb      0f82c8000000   jb 0x180052b99
│ │││││││   0x180052ad1      ba15000000     mov edx, reloc.WS2_32.dll_setsockopt ; 0x15
│ │││││││   0x180052ad6      4c8d0deb9f..   lea r9, str.PushNextWriteToClientPacket ; 0x1800acac8 ; u"PushNextWriteToClientPacket"
│ ────────< 0x180052add      e970ffffff     jmp 0x180052a52
│ │││││││   ; CODE XREFS from fcn.180052988 @ 0x1800529d4(x), 0x1800529ed(x), 0x180052a06(x)
│ ││││└└└─> 0x180052ae2      bb57000780     mov ebx, 0x80070057        ; 'W'
│ ││││      0x180052ae7      488b051a55..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│ ││││      0x180052aee      493bc7         cmp rax, r15
│ ││││  ┌─< 0x180052af1      7430           je 0x180052b23
│ ││││  │   0x180052af3      f6401c01       test byte [rax + 0x1c], 1
│ ││││ ┌──< 0x180052af7      742a           je 0x180052b23
│ ││││ ││   0x180052af9      80781902       cmp byte [rax + 0x19], 2
│ ││││┌───< 0x180052afd      7224           jb 0x180052b23
│ │││││││   0x180052aff      e8bc11fbff     call fcn.180003cc0
│ │││││││   0x180052b04      488b0dfd54..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│ │││││││   0x180052b0b      4c8d058ea0..   lea r8, [0x1800acba0]
│ │││││││   0x180052b12      448bc8         mov r9d, eax
│ │││││││   0x180052b15      ba12000000     mov edx, reloc.ACTIVEDS.dll_FreeADsStr ; reloc.WLDAP32.dll_Ordinal_18
│ │││││││                                                              ; 0x12
│ │││││││   0x180052b1a      488b4910       mov rcx, qword [rcx + 0x10]
│ │││││││   0x180052b1e      e83d12fbff     call fcn.180003d60
│ │││││││   ; CODE XREFS from fcn.180052988 @ 0x180052af1(x), 0x180052af7(x), 0x180052afd(x)
│ ││││└└└─> 0x180052b23      33d2           xor edx, edx               ; int c
│ ││││      0x180052b25      488d4c2450     lea rcx, [s]               ; void *s
│ ││││      0x180052b2a      41b808020000   mov r8d, 0x208             ; 520 ; size_t n
│ ││││      0x180052b30      e8355f0400     call sub.msvcrt.dll_memset ; void *memset(void *s, int c, size_t n)
│ ││││      0x180052b35      488d051c9f..   lea rax, str.CChannelIO::WriteToClient ; 0x1800aca58 ; u"CChannelIO::WriteToClient"
│ ││││      0x180052b3c      c744243057..   mov dword [var_30h], 0x80070057 ; 'W'
│ ││││                                                                 ; [0x80070057:4]=-1
│ ││││      0x180052b44      c7442428a1..   mov dword [var_28h], 0x1a1 ; [0x1a1:4]=-1 ; 417
│ ││││      0x180052b4c      4c8d0ded83..   lea r9, str.Calling_DisconnectChannel__. ; 0x1800aaf40 ; u"Calling DisconnectChannel()."
│ ││││      0x180052b53      4c8d0546e7..   lea r8, str.Disconnect_trace:_s_in__s_at__d_err_0x_x_ ; str.Disconnect_trace:_s_in__s_at__d_err_0x_x_
│ ││││                                                                 ; 0x1800a12a0 ; u"Disconnect trace:'%s' in %s at %d err=[0x%x]"
│ ││││      0x180052b5a      4889442420     mov qword [var_20h_2], rax
│ ││││      0x180052b5f      ba04010000     mov edx, 0x104             ; 260
│ ││││      0x180052b64      488d4c2450     lea rcx, [s]
│ ││││      0x180052b69      e82e17fbff     call fcn.18000429c
│ ││││      0x180052b6e      488d4c2450     lea rcx, [s]
│ ││││      0x180052b73      e810b5fdff     call fcn.18002e088
│ ││││      0x180052b78      488b4f18       mov rcx, qword [rdi + 0x18]
│ ││││      0x180052b7c      41b8d8590780   mov r8d, 0x800759d8
│ ││││      0x180052b82      8b5710         mov edx, dword [rdi + 0x10]
│ ││││      0x180052b85      488b01         mov rax, qword [rcx]
│ ││││      0x180052b88      488b4038       mov rax, qword [rax + 0x38]
│ ││││      0x180052b8c      ff150ebc0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│ ││││      ; CODE XREFS from fcn.180052988 @ 0x180052a66(x), 0x180052a95(x)
│ ───└────> 0x180052b92      488b0d6f54..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│ │││       ; XREFS: CODE 0x180052a2c  CODE 0x180052a36  CODE 0x180052a40  
│ │││       ; XREFS: CODE 0x180052ab7  CODE 0x180052ac1  CODE 0x180052acb  
│ └└└─────> 0x180052b99      85db           test ebx, ebx
│       ┌─< 0x180052b9b      7939           jns 0x180052bd6
│       │   0x180052b9d      493bcf         cmp rcx, r15
│      ┌──< 0x180052ba0      7434           je 0x180052bd6
│      ││   0x180052ba2      f6411c01       test byte [rcx + 0x1c], 1
│     ┌───< 0x180052ba6      742e           je 0x180052bd6
│     │││   0x180052ba8      80791902       cmp byte [rcx + 0x19], 2
│    ┌────< 0x180052bac      7228           jb 0x180052bd6
│    ││││   0x180052bae      e80d11fbff     call fcn.180003cc0
│    ││││   0x180052bb3      488b0d4e54..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│    ││││   0x180052bba      4c8d05df9f..   lea r8, [0x1800acba0]
│    ││││   0x180052bc1      448bc8         mov r9d, eax
│    ││││   0x180052bc4      895c2420       mov dword [var_20h_2], ebx
│    ││││   0x180052bc8      ba16000000     mov edx, SafeArrayGetElement ; 0x16 ; 22
│    ││││   0x180052bcd      488b4910       mov rcx, qword [rcx + 0x10]
│    ││││   0x180052bd1      e81602fbff     call fcn.180002dec
│    ││││   ; CODE XREFS from fcn.180052988 @ 0x180052aa7(x), 0x180052b9b(x), 0x180052ba0(x), 0x180052ba6(x), 0x180052bac(x)
│ ───└└└└─> 0x180052bd6      8bc3           mov eax, ebx
│           0x180052bd8      488b8c2460..   mov rcx, qword [var_260h]
│           0x180052be0      4833cc         xor rcx, rsp
│           0x180052be3      e8c8510400     call fcn.180097db0
│           0x180052be8      4c8d9c2470..   lea r11, [var_270h]
│           0x180052bf0      498b5b30       mov rbx, qword [r11 + 0x30]
│           0x180052bf4      498b6b38       mov rbp, qword [r11 + 0x38]
│           0x180052bf8      498be3         mov rsp, r11
│           0x180052bfb      415f           pop r15
│           0x180052bfd      5f             pop rdi
│           0x180052bfe      5e             pop rsi
└           0x180052bff      c3             ret
