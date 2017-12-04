using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncherReborn;
using System.Management;

namespace GameLauncher {
    static class Program {
        [STAThread]

        /*private static void EasterEgg(object sender, UnhandledExceptionEventArgs args) {
            if (args.IsTerminating) {
                AppDomain.CurrentDomain.UnhandledException -= EasterEgg;
                Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
            }
        }*/

        static void Main() {
            int SysVersion = (int)Environment.OSVersion.Platform;
            bool mono = DetectLinux.MonoDetected();
            bool wine = DetectLinux.WineDetected();
            bool linux = DetectLinux.LinuxDetected();

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            if(!Directory.Exists("Languages")) {
                Directory.CreateDirectory("Languages");
            }

            try {
                WebClientWithTimeout client = new WebClientWithTimeout();
                client.DownloadFile("https://raw.githubusercontent.com/metonator/GameLauncher_NFSW-translations/master/Languages/English.lng", "Languages\\Default.lng");
            } catch { }

            try {
                File.Delete(Directory.GetCurrentDirectory() + "\\tempname.zip");
            } catch { }

            if (mono == true) {
                MessageBox.Show(null, "Mono support is still under alpha stage. Therefore, launcher could not launch.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!File.Exists("LZMA.dll")) {
                File.WriteAllBytes("LZMA.dll", ExtractResource.AsByte("GameLauncher.LZMA.LZMA.dll"));
            }

            if (!File.Exists("discord-rpc.dll")) {
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));
            }

            if (!File.Exists("GameLauncherUpdater.exe")) {
                File.WriteAllBytes("GameLauncherUpdater.exe", ExtractResource.AsByte("GameLauncher.Updater.GameLauncherUpdater.exe"));
            }

            if(NFSW.isNFSWRunning()) {
                MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
            }

            Mutex mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR"); //Forgot about other launchers...
            try {
                if (mutex.WaitOne(0, false)) {
                    //First of all, we need to check if files exists
                    String[] files = { "Newtonsoft.Json.dll", "LZMA.dll", "ICSharpCode.SharpZipLib.dll" };
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

                    if (Environment.OSVersion.Version.Major >= 6) {
                        User32.SetProcessDPIAware();
                    }

                    //AppDomain.CurrentDomain.UnhandledException += EasterEgg;

                    //try {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new mainScreen());
                    //} catch(Exception) {
                        //Temporarely we gonna kill it.
                    //  Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    //}
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
