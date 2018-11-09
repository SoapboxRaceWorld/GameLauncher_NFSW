using GameLauncher.App.Classes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GameLauncherReborn {
    class Self {
        public static string mainserver = "http://launcher.soapboxrace.world"; //mirroring launchpad (VISTA USERS)

        public static string serverlisturl = mainserver + "/servers";
        public static string internetcheckurl = mainserver + "/generate_204.php";
		public static string statsurl = mainserver + "/stats";

		private static IniFile SettingFile = new IniFile("Settings.ini");

        public static int ProxyPort = new Random().Next(7000, 7100);

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

        public static void Restart(string param = "") {
            Application.Restart();
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

		public static bool validateEmail(string email) {
			String theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
								   + "@"
								   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

			return Regex.IsMatch(email, theEmailPattern);
		}

		public static bool isTempFolder(string directory) {
			return directory.Contains("Temp"); //too lazy for regex
		}
	}
}