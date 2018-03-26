using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GameLauncher.HashPassword {
    class SHA {
        public static string HashPassword(string input) {
            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(input))) {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
