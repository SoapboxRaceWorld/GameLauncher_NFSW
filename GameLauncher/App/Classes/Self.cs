using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace GameLauncherReborn {
    class Self {
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

        public static bool CheckForInternetConnection() {
            try {
                using (var client = new WebClientWithTimeout()) {
                    using (client.OpenRead("http://clients3.google.com/generate_204")) {
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