using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GameLauncher.App.Classes {
    internal static class LZMA  {
        [DllImport("LZMA.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int LzmaUncompress(byte[] dest, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);

        [DllImport("LZMA.dll", CharSet = CharSet.None, ExactSpelling = false)]
        public static extern int LzmaUncompressBuf2File(string destFile, ref IntPtr destLen, byte[] src, ref IntPtr srcLen, byte[] outProps, IntPtr outPropsSize);
    }
}
