using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;

namespace GameLauncher {
    static class Program {
        [STAThread]
        static void Main() {
            int SysVersion = (int)Environment.OSVersion.Platform;
            bool mono = DetectLinux.MonoDetected();
            bool wine = DetectLinux.WineDetected();
            bool linux = DetectLinux.LinuxDetected();
            string extraLinuxInfo = "";

            //Console log with warning
            if (mono == true) {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));
                MessageBox.Show(null, "Detected OS: Linux using Mono. Linux support is still under alpha stage. Therefore, launcher could not launch.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } else if (wine == true) {
                MessageBox.Show(null, "Detected OS: Linux using Wine. Linux support is still under alpha stage. Therefore, launcher could not launch.", "GameLauncher.exe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //Add LZMA.dll on the fly (used for decompression of section{int}.dll files)
            if (!File.Exists("LZMA.dll")) {
                File.WriteAllBytes("LZMA.dll", ExtractResource.AsByte("GameLauncher.LZMA.LZMA.dll"));
            }

            Mutex mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR"); //Forgot about other launchers...
            try {
                if (mutex.WaitOne(0, false)) {
                    //First of all, we need to check if files exists
                    String[] files = { "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll", "LZMA.dll" };
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
                        Process.Start(@"https://github.com/metonator/GameLauncher_NFSW/releases");
                        Environment.Exit(1);
                    }

                    if (Environment.OSVersion.Version.Major >= 6) {
                        User32.SetProcessDPIAware();
                    }

                    try {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new mainScreen());
                    } catch(Exception ex) {
                        if(linux == true) {
                            extraLinuxInfo = "\n\nAditionally, please report that you're using Wine/Mono Runtime and your Linux Distro";
                        }

                        MessageBox.Show(null, "Failed to launch GameLauncher. " + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace + extraLinuxInfo, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(1);
                    }
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
