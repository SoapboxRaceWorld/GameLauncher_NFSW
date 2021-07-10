using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GameLauncher.App.Classes.Hash
{
    class SHA
    {
        public static string Hashes(string input)
        {
            HashAlgorithm algorithm = SHA1.Create();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string Files(string filename)
        {
            if (!File.Exists(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(filename)))) return String.Empty;

            SHA1 sha1 = new SHA1CryptoServiceProvider();

            byte[] retVal = new byte[] { };

            using (var test = File.OpenRead(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(filename))))
            {
                retVal = sha1.ComputeHash(test);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }
    }
}
