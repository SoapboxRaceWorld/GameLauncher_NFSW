using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.App.Classes.Logger;
using GameLauncherReborn;
using Microsoft.Win32;
using CommandLine;
using System.Globalization;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.Events;
using Newtonsoft.Json;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.ModNet;

namespace GameLauncher
{
    internal static class Program
    {
        public static string LatestUpdaterBuildVersion = "1.0.0.4";

        internal class Arguments
        {
            [Option('p', "parse", Required = false, HelpText = "Parses URI")]
            public string Parse { get; set; }
        }

        [STAThread]
        internal static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Arguments>(args).WithParsed(Main2);
        }

        private static void Main2(Arguments args)
        {

            if (!DetectLinux.LinuxDetected())
            {
                //Check if User has .NETFramework 4.6.2 or later Installed
                const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";

                using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey))
                {
                    if (ndpKey != null && ndpKey.GetValue("Release") != null && (int)ndpKey.GetValue("Release") >= 394802)
                    {
                        //Install Carbon Crew CA
                        CertificateStore.Check();
                    }
                    else
                    {
                        DialogResult frameworkError = MessageBox.Show(null, "This application requires one of the following versions of the .NET Framework:\n" +
                            " .NETFramework, Version=v4.6.2 \n\nDo you want to install this .NET Framework version now?", "GameLauncher.exe - This application could not be started.", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                        if (frameworkError == DialogResult.Yes)
                        {
                            Process.Start("https://dotnet.microsoft.com/download/dotnet-framework");
                        }
                        Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                    }
                }
            }

            File.Delete("communication.log");
            File.Delete("launcher.log");

            Log.StartLogging();

            FileSettingsSave.NullSafeSettings();
            FileAccountSave.NullSafeAccount();

            Self.currentLanguage = CultureInfo.CurrentCulture.Name.Split('-')[0].ToUpper();
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            if (UriScheme.IsCommandLineArgumentsInstalled())
            {
                UriScheme.InstallCommandLineArguments(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), AppDomain.CurrentDomain.FriendlyName));
                if (args.Parse != null)
                {
                    new UriScheme(args.Parse);
                }
            }

            if (EnableInsider.ShouldIBeAnInsider() == true)
            {
                Log.Build("INSIDER: GameLauncher " + Application.ProductVersion + "_" + EnableInsider.BuildNumber());
            }
            else
            {
                Log.Build("BUILD: GameLauncher " + Application.ProductVersion);
            }

            if (Properties.Settings.Default.IsRestarting)
            {
                Properties.Settings.Default.IsRestarting = false;
                Properties.Settings.Default.Save();
                Thread.Sleep(3000);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);

            /* Set Launcher Directory */
            Log.Info("CORE: Setting up current directory: " + Path.GetDirectoryName(Application.ExecutablePath));
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Application.ExecutablePath));

            if (!DetectLinux.LinuxDetected()) 
            {
                Log.Info("CORE: Checking current directory");

                switch (Self.CheckFolder(Directory.GetCurrentDirectory())) 
                {
                    case FolderType.IsTempFolder:
                        MessageBox.Show(null, "Please, extract me and my DLL files before executing...", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsUsersFolders:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher.\n\nSpecial Folders such as:" +
                            "\n\nDownloads, Documents, Desktop, Videos, Music, OneDrive, or Any Type of User Folders" +
                            "\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsProgramFilesFolder:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher." +
                            "\n\nSpecial Folders such as:\n\nProgram Files or Program Files (x86)\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                    case FolderType.IsWindowsFolder:
                        MessageBox.Show(null, "Please, choose a different directory for the game launcher." +
                            "\n\nSpecial Folder such as:\n\nWindows\n\nAre Disadvised", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        Environment.Exit(0);
                        break;
                }

                if (!Self.HasWriteAccessToFolder(Path.GetDirectoryName(Application.ExecutablePath)))
                {
                    MessageBox.Show("This application requires admin priviledge");
                }

                //Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon
                try
                {
                    try
                    {
                        switch (APIStatusChecker.CheckStatus("http://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest"))
                        {
                            case API.Online:
                                WebClient update_data = new WebClient();
                                update_data.CancelAsync();
                                update_data.Headers.Add("user-agent", "GameLauncherUpdater " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                update_data.DownloadStringAsync(new Uri("http://api.github.com/repos/SoapboxRaceWorld/GameLauncherUpdater/releases/latest"));
                                update_data.DownloadStringCompleted += (sender, e) => {
                                    GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(e.Result);

                                    if (GHAPI.TagName != null)
                                    {
                                        Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GHAPI.TagName);
                                        LatestUpdaterBuildVersion = GHAPI.TagName;
                                    }
                                    Log.Info("LAUNCHER UPDATER: Latest Version -> " + LatestUpdaterBuildVersion);
                                };
                                break;
                            default:
                                Log.Error("LAUNCHER UPDATER: Failed to Retrive Latest Updater Information from GitHub");
                                break;
                        }
                    }
                    catch
                    {
                        var GetLatestUpdaterBuildVersion = new WebClient().DownloadString(Self.secondstaticapiserver + "/Version.txt");
                        if (!string.IsNullOrEmpty(GetLatestUpdaterBuildVersion))
                        {
                            Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GetLatestUpdaterBuildVersion);
                            LatestUpdaterBuildVersion = GetLatestUpdaterBuildVersion;
                        }
                    }
                    Log.Info("LAUNCHER UPDATER: Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                }
                catch (Exception ex)
                {
                    Log.Error("LAUNCHER UPDATER: Failed to get new version file: " + ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                if (!Self.HasWriteAccessToFolder(FileSettingsSave.GameInstallation))
                {
                    MessageBox.Show("This application requires admin priviledge. Restarting...");
                }
            }

            //INFO: this is here because this dll is necessary for downloading game files and I want to make it async.
            //Updated RedTheKitsune Code so it downloads the file if its missing. It also restarts the launcher if the user click on yes on Prompt. - DavidCarbon
            if (!File.Exists("LZMA.dll"))
            {
                try
                {
                    Log.Warning("CORE: Starting LZMA downloader");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileAsync(new Uri(Self.fileserver + "/LZMA.dll"), "LZMA.dll");
                    }

                    DialogResult restartApp = MessageBox.Show(null, "Downloaded Missing LZMA.dll File. \nPlease Restart Launcher, Thanks!", "GameLauncher Restart Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (restartApp == DialogResult.Yes)
                    {
                        Properties.Settings.Default.IsRestarting = true;
                        Properties.Settings.Default.Save();
                        Application.Restart();

                    }
                    
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }
                catch (Exception ex)
                {
                    Log.Error("CORE: Failed to download LZMA. " + ex.Message);
                }
            }

            //StaticConfiguration.DisableErrorTraces = false;

            if (!File.Exists("servers.json"))
            {
                try 
                {
                    File.WriteAllText("servers.json", "[]");
                } catch { /* ignored */ }
            }

            if (Properties.Settings.Default.IsRestarting)
            {
                Properties.Settings.Default.IsRestarting = false;
                Properties.Settings.Default.Save();
                Thread.Sleep(3000);
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (!DetectLinux.LinuxDetected())
            {

            }

            if (Debugger.IsAttached)
            {
                ShowMainScreen();
            } 
            else
            {
                if (NFSW.IsNFSWRunning())
                {
                    MessageBox.Show(null, "An instance of Need for Speed: World is already running", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                }

                var mutex = new Mutex(false, "GameLauncherNFSW-MeTonaTOR");
                try
                {
                    if (mutex.WaitOne(0, false))
                    {
                        string[] files = {
                            "CommandLine.dll - 2.8.0",
                            "DiscordRPC.dll - 1.0.169.0",
                            "Flurl.dll - 3.0.1",
                            "Flurl.Http.dll - 2.4.2",
                            "INIFileParser.dll - 2.5.2",
                            "LZMA.dll - 9.10 beta",
                            "Microsoft.WindowsAPICodePack.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.Shell.dll - 1.1.0.0",
                            "Microsoft.WindowsAPICodePack.ShellExtensions.dll - 1.1.0.0",
                            "Nancy.dll - 2.0.0",
                            "Nancy.Hosting.Self.dll - 2.0.0",
                            "Newtonsoft.Json.dll - 12.0.3",
                            "System.Runtime.InteropServices.RuntimeInformation.dll - 4.6.24705.01. Commit Hash: 4d1af962ca0fede10beb01d197367c2f90e92c97",
                            "System.ValueTuple.dll - 4.6.26515.06 @BuiltBy: dlab-DDVSOWINAGE059 @Branch: release/2.1 @SrcCode: https://github.com/dotnet/corefx/tree/30ab651fcb4354552bd4891619a0bdd81e0ebdbf",
                            "WindowsFirewallHelper.dll - 1.6.3.40"
                        };

                        var missingfiles = new List<string>();

                        if (!DetectLinux.LinuxDetected())
                        { //MONO Hates that...
                            foreach (var file in files) {
                                var splitFileVersion = file.Split(new string[] { " - " }, StringSplitOptions.None);

                                if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + splitFileVersion[0]))
                                {
                                    missingfiles.Add(splitFileVersion[0] + " - Not Found");
                                } 
                                else
                                {
                                    try
                                    {
                                        var versionInfo = FileVersionInfo.GetVersionInfo(splitFileVersion[0]);
                                        string[] versionsplit = versionInfo.ProductVersion.Split('+');
                                        string version = versionsplit[0];

                                        if (version == "")
                                        {
                                            missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                        } 
                                        else
                                        { 
                                            if (Self.CheckArchitectureFile(splitFileVersion[0]) == false) 
                                            {
                                                missingfiles.Add(splitFileVersion[0] + " - Wrong Architecture");
                                            } 
                                            else
                                            {
                                                if (version != splitFileVersion[1])
                                                {
                                                    missingfiles.Add(splitFileVersion[0] + " - Invalid Version (" + splitFileVersion[1] + " != " + version + ")");
                                                }
                                            }
                                        }
                                    } 
                                    catch
                                    {
                                        missingfiles.Add(splitFileVersion[0] + " - Invalid File");
                                    }
                                }
                            }
                        }
                        if (missingfiles.Count != 0)
                        {
                            var message = "Cannot launch GameLauncher. The following files are invalid:\n\n";

                            foreach (var file in missingfiles)
                            {
                                message += "• " + file + "\n";
                            }

                            MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            ShowMainScreen();
                        }
                    } 
                    else
                    {
                        MessageBox.Show(null, "An instance of Launcher is already running.", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                } 
                finally
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

        private static void ShowMainScreen()
        {
            if (!string.IsNullOrEmpty(FileSettingsSave.GameInstallation))
            {
                var linksPath = Path.Combine(FileSettingsSave.GameInstallation + "\\.links");
                ModNetLinksCleanup.CleanLinks(linksPath);
            }

            Log.Visuals("CORE: Starting Splash Screen");
            Application.Run(new SplashScreen());
        }
    }
}
