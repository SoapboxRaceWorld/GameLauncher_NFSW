using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO.Compression;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using GameLauncher.App.Classes;
using GameLauncher.HashPassword;

namespace GameLauncherReborn {
    public class WebClientWithTimeout : WebClient {
        private static string GameLauncherHash = string.Empty;
        public static string Value() {
            if (string.IsNullOrEmpty(GameLauncherHash)) {
                GameLauncherHash = createHash(AppDomain.CurrentDomain.FriendlyName);
            }

            return GameLauncherHash;
        }

        public static string createHash(string filename) {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] retVal = sha1.ComputeHash(File.OpenRead(filename));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "GameLauncher (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)";
			if (!DetectLinux.NativeLinuxDetected())
			{
				request.Headers["X-HWID"] = Security.FingerPrint.Value();
			} else {
				request.Headers["X-HWID"] = "1234";
			}
            request.Headers["X-GameLauncherHash"] = Value();
            request.Timeout = 30000;

            return request;
        }
    }
}