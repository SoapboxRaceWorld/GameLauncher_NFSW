using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System.Net;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.InsiderKit;

namespace GameLauncher.App.Classes.LauncherCore.LauncherUpdater
{
    class LauncherUpdateCheck
    {
        public PictureBox status;
        public Label text;
        public Label description;

        public static string CurrentLauncherBuild = Application.ProductVersion;
        private static string LatestLauncherBuild;

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription)
        {
            status = statusImage;
            text = statusText;
            description = statusDescription;
        }

        public static void Latest()
        {
            if (!DetectLinux.LinuxDetected())
            {
                bool MainAPI = true;

                try
                {
                    FunctionStatus.TLS();
                    WebClient Client = new WebClient();
                    Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    var json_data = Client.DownloadString(URLs.Main + "/update.php?version=" + Application.ProductVersion);
                    UpdateCheckResponse MAPI = JsonConvert.DeserializeObject<UpdateCheckResponse>(json_data);

                    if (MAPI.Payload.LatestVersion != null)
                    {
                        LatestLauncherBuild = MAPI.Payload.LatestVersion;
                        Log.Info("LAUNCHER UPDATE: Latest Version -> " + MAPI.Payload.LatestVersion);
                    }
                }
                catch (Exception error)
                {
                    MainAPI = false;
                    Log.Error("LAUNCHER UPDATE: " + error.Message);
                }

                if (MainAPI != true)
                {
                    bool GitHubAPI = true;

                    try
                    {
                        FunctionStatus.TLS();
                        WebClient Client = new WebClient();
                        Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                        var json_data = Client.DownloadString(URLs.GitHub_Launcher);
                        GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(json_data);

                        if (GHAPI.TagName != null)
                        {
                            LatestLauncherBuild = GHAPI.TagName;
                            Log.Info("LAUNCHER UPDATE: GitHub Latest Version -> " + GHAPI.TagName);
                        }
                    }
                    catch (Exception error)
                    {
                        GitHubAPI = false;
                        Log.Error("LAUNCHER UPDATE: GitHub " + error.Message);
                    }

                    if (GitHubAPI != true)
                    {
                        Log.Error("LAUNCHER UPDATE: Failed to Retrive Latest Build Information from two APIs ");
                    }
                }
            }

            /* (End Process) Close Splash Screen */
            if (Program.IsSplashScreenLive == true)
            {
                Program.SplashScreen.Abort();
            }

            FunctionStatus.LoadingComplete = true;

            /* Do First Time Run Checks */
            FunctionStatus.FirstTimeRun();
        }

        public void ChangeVisualStatus()
        {
            if (!string.IsNullOrEmpty(LatestLauncherBuild))
            {
                var Revisions = CurrentLauncherBuild.CompareTo(LatestLauncherBuild);

                if (Revisions > 0)
                {
                    string WhatBuildAmI;
                    if (EnableInsiderDeveloper.Allowed())
                    {
                        WhatBuildAmI = "Developer";
                    }
                    else if (EnableInsiderBetaTester.Allowed())
                    {
                        WhatBuildAmI = "Beta";
                    }
                    else
                    {
                        WhatBuildAmI = "Unofficial";
                    }

                    text.Text = "Launcher Status:\n - " + WhatBuildAmI + " Build";
                    status.BackgroundImage = Theming.UpdateIconWarning;
                    text.ForeColor = Theming.Alert;
                    description.Text = "Version: v" + Application.ProductVersion;

                    if (!string.IsNullOrEmpty(FileSettingsSave.IgnoreVersion))
                    {
                        FileSettingsSave.IgnoreVersion = String.Empty;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You are currenly using a " + WhatBuildAmI + " Build!");
                    }
                }
                else if (Revisions == 0)
                {
                    text.Text = "Launcher Status:\n - Current Version";
                    status.BackgroundImage = Theming.UpdateIconSuccess;
                    text.ForeColor = Theming.Sucess;
                    description.Text = "Version: v" + Application.ProductVersion;

                    if (FileSettingsSave.IgnoreVersion == Application.ProductVersion)
                    {
                        FileSettingsSave.IgnoreVersion = String.Empty;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Latest Game Launcher!");
                    }
                }
                else
                {
                    text.Text = "Launcher Status:\n - Update Available";
                    status.BackgroundImage = Theming.UpdateIconWarning;
                    text.ForeColor = Theming.Alert;
                    description.Text = "New Version: " + LatestLauncherBuild.ToString();

                    if (FileSettingsSave.IgnoreVersion == LatestLauncherBuild.ToString())
                    {
                        /* No Update Popup
                           Blame DavidCarbon if this Breaks (to some degree), not Zacam...*/
                    }
                    else
                    {
                        DialogResult updateConfirm = new UpdatePopup(LatestLauncherBuild).ShowDialog();

                        if (updateConfirm == DialogResult.OK)
                        {
                            if (File.Exists("GameLauncherUpdater.exe"))
                            {
                                Process.Start(@"GameLauncherUpdater.exe", Process.GetCurrentProcess().Id.ToString());
                            }
                            else
                            {
                                Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                            }
                        };

                        /* Check if User clicked Ignore so it doesn't update "IgnoreUpdateVersion" */
                        if (updateConfirm == DialogResult.Cancel)
                        {
                            FileSettingsSave.IgnoreVersion = String.Empty;
                        };

                        /* Write to Settings.ini to Skip Update */
                        if (updateConfirm == DialogResult.Ignore)
                        {
                            FileSettingsSave.IgnoreVersion = LatestLauncherBuild.ToString();
                        };
                    }
                    FileSettingsSave.SaveSettings();
                }
            }
            else
            {
                text.Text = "Launcher Status:\n - Backend Error";
                status.BackgroundImage = Theming.UpdateIconError;
                text.ForeColor = Theming.Error;
                description.Text = "Version: v" + Application.ProductVersion;
            }
        }
    }
}
