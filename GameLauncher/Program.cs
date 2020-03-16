using System;
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
using IniParser;
using GameLauncher.App.Classes.GPU;
using static MeTonaTOR.MessageBox;
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;
//using Memes;

namespace GameLauncher {
    internal static class Program {
        [STAThread]
        internal static void Main() {
            try {
                new WebClient().DownloadData("http://l.mtntr.pl/generate_204.php");
            } catch(Exception) {
                MessageBox.Show("There's no internet connection, launcher might crash");
            }

            Console.WriteLine("Application path: " + Path.GetDirectoryName(Application.ExecutablePath));

            /*GPU getinfo = null;
            
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
            
            MeTonaTOR.MessageBox.Show(getinfo.DriverVersion());*/

            if (!Self.hasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath))) {
                MessageBox.Show("This application requires admin priviledge. Restarting...");
                Self.runAsAdmin();
            }

            IniFile _settingFile = new IniFile("Settings.ini");

            if (DetectLinux.LinuxDetected()) {
                if (!_settingFile.KeyExists("InstallationDirectory")) {
                    _settingFile.Write("InstallationDirectory", "GameFiles");
                }

                if (!_settingFile.KeyExists("CDN")) {
                    try {
                        List<CDNObject> CDNList = new List<CDNObject>();
                        WebClientWithTimeout wc3 = new WebClientWithTimeout();
                        String _slresponse = wc3.DownloadString(Self.CDNUrlList);
                        CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse);
                        _settingFile.Write("CDN", CDNList.First().url);
                    } catch {
                        _settingFile.Write("CDN", "http://cdn.worldunited.gg/gamefiles/packed/");
                    }
                }
            }

            if (!_settingFile.KeyExists("DisableVerifyHash")) {
                _settingFile.Write("DisableVerifyHash", "1");
            }

            if (!string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Console.WriteLine("Game path: " + _settingFile.Read("InstallationDirectory"));

                if (!Self.hasWriteAccessToFolder(_settingFile.Read("InstallationDirectory"))) {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                    Self.runAsAdmin();
                }
            }

            File.Delete("log.txt");

            Log.StartLogging();

            //StaticConfiguration.DisableErrorTraces = false;

            Log.Debug("Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

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
                Application.Run(new MainScreen());
            } else {
                if (NFSW.isNFSWRunning()) {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try {
                    if (mutex.WaitOne(0, false)) {
                        string[] files = {
                            "DiscordRPC.dll - 1.0.150.0",
                            "Flurl.dll - 2.8.2",
                            "Flurl.Http.dll - 2.4.2",
                            "INIFileParser.dll - 2.5.2",
                            "LZMA.dll - 9.10 beta",
                            "Microsoft.WindowsAPICodePack.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.Shell.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.ShellExtensions.dll - 1.1.0.0",
                            "Nancy.dll - 2.0.0",
                            "Nancy.Hosting.Self.dll - 2.0.0",
                            "Newtonsoft.Json.dll - 12.0.3",
                            "System.Runtime.InteropServices.RuntimeInformation.dll - 4.6.24705.01. Commit Hash: 4d1af962ca0fede10beb01d197367c2f90e92c97"
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
                            Application.Run(new MainScreen());
                        }
                    } else {
                        MessageBox.Show(null, "An instance of Launcher is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } finally {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

        static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e) {
            Exception exception = (Exception)e.ExceptionObject;

            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog(exception)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }

        static void ThreadExceptionEventHandler(object sender, ThreadExceptionEventArgs e) {
            Exception exception = (Exception)e.Exception;

            using (ThreadExceptionDialog dialog = new ThreadExceptionDialog(exception)) {
                dialog.ShowDialog();
            }

            Application.Exit();
            Environment.Exit(0);
        }
    }
}
