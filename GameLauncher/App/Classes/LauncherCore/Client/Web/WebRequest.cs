using System;
using System.Windows.Forms;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.LauncherCore.RPC;
using System.Net;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.SystemPlatform.Windows;

namespace GameLauncher.App.Classes.LauncherCore.Client.Web
{
    public class WebClientWithTimeout : WebClient
    {
        private static string Hash = string.Empty;

        public static string Value()
        {
            if (string.IsNullOrWhiteSpace(Hash))
            {
                Hash = SHA.HashFile(AppDomain.CurrentDomain.FriendlyName);
            }

            return Hash;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            if (DetectLinux.LinuxDetected())
            {
                address = new UriBuilder(address)
                {
                    Scheme = Uri.UriSchemeHttp,
                    Port = address.IsDefaultPort ? -1 : address.Port /* -1 => default port for scheme */
                }.Uri;
            }

            FunctionStatus.TLS();

            if (!address.AbsolutePath.Contains("auth")) Log.UrlCall("WEBCLIENTWITHTIMEOUT: Calling URL -> " + address);

            ServicePointManager.FindServicePoint(address).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-HWID"] = HardwareID.FingerPrint.Value();
            request.Headers["X-HiddenHWID"] = HardwareID.FingerPrint.ValueAlt();
            request.Headers["X-UserAgent"] = "GameLauncherReborn " + Application.ProductVersion + " WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = Value();
            request.Headers["X-GameLauncherCertificate"] = CertificateStore.LauncherSerial;
            request.Headers["X-DiscordID"] = DiscordLauncherPresense.UserID;
            request.Proxy = null;
            request.Timeout = 5000;
            request.KeepAlive = false;

            return request;
        }
    }
}
