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

        [Obsolete("WebClient.createHash is deprecated, please use SHA.HashFile instead.")]
        public static string createHash(string filename) {
            return SHA.HashFile(filename);
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "GameLauncher (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-HWID"] = Security.FingerPrint.Value();
            request.Headers["X-UserAgent"] = "GameLauncherReborn "+Application.ProductVersion+" WinForms (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = Value();
            request.Timeout = 30000;

            return request;
        }
    }
}