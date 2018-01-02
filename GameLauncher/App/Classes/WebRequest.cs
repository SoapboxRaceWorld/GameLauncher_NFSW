using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace GameLauncherReborn {
    public class WebClientWithTimeout : WebClient {
        public static string createHash(string filename) {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(File.OpenRead(filename));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++) {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }

        protected override WebRequest GetWebRequest(Uri address) {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
                request.UserAgent = "GameLauncher (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)";
                request.Headers["X-HWID"] = Security.FingerPrint.Value();
                request.Headers["X-GameLauncherHash"] = createHash(AppDomain.CurrentDomain.FriendlyName);
                request.Timeout = 10000;
                return request;
        }
    }
}