using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GameLauncher
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //First of all, we need to check if files exists
            String[] files = { "SlimDX.dll", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll" };
            List<string> missingfiles = new List<string>();
            foreach (string file in files) {
                if (!File.Exists(file)) {
                    missingfiles.Add(file);
                }
            }

            if(missingfiles.Count != 0) {
                string message = "Cannot launch GameLauncher. The following files are missing:\n\n";
                foreach (string file in missingfiles) {
                    message += file + "\n";
                }
                MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainScreen());
        }
    }
}
