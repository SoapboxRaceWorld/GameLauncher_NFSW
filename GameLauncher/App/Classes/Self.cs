using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            ProcessStartInfo startInfo = Process.GetCurrentProcess().StartInfo;
            startInfo.FileName = Application.ExecutablePath;

            if(!String.IsNullOrEmpty(param)) {
                startInfo.Arguments = param;
            }

            var exit = typeof(Application).GetMethod("ExitInternal", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            exit.Invoke(null, null);
            Process.Start(startInfo);
        }

        public static long getTimestamp() {
            long ticks = DateTime.UtcNow.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000;
            return ticks;
        }

        public static void centerScreen(Form form) {
            form.StartPosition = FormStartPosition.Manual;
            form.Top = (Screen.PrimaryScreen.Bounds.Height - form.Height) / 2;
            form.Left = (Screen.PrimaryScreen.Bounds.Width - form.Width) / 2;
        }
    }
}