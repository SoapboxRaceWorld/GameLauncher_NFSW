using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameLauncher.HashPassword
{
    class SHA {
        public static string HashPassword(string input) {
            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(input))) {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string HashFile(string filename) {
            if(!File.Exists(filename)) return String.Empty;

            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] retVal = new byte[] { };

            using (var test = File.OpenRead(filename)) {
                retVal = sha1.ComputeHash(test);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }
    }
}
