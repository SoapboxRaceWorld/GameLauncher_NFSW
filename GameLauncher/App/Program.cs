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
            bool mono = DetectLinux.MonoDetected();
            bool wine = DetectLinux.WineDetected();
            bool linux = DetectLinux.NativeLinuxDetected();

			Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

			if (Self.isTempFolder(Directory.GetCurrentDirectory())) {
				MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				Environment.Exit(0);
			}

            if (!Directory.Exists("Languages")) {
                Directory.CreateDirectory("Languages");
            }

            try {
                File.Delete("Languages/Default.lng");
                File.WriteAllText("Languages/Default.lng", ExtractResource.AsString("GameLauncher.Language.Default.lng"));
            }
            catch { }

            try {
                File.Delete(Directory.GetCurrentDirectory() + "\\tempname.zip");
            } catch { }

            if (!File.Exists("LZMA.dll")) {
                File.WriteAllBytes("LZMA.dll", ExtractResource.AsByte("GameLauncher.LZMA.LZMA.dll"));
            }

            if (!linux && !File.Exists("discord-rpc.dll")) {
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));
            }

			if (linux && !File.Exists("libdiscord-rpc.so")) {
				File.WriteAllBytes("libdiscord-rpc.so", ExtractResource.AsByte("GameLauncher.Discord.libdiscord-rpc.so"));
            }

            if (File.Exists("GameLauncherUpdater.exe")) {
                File.Delete("GameLauncherUpdater.exe");
            }

            try {
                File.Delete("GL_Update.exe");
                File.WriteAllBytes("GL_Update.exe", ExtractResource.AsByte("GameLauncher.Updater.GL_Update.exe"));
            } catch { }

			if(!File.Exists("servers.txt")) {
				try {
					File.Create("servers.txt");
				} catch { }
			}

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
                        String[] files = { "Newtonsoft.Json.dll", "INIFileParser.dll", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll" };
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

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
