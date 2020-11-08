using System;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameLauncher.Properties;
using GameLauncherReborn;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes.Events
{
    class LauncherUpdateCheck
    {
        PictureBox status;
        Label text;
        Label description;

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription)  {
            status = statusImage;
            text = statusText;
            description = statusDescription;
        }

        public void CheckAvailability() {
            text.Text = "Launcher Status - Checking...";
            description.Text = "Version : v" + Application.ProductVersion;
            status.Image = Properties.Resources.ac_unknown;
            text.ForeColor = Color.FromArgb(0x848484);

            try { 
                WebClient update_data = new WebClient();
                update_data.CancelAsync();
                update_data.DownloadStringAsync(new Uri(Self.mainserver + "/update.php?version=" + Application.ProductVersion));
                update_data.DownloadStringCompleted += (sender, e) => {
                    if(description.InvokeRequired == true) {
                        description.Invoke(new Action(delegate () {
                            description.Visible = true;
                        }));
                    } else {
                        description.Visible = true;
                    }

                    if (e.Cancelled) {
                        text.Text = "Launcher Status - Error";
                        status.Image = Properties.Resources.ac_error;
                        text.ForeColor = Color.FromArgb(254, 0, 0);
                        description.Text = "Event cancelled.";
                    } else if (e.Error != null) {
                        text.Text = "Launcher Status";
                        status.Image = Properties.Resources.ac_success;
                        text.ForeColor = Color.FromArgb(0x9fc120);
                        description.Text = "Version : v" + Application.ProductVersion;
                    } else {
                        UpdateCheckResponse updater = JsonConvert.DeserializeObject<UpdateCheckResponse>(e.Result);

                        try { 
                            if(updater.Code == 0) { 
                                if (updater.Payload.UpdateExists == false) {
                                    if(updater.Payload.LatestVersion.CompareTo(updater.Payload.ClientVersion) >= 0) {
                                        text.Text = "Launcher Status - Updated";
                                        status.Image = Properties.Resources.ac_success;
                                        text.ForeColor = Color.FromArgb(0x9fc120);
                                    } else {
                                        text.Text = "Launcher Status - Prerelease";
                                        status.Image = Properties.Resources.ac_warning;
                                        text.ForeColor = Color.Yellow;
                                    }

                                    description.Text = "Version : v" + Application.ProductVersion;
                                } else {
                                    text.Text = "Launcher Status - Available";
                                    status.Image = Properties.Resources.ac_warning;
                                    text.ForeColor = Color.Yellow;
                                    description.Text = "New Version : " + updater.Payload.LatestVersion;

                                    IniFile _settingFile = new IniFile("Settings.ini");

                                    if (_settingFile.Read("IgnoreUpdateVersion") == updater.Payload.LatestVersion)
                                    {
                                        //No Update Popup
                                        //Blame DavidCarbon if this Breaks (to some degree), not Zacam...
                                    }
                                    else
                                    {
                                        DialogResult updateConfirm = new UpdatePopup(updater).ShowDialog();

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
                                            Settings.Default.IgnoreUpdateVersion = String.Empty;
                                        };

                                        //Write to Settings.ini to Skip Update
                                        if (updateConfirm == DialogResult.Ignore)
                                        {
                                            Settings.Default.IgnoreUpdateVersion = updater.Payload.LatestVersion;
                                        };
                                    }
                                    Settings.Default.Save();
                                }
                            } else {
                                text.Text = "Launcher Status - GitHub Error";
                                status.Image = Properties.Resources.ac_error;
                                text.ForeColor = Color.FromArgb(254, 0, 0);
                                description.Text = "Version : v" + Application.ProductVersion;
                            }
                        } catch(Exception) {
                            if(text.InvokeRequired == true) //checks skip, because we only need to know if we can access ui from actual thread
                            {
                                text.Invoke(new Action(delegate ()
                                {
                                    text.Text = "Launcher Status - Backend Error";
                                    text.ForeColor = Color.FromArgb(254, 0, 0);
                                }));
                                status.Invoke(new Action(delegate ()
                                {
                                    status.Image = Properties.Resources.ac_error;
                                }));
                                description.Invoke(new Action(delegate ()
                                {
                                    description.Text = "Version : v" + Application.ProductVersion;
                                }));
                            }
                            else
                            {
                                text.Text = "Launcher Status - Backend Error";
                                status.Image = Properties.Resources.ac_error;
                                text.ForeColor = Color.FromArgb(254, 0, 0);
                                description.Text = "Version : v" + Application.ProductVersion;
                            }
                        }
                    }
                };
            } catch {
                text.Text = "Launcher Status - Internal Error";
                status.Image = Properties.Resources.ac_error;
                text.ForeColor = Color.FromArgb(254, 0, 0);
                description.Text = "Version : v" + Application.ProductVersion;
            }
        }
    }
}
