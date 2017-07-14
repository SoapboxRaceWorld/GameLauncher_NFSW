using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher {
    static class Program {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main() {
            Mutex mutex = new Mutex(false, "{d939b470-4a33-4441-959c-d2733c1618d2}");
            try {
                if (mutex.WaitOne(0, false)) {
                    //First of all, we need to check if files exists
                    String[] files = { "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll" };
                    List<string> missingfiles = new List<string>();

                    foreach (string file in files) {
                        if (!File.Exists(file)) {
                            missingfiles.Add(file);
                        }
                    }

                    if (missingfiles.Count != 0) {
                        string message = "Cannot launch GameLauncher. The following files are missing:\n\n";

                        foreach (string file in missingfiles) {
                            message += file + "\n";
                        }

                        message += "\n\nYou will be moved to the project page for re-download.";

                        MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Process.Start(@"https://github.com/metonator/GameLauncher_NFSW/releases");
                        Environment.Exit(1);
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new mainScreen());
                } else {
                    MessageBox.Show(null, "An instance of the application is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } finally {
                if (mutex != null) {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}
