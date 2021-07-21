using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
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
                Log.Checking("LAUNCHER UPDATER: Is Update to Date or not");
                DiscordLauncherPresense.Status("Start Up", "Checking Launcher and Updater Release Information");

                /* Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon */
                try
                {
                    FunctionStatus.TLS();
                    Uri URLCall = new Uri(URLs.GitHub_Updater);
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    WebClient Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");

                    string json_data = Client.DownloadString(URLCall);

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
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                    }

                    if (LatestUpdaterBuildVersion == "1.0.0.4")
                    {
                        Log.Info("LAUNCHER UPDATER: Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                }

                /* Check if File needs to be Downloaded or Require an Update */

                if (!File.Exists(Locations.NameUpdater))
                {
                    Log.Info("LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
                    try
                    {
                        FunctionStatus.TLS();
                        Uri URLCall = new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        WebClient Client = new WebClient
                        {
                            Encoding = Encoding.UTF8
                        };
                        Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion 
                            + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            if (new FileInfo(Locations.NameUpdater).Length == 0)
                            {
                                File.Delete(Locations.NameUpdater);
                            }
                        };
                        Client.DownloadFile(URLCall, Locations.NameUpdater);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                    }
                }
                else if (File.Exists(Locations.NameUpdater))
                {
                    try
                    {
                        var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(
                            Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameUpdater)));
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
                            Log.Info("LAUNCHER UPDATER: Downloading New " + Locations.NameUpdater);
                            File.Delete(Locations.NameUpdater);

                            try
                            {
                                FunctionStatus.TLS();
                                Uri URLCall = new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                                WebClient Client = new WebClient
                                {
                                    Encoding = Encoding.UTF8
                                };
                                Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion 
                                    + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                                Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                                {
                                    if (new FileInfo(Locations.NameUpdater).Length == 0)
                                    {
                                        File.Delete(Locations.NameUpdater);
                                    }
                                };
                                Client.DownloadFile(URLCall, Locations.NameUpdater);
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                            }
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                    }
                }

                Log.Completed("LAUNCHER UPDATER: Done");
            }

            Log.Info("LAUNCHER UPDATE: Moved to Function");
            /* (Start Process) Check Latest Launcher Version */
            LauncherUpdateCheck.Latest();
        }
    }
}
