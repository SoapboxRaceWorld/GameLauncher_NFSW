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

        public LauncherUpdateCheck(PictureBox statusImage, Label statusText, Label statusDescription)  {
            status = statusImage;
            text = statusText;
            description = statusDescription;
        }

        public void checkAvailability() {
            text.Text = "Launcher Status - Checking...";
            description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
            status.Image = Properties.Resources.ac_unknown;
            text.ForeColor = Color.FromArgb(0x848484);

            try { 
                WebClientWithTimeout update_data = new WebClientWithTimeout();
                update_data.CancelAsync();
                update_data.DownloadStringAsync(new Uri(Self.mainserver + "/update.php?version=" + Application.ProductVersion));
                update_data.DownloadStringCompleted += (sender, e) => {
                    if(description.InvokeRequired == true)
                    {
                        description.Invoke(new Action(delegate ()
                        {
                            description.Visible = true;
                        }));
                    } else
                    {
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
                        description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
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

                                    description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
                                } else {
                                    text.Text = "Launcher Status - Available";
                                    status.Image = Properties.Resources.ac_warning;
                                    text.ForeColor = Color.Yellow;
                                    description.Text = "New Version : " + updater.Payload.LatestVersion;

                                    var settingFile = new IniFile("Settings.ini");
                                    if (settingFile.Read("IgnoreUpdateVersion") != updater.Payload.LatestVersion) {
                                        var dia = new TaskDialog {
                                            Caption = "Update",
                                            InstructionText = "An update is available!",
                                            DetailsExpanded = true,
                                            Icon = TaskDialogStandardIcon.Information,
                                            DetailsCollapsedLabel = "Show Changelog",
                                            Text = "An update is available. Do you want to download it?\nYour version: " +
                                                   Application.ProductVersion + "\nUpdated version: " + updater.Payload.LatestVersion,
                                            DetailsExpandedText =
                                                new WebClientWithTimeout().DownloadString(Self.mainserver + "/launcher/changelog"),
                                            ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter
                                        };

                                        var update = new TaskDialogCommandLink("update", "Yes", "Launcher will be updated to " + updater.Payload.LatestVersion + ".");
                                        var cancel = new TaskDialogCommandLink("cancel", "No", "Launcher will ask you to update on the next launch.");
                                        var skipupdate = new TaskDialogCommandLink("skipupdate", "Ignore", "This update will be skipped. A new prompt will apear as soon as a newer update is available.");

                                        update.UseElevationIcon = true;

                                        skipupdate.Click += (sender3, e3) => {
                                            settingFile.Write("IgnoreUpdateVersion", updater.Payload.LatestVersion);
                                            dia.Close();
                                    
                                        };

                                        cancel.Click += (sender3, e3) => {
                                            dia.Close();
                                        };

                                        update.Click += (sender3, e3) => {
                                            if (File.Exists("GameLauncherUpdater.exe")) {
                                                Process.Start(@"GameLauncherUpdater.exe", Process.GetCurrentProcess().Id.ToString());
                                            } else {
                                                Process.Start(@"https://github.com/worldunitedgg/GameLauncher_NFSW/releases/latest");
                                            }

                                            dia.Close();
                                        };

                                        dia.Controls.Add(update);
                                        dia.Controls.Add(cancel);
                                        dia.Controls.Add(skipupdate);

                                        dia.Show();
                                    }
                                }
                            } else {
                                text.Text = "Launcher Status - GitHub Error";
                                status.Image = Properties.Resources.ac_error;
                                text.ForeColor = Color.FromArgb(254, 0, 0);
                                description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
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
                                    description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
                                }));
                            }
                            else
                            {
                                text.Text = "Launcher Status - Backend Error";
                                status.Image = Properties.Resources.ac_error;
                                text.ForeColor = Color.FromArgb(254, 0, 0);
                                description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
                            }
                        }
                    }
                };
            } catch {
                text.Text = "Launcher Status - Internal Error";
                status.Image = Properties.Resources.ac_error;
                text.ForeColor = Color.FromArgb(254, 0, 0);
                description.Text = "Version : v" + Application.ProductVersion + "build-" + SHA.HashFile(AppDomain.CurrentDomain.FriendlyName).Substring(0, 7);
            }
        }
    }
}
