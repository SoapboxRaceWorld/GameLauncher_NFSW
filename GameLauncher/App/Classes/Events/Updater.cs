using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeProject.Downloader;
using GameLauncher.HashPassword;
using GameLauncherReborn;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes.Events {
    class LauncherUpdateCheck
    {
        PictureBox status;
        Label text;
        Label description;
        Label progress;
        ProgressBarEx progressBar;

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription, Label progressText, ProgressBarEx extractingProgress)  {
            status = statusImage;
            text = statusText;
            description = statusDescription;
            progress = progressText;
            progressBar = extractingProgress;
        }

        public void checkAvailability() {
            text.Text = "Launcher Status - Checking...";
            description.Text = "Version : v" + Application.ProductVersion;
            status.Image = Properties.Resources.ac_unknown;
            text.ForeColor = Color.FromArgb(0x848484);

            try { 
                WebClientWithTimeout update_data = new WebClientWithTimeout();
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

                                    Application.DoEvents();
                                    Thread.Sleep(3000);

                                    DialogResult updateConfirm = new UpdatePopup(updater).ShowDialog();

                                    if(updateConfirm == DialogResult.OK) {
                                        progress.Text = "DOWNLOADING GAMELAUNCHERUPDATER.EXE";
                                        new WebClientWithTimeout().DownloadFile(new Uri(Self.fileserver + "/GameLauncherUpdater.exe"), "GameLauncherUpdater.exe");
                                        Process.Start(@"GameLauncherUpdater.exe", Process.GetCurrentProcess().Id.ToString());
                                        //Let's fetch new downloader
                                        /*Properties.Settings.Default.IsRestarting = false;
                                        Properties.Settings.Default.Save();

                                        WebClientWithTimeout updateDownload = new WebClientWithTimeout();
                                        updateDownload.DownloadFileAsync(new Uri(updater.Payload.Update.DownloadUrl), "update.sbrw");
                                        updateDownload.DownloadProgressChanged += (x, y) => {
                                            progress.Text = "DOWNLOADING UPDATE: " + y.ProgressPercentage + "%";
                                        };

                                        updateDownload.DownloadFileCompleted += (x, y) => {
                                            progress.Text = "READY!";

                                            if (File.Exists("Update.exe")) {
                                                Process.Start(@"Update.exe");
                                            } else {
                                                Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                                            }

                                            Process.GetProcessById(Process.GetCurrentProcess().Id).Kill();
                                        };*/
                                    };
                                }
                            } else {
                                text.Text = "Launcher Status - GitHub Error";
                                status.Image = Properties.Resources.ac_error;
                                text.ForeColor = Color.FromArgb(254, 0, 0);
                                description.Text = "Version : v" + Application.ProductVersion;
                            }
                        } catch(Exception ex) {
                            MessageBox.Show(ex.Message);

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
