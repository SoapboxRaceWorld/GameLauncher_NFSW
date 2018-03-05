using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLauncher {
    class Decryptor {
        unsafe public static void apply_cipher(sbyte pBuf, uint len) {
            uint eax;
            uint ecx = 0;
            uint edi = 0x00519753;
            for (int i = 0; i < len; ++i) {
                eax = edi;
                eax ^= 0x1D872B41;
                ecx = eax >> 0x5;
                ecx ^= eax;
                edi = ecx << 0x1B;
                edi ^= ecx;
                ecx = 0x00B0ED68;
                edi ^= eax;
                eax = edi >> 0x17;
                pBuf ^= (sbyte)eax;
                ++pBuf;
            }
        }
    }
}
