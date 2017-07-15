using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes {
    class Downloader {
        public string DownloadFiles(string url) {
            
            return url;
        }

        public static string DecompressLZMA(byte[] compressedFile) {
            IntPtr intPtr = new IntPtr((int)compressedFile.Length - 13);
            byte[] numArray = new byte[checked(intPtr.ToInt64())];
            IntPtr intPtr1 = new IntPtr(5);
            byte[] numArray1 = new byte[5];
            compressedFile.CopyTo(numArray, 13);

            for (int i = 0; i < 5; i++) {
                numArray1[i] = compressedFile[i];
            }

            int num = 0;

            for (int j = 0; j < 8; j++) {
                num = num + (compressedFile[j + 5] << (8 * j & 31));
            }

            IntPtr intPtr2 = new IntPtr(num);
            byte[] numArray2 = new byte[num];
            int num1 = LZMA.LzmaUncompress(numArray2, ref intPtr2, numArray, ref intPtr, numArray1, intPtr1);

            if (num1 != 0) {
                MessageBox.Show("Failed to uncompress file.");
            }

            numArray = null;
            return new string(Encoding.UTF8.GetString(numArray2).ToCharArray());
        }
    }
}
