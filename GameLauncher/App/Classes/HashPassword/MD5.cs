using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.HashPassword
{
    class MDFive {
        public static string HashPassword(string input) {
            HashAlgorithm algorithm = MD5.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(input))) {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string HashFile(string filename) {
            if (!File.Exists(filename)) return String.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = new byte[] { };

            using (var test = File.OpenRead(filename)) {
                retVal = md5.ComputeHash(test);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }
    }
}
