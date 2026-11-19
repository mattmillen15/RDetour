│           0x180054700      2420           and al, 0x20               ; 32
│           0x180054702      df5907         fistp word [rcx + 7]
│           0x180054705      804c8d05a3     or byte [rbp + rcx*4 + 5], 0xa3 ; [0xa3:1]=255 ; 163
│           0x18005470a      840500488b0d   test byte [0x18d908f10], al ; [0x18d908f10:1]=255
│           0x180054710      f4             hlt
│           0x180054711      3807           cmp byte [rdi], al
│           0x180054713      00488b         add byte [rax - 0x75], cl
│           0x180054716      4910e8         adc r8b, bpl
│           0x180054719      cf             iretd
│           0x18005471a      e6fa           out 0xfa, al
│           0x18005471c      ff             invalid
│       ┌─< 0x18005471d      e964010000     jmp 0x180054886
│       │   ; CODE XREF from fcn.180054620 @ 0x1800546c6(x)
│       │   0x180054722      b928000000     mov ecx, Ordinal_41        ; '('
│       │                                                              ; 0x28 ; 40
│       │   0x180054727      e8f8300400     call fcn.180097824
│       │   0x18005472c      488bd8         mov rbx, rax
│       │   0x18005472f      4889442470     mov qword [var_70h], rax
│       │   0x180054734      4885c0         test rax, rax
│      ┌──< 0x180054737      7460           je 0x180054799
│      ││   0x180054739      488d05d8ca..   lea rax, [0x1800a1218]
│      ││   0x180054740      48894308       mov qword [rbx + 8], rax
│      ││   0x180054744      488d059d6d..   lea rax, [0x18009b4e8]
│      ││   0x18005474b      48894318       mov qword [rbx + 0x18], rax
│      ││   0x18005474f      c743200100..   mov dword [rbx + 0x20], 1
│      ││   0x180054756      488d05fb6a..   lea rax, [0x18009b258]
│      ││   0x18005475d      488903         mov qword [rbx], rax
│      ││   0x180054760      488b4308       mov rax, qword [rbx + 8]
│      ││   0x180054764      48634804       movsxd rcx, dword [rax + 4]
│      ││   0x180054768      488d05016b..   lea rax, [0x18009b270]
│      ││   0x18005476f      4889441908     mov qword [rcx + rbx + 8], rax
│      ││   0x180054774      488d05ed7e..   lea rax, [0x18009c668]
│      ││   0x18005477b      488903         mov qword [rbx], rax
│      ││   0x18005477e      488b4308       mov rax, qword [rbx + 8]
│      ││   0x180054782      48634804       movsxd rcx, dword [rax + 4]
│      ││   0x180054786      488d05e37e..   lea rax, [0x18009c670]
│      ││   0x18005478d      4889441908     mov qword [rcx + rbx + 8], rax
│      ││   0x180054792      4883631000     and qword [rbx + 0x10], 0
│     ┌───< 0x180054797      eb02           jmp 0x18005479b
│     │││   ; CODE XREF from fcn.180054620 @ 0x180054737(x)
│     │└──> 0x180054799      33db           xor ebx, ebx
│     │ │   ; CODE XREF from fcn.180054620 @ 0x180054797(x)
│     └───> 0x18005479b      4885db         test rbx, rbx
│      ┌──< 0x18005479e      7557           jne 0x1800547f7
│      ││   0x1800547a0      488b056138..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│      ││   0x1800547a7      483bc6         cmp rax, rsi
│     ┌───< 0x1800547aa      7441           je 0x1800547ed
│     │││   0x1800547ac      f6401c08       test byte [rax + 0x1c], 8
│    ┌────< 0x1800547b0      743b           je 0x1800547ed
│    ││││   0x1800547b2      80781902       cmp byte [rax + 0x19], 2
│   ┌─────< 0x1800547b6      7235           jb 0x1800547ed
│   │││││   0x1800547b8      e803f5faff     call fcn.180003cc0
│   │││││   0x1800547bd      448bc8         mov r9d, eax
│   │││││   0x1800547c0      8d532d         lea edx, [rbx + 0x2d]
│   │││││   0x1800547c3      488d056686..   lea rax, str.CChannelSink  ; 0x1800ace30 ; "CChannelSink"
│   │││││   0x1800547ca      4889442420     mov qword [var_20h], rax
│   │││││   0x1800547cf      4c8d05da83..   lea r8, [0x1800acbb0]
│   │││││   0x1800547d6      488b0d2b38..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│   │││││   0x1800547dd      488b4910       mov rcx, qword [rcx + 0x10]
│   │││││   0x1800547e1      e8bef9faff     call fcn.1800041a4
│   │││││   0x1800547e6      488b051b38..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│   │││││   ; CODE XREFS from fcn.180054620 @ 0x1800547aa(x), 0x1800547b0(x), 0x1800547b6(x)
│   └└└───> 0x1800547ed      bf0e000780     mov edi, 0x8007000e        ; '\x0e'
│     ┌───< 0x1800547f2      e996000000     jmp 0x18005488d
│     │││   ; CODE XREF from fcn.180054620 @ 0x18005479e(x)
│     │└──> 0x1800547f7      498b4608       mov rax, qword [r14 + 8]
│     │ │   0x1800547fb      48634804       movsxd rcx, dword [rax + 4]
│     │ │   0x1800547ff      4883c108       add rcx, 8
│     │ │   0x180054803      4903ce         add rcx, r14
│     │ │   0x180054806      488b01         mov rax, qword [rcx]
│     │ │   0x180054809      488b00         mov rax, qword [rax]
│     │ │   0x18005480c      ff158e9f0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│     │ │   0x180054812      4c897310       mov qword [rbx + 0x10], r14
│     │ │   0x180054816      48895c2438     mov qword [var_38h], rbx
│     │ │   0x18005481b      8a4718         mov al, byte [rdi + 0x18]
│     │ │   0x18005481e      88442430       mov byte [var_30h], al
│     │ │   0x180054822      488b4710       mov rax, qword [rdi + 0x10]
│     │ │   0x180054826      4889442428     mov qword [var_28h], rax
│     │ │   0x18005482b      0fb7471c       movzx eax, word [rdi + 0x1c]
│     │ │   0x18005482f      6689442420     mov word [var_20h], ax
│     │ │   0x180054834      440fb74f1a     movzx r9d, word [rdi + 0x1a]
│     │ │   0x180054839      448b4708       mov r8d, dword [rdi + 8]
│     │ │   0x18005483d      488b17         mov rdx, qword [rdi]
│     │ │   0x180054840      488bcd         mov rcx, rbp
│     │ │   0x180054843      e88064ffff     call fcn.18004acc8
│     │ │   0x180054848      8bf8           mov edi, eax
│     │ │   0x18005484a      85c0           test eax, eax
│     │┌──< 0x18005484c      7938           jns 0x180054886
│     │││   0x18005484e      488b05b337..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│     │││   0x180054855      483bc6         cmp rax, rsi
│    ┌────< 0x180054858      7433           je 0x18005488d
│    ││││   0x18005485a      f6401c08       test byte [rax + 0x1c], 8
│   ┌─────< 0x18005485e      742d           je 0x18005488d
│   │││││   0x180054860      80781902       cmp byte [rax + 0x19], 2
│  ┌──────< 0x180054864      7227           jb 0x18005488d
│  ││││││   0x180054866      ba2e000000     mov edx, reloc.WLDAP32.dll_Ordinal_46 ; 0x2e
│  ││││││   0x18005486b      897c2420       mov dword [var_20h], edi
│  ││││││   0x18005486f      4c8d0dca85..   lea r9, str.pTunnel__CreateChannel ; 0x1800ace40 ; u"pTunnel->CreateChannel"
│  ││││││   0x180054876      4c8d053383..   lea r8, [0x1800acbb0]
│  ││││││   0x18005487d      488b4810       mov rcx, qword [rax + 0x10]
│  ││││││   0x180054881      e856fcfaff     call fcn.1800044dc
│  ││││││   ; CODE XREFS from fcn.180054620 @ 0x18005471d(x), 0x18005484c(x)
│  ││││└└─> 0x180054886      488b057b37..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│  ││││     ; XREFS: CODE 0x1800546d7  CODE 0x1800546e1  CODE 0x1800546eb  
│  ││││     ; XREFS: CODE 0x1800547f2  CODE 0x180054858  CODE 0x18005485e  
│  ││││     ; XREFS: CODE 0x180054864  
│  └└└└───> 0x18005488d      83bdc00800..   cmp dword [rbp + 0x8c0], 0
│       ┌─< 0x180054894      7413           je 0x1800548a9
│       │   0x180054896      488b8db808..   mov rcx, qword [rbp + 0x8b8]
│       │   0x18005489d      e8b2fbfaff     call fcn.180004454
│       │   0x1800548a2      488b055f37..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│       │   ; CODE XREF from fcn.180054620 @ 0x180054894(x)
│       └─> 0x1800548a9      4885db         test rbx, rbx
│       ┌─< 0x1800548ac      7423           je 0x1800548d1
│       │   0x1800548ae      488b4308       mov rax, qword [rbx + 8]
│       │   0x1800548b2      48634804       movsxd rcx, dword [rax + 4]
│       │   0x1800548b6      4883c108       add rcx, 8
│       │   0x1800548ba      4803cb         add rcx, rbx
│       │   0x1800548bd      488b01         mov rax, qword [rcx]
│       │   0x1800548c0      488b4008       mov rax, qword [rax + 8]
│       │   0x1800548c4      ff15d69e0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│       │   0x1800548ca      488b053737..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│       │   ; CODE XREF from fcn.180054620 @ 0x1800548ac(x)
│       └─> 0x1800548d1      85ff           test edi, edi
│       ┌─< 0x1800548d3      0f89a1000000   jns 0x18005497a
│      ┌──< 0x1800548d9      eb57           jmp 0x180054932
│      ││   ; CODE XREFS from fcn.180054620 @ 0x180054696(x), 0x18005469f(x)
│      ││   0x1800548db      bf57000780     mov edi, 0x80070057        ; 'W'
│      ││   0x1800548e0      488d352137..   lea rsi, [0x1800c8008]
│      ││   0x1800548e7      488b051a37..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│      ││   0x1800548ee      483bc6         cmp rax, rsi
│     ┌───< 0x1800548f1      7478           je 0x18005496b
│     │││   0x1800548f3      f6401c01       test byte [rax + 0x1c], 1
│    ┌────< 0x1800548f7      7439           je 0x180054932
│    ││││   0x1800548f9      80781902       cmp byte [rax + 0x19], 2
│   ┌─────< 0x1800548fd      7233           jb 0x180054932
│   │││││   0x1800548ff      e8bcf3faff     call fcn.180003cc0
│   │││││   0x180054904      ba2b000000     mov edx, Ordinal_46        ; '+'
│   │││││                                                              ; 0x2b ; 43
│   │││││   0x180054909      c744242057..   mov dword [var_20h], 0x80070057 ; 'W'
│   │││││                                                              ; [0x80070057:4]=-1
│   │││││   ; CODE XREF from fcn.180054620 @ 0x18005468e(x)
│   │││││   0x180054911      488b0df036..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│   │││││   0x180054918      448bc8         mov r9d, eax
│   │││││   0x18005491b      4c8d058e82..   lea r8, [0x1800acbb0]
│   │││││   0x180054922      488b4910       mov rcx, qword [rcx + 0x10]
│   │││││   0x180054926      e8c1e4faff     call fcn.180002dec
│   │││││   0x18005492b      488b05d636..   mov rax, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│   │││││   ; CODE XREFS from fcn.180054620 @ 0x18005466e(x), 0x180054678(x), 0x1800548d9(x), 0x1800548f7(x), 0x1800548fd(x)
│   └└─└──> 0x180054932      483bc6         cmp rax, rsi
│     │┌──< 0x180054935      7434           je 0x18005496b
│     │││   0x180054937      f6401c01       test byte [rax + 0x1c], 1
│    ┌────< 0x18005493b      742e           je 0x18005496b
│    ││││   0x18005493d      80781902       cmp byte [rax + 0x19], 2
│   ┌─────< 0x180054941      7228           jb 0x18005496b
│   │││││   0x180054943      e878f3faff     call fcn.180003cc0
│   │││││   0x180054948      448bc8         mov r9d, eax
│   │││││   0x18005494b      ba2f000000     mov edx, 0x2f              ; '/' ; 47
│   │││││   0x180054950      897c2420       mov dword [var_20h], edi
│   │││││   0x180054954      4c8d055582..   lea r8, [0x1800acbb0]
│   │││││   0x18005495b      488b0da636..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│   │││││   0x180054962      488b4910       mov rcx, qword [rcx + 0x10]
│   │││││   0x180054966      e881e4faff     call fcn.180002dec
│   │││││   ; CODE XREFS from fcn.180054620 @ 0x180054664(x), 0x1800548f1(x), 0x180054935(x), 0x18005493b(x), 0x180054941(x)
│   └└└└──> 0x18005496b      833d364807..   cmp dword [0x1800c91a8], 0 ; [0x1800c91a8:4]=0
│      ┌──< 0x180054972      7406           je 0x18005497a
│      ││   0x180054974      ff052a480700   inc dword [0x1800c91a4]    ; [0x1800c91a4:4]=0
│      ││   ; CODE XREFS from fcn.180054620 @ 0x1800548d3(x), 0x180054972(x)
│      └└─> 0x18005497a      8bc7           mov eax, edi
│           0x18005497c      4c8d5c2450     lea r11, [var_50h]
│           0x180054981      498b5b28       mov rbx, qword [r11 + 0x28]
│           0x180054985      498b6b30       mov rbp, qword [r11 + 0x30]
│           0x180054989      498be3         mov rsp, r11
│           0x18005498c      415e           pop r14
│           0x18005498e      5f             pop rdi
│           0x18005498f      5e             pop rsi
└           0x180054990      c3             ret
            0x180054991      cc             int3
            0x180054992      cc             int3
            0x180054993      cc             int3
            0x180054994      cc             int3
            0x180054995      cc             int3
            0x180054996      cc             int3
            0x180054997      cc             int3
            ; CALL XREF from fcn.1800543f8 @ 0x1800545e7(x)
            ; CALL XREF from fcn.18005b9a0 @ 0x18005ba02(x)
            ; CALL XREF from fcn.180066428 @ 0x180066599(x)
            ; CALL XREF from fcn.18008d0e0 @ 0x18008d2a6(x)
┌ 129: fcn.180054998 (int64_t arg1);
│ `- args(rcx) vars(2:sp[0x20..0x40])
│           0x180054998      4885c9         test rcx, rcx              ; arg1
│       ┌─< 0x18005499b      747b           je 0x180054a18
│       │   0x18005499d      48895c2408     mov qword [var_8h], rbx
│       │   0x1800549a2      57             push rdi
│       │   0x1800549a3      4883ec30       sub rsp, 0x30
│       │   0x1800549a7      488b01         mov rax, qword [rcx]       ; arg1
│       │   0x1800549aa      488bd9         mov rbx, rcx               ; arg1
│       │   0x1800549ad      488b4028       mov rax, qword [rax + 0x28]
│       │   0x1800549b1      ff15e99d0400   call qword [0x18009e7a0]   ; [0x18009e7a0:8]=0x180098a90
│       │   0x1800549b7      8bf8           mov edi, eax
│       │   0x1800549b9      488b0d4836..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│       │   0x1800549c0      488d054136..   lea rax, [0x1800c8008]
│       │   0x1800549c7      483bc8         cmp rcx, rax
│      ┌──< 0x1800549ca      7434           je 0x180054a00
│      ││   0x1800549cc      f6411c01       test byte [rcx + 0x1c], 1
│     ┌───< 0x1800549d0      742e           je 0x180054a00
│     │││   0x1800549d2      80791902       cmp byte [rcx + 0x19], 2
│    ┌────< 0x1800549d6      7228           jb 0x180054a00
│    ││││   0x1800549d8      e8e3f2faff     call fcn.180003cc0
│    ││││   0x1800549dd      488b0d2436..   mov rcx, qword [0x1800c8008] ; [0x1800c8008:8]=0x1800c8008
│    ││││   0x1800549e4      4c8d05c581..   lea r8, [0x1800acbb0]
│    ││││   0x1800549eb      448bc8         mov r9d, eax
│    ││││   0x1800549ee      897c2420       mov dword [var_20h], edi
│    ││││   0x1800549f2      ba31000000     mov edx, 0x31              ; '1' ; 49
│    ││││   0x1800549f7      488b4910       mov rcx, qword [rcx + 0x10]
│    ││││   0x1800549fb      e8ece3faff     call fcn.180002dec
