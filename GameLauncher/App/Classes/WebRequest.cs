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
                GameLauncherHash = SHA.HashFile(AppDomain.CurrentDomain.FriendlyName);
            }

            return GameLauncherHash;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            if(DetectLinux.LinuxDetected()) {
                address = new UriBuilder(address) {
                    Scheme = Uri.UriSchemeHttp,
                    Port = address.IsDefaultPort ? -1 : address.Port // -1 => default port for scheme
                }.Uri;
            }
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = (Self.userAgent == null) ? "GameLauncher (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)" : Self.userAgent;
            request.Headers["X-HWID"] = Security.FingerPrint.Value();
            request.Headers["X-UserAgent"] = "GameLauncherReborn "+Application.ProductVersion+ " WinForms (+https://github.com/worldunitedgg/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = Value();
            request.Headers["X-DiscordID"] = Self.discordid;
            //request.Timeout = 30000;

            return request;
        }
    }
}
