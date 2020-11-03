using System.Security.Cryptography;
using System.Text;

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

        /* Moved MD5 HashFile to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/72c6c655b0e56481a17f730099cddd1f5d427a94/MD5.cs */
    }
}
