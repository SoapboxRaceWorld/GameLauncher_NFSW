using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using GameLauncher.App.Classes;
using GameLauncher.App;
using Microsoft.WindowsAPICodePack;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace GameLauncher {
    class Updater {
        internal static void checkForUpdate(object sender, EventArgs e) {
            try {
                var client = new WebClientWithTimeout();

                Uri StringToUri = new Uri("http://launcher.soapboxrace.world/checkUpdate.php?version=" + Application.ProductVersion);
                client.CancelAsync();
                client.DownloadStringAsync(StringToUri);
                client.DownloadStringCompleted += (sender2, e2) => {
                    try {
                        CheckVersion json = JsonConvert.DeserializeObject<CheckVersion>(e2.Result);

                        if (json.update.info == true) {
                            IniFile SettingFile = new IniFile("Settings.ini");
                            if (SettingFile.Read("IgnoreUpdateVersion") != json.github_build) {
                                TaskDialog dia = new TaskDialog();
                                dia.Caption = "Update";
                                dia.InstructionText = "An update is available!";
                                dia.DetailsExpanded = true;
                                dia.Icon = TaskDialogStandardIcon.Information;
                                dia.DetailsCollapsedLabel = "Show Changelog";
                                dia.Text = "An update is available. Do you wanna download it?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + json.github_build;
                                dia.DetailsExpandedText = new WebClientWithTimeout().DownloadString("https://launcher.soapboxrace.world/changelog/text.php");
                                dia.ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter;

                                TaskDialogCommandLink update = new TaskDialogCommandLink("update", "Yes", "Launcher will be updated to " + json.github_build + ".");
                                TaskDialogCommandLink cancel = new TaskDialogCommandLink("cancel", "No", "Launcher will ask for update on next launch.");
                                TaskDialogCommandLink skipupdate = new TaskDialogCommandLink("skipupdate", "Ignore", "This update will be skipped. Will ask again if new update will appear");

                                update.UseElevationIcon = true;

                                skipupdate.Click += (sender3, e3) => {
                                    SettingFile.Write("IgnoreUpdateVersion", json.github_build);
                                    dia.Close();
                                };

                                cancel.Click += (sender3, e3) => {
                                    dia.Close();
                                };

                                update.Click += (sender3, e3) => {
                                    if (File.Exists("GL_Update.exe")) {
                                        Process.Start(@"GL_Update.exe", Process.GetCurrentProcess().Id.ToString());
                                    } else {
                                        Process.Start(@"https://github.com/SoapboxRaceWorld/GameLauncher_NFSW/releases/latest");
                                    }

                                    dia.Close();
                                };

                                dia.Controls.Add(update);
                                dia.Controls.Add(cancel);
                                dia.Controls.Add(skipupdate);

                                dia.Show();

                                //new UpdatePopup(json.github_build).Show();
                            }
                        } else {
                            try {
                                if (((Form)sender).Name == "mainScreen") {}
                            } catch {
                                MessageBox.Show("Your launcher is up-to-date");
                            }
                        }
                    } catch {
                        try {
                            if (((Form)sender).Name == "mainScreen") { }
                        } catch {
                            MessageBox.Show("Failed to check for update");
                        }
                    }
                };
            } catch {
                MessageBox.Show("Failed to check for update");
            }
        }
    }
}
