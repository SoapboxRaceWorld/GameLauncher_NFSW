using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GameLauncher.Resources
{
    class FontWrapper {
        internal static class UnsafeNativeMethods {
            [DllImport("gdi32.dll")]
            public static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        }

        private PrivateFontCollection mPrivateFontCollection;
        private Dictionary<string, int> mFontMapping;
        private static FontWrapper _instance = new FontWrapper();
        public static FontWrapper Instance {
            get {
                return FontWrapper._instance;
            }
        }

        private FontWrapper() {
            this.mPrivateFontCollection = new PrivateFontCollection();
            this.mFontMapping = new Dictionary<string, int>();
        }

        public FontFamily GetFontFamily(string fontName) {
            if (this.mFontMapping.ContainsKey(fontName)) {
                return this.mPrivateFontCollection.Families[this.mFontMapping[fontName]];
            }

            int num = this.LoadEmbeddedFont(fontName);
            return this.mPrivateFontCollection.Families[num];
        }

        private int LoadEmbeddedFont(string fontName) {
            Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("GameLauncher.Fonts." + fontName);
            IntPtr intPtr = Marshal.AllocCoTaskMem((int)manifestResourceStream.Length);
            byte[] array = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
            Marshal.Copy(array, 0, intPtr, (int)manifestResourceStream.Length);
            uint num = 0u;
            FontWrapper.UnsafeNativeMethods.AddFontMemResourceEx(intPtr, (uint)array.Length, IntPtr.Zero, ref num);
            this.mPrivateFontCollection.AddMemoryFont(intPtr, (int)manifestResourceStream.Length);
            manifestResourceStream.Close();
            Marshal.FreeCoTaskMem(intPtr);
            this.mFontMapping.Add(fontName, this.mPrivateFontCollection.Families.Length - 1);
            return this.mPrivateFontCollection.Families.Length - 1;
        }
    }
}
