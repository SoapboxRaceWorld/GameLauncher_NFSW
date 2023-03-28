using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Net.Cache;
using System.Threading.Tasks;
using SBRW.Launcher.Core.Extension.String_;

namespace SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater
{
    class UpdaterExecutable
    {
        /* Hardcoded Default Version for Updater Version  */
        private static string LatestUpdaterBuildVersion { get; set; } = "1.0.1.24";
        private static string VersionJSON { get; set; } = string.Empty;

        /* Check If Updater Exists or Requires an Update */
        public static async void Check()
        {
            LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER UPDATER", "Is Version Up to Date or not");
            Presence_Launcher.Status(0, "Checking Launcher and Updater Release Information");

            /* Update this text file if a new GameLauncherUpdater.exe has been delployed - DavidCarbon */
            await Task.Run(() =>
            {
                try
                {
                    bool IsGithubOnline = false;
                    Uri URLCall = new Uri(URLs.GitHub_Updater);
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    if (!Launcher_Value.Launcher_Alternative_Webcalls())
                    {
                        Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                    }
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
                        API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                            Error, Error.Response as HttpWebResponse);

                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER", Error.InnerException.Message, false, true);
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER", string.Empty, Error, string.Empty, true);

                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER", Error.InnerException.Message, false, true);
                        }
                    }
                    finally
                    {
                        Client?.Dispose();

                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }

                    bool IsJsonValid = false;

                    try
                    {
                        if (VersionJSON.Valid_Json() && IsGithubOnline)
                        {
#pragma warning disable CS8600 // Null Safe Check Done Above
                            GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(VersionJSON);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                            if (GHAPI != null && GHAPI.TagName != null)
                            {
                                LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Setting Latest Version -> " + GHAPI.TagName);
                                LatestUpdaterBuildVersion = GHAPI.TagName;
                                IsJsonValid = true;
                            }

                            LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Latest Version -> " + LatestUpdaterBuildVersion);
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(4, "LAUNCHER UPDATER", "Received Invalid JSON Data");
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER", string.Empty, Error, string.Empty, true);

                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER", Error.InnerException.Message, false, true);
                        }
                    }
                    finally
                    {
                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }

                    if (!IsGithubOnline || !IsJsonValid)
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Fail Safe Latest Version -> " + LatestUpdaterBuildVersion);
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER", string.Empty, Error, string.Empty, true);

                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    if (!string.IsNullOrWhiteSpace(VersionJSON))
                    {
                        VersionJSON = string.Empty;
                    }

                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }
            });

            /* Check if File needs to be Downloaded or Require an Update */
            string UpdaterPath = Path.Combine(Locations.LauncherFolder, Locations.NameUpdater);

            if (!File.Exists(UpdaterPath))
            {
                LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Starting GameLauncherUpdater downloader");

                await Task.Run(() =>
                {
                    try
                    {
                        Uri URLCall =
                            new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                        if (!Launcher_Value.Launcher_Alternative_Webcalls())
                        {
                            Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                        }
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
                                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE Error #1]", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE DL #1]", Error.InnerException.Message, false, true);
                                    }
                                }
                                finally
                                {
                                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                                    GC.Collect(); 
                                    #endif
                                }
                            }
                        };

                        try
                        {
                            Client.DownloadFile(URLCall, UpdaterPath);
                        }
                        catch (WebException Error)
                        {
                            API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                                Error, Error.Response as HttpWebResponse);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE DL #1]", Error.InnerException.Message, false, true);
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE DL #1]", string.Empty, Error, string.Empty, true);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE DL #1]", Error.InnerException.Message, false, true);
                            }
                        }
                        finally
                        {
                            Client?.Dispose();

                            #if !(RELEASE_UNIX || DEBUG_UNIX) 
                            GC.Collect(); 
                            #endif
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATER [!FileExists]", string.Empty, Error, string.Empty, true);
                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [!FileExists]", Error.InnerException.Message, false, true);
                        }
                    }
                    finally
                    {
                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }
                });
            }
            else if (File.Exists(UpdaterPath))
            {
                try
                {
                    FileVersionInfo LauncherUpdaterBuild = FileVersionInfo.GetVersionInfo(UpdaterPath);
                    string LauncherUpdaterBuildNumber = LauncherUpdaterBuild.FileVersion??"0.0.0.0";
                    bool UpdaterBuildNumber_Outdated = LauncherUpdaterBuildNumber.Outdated(LatestUpdaterBuildVersion);

                    LogToFileAddons.Parent_Log_Screen(8, "LAUNCHER UPDATER BUILD", "GameLauncherUpdater " + LauncherUpdaterBuildNumber, false, true);

                    if (UpdaterBuildNumber_Outdated)
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Old Updater Found!", false, true);
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Latest GameLauncherUpdater!", false, true);
                    }


                    if (UpdaterBuildNumber_Outdated)
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATER", "Downloading New " + Locations.NameUpdater, false, true);
                        File.Delete(Locations.NameUpdater);

                        Uri URLCall =
                            new Uri("https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe");
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                        ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                        var Client = new WebClient
                        {
                            Encoding = Encoding.UTF8,
                            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                        };
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                        if (!Launcher_Value.Launcher_Alternative_Webcalls())
                        {
                            Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) };
                        }
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
                                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE Error #2]", string.Empty, Error, string.Empty, true);
                                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                                    {
                                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE Error #2]", Error.InnerException.Message, false, true);
                                    }
                                }
                                finally
                                {
#if !(RELEASE_UNIX || DEBUG_UNIX)
                                    GC.Collect();
#endif
                                }
                            }
                        };

                        try
                        {
                            await Task.Run(() =>
                            {
                                Client.DownloadFile(URLCall, Locations.NameUpdater);
                            });
                        }
                        catch (WebException Error)
                        {
                            API_Core.StatusCodes(
                                "https://github.com/SoapboxRaceWorld/GameLauncherUpdater/releases/latest/download/GameLauncherUpdater.exe",
                                Error, Error.Response as HttpWebResponse);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE DL #2]", Error.InnerException.Message, false, true);
                            }
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("LAUNCHER UPDATER [EXE DL #2]", string.Empty, Error, string.Empty, true);
                            if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                            {
                                LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [EXE DL #2]", Error.InnerException.Message, false, true);
                            }
                        }
                        finally
                        {
                            Client?.Dispose();

#if !(RELEASE_UNIX || DEBUG_UNIX)
                            GC.Collect();
#endif
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATER [FileExists]", string.Empty, Error, string.Empty, true);

                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATER [FileExists]", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }
            }
            LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER UPDATER", "Done");

            LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATE", "Moved to Function");
            /* (Start Process) Check Latest Launcher Version */
            LauncherUpdateCheck.Latest();
        }
    }
}
