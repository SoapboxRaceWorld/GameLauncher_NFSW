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
    }
}
