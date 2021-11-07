using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Proxy;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.UI_Forms.Splash_Screen;
using GameLauncher.App.UI_Forms.UpdatePopup_Screen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.LauncherUpdater
{
    class LauncherUpdateCheck
    {
        public PictureBox status;
        public Label text;
        public Label description;

        public static string CurrentLauncherBuild = Application.ProductVersion;
        public static string LatestLauncherBuild;
        public static bool UpgradeAvailable = false;
        private static bool SkipAvailableUpgrade = false;
        public static string VersionJSON;
        private static bool ValidJSONDownload = false;
        public static int Revisions;
        public static bool UpdatePopupStoppedSplashScreen = false;

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription)
        {
            status = statusImage;
            text = statusText;
            description = statusDescription;
        }

        public static void Latest()
        {
            Log.Checking("LAUNCHER UPDATE: Is Version Up to Date or not");
            DiscordLauncherPresence.Status("Start Up", "Checking Latest Launcher Release Information");
            try
            {
                Uri URLCall = new Uri((EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed()) ?
                    URLs.GitHub_Launcher_Beta : URLs.GitHub_Launcher_Stable);
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
                    VisualsAPIChecker.GitHubAPI = true;
                }
                catch (WebException Error)
                {
                    APIChecker.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                        Error, (HttpWebResponse)Error.Response);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LAUNCHER UPDATE [GITHUB]", null, Error, null, true);
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }

                if (IsJSONValid.ValidJson(VersionJSON) && VisualsAPIChecker.GitHubAPI)
                {
                    LatestLauncherBuild = (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed()) ?
                        JsonConvert.DeserializeObject<List<GitHubRelease>>(VersionJSON)[0].TagName :
                        JsonConvert.DeserializeObject<GitHubRelease>(VersionJSON).TagName;
                    Log.Info("LAUNCHER UPDATE: GitHub Latest Version -> " + LatestLauncherBuild);
                    ValidJSONDownload = true;
                }
                else
                {
                    Log.Error("LAUNCHER UPDATE: Failed to retrieve Latest Build information from GitHub");
                    ValidJSONDownload = false;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("LAUNCHER UPDATE [GITHUB]", null, Error, null, true);
            }

            Log.Completed("LAUNCHER UPDATE: Done");

            if (!UpdateStatusResult())
            {
                Log.Info("FIRST TIME RUN: Moved to Function");
                /* Do First Time Run Checks */
                FunctionStatus.FirstTimeRun();
            }
            else
            {
                if (DiscordLauncherPresence.Running())
                {
                    DiscordLauncherPresence.Stop("Close");
                }

                if (ServerProxy.Running())
                {
                    ServerProxy.Instance.Stop("Force Close");
                }

                Application.Exit();
            }
        }

        private static bool UpdateStatusResult()
        {
            bool StatusUpdate = false;
            if (!string.IsNullOrWhiteSpace(LatestLauncherBuild))
            {
                Revisions = CurrentLauncherBuild.CompareTo(LatestLauncherBuild);

                if (Revisions < 0)
                {
                    Log.Info("LAUNCHER POPUP: Checking if Popup is Required");

                    if (FileSettingsSave.IgnoreVersion != LatestLauncherBuild)
                    {
                        FunctionStatus.LoadingComplete = true;
                        SplashScreen.ThreadStatus("Stop");
                        UpdatePopupStoppedSplashScreen = true;

                        DialogResult UserResult = new UpdatePopup().ShowDialog();

                        if (UserResult == DialogResult.OK)
                        {
                            StatusUpdate = true;
                            string UpdaterPath = Strings.Encode(Path.Combine(Locations.LauncherFolder, Locations.NameUpdater));
                            if (File.Exists(UpdaterPath))
                            {
                                Process.Start(UpdaterPath, Process.GetCurrentProcess().Id.ToString() + " " +
                                    (EnableInsiderBetaTester.Allowed() || EnableInsiderDeveloper.Allowed() ? "Preview" : "Stable"));
                            }
                            else
                            {
                                Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                                MessageBox.Show(null, Translations.Database("LauncherUpdateCheck_FS"), "GameLauncher", MessageBoxButtons.OK);
                            }
                        }
                        else if (UserResult == DialogResult.Ignore)
                        {
                            /* Save and Allow Version Update Skip Once user Reaches Main Screen */
                            SkipAvailableUpgrade = true;
                        }
                    }
                    else
                    {
                        Log.Completed("LAUNCHER POPUP: User Saved Skip Version Detected");
                    }
                }
                else
                {
                    Log.Completed("LAUNCHER POPUP: Update to Date");
                }
            }
            else
            {
                Log.Completed("LAUNCHER POPUP: Unable to run Update Popup (Null String)");
            }

            if (VersionJSON != null)
            {
                VersionJSON = null;
            }

            return StatusUpdate;
        }

        public void ChangeVisualStatus()
        {
            if (!string.IsNullOrWhiteSpace(LatestLauncherBuild))
            {
                if (Revisions > 0)
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
                    status.BackgroundImage = Theming.UpdateIconWarning;
                    text.ForeColor = Theming.Warning;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Stable") + " " + LatestLauncherBuild +
                        "\n" + Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Current") + " " + Application.ProductVersion;

                    if (!string.IsNullOrWhiteSpace(FileSettingsSave.IgnoreVersion))
                    {
                        FileSettingsSave.IgnoreVersion = String.Empty;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. " +
                            "You are currenly using a " + WhatBuildAmI + " Build!");
                    }
                }
                else if (Revisions == 0)
                {
                    text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_No_Update");
                    status.BackgroundImage = Theming.UpdateIconSuccess;
                    text.ForeColor = Theming.Sucess;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + Application.ProductVersion;

                    if (FileSettingsSave.IgnoreVersion == Application.ProductVersion)
                    {
                        FileSettingsSave.IgnoreVersion = String.Empty;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Latest Game Launcher!");
                    }
                }
                else
                {
                    text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Update");
                    status.BackgroundImage = Theming.UpdateIconWarning;
                    text.ForeColor = Theming.Warning;
                    description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Update_New") + " " + LatestLauncherBuild + "\n" +
                        Translations.Database("LauncherUpdateCheck_VS_Insider_Text_Current") + " " + Application.ProductVersion;
                    UpgradeAvailable = true;
                    if (SkipAvailableUpgrade)
                    {
                        FileSettingsSave.IgnoreVersion = LatestLauncherBuild;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: User had skipped latest Launcher Version!");
                    }
                }
            }
            else if (VisualsAPIChecker.GitHubAPI && !ValidJSONDownload)
            {
                text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Invalid_JSON");
                status.BackgroundImage = Theming.UpdateIconError;
                text.ForeColor = Theming.Error;
                description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + Application.ProductVersion;
            }
            else
            {
                text.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Invalid_Error");
                status.BackgroundImage = Theming.UpdateIconUnknown;
                text.ForeColor = Theming.ThirdTextForeColor;
                description.Text = Translations.Database("LauncherUpdateCheck_VS_Text_Version") + " " + Application.ProductVersion;
            }
        }
    }
}
