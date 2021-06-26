using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.LauncherUpdater
{
    class UpdaterExecutable
    {
        /* Hardcoded Default Version for Updater Version  */
        private static string LatestUpdaterBuildVersion = "1.0.0.4";

        /* Check If Updater Exists or Requires an Update */
        public static void Check()
        {
            if (!DetectLinux.LinuxDetected())
            {
                DiscordLauncherPresense.Status("Start Up", "Checking Launcher and Updater Release Information");

                /* Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon */
                try
                {
                    FunctionStatus.TLS();
                    Uri URLCall = new Uri(URLs.GitHub_Updater);
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    WebClient Client = new WebClient();
                    Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    var json_data = Client.DownloadString(URLCall);
                    try
                    {
                        GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(json_data);

                        if (GHAPI.TagName != null)
                        {
                            Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GHAPI.TagName);
                            LatestUpdaterBuildVersion = GHAPI.TagName;
                        }

                        Log.Info("LAUNCHER UPDATER: Latest Version -> " + LatestUpdaterBuildVersion);
                    }
                    catch (Exception error)
                    {
                        Log.Error("LAUNCHER UPDATER: " + error.Message);
                    }

                    if (LatestUpdaterBuildVersion == "1.0.0.4")
                    {
                        Log.Info("LAUNCHER UPDATER: Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LAUNCHER UPDATER: Failed to get new version file: " + error.Message);
                }

                /* Check if File needs to be Downloaded or Require an Update */

                if (!File.Exists("GameLauncherUpdater.exe"))
                {
                    Log.Info("LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
                    try
                    {
                        FunctionStatus.TLS();
                        Uri URLCall = new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        WebClient Client = new WebClient();
                        Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            if (new FileInfo("GameLauncherUpdater.exe").Length == 0)
                            {
                                File.Delete("GameLauncherUpdater.exe");
                            }
                        };
                        Client.DownloadFile(URLCall, "GameLauncherUpdater.exe");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("LAUCHER UPDATER: Failed to download updater. " + ex.Message);
                    }
                }
                else if (File.Exists("GameLauncherUpdater.exe"))
                {
                    String GameLauncherUpdaterLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameLauncherUpdater.exe");
                    var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(GameLauncherUpdaterLocation);
                    var LauncherUpdaterBuildNumber = LauncherUpdaterBuild.FileVersion;
                    var UpdaterBuildNumberResult = LauncherUpdaterBuildNumber.CompareTo(LatestUpdaterBuildVersion);

                    Log.Build("LAUNCHER UPDATER BUILD: GameLauncherUpdater " + LauncherUpdaterBuildNumber);
                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: " + UpdaterBuildNumberResult + " Builds behind latest Updater!");
                    }
                    else
                    {
                        Log.Info("LAUNCHER UPDATER: Latest GameLauncherUpdater!");
                    }

                    if (UpdaterBuildNumberResult < 0)
                    {
                        Log.Info("LAUNCHER UPDATER: Downloading New GameLauncherUpdater.exe");
                        File.Delete("GameLauncherUpdater.exe");

                        try
                        {
                            FunctionStatus.TLS();
                            Uri URLCall = new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                            ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                            WebClient Client = new WebClient();
                            Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                            Client.DownloadFile(URLCall, "GameLauncherUpdater.exe");
                        }
                        catch (Exception ex)
                        {
                            Log.Error("LAUNCHER UPDATER: Failed to download new updater. " + ex.Message);
                        }
                    }
                }
            }

            /* (Start Process) Check Latest Launcher Version */
            LauncherUpdateCheck.Latest();
        }
    }
}
