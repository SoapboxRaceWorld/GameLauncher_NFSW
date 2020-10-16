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
using System.Reflection;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Win32;
using CommandLine;
using System.Globalization;

namespace GameLauncher {
    internal static class Program {
        internal class Arguments {
            [Option('p', "parse", Required = false, HelpText = "Parses URI")]
            public string Parse { get; set; }
        }

        [STAThread]
        internal static void Main(string[] args) {
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(Main2);
        }

        private static void Main2(Arguments args) {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            if (UriScheme.IsCommandLineArgumentsInstalled()) {
                UriScheme.InstallCommandLineArguments(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), AppDomain.CurrentDomain.FriendlyName));
                if(args.Parse != null) {
                    new UriScheme(args.Parse);
                }
            }

            IniFile _settingFile = new IniFile("Settings.ini");

            //Windows 7 Fix
            if (!(_settingFile.KeyExists("PatchesApplied"))) {
                String _OS = (string)Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("productName");
                if(_OS.Contains("Windows 7")) {
                    if (Self.getInstalledHotFix("KB3020369") == false || Self.getInstalledHotFix("KB3125574") == false) {
                        String messageBoxPopupKB = String.Empty;
                        messageBoxPopupKB  = "Hey Windows 7 User, in order to play on this server, we need to make additional tweaks to your system.\n";
                        messageBoxPopupKB += "We must make sure you have those Windows Update packages installed:\n\n";

                        if (Self.getInstalledHotFix("KB3020369") == false) messageBoxPopupKB += "- Update KB3020369\n";
                        if (Self.getInstalledHotFix("KB3125574") == false) messageBoxPopupKB += "- Update KB3125574\n";

                        messageBoxPopupKB += "\nAditionally, we must add a value to the registry:\n";

                        messageBoxPopupKB += "- HKLM/SYSTEM/CurrentControlSet/Control/SecurityProviders\n/SCHANNEL/Protocols/TLS 1.2/Client\n";
                        messageBoxPopupKB += "- Value: DisabledByDefault -> 0\n\n";

                        messageBoxPopupKB += "Would you like to add those values?";
                        DialogResult replyPatchWin7 = MessageBox.Show(null, messageBoxPopupKB, "GameLauncherReborn", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if(replyPatchWin7 == DialogResult.Yes) {
                            RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client");
                            key.SetValue("DisabledByDefault", 0x0);

                            MessageBox.Show(null, "Registry option set, Remember that the following patch might work after a system reboot", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        } else {
                            MessageBox.Show(null, "Roger that, There will be some issues connecting to the servers.", "GameLauncherReborn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        _settingFile.Write("PatchesApplied", "1");
                    }
                }
            }

            Console.WriteLine("Application path: " + Path.GetDirectoryName(Application.ExecutablePath));

            if (!Self.hasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath))) {
                MessageBox.Show("This application requires admin priviledge");
            }

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
                        try {
                            List<CDNObject> CDNList = new List<CDNObject>();
                            WebClientWithTimeout wc3 = new WebClientWithTimeout();
                            String _slresponse = wc3.DownloadString(Self.CDNUrlStaticList);
                            CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse);
                            _settingFile.Write("CDN", CDNList.First().url);
                        } catch {
                            _settingFile.Write("CDN", "http://cdn.worldunited.gg/gamefiles/packed/");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Console.WriteLine("Game path: " + _settingFile.Read("InstallationDirectory"));

                if (!Self.hasWriteAccessToFolder(_settingFile.Read("InstallationDirectory"))) {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                }
            }

            File.Delete("communication.log");
            File.Delete("launcher.log");

            Log.StartLogging();
            Log.Debug("GameLauncher " + Application.ProductVersion);

            if (_settingFile.KeyExists("InstallationDirectory")) {
                if(!File.Exists(_settingFile.Read("InstallationDirectory"))) {
                    Directory.CreateDirectory(_settingFile.Read("InstallationDirectory"));
                }
            }

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
                        wc.DownloadFileAsync(new Uri(Self.fileserver + "/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
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

            if (Properties.Settings.Default.IsRestarting) {
                Properties.Settings.Default.IsRestarting = false;
                Properties.Settings.Default.Save();
                Thread.Sleep(3000);
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
                            "CommandLine.dll - 2.8.0",
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
    }
}
