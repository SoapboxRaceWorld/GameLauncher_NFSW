using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncherReborn;

namespace GameLauncher
{
    internal static class Program
    {
        [STAThread]
        internal static void Main()
        {
            var linux = DetectLinux.NativeLinuxDetected();

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath) ?? throw new InvalidOperationException());

            if (Self.isTempFolder(Directory.GetCurrentDirectory()))
            {
                MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(0);
            }

            if (!Directory.Exists("Languages"))
            {
                Directory.CreateDirectory("Languages");
            }

            try
            {
                File.Delete("Languages/Default.lng");
                File.WriteAllText("Languages/Default.lng", ExtractResource.AsString("GameLauncher.Language.Default.lng"));
            }
            catch
            {
                // ignored
            }

            try
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\tempname.zip");
            }
            catch
            {
                // ignored
            }

            if (!File.Exists("LZMA.dll"))
            {
                File.WriteAllBytes("LZMA.dll", ExtractResource.AsByte("GameLauncher.LZMA.LZMA.dll"));
            }

            if (!linux && !File.Exists("discord-rpc.dll"))
            {
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));
            }

            if (linux && !File.Exists("libdiscord-rpc.so"))
            {
                File.WriteAllBytes("libdiscord-rpc.so", ExtractResource.AsByte("GameLauncher.Discord.libdiscord-rpc.so"));
            }

			if (File.Exists("GL_Update.exe")) {
				File.Delete("GL_Update.exe");
			}

			if(!File.Exists("GameLauncherUpdater.exe")) {
				try
				{
					File.WriteAllBytes("GameLauncherUpdater.exe", new WebClientWithTimeout().DownloadData("http://launcher.soapboxrace.world/GameLauncherUpdater.exe"));
				}
				catch
				{
					// ignored
				}
			}
            if (!File.Exists("servers.json"))
            {
                try
                {
                    File.WriteAllText("servers.json", "[]");
                }
                catch
                {
                    // ignored
                }
            }

            if (Debugger.IsAttached)
            {
                ServerProxy.Instance.Start();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainScreen());
            }
            else
            {
                if (NFSW.isNFSWRunning())
                {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try
                {
                    if (mutex.WaitOne(0, false))
                    {
                        string[] files =
                        {
                            "Newtonsoft.Json.dll",
                            "INIFileParser.dll",
                            "Microsoft.WindowsAPICodePack.dll",
                            "Microsoft.WindowsAPICodePack.Shell.dll",
                            "Flurl.dll",
                            "Flurl.Http.dll",
                            "BlackListedServers.dat"
                        };
                        var missingfiles = new List<string>();

                        foreach (var file in files)
                        {
                            if (!File.Exists(file))
                            {
                                missingfiles.Add(file);
                            }
                        }

                        if (missingfiles.Count != 0)
                        {
                            var message = "Cannot launch GameLauncher. The following files are missing:\n\n";

                            foreach (var file in missingfiles)
                            {
                                message += "• " + file + "\n";
                            }

                            message += "\nCurrent directory: " + Directory.GetCurrentDirectory();

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Environment.Exit(1);
                        }

                        ServerProxy.Instance.Start();

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new MainScreen());
                    }
                    else
                    {
                        MessageBox.Show(null, "An instance of the application is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }
    }
}
