using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Proxy.Nancy_;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using SBRW.Launcher.App.UI_Forms.Update_Popup_Screen;
using SBRW.Launcher.Core.Theme;
using System.Net.Cache;
using SBRW.Launcher.App.UI_Forms;
using System.Threading.Tasks;
using SBRW.Launcher.Core.Extension.String_;

namespace SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater
{
    class LauncherUpdateCheck
    {
        public PictureBox status { get; set; }
        public Label text { get; set; }
        public Label description { get; set; }

        public static string CurrentLauncherBuild { get { return EnableInsiderDeveloper.Allowed() ? InsiderInfo.BuildNumberOnly().Replace('-', '.') : Application.ProductVersion; } }
        public static string LatestLauncherBuild { get; set; } = string.Empty;
        public static bool UpgradeAvailable { get; set; }
        private static bool SkipAvailableUpgrade { get; set; }
        public static string VersionJSON { get; set; } = string.Empty;
        public static int Version_JSON_Index { get; set; }
        private static bool ValidJSONDownload { get; set; }
        public static bool UpdatePopupStoppedSplashScreen { get; set; }

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription)
        {
            status = statusImage;
            text = statusText;
            description = statusDescription;
        }

        private static string Insider_Release_Tag(string JSON_Data)
        {
            string Temp_Latest_Launcher_Build = string.Empty;

            if (JSON_Data.Valid_Json())
            {
                int Top_Ten = 0;
                bool Latest_Found_Build = false;

                List<GitHubRelease> Scrollable_List = new List<GitHubRelease>();
#pragma warning disable CS8604 // Possible null reference argument.
                Scrollable_List.AddRange(JsonConvert.DeserializeObject<List<GitHubRelease>>(JSON_Data));
#pragma warning restore CS8604 // Possible null reference argument.

                if (Scrollable_List.Count > 0)
                {
                    foreach (GitHubRelease GH_Releases in Scrollable_List)
                    {
                        if (!string.IsNullOrWhiteSpace(GH_Releases.TagName))
                        {
                            if (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed())
                            {
                                Log.Info("Github " + (GH_Releases.Pre_Release ? "Pre-" : "") + "Release Version Tag: " + GH_Releases.TagName);
                            }

                            if (!GH_Releases.Pre_Release && !Latest_Found_Build)
                            {
                                Latest_Found_Build = true;
                                Temp_Latest_Launcher_Build = GH_Releases.TagName;
                            }

                            if (CurrentLauncherBuild.Outdated(GH_Releases.TagName))
                            {
                                Version_JSON_Index = Top_Ten;
                                return GH_Releases.TagName;
                            }
                            else if (Top_Ten >= 20)
                            {
                                break;
                            }
                            else
                            {
                                Top_Ten++;
                            }
                        }
                    }
                }
            }

            return Temp_Latest_Launcher_Build;
        }

        public static async void Latest(bool Start_Up_Function = true)
        {
            LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER UPDATE", "Is Version Up to Date or not" + (Start_Up_Function ? "" : " (Settings)"));

            if (Start_Up_Function)
            {
                Presence_Launcher.Status(0, "Checking Latest Launcher Release Information");
            }
            
            await Task.Run(() =>
            {
                try
                {
                    Uri URLCall = new Uri(EnableInsiderDeveloper.Allowed() ? URLs.GitHub_Launcher_Development : EnableInsiderBetaTester.Allowed() ?
                        URLs.GitHub_Launcher_Beta : URLs.GitHub_Launcher_Stable);
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
                        VisualsAPIChecker.GitHubAPI = true;
                    }
                    catch (WebException Error)
                    {
                        API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                            Error, Error.Response as HttpWebResponse);
                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATE [GITHUB]", Error.InnerException.Message, false, true);
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("LAUNCHER UPDATE [GITHUB]", string.Empty, Error, string.Empty, true);
                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATE [GITHUB]", Error.InnerException.Message, false, true);
                        }
                    }
                    finally
                    {
                        Client?.Dispose();

                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }

                    if (VersionJSON.Valid_Json() && VisualsAPIChecker.GitHubAPI)
                    {
#pragma warning disable CS8602 // Null Safe Check Done Above
                        LatestLauncherBuild = (!EnableInsiderDeveloper.Allowed() && EnableInsiderBetaTester.Allowed()) ?
                            Insider_Release_Tag(VersionJSON) : JsonConvert.DeserializeObject<GitHubRelease>(VersionJSON).TagName.Replace('-', '.');
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER UPDATE", "GitHub Latest Version -> " + LatestLauncherBuild);
                        ValidJSONDownload = true;
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATE", "Failed to retrieve Latest Build information from GitHub" + (Start_Up_Function ? "" : " (Settings)"));
                        ValidJSONDownload = false;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATE [GITHUB]", string.Empty, Error, string.Empty, true);
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LAUNCHER UPDATE [GITHUB]", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }
            });
            LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER UPDATE", "Done" + (Start_Up_Function ? "" : " (Settings)"));

            if (!UpdateStatusResult())
            {
                if (Start_Up_Function)
                {
                    LogToFileAddons.Parent_Log_Screen(1, "FIRST TIME RUN", "Moved to Function");
                    /* Do First Time Run Checks */
                    Parent_Screen.First_Time_Run();
                }
            }
            else
            {
                if (Presence_Launcher.Running())
                {
                    Presence_Launcher.Stop("Close");
                }

                if (Proxy_Settings.Running())
                {
                    Proxy_Server.Instance.Stop("Force Close");
                }

                Application.Exit();
            }
        }

        public static bool UpdateStatusResult(bool Disable_Startup_Functions = false)
        {
            bool StatusUpdate = false;
            if (!string.IsNullOrWhiteSpace(LatestLauncherBuild))
            {
                if (CurrentLauncherBuild.Outdated(LatestLauncherBuild))
                {
                    if (!Disable_Startup_Functions)
                    {
                        LogToFileAddons.Parent_Log_Screen(1, "LAUNCHER POPUP", "Checking if Popup is Required");
                    }

                    if (Save_Settings.Live_Data.Update_Version_Skip != LatestLauncherBuild || Disable_Startup_Functions)
                    {
                        if (!Disable_Startup_Functions)
                        {
                            UpdatePopupStoppedSplashScreen = true;
                        }

                        DialogResult UserResult = new Screen_Update_Popup().ShowDialog();

                        if (UserResult == DialogResult.OK)
                        {
                            StatusUpdate = true;
                            string UpdaterPath = Path.Combine(Locations.LauncherFolder, Locations.NameUpdater);
                            if (File.Exists(UpdaterPath))
                            {
                                LogToFileAddons.Parent_Log_Screen(2, "LAUNCHER POPUP", Process.GetCurrentProcess().Id.ToString() + " " +
                                    (EnableInsiderDeveloper.Allowed() ? "Developer" + " \"" + CurrentLauncherBuild + "\"": EnableInsiderBetaTester.Allowed() ? "Preview" : "Stable"));
                            
                                Process.Start(UpdaterPath, Process.GetCurrentProcess().Id.ToString() + " " +
                                    (EnableInsiderDeveloper.Allowed() ? "Developer" + " \"" + CurrentLauncherBuild + "\"": EnableInsiderBetaTester.Allowed() ? "Preview" : "Stable"));
                            }
                            else
                            {
#if NETFRAMEWORK
                                if (EnableInsiderDeveloper.Allowed())
                                {
                                    Process.Start(@"https://github.com/DavidCarbon-SBRW/SBRW.Launcher.Releases/releases/latest");
                                }
                                else if (EnableInsiderBetaTester.Allowed())
                                {
                                    Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases");
                                }
                                else
                                {
                                    Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                                }
#else
                                if (EnableInsiderDeveloper.Allowed())
                                {
                                    Process.Start("explorer.exe", "https://github.com/DavidCarbon-SBRW/SBRW.Launcher.Releases/releases/latest");
                                }
                                else if (EnableInsiderBetaTester.Allowed())
                                {
                                    Process.Start("explorer.exe", "https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases");
                                }
                                else
                                {
                                    Process.Start("explorer.exe", "https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                                }
#endif
                                MessageBox.Show(null, Translations.Database("LauncherUpdateCheck_FS"), "GameLauncher", MessageBoxButtons.OK);
                            }
                        }
                        else if (UserResult == DialogResult.Ignore)
                        {
                            /* Save and Allow Version Update Skip Once user Reaches Main Screen */
                            SkipAvailableUpgrade = true;
                        }
                    }
                    else if (!Disable_Startup_Functions)
                    {
                        LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER POPUP", "User Saved Skip Version Detected");
                    }
                }
                else if (!Disable_Startup_Functions)
                {
                    LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER POPUP", "Update to Date");
                }
            }
            else if (!Disable_Startup_Functions)
            {
                LogToFileAddons.Parent_Log_Screen(3, "LAUNCHER POPUP", "Unable to run Update Popup (Null String)");
            }

            if (!Disable_Startup_Functions)
            {
                FunctionStatus.LoadingComplete = true;
            }

            return StatusUpdate;
        }

        public void ChangeVisualStatus()
        {
            if (!string.IsNullOrWhiteSpace(LatestLauncherBuild))
            {
                if (CurrentLauncherBuild.Preview(LatestLauncherBuild))
                {
                    string WhatBuildAmI;
                    if (EnableInsiderDeveloper.Allowed())
                    {
                        WhatBuildAmI = Translations.Database("LauncherUpdateCheck_VS_Insider_Dev");
                    }
                    else if (EnableInsiderBetaTester.Allowed())
                    {
                        WhatBuildAmI = Translations.Database("LauncherUpdateCheck_VS_Insider_Beta");
                    }
                    else
                    {
                        WhatBuildAmI = Translations.Database("LauncherUpdateCheck_VS_Insider_Unofficial");
                    }

                    text.Text = Translations.Database("LauncherUpdateCheck_VS_Insider") + "\n - " + WhatBuildAmI +
                        " " + Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Build");
                    status.BackgroundImage = Image_Icon.Engine_Warning;
                    text.ForeColor = Color_Text.S_Warning;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Stable") + " " + LatestLauncherBuild +
                        "\n" + Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Current") + " " + CurrentLauncherBuild;

                    if (!string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Update_Version_Skip))
                    {
                        Save_Settings.Live_Data.Update_Version_Skip = string.Empty;
                        Save_Settings.Save();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. " +
                            "You are currenly using a " + WhatBuildAmI + " Build!");
                    }
                }
                else if (CurrentLauncherBuild.Current(LatestLauncherBuild))
                {
                    text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_No_Update");
                    status.BackgroundImage = Image_Icon.Engine_Good;
                    text.ForeColor = Color_Text.S_Sucess;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + CurrentLauncherBuild;

                    if (Save_Settings.Live_Data.Update_Version_Skip == CurrentLauncherBuild)
                    {
                        Save_Settings.Live_Data.Update_Version_Skip = string.Empty;
                        Save_Settings.Save();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Latest Game Launcher!");
                    }
                }
                else
                {
                    text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Update");
                    status.BackgroundImage = Image_Icon.Engine_Warning;
                    text.ForeColor = Color_Text.S_Warning;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Update_New") + " " + LatestLauncherBuild + "\n" +
                        Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Current") + " " + CurrentLauncherBuild;
                    UpgradeAvailable = true;
                    if (SkipAvailableUpgrade)
                    {
                        Save_Settings.Live_Data.Update_Version_Skip = LatestLauncherBuild;
                        Save_Settings.Save();
                        Log.Info("IGNOREUPDATEVERSION: User had skipped latest Launcher Version!");
                    }
                }
            }
            else if (VisualsAPIChecker.GitHubAPI && !ValidJSONDownload)
            {
                text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Invalid_JSON");
                status.BackgroundImage = Image_Icon.Engine_Error;
                text.ForeColor = Color_Text.S_Error;
                description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + CurrentLauncherBuild;
            }
            else
            {
                text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Invalid_Error");
                status.BackgroundImage = Image_Icon.Engine_Unknown;
                text.ForeColor = Color_Text.L_Three;
                description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + CurrentLauncherBuild;
            }
        }
    }
}
