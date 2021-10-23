using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
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
        private static string LatestUpdaterBuildVersion = "1.0.0.8";
        private static string VersionJSON;

        /* Check If Updater Exists or Requires an Update */
        public static void Check()
        {
            Log.Checking("LAUNCHER UPDATER: Is Version Up to Date or not");
            DiscordLauncherPresence.Status("Start Up", "Checking Launcher and Updater Release Information");

            /* Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon */
            try
            {
                bool IsGithubOnline = false;
                Uri URLCall = new Uri(URLs.GitHub_Updater);
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };
                if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                else
                {
                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                }

                try
                {
                    VersionJSON = Client.DownloadString(URLCall);
                    IsGithubOnline = true;
                }
                catch (WebException Error)
                {
                    APIChecker.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                        Error, (HttpWebResponse)Error.Response);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }

                bool IsJsonValid = false;

                try
                {
                    if (IsJSONValid.ValidJson(VersionJSON) && IsGithubOnline)
                    {
                        GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(VersionJSON);

                        if (GHAPI.TagName != null)
                        {
                            Log.Info("LAUNCHER UPDATER: Setting Latest Version -> " + GHAPI.TagName);
                            LatestUpdaterBuildVersion = GHAPI.TagName;
                            IsJsonValid = true;
                        }

                        Log.Info("LAUNCHER UPDATER: Latest Version -> " + LatestUpdaterBuildVersion);

                        if (GHAPI != null)
                        {
                            GHAPI = null;
                        }
                    }
                    else
                    {
                        Log.Warning("LAUNCHER UPDATER: Received Invalid JSON Data");
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
                }

                if (!IsGithubOnline || !IsJsonValid)
                {
                    Log.Info("LAUNCHER UPDATER: Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("LAUNCHER UPDATER", null, Error, null, true);
            }
            finally
            {
                if (VersionJSON != null)
                {
                    VersionJSON = null;
                }
            }

            /* Check if File needs to be Downloaded or Require an Update */
            string UpdaterPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameUpdater));

            if (!File.Exists(UpdaterPath))
            {
                Log.Info("LAUNCHER UPDATER: Starting GameLauncherUpdater downloader");
                try
                {
                    Uri URLCall =
                        new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                    else
                    {
                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }
                    Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                    {
                        if (File.Exists(UpdaterPath))
                        {
                            try
                            {
                                if (new FileInfo(UpdaterPath).Length == 0)
                                {
                                    File.Delete(UpdaterPath);
                                }
                            }
                            catch (Exception Error)
                            {
                                LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE Error #1]", null, Error, null, true);
                            }
                        }
                    };

                    try
                    {
                        Client.DownloadFile(URLCall, UpdaterPath);
                    }
                    catch (WebException Error)
                    {
                        APIChecker.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                            Error, (HttpWebResponse)Error.Response);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE DL #1]", null, Error, null, true);
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [!FileExists]", null, Error, null, true);
                }
            }
            else if (File.Exists(UpdaterPath))
            {
                try
                {
                    var LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(UpdaterPath);
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

                        Uri URLCall =
                            new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8
                        };
                        if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                        else
                        {
                            Client.Headers.Add("user-agent", "SBRW Launcher " +
                            Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        }
                        Client.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                        {
                            if (File.Exists(UpdaterPath))
                            {
                                try
                                {
                                    if (new FileInfo(UpdaterPath).Length == 0)
                                    {
                                        File.Delete(UpdaterPath);
                                    }
                                }
                                catch (Exception Error)
                                {
                                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE Error #2]", null, Error, null, true);
                                }
                            }
                        };

                        try
                        {
                            Client.DownloadFile(URLCall, Locations.NameUpdater);
                        }
                        catch (WebException Error)
                        {
                            APIChecker.StatusCodes(
                                "https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe",
                                Error, (HttpWebResponse)Error.Response);
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE DL #2]", null, Error, null, true);
                        }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [FileExists]", null, Error, null, true);
                }
            }

            Log.Completed("LAUNCHER UPDATER: Done");

            Log.Info("LAUNCHER UPDATE: Moved to Function");
            /* (Start Process) Check Latest Launcher Version */
            LauncherUpdateCheck.Latest();
        }
    }
}
