﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
using GameLauncherReborn;
using Nancy;
using SharpRaven;
using SharpRaven.Data;
using IniParser;
using GameLauncher.App.Classes.GPU;
//using Memes;

namespace GameLauncher {
    internal static class Program {
        [STAThread]
        internal static void Main()
        {
            Console.WriteLine("Application path: " + Path.GetDirectoryName(Application.ExecutablePath));

            GPU getinfo = null;
            
            switch(GPUHelper.getManufacturer()) {
                case GPUHelper.GPUManufacturer.NVIDIA:
                    getinfo = new NVIDIA();
                    break;
                case GPUHelper.GPUManufacturer.AMD:
                    getinfo = new AMD();
                    break;
                case GPUHelper.GPUManufacturer.INTEL:
                    getinfo = new INTEL();
                    break;
                default:
                    getinfo = null;
                    break;
            }
            
            MessageBox.Show(getinfo.DriverVersion());

            if (!Self.hasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath))) {
                MessageBox.Show("This application requires admin priviledge. Restarting...");
                Self.runAsAdmin();
            }

            IniFile _settingFile = new IniFile("Settings.ini");

            if (!string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Console.WriteLine("Game path: " + _settingFile.Read("InstallationDirectory"));

                if (!Self.hasWriteAccessToFolder(_settingFile.Read("InstallationDirectory"))) {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                    Self.runAsAdmin();
                }
            }

            File.Delete("log.txt");

            Log.StartLogging();

            StaticConfiguration.DisableErrorTraces = false;

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

			if(!File.Exists("GameLauncherUpdater.exe")) {
                Log.Debug("Starting GameLauncherUpdater downloader");
                try {
                    using (WebClientWithTimeout wc = new WebClientWithTimeout()) {
                        wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) => {
                            if (new FileInfo("GameLauncherUpdater.exe").Length == 0) {
                                File.Delete("GameLauncherUpdater.exe");
                            }
                        };
                        wc.DownloadFileAsync(new Uri(Self.mainserver + "/files/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                    }
                } catch(Exception ex) {
                    Log.Debug("Failed to download updater. " + ex.Message);
                }
            }

            HashChecker.CheckLauncher(Application.ExecutablePath);
            //HashChecker.CheckLauncherFolder(Path.GetDirectoryName(Application.ExecutablePath));

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
                            "SharpRaven.dll - 2.4.0",
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

                        if(!DetectLinux.LinuxDetected()) { //MONO Hates that...
                            foreach (var file in files) {
                                var splitFileVersion = file.Split(new string[] { " - " }, StringSplitOptions.None);

                                if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + splitFileVersion[0])) {
                                    missingfiles.Add(splitFileVersion[0] + " - Not Found");
                                } else {
                                    try { 
                                        var versionInfo = FileVersionInfo.GetVersionInfo(splitFileVersion[0]);
                                        string[] versionsplit = versionInfo.ProductVersion.Split('+');
                                        string version = versionsplit[0];

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

        static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e) {
            Exception exception = (Exception)e.ExceptionObject;
            exception.Data.Add("BuildHash", SHA.HashFile(AppDomain.CurrentDomain.FriendlyName));

            var ravenClient = new RavenClient("https://12973f6fa1054f51a8e3a840e7dc021c@sentry.io/1325472");
            ravenClient.Capture(new SentryEvent(exception));

            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog(exception)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }

        static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e) {
            Exception exception = (Exception)e.Exception;
            exception.Data.Add("BuildHash", SHA.HashFile(AppDomain.CurrentDomain.FriendlyName));

            var ravenClient = new RavenClient("https://12973f6fa1054f51a8e3a840e7dc021c@sentry.io/1325472");
            ravenClient.Capture(new SentryEvent(exception));

            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog(exception)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
