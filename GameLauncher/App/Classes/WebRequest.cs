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
using GameLauncher.App.Classes.Logger;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace GameLauncherReborn {
    public class WebClientWithTimeout : WebClient {
        private static string GameLauncherHash = string.Empty;
        private static long addrange = 0;
        private static int timeout = 3000;
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

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => {
                bool isOk = true;
                if (sslPolicyErrors != SslPolicyErrors.None) {
                    for (int i = 0; i < chain.ChainStatus.Length; i++) {
                        if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
                            continue;
                        }
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)certificate);
                        if (!chainIsValid) {
                            isOk = false;
                            break;
                        }
                    }
                }
                return isOk;
            };

            if(!address.AbsolutePath.Contains("auth")) Log.UrlCall("Calling URL: " + address);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-HWID"] = Security.FingerPrint.Value();
            request.Headers["X-UserAgent"] = "GameLauncherReborn "+Application.ProductVersion+ " WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = Value();
            request.Headers["X-DiscordID"] = Self.discordid;
            
            if(addrange != 0) {
                request.AddRange(addrange);
            }

            request.Proxy = null;
            request.Timeout = timeout;

            return request;
        }

        internal void AddRange(long filesize) {
            addrange = filesize;
        }

        internal void Timeout(int time) {
            timeout = time;
        }
    }
}
