using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using System.Threading.Tasks;
using GameLauncher.App.Classes.LauncherCore.Client.Web;

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

        public static async Task Latest()
        {
            if (!DetectLinux.LinuxDetected())
            {
                await Task.Run(() => Check());
            }
        }

        public static void Check()
        {
            bool MainAPI = true;

            using (WebClientWithTimeout Client = new WebClientWithTimeout())
            {
                try
                {
                    var json_data = Client.DownloadString(URLs.mainserver + "/update.php?version=" + Application.ProductVersion);
                    UpdateCheckResponse MAPI = JsonConvert.DeserializeObject<UpdateCheckResponse>(json_data);

                    if (MAPI.Payload.LatestVersion != null)
                    {
                        LatestLauncherBuild = MAPI.Payload.LatestVersion;
                        Log.Info("UPDATER: Latest Version -> " + MAPI.Payload.LatestVersion);
                    }
                }
                catch (Exception error)
                {
                    MainAPI = false;
                    Log.Error("LAUNCHER UPDATER: " + error.Message);
                }
            }

            if (MainAPI != true)
            {
                bool GitHubAPI = true;

                using (WebClientWithTimeout Client = new WebClientWithTimeout())
                {
                    try
                    {
                        var json_data = Client.DownloadString("http://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                        GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(json_data);

                        if (GHAPI.TagName != null)
                        {
                            LatestLauncherBuild = GHAPI.TagName;
                            Log.Info("UPDATER: Latest Version -> " + GHAPI.TagName);
                        }
                    }
                    catch (Exception error)
                    {
                        GitHubAPI = false;
                        Log.Error("LAUNCHER UPDATER: " + error.Message);
                    }
                }

                if (GitHubAPI != true)
                {
                    Log.Error("UPDATER: Failed to Retrive Latest Build Information from two APIs ");
                }
            }
        }

        public void ChangeVisualStatus()
        {
            if (!string.IsNullOrEmpty(LatestLauncherBuild))
            {
                var Revisions = CurrentLauncherBuild.CompareTo(LatestLauncherBuild);

                if (Revisions > 0)
                {
                    text.Text = "Launcher Status:\n - Insider Build";
                    status.BackgroundImage = Theming.UpdateIconWarning;
                    text.ForeColor = Theming.Alert;
                    description.Text = "Version: v" + Application.ProductVersion;

                    if (!string.IsNullOrEmpty(FileSettingsSave.IgnoreVersion))
                    {
                        FileSettingsSave.IgnoreVersion = String.Empty;
                        FileSettingsSave.SaveSettings();
                        Log.Info("IGNOREUPDATEVERSION: Cleared OLD IgnoreUpdateVersion Build Number. You're now on the Insider Build Branch!");
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
