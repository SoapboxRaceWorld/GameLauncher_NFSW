using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncherReborn;
using System.Management;
using GameLauncher.App;

namespace GameLauncher {
    static class Program {
        [STAThread]
        static void Main() {       
            int SysVersion = (int)Environment.OSVersion.Platform;
            bool mono = DetectLinux.MonoDetected();
            bool wine = DetectLinux.WineDetected();
            bool linux = DetectLinux.NativeLinuxDetected();

            /*if(Environment.OSVersion.Version.Major <= 5 && !linux) {
                MessageBox.Show(null, "Windows XP Support has been terminated. Please upgrade your Operating System to 'Vista' or newer.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(Environment.ExitCode);
            }*/

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            if (!Directory.Exists("Languages")) {
                Directory.CreateDirectory("Languages");
            }

            /*try {
                WebClientWithTimeout client = new WebClientWithTimeout();
                File.Delete("Languages\\Default.lng");
                client.DownloadFile("https://raw.githubusercontent.com/metonator/GameLauncher_NFSW-translations/master/Languages/English.lng", "Languages\\Default.lng");
            } catch { }*/

            try {
                File.Delete("Languages/Default.lng");
                File.WriteAllText("Languages/Default.lng", ExtractResource.AsString("GameLauncher.Language.Default.lng"));
            }
            catch { }

            try {
                File.Delete(Directory.GetCurrentDirectory() + "\\tempname.zip");
            } catch { }

			if (linux)
			{
				MessageBox.Show(null, "Native Linux support is still under alpha stage. Therefore, launcher or game could crash.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
			else if (mono == true)
			{
				MessageBox.Show(null, "Mono support is still under alpha stage. Therefore, launcher could not launch.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

            if (!File.Exists("LZMA.dll")) {
                File.WriteAllBytes("LZMA.dll", ExtractResource.AsByte("GameLauncher.LZMA.LZMA.dll"));
            }

            if (!linux && !File.Exists("discord-rpc.dll")) {
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));
            }

			if (linux && !File.Exists("libdiscord-rpc.so"))
            {
				File.WriteAllBytes("libdiscord-rpc.so", ExtractResource.AsByte("GameLauncher.Discord.libdiscord-rpc.so"));
            }

            if (File.Exists("GameLauncherUpdater.exe")) {
                File.Delete("GameLauncherUpdater.exe");
            }

            try {
                File.Delete("GL_Update.exe");
                File.WriteAllBytes("GL_Update.exe", ExtractResource.AsByte("GameLauncher.Updater.GL_Update.exe"));
            } catch { }


            if(Debugger.IsAttached) {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new mainScreen());
            } else {
                if (NFSW.isNFSWRunning()) {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                Mutex mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try {
                    if (mutex.WaitOne(0, false)) {
                        String[] files = { "Newtonsoft.Json.dll", "LZMA.dll" };
                        List<string> missingfiles = new List<string>();

                        foreach (string file in files) {
                            if (!File.Exists(file)) {
                                missingfiles.Add(file);
                            }
                        }

                        if (missingfiles.Count != 0) {
                            string message = "Cannot launch GameLauncher. The following files are missing:\n\n";

                            foreach (string file in missingfiles) {
                                message += "• " + file + "\n";
                            }

                            message += "\nCurrent directory: " + Directory.GetCurrentDirectory();
                            message += "\nYou will be moved to the project page for re-download.";

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases");
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
}
