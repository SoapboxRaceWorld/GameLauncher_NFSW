using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Bugsnag;
using GameLauncher.App;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
using GameLauncherReborn;

namespace GameLauncher {
    internal static class Program {
        [STAThread]
        internal static void Main() {

            File.Delete("log.txt");

            Log.Debug("Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            Form SplashScreen2 = null;

            Log.Debug("Checking current directory");

            if (Self.isTempFolder(Directory.GetCurrentDirectory())) {
                MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(0);
            }

            if (!DetectLinux.UnixDetected() && !File.Exists("discord-rpc.dll"))
                File.WriteAllBytes("discord-rpc.dll", ExtractResource.AsByte("GameLauncher.Discord.discord-rpc.dll"));

            if (DetectLinux.LinuxDetected() && !File.Exists("libdiscord-rpc.so"))
                File.WriteAllBytes("libdiscord-rpc.so", ExtractResource.AsByte("GameLauncher.Discord.libdiscord-rpc.so"));

            if (DetectLinux.MacOSDetected() && !File.Exists("libdiscord-rpc.dylib"))
                File.WriteAllBytes("libdiscord-rpc.dylib", ExtractResource.AsByte("GameLauncher.Discord.libdiscord-rpc.dylib"));

			if(!File.Exists("GameLauncherUpdater.exe")) {
				try {
					File.WriteAllBytes("GameLauncherUpdater.exe", new WebClientWithTimeout().DownloadData("http://launcher.soapboxrace.world/GameLauncherUpdater.exe"));
                } catch { /* ignored */ }
            }

            if (!File.Exists("servers.json")) {
                try {
                    File.WriteAllText("servers.json", "[]");
                } catch { /* ignored */ }
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (Debugger.IsAttached) {
                Log.Debug("Checking Proxy");
                ServerProxy.Instance.Start();
                Log.Debug("Starting MainScreen");
                Application.Run(new MainScreen(SplashScreen2));
            } else {
                if (NFSW.isNFSWRunning()) {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try {
                    if (mutex.WaitOne(0, false)) {
                        string[] files = {
                            "Bugsnag.dll - 2.2.0",
                            "Flurl.dll - 2.8.0",
                            "Flurl.Http.dll - 2.3.2",
                            "INIFileParser.dll - 2.5.2",
                            "Microsoft.WindowsAPICodePack.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.Shell.dll - 1.1.0.0",
                            "Nancy.dll - 1.4.4",
                            "Nancy.Hosting.Self.dll - 1.4.1",
                            "Newtonsoft.Json.dll - 11.0.2",
                        };

                        var missingfiles = new List<string>();

                        foreach (var file in files) {
                            var splitFileVersion = file.Split(new string[] { " - " }, StringSplitOptions.None);

                            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + splitFileVersion[0])) {
                                missingfiles.Add(splitFileVersion[0] + " - Not Found");
                            } else {
                                try { 
                                    var versionInfo = FileVersionInfo.GetVersionInfo(splitFileVersion[0]);
                                    string version = versionInfo.ProductVersion;

                                    if(version == "") {
                                        missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                    } else { 
                                        if(Self.CheckArchitectureFile(splitFileVersion[0]) == false) {
                                            missingfiles.Add(splitFileVersion[0] + " - Wrong Architecture");
                                        } else { 
                                            if(version != splitFileVersion[1]) {
                                                missingfiles.Add(splitFileVersion[0] + " - Invalid Version (" + splitFileVersion[1] + " != " + version + ")");
                                            }
                                        }
                                    }
                                } catch {
                                    missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                }
                            }
                        }

                        if (missingfiles.Count != 0) {
                            var message = "Cannot launch GameLauncher. The following files are invalid:\n\n";

                            foreach (var file in missingfiles) {
                                message += "• " + file + "\n";
                            }

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } else {
                            Log.Debug("Checking Proxy");
                            ServerProxy.Instance.Start();

                            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionEventHandler);
                            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionEventHandler);
                            
                            Log.Debug("Starting MainScreen");
                            Application.Run(new MainScreen(SplashScreen2));
                        }
                    } else {
                        MessageBox.Show(null, "An instance of the application is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } finally {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

       /* static void Discord_Ready(ref DiscordRpc.DiscordUser pUser) {
            Invoke(new Action<DiscordRpc.DiscordUser>((user) => {
                pbAvatarImage.ImageLocation = $"https://cdn.discordapp.com/avatars/{user.userId}/{user.avatar}.png?size=128";
                lbUsername.Text = $"{user.username}#{user.discriminator}\n{user.userId}";

                
            }), pUser);

            CurrentUser = pUser;
        }*/

        static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e) {
            Bugsnag.Client bugsnag = new Bugsnag.Client(new Configuration("ba9a1f366203b461b2f031a12b9a0e41"));
            bugsnag.Notify((Exception)e.ExceptionObject);

            Log.Error(((Exception)e.ExceptionObject).Message);
            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog((Exception)e.ExceptionObject)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }

        static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e) {
            Bugsnag.Client bugsnag = new Bugsnag.Client(new Configuration("ba9a1f366203b461b2f031a12b9a0e41"));
            bugsnag.Notify(e.Exception);

            Log.Error(e.Exception.Message);

            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog(e.Exception)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
