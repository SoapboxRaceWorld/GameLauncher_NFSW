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
    public class WebClientWithTimeout : WebClient
    {
        private Panel panel1;
        private static string GameLauncherHash = string.Empty;
        public static string Value() {
            if (string.IsNullOrEmpty(GameLauncherHash)) {
                GameLauncherHash = SHA.HashFile(AppDomain.CurrentDomain.FriendlyName);
            }

            return GameLauncherHash;
        }

        protected override WebRequest GetWebRequest(Uri address) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "GameLauncher (+https://github.com/SoapboxRaceWorld/GameLauncher_NFSW)"; //this must remain untouched.
            request.Headers["X-HWID"] = Security.FingerPrint.Value();
            request.Headers["X-UserAgent"] = "GameLauncherReborn "+Application.ProductVersion+ " WinForms (+https://github.com/worldunitedgg/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = Value();
            request.Headers["X-DiscordID"] = Self.DiscordID;
            request.Timeout = 30000;

            return request;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebClientWithTimeout));
            this.panel1 = new System.Windows.Forms.Panel();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // WebClientWithTimeout
            // 
            this.Headers = ((System.Net.WebHeaderCollection)(resources.GetObject("$this.Headers")));

        }
    }
}