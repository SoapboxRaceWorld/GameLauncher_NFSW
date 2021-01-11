using System;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameLauncher.Properties;
using GameLauncherReborn;
using Newtonsoft.Json;
using GameLauncher.App.Classes.Logger;
using System.Threading.Tasks;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;

namespace GameLauncher.App.Classes.Events
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

        public async void CheckAvailability()
        {
            text.Text = "Launcher - Checking...";
            description.Text = "Version: v" + Application.ProductVersion;
            status.Image = Properties.Resources.ac_unknown;
            text.ForeColor = Color.FromArgb(0x848484);

            switch (APIStatusChecker.CheckStatus(Self.mainserver + "/update.php?version=" + Application.ProductVersion))
            {
                case API.Online:
                    MainAPI();
                    break;
                default:
                    GitHubAPI();
                    break;
            }

            await Task.Delay(2000);
            //Set Visual Status
            ChangeVisualStatus();
        }

        private void ChangeVisualStatus()
        {
            if (!string.IsNullOrEmpty(LatestLauncherBuild))
            {
                var Revisions = CurrentLauncherBuild.CompareTo(LatestLauncherBuild);

                if (Revisions > 0)
                {
                    text.Text = "Launcher Status:\n - Insider Build";
                    status.Image = Properties.Resources.ac_warning;
                    text.ForeColor = Color.Yellow;
                    description.Text = "Version: v" + Application.ProductVersion;
                }
                else if (Revisions == 0)
                {
                    text.Text = "Launcher Status:\n - Current Version";
                    status.Image = Properties.Resources.ac_success;
                    text.ForeColor = Color.FromArgb(0x9fc120);
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
                    status.Image = Properties.Resources.ac_warning;
                    text.ForeColor = Color.Yellow;
                    description.Text = "New Version: " + LatestLauncherBuild.ToString();

                    if (FileSettingsSave.IgnoreVersion == LatestLauncherBuild.ToString())
                    {
                        //No Update Popup
                        //Blame DavidCarbon if this Breaks (to some degree), not Zacam...
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

                        //Check if User clicked Ignore so it doesn't update "IgnoreUpdateVersion"
                        if (updateConfirm == DialogResult.Cancel)
                        {
                            FileSettingsSave.IgnoreVersion = String.Empty;
                        };

                        //Write to Settings.ini to Skip Update
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
                status.Image = Properties.Resources.ac_error;
                text.ForeColor = Color.FromArgb(254, 0, 0);
                description.Text = "Version: v" + Application.ProductVersion;
            }
            //----------------------//
        }

        private void MainAPI()
        {
            WebClient update_data = new WebClient();
            update_data.CancelAsync();
            update_data.Headers.Add("user-agent", "GameLauncherUpdater " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
            update_data.DownloadStringAsync(new Uri(Self.mainserver + "/update.php?version=" + Application.ProductVersion));
            update_data.DownloadStringCompleted += (sender, e) => {
                UpdateCheckResponse MAPI = JsonConvert.DeserializeObject<UpdateCheckResponse>(e.Result);

                if (MAPI.Payload.LatestVersion != null)
                {
                    LatestLauncherBuild = MAPI.Payload.LatestVersion;
                    Log.Info("UPDATER: Latest Version -> " + MAPI.Payload.LatestVersion);
                }
            };
        }

        private void GitHubAPI()
        {
            Log.Warning("UPDATER: Falling back to GitHub API");
            switch (APIStatusChecker.CheckStatus("http://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest"))
            {
                case API.Online:
                    WebClient update_data = new WebClient();
                    update_data.CancelAsync();
                    update_data.Headers.Add("user-agent", "GameLauncherUpdater " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    update_data.DownloadStringAsync(new Uri("http://api.github.com/repos/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest"));
                    update_data.DownloadStringCompleted += (sender, e) => {
                        GitHubRelease GHAPI = JsonConvert.DeserializeObject<GitHubRelease>(e.Result);

                        if (GHAPI.TagName != null)
                        {
                            LatestLauncherBuild = GHAPI.TagName;
                            Log.Info("UPDATER: Latest Version -> " + GHAPI.TagName);
                        }
                    };
                    break;
                default:
                    Log.Error("UPDATER: Failed to Retrive Latest Build Information from two APIs ");
                    break;
            }
        }
    }
}
