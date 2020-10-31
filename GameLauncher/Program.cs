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
        //Update this if a new GameLauncherUpdater.exe has been delployed - DavidCarbon
        private static string LatestUpdaterBuildVersion = "1.0.0.4";

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

            try
            {
                //Check Using Backup API
                HttpWebRequest requestBkupServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.staticapiserver + "/generate_204/");
                requestBkupServerListAPI.AllowAutoRedirect = false;
                requestBkupServerListAPI.Method = "HEAD";
                requestBkupServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                try
                {
                    HttpWebResponse bkupServerListResponseAPI = (HttpWebResponse)requestBkupServerListAPI.GetResponse();
                    bkupServerListResponseAPI.Close();
                }
                catch (WebException)
                {
                    HttpWebRequest requestMainServerListAPI = (HttpWebRequest)HttpWebRequest.Create(Self.mainserver + "/cdn_list.json");
                    requestMainServerListAPI.AllowAutoRedirect = false;
                    requestMainServerListAPI.Method = "HEAD";
                    requestMainServerListAPI.UserAgent = "GameLauncher (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    try
                    {
                        HttpWebResponse bkupServerListResponseAPI = (HttpWebResponse)requestMainServerListAPI.GetResponse();
                        bkupServerListResponseAPI.Close();
                    }
                    catch { }
                }
            }
            catch
            {
                DialogResult restartAppNoApis = MessageBox.Show(null, "There's no internet connection, Launcher might crash \n \nClick Yes to Close Launcher \nor \nClick No Continue", "GameLauncher has Stopped, Failed To Connect To API", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (restartAppNoApis == DialogResult.No)
                {
                    MessageBox.Show("Good Luck... \n No Really \n ...Good Luck", "GameLauncher Will Continue, When It Failed To Connect To API");
                }

                if (restartAppNoApis == DialogResult.Yes)
                {
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

            }

            IniFile _settingFile = new IniFile("Settings.ini");

            if (!DetectLinux.LinuxDetected()) {
            //Windows 7 Fix
            if (!(_settingFile.KeyExists("PatchesApplied"))) {
                String _OS = (string)Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion").GetValue("productName");
                if(_OS.Contains("Windows 7")) {
                        if (Self.GetInstalledHotFix("KB3020369") == false || Self.GetInstalledHotFix("KB3125574") == false) {
                        String messageBoxPopupKB = String.Empty;
                        messageBoxPopupKB  = "Hey Windows 7 User, in order to play on this server, we need to make additional tweaks to your system.\n";
                        messageBoxPopupKB += "We must make sure you have those Windows Update packages installed:\n\n";

                            if (Self.GetInstalledHotFix("KB3020369") == false) messageBoxPopupKB += "- Update KB3020369\n";
                            if (Self.GetInstalledHotFix("KB3125574") == false) messageBoxPopupKB += "- Update KB3125574\n";

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

                if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x86))
                {
                    var result = MessageBox.Show(
                        "You do not have the 32-bit 2015-2019 VC++ Redistributable Package installed. Click OK to install it.",
                        "Compatibility",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning);

                    if (result != DialogResult.OK)
                    {
                        MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    var wc = new WebClientWithTimeout();
                    wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x86.exe", "VC_redist.x86.exe");
                    var proc = Process.Start(new ProcessStartInfo
                    {
                        Verb = "runas",
                        Arguments = "/quiet",
                        FileName = "VC_redist.x86.exe"
                    });

                    if (proc == null)
                    {
                        MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }

                if (Environment.Is64BitOperatingSystem == true)
                {
                    if (!RedistributablePackage.IsInstalled(RedistributablePackageVersion.VC2015to2019x64))
                    {
                        var result = MessageBox.Show(
                            "You do not have the 64-bit 2015-2019 VC++ Redistributable Package installed. Click OK to install it.",
                            "Compatibility",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Warning);

                        if (result != DialogResult.OK)
                        {
                            MessageBox.Show("The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        var wc = new WebClientWithTimeout();
                        wc.DownloadFile("https://aka.ms/vs/16/release/VC_redist.x64.exe", "VC_redist.x64.exe");
                        var proc = Process.Start(new ProcessStartInfo
                        {
                            Verb = "runas",
                            Arguments = "/quiet",
                            FileName = "VC_redist.x64.exe"
                        });

                        if (proc == null)
                        {
                            MessageBox.Show("Failed to run package installer. The game will not be started.", "Compatibility", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }

            Console.WriteLine("Application path: " + Path.GetDirectoryName(Application.ExecutablePath));

            if (!Self.HasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath))) {
                MessageBox.Show("This application requires admin priviledge");
            }

            Log.StartLogging();

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
                        _settingFile.Write("CDN", CDNList.First().Url);
                    }
                    catch {
                        try {
                            List<CDNObject> CDNList = new List<CDNObject>();
                            WebClientWithTimeout wc3 = new WebClientWithTimeout();
                            String _slresponse = wc3.DownloadString(Self.CDNUrlStaticList);
                            CDNList = JsonConvert.DeserializeObject<List<CDNObject>>(_slresponse);
                            _settingFile.Write("CDN", CDNList.First().Url);
                        }
                        catch {
                            _settingFile.Write("CDN", "http://cdn.worldunited.gg/gamefiles/packed/");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(_settingFile.Read("InstallationDirectory"))) {
                Console.WriteLine("Game path: " + _settingFile.Read("InstallationDirectory"));

                if (!Self.HasWriteAccessToFolder(_settingFile.Read("InstallationDirectory"))) {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                }
            }

            File.Delete("communication.log");
            File.Delete("launcher.log");

            Log.Debug("BUILD: GameLauncher " + Application.ProductVersion);

            if (_settingFile.KeyExists("InstallationDirectory")) {
                if(!File.Exists(_settingFile.Read("InstallationDirectory"))) {
                    Directory.CreateDirectory(_settingFile.Read("InstallationDirectory"));
                }
            }

            if (!_settingFile.KeyExists("IgnoreUpdateVersion"))
            {
                _settingFile.Write("IgnoreUpdateVersion", String.Empty);
            }

            //INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
            //Updated RedTheKitsune Code so it downloads the file if its missing. It also restarts the launcher if the user click on yes on Prompt. - DavidCarbon
            if (!File.Exists("LZMA.dll"))
            {
                try
                {
                    Log.Debug("CORE: Starting LZMA downloader");
                    using (WebClientWithTimeout wc = new WebClientWithTimeout())
                    {
                        wc.DownloadFileAsync(new Uri(Self.fileserver + "/LZMA.dll"), "LZMA.dll");
                    }

                    DialogResult restartApp = MessageBox.Show(null, "Downloaded Missing LZMA.ddl File. \nPlease Restart Launcher, Thanks!", "GameLauncher Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (restartApp == DialogResult.Yes)
                    {
                        Properties.Settings.Default.IsRestarting = true;
                        Properties.Settings.Default.Save();
                        Application.Restart();
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();

                    }
                    if (restartApp == DialogResult.No)
                    {
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug("CORE: Failed to download LZMA. " + ex.Message);
                }
            }

            //StaticConfiguration.DisableErrorTraces = false;

            Log.Debug("CORE: Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            Log.Debug("CORE: Checking current directory");

            if (Self.IsTempFolder(Directory.GetCurrentDirectory())) {
                MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Environment.Exit(0);
            }

			if(!File.Exists("GameLauncherUpdater.exe")) {
                Log.Debug("CORE LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
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
                    Log.Debug("CORE LAUCHER UPDATER: Failed to download updater. " + ex.Message);
                }
            }
            else if (File.Exists("GameLauncherUpdater.exe"))
            {
                String GameLauncherUpdaterLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameLauncherUpdater.exe");
                var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(GameLauncherUpdaterLocation);
                var LauncherUpdaterBuildNumber = LauncherUpdaterBuild.FileVersion;
                var UpdaterBuildNumberResult = LauncherUpdaterBuildNumber.CompareTo(LatestUpdaterBuildVersion);

                if (UpdaterBuildNumberResult < 0)
                {
                    Log.Debug("CORE LAUNCHER UPDATER: " + UpdaterBuildNumberResult + " Builds behind latest Updater!");
                }
                else
                {
                    Log.Debug("CORE LAUNCHER UPDATER: Latest GameLauncherUpdater!");
                }

                if (UpdaterBuildNumberResult < 0) {
                    Log.Debug("CORE LAUNCHER UPDATER: Downloading New GameLauncherUpdater.exe");
                    File.Delete("GameLauncherUpdater.exe");
                    try
                    {
                        using (WebClientWithTimeout wc = new WebClientWithTimeout())
                        {
                            wc.DownloadFileAsync(new Uri(Self.fileserver + "/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("CORE LAUNCHER UPDATER: Failed to download new updater. " + ex.Message);
                    }
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
                Log.Debug("DEBUGGER PROXY: Starting Proxy");
                ServerProxy.Instance.Start();

                Log.Debug("DEBUGGER CORE: Starting MainScreen");
                Application.Run(new MainScreen());

            } else {
                if (NFSW.IsNFSWRunning()) {
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
                            Log.Debug("PROXY: Starting Proxy");
                            ServerProxy.Instance.Start();
                            
                            Log.Debug("CORE: Starting MainScreen");
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
