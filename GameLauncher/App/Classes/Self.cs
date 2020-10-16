using GameLauncher.App.Classes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Flurl;
using Flurl.Http;
using System.Management;
using System.Net;

namespace GameLauncherReborn {
    class Self {
        public static string mainserver = "https://api.worldunited.gg";
        public static string fileserver = "https://files.worldunited.gg";
        public static string staticapiserver = "http://api-sbrw.davidcarbon.download";

        public static string[] serverlisturl = new string[] {
            mainserver + "/serverlist.json",
            staticapiserver + "/serverlist.json"
        };

        public static string statsurl = mainserver + "/stats";
        public static string CDNUrlList = mainserver + "/cdn_list.json";
        public static string CDNUrlStaticList = staticapiserver + "/cdn_list.json";

        private static IniFile SettingFile = new IniFile("Settings.ini");

        public static string DiscordRPCID = "540651192179752970";

        public static int ProxyPort = new Random().Next(6260, 8269);
        public static Boolean sendRequest = true;

        public static String userAgent = null;

        public static Boolean CanDisableGame = true;
        public static String gamedir = null;

        public static string rememberjson = "";
        public static string discordid = String.Empty;
        public static string MapZoneRPC = String.Empty;

        public static void runAsAdmin() {
            string[] args = Environment.GetCommandLineArgs();

            ProcessStartInfo processStartInfo = new ProcessStartInfo() {
                Verb = "runas",
                FileName = Application.ExecutablePath
            };

            if ((int)args.Length > 0) {
                processStartInfo.Arguments = args[0];
            }

            try {
                Process.Start(processStartInfo);
            } catch (Exception exception1) {
                MessageBox.Show("Failed to self-run as admin: " + exception1.Message);
            }
        }

        public static long getTimestamp(bool valid = false) {
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;

            if(valid == true) {
                ticks /= 10000000;
            } else {
                ticks /= 10000;
            }

            return ticks;
        }

		public static bool hasWriteAccessToFolder(string path) {
			try {
				File.Create(path + "temp.txt").Close();
				File.Delete(path + "temp.txt");
			} catch {
				return false;
			}

			return true;
		}

        public static async Task SubmitError(Exception exception)
        {
            var mainsrv = DetectLinux.LinuxDetected() ? mainserver.Replace("https", "http") : mainserver;
            Url url = new Url(mainsrv + "/error-report");
            await url.PostJsonAsync(new
            {
                message = exception.Message ?? "no message",
                stackTrace = exception.StackTrace ?? "no stack trace"
            });
        }

        public static string CountryName(string twoLetterCountryCode) {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures) {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (region.TwoLetterISORegionName.ToUpper() == twoLetterCountryCode.ToUpper()) {
                    return region.EnglishName;
                }
            }

            return "Unknown";
        }

        public static void centerScreen(Form form) {
            form.StartPosition = FormStartPosition.Manual;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
        }

		public static bool validateEmail(string email) {
			String theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
								   + "@"
								   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

			return Regex.IsMatch(email, theEmailPattern);
		}

		public static bool isTempFolder(string directory) {
			return directory.Contains("Temp");
		}

        public static string CleanFromUnknownChars(string s) {
            StringBuilder sb = new StringBuilder(s.Length);
            foreach (char c in s) {
                if(
                    (int)c >= 48 && (int)c <= 57 || 
                    (int)c == 60 || 
                    (int)c == 62 || 
                    (int)c >= 65 && (int)c <= 90 || 
                    (int)c >= 97 && (int)c <= 122 || 
                    (int)c == 47 || 
                    (int)c == 45 ||
                    (int)c == 46
                ) {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static bool CheckArchitectureFile(string fileName) {
            const int PE_POINTER_OFFSET = 60;
            const int MACHINE_OFFSET = 4;
            byte[] data = new byte[4096];

            using (Stream s = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
                s.Read(data, 0, 4096);
            }

            int PE_HEADER_ADDR = BitConverter.ToInt32(data, PE_POINTER_OFFSET);
            int machineUint = BitConverter.ToUInt16(data, PE_HEADER_ADDR + MACHINE_OFFSET);
            return machineUint == 0x014c;
        }

        public static bool getInstalledHotFix(string identification) {
            var search = new ManagementObjectSearcher("SELECT HotFixID FROM Win32_QuickFixEngineering");
            var collection = search.Get();

            foreach (ManagementObject quickFix in collection) {
                Console.WriteLine("Updates installed: " + quickFix["HotFixID"].ToString());
                if(quickFix["HotFixID"].ToString() == identification) {
                    return true;
                }
            }

            return false;
        }

        public static string HostName2IP(string hostname) {
            IPHostEntry iphost = Dns.GetHostEntry(hostname);
            IPAddress[] addresses = iphost.AddressList;
            return addresses[0].ToString();
        }
    }
}
