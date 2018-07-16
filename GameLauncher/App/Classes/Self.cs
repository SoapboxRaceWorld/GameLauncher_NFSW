using GameLauncher.App.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace GameLauncherReborn {
    class Self {
		public static string mainserver = "https://launcher.soapboxrace.world";
		//public static string mirrorserver = "http://mirror.nfsw.mtntr.eu";

		public static string serverlisturl = mainserver + "/serverlist.txt";
		public static string internetcheckurl = mainserver + "/generate_204.php";
		public static string statsurl = mainserver + "/stats";

		private static IniFile SettingFile = new IniFile("Settings.ini");

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
                MessageBox.Show("Failed to self-run as admin: " + exception1);
            }
        }

        public static void Restart(string param = "") {
            Application.Restart();
        }

        public static string CountryName(string twoLetterCountryCode) {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures) {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (region.TwoLetterISORegionName.ToUpper() == twoLetterCountryCode.ToUpper()) {
                    return region.EnglishName;
                }
            }

            return String.Empty;
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
			} catch (UnauthorizedAccessException) {
				return false;
			}

			return true;
		}

		public static string getDiscordRPCImageIDFromServerName(string ServerName, string response) {
			string returnvalue = "nfsw";

			try {
				String[] substrings = response.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
				foreach (var substring in substrings) {
					if (!String.IsNullOrEmpty(substring)) {
						String[] substrings2 = substring.Split(new string[] { ";" }, StringSplitOptions.None);

						if(substrings2[0] == ServerName) {
							returnvalue = substrings2[2];
						}
					}
				}
			} catch {
				returnvalue = "nfsw";
			}

			return returnvalue;
		}

        public static bool CheckForInternetConnection() {
            try {
                using (var client = new WebClientWithTimeout()) {
                    using (client.OpenRead(internetcheckurl)) {
                        return true;
                    }
                }
            } catch {
                return false;
            }
        }

        public static void centerScreen(Form form) {
            form.StartPosition = FormStartPosition.Manual;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
        }
    }
}