using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Client.Web
{
    class WebCalls
    {
        public static bool Alternative() => FileSettingsSave.WebCallMethod == "WebClient";
    }

    class WebHelpers
    {
        private static string Hash = string.Empty;

        public static string Value()
        {
            if (string.IsNullOrWhiteSpace(Hash))
            {
                Hash = SHA.Files(Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameLauncher)));
            }

            return Hash;
        }
    }

    public class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            if (UnixOS.Detected())
            {
                address = new UriBuilder(address)
                {
                    Scheme = Uri.UriSchemeHttp,
                    Port = address.IsDefaultPort ? -1 : address.Port /* -1 => default port for scheme */
                }.Uri;
            }

            if (!address.AbsolutePath.Contains("auth"))
            {
                if (!(address.OriginalString.Contains("section") && address.OriginalString.Contains(".dat")))
                {
                    if (!FunctionStatus.ExternalToolsWasUsed)
                    {
                        Log.UrlCall("WEBCLIENTWITHTIMEOUT: Calling URL -> " + address);
                    }
                }
            }

            ServicePointManager.FindServicePoint(address).ConnectionLeaseTimeout =
                (int)((FunctionStatus.ExternalToolsWasUsed) ? TimeSpan.FromSeconds(30).TotalMilliseconds : TimeSpan.FromSeconds(5).TotalMilliseconds);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = "SBRW Launcher " + Application.ProductVersion +
                (FunctionStatus.ExternalToolsWasUsed ? " - (" + InsiderInfo.BuildNumberOnly() + ")" :
                " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
            request.Headers["X-HWID"] = HardwareID.FingerPrint.Value();
            request.Headers["X-HiddenHWID"] = HardwareID.FingerPrint.ValueAlt();
            request.Headers["X-UserAgent"] = "GameLauncherReborn " +
                Application.ProductVersion + " WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
            request.Headers["X-GameLauncherHash"] = WebHelpers.Value();
            request.Headers["X-GameLauncherCertificate"] = CertificateStore.LauncherSerial;
            request.Headers["X-DiscordID"] = DiscordLauncherPresence.UserID;
            request.Timeout = (int)(FunctionStatus.ExternalToolsWasUsed ?
                TimeSpan.FromSeconds(30).TotalMilliseconds : TimeSpan.FromSeconds(5).TotalMilliseconds);
            request.KeepAlive = false;

            return request;
        }
    }
}
