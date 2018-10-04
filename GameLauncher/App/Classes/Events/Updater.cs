using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using GameLauncherReborn;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes.Events
{
    // FIXME: use translatable language strings here
    internal class Updater
    {
        internal static void CheckForUpdate(object sender, EventArgs e)
        {
            try
            {
                var client = new WebClientWithTimeout();

                var uri = new Uri(Self.mainserver + "/launcher/update?version=" + Application.ProductVersion);
                client.CancelAsync();
                client.DownloadStringAsync(uri);
                client.DownloadStringCompleted += (sender2, e2) =>
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject<UpdateCheckResponse>(e2.Result);

                        if (json.Code != 0)
                        {
                            MessageBox.Show("Launchpad update service returned an error: " + json.Code);
                            return;
                        }

                        if (json.Payload.UpdateExists)
                        {
                            var settingFile = new IniFile("Settings.ini");
                            if (settingFile.Read("IgnoreUpdateVersion") != json.Payload.LatestVersion)
                            {
                                var dia = new TaskDialog
                                {
                                    Caption = "Update",
                                    InstructionText = "An update is available!",
                                    DetailsExpanded = true,
                                    Icon = TaskDialogStandardIcon.Information,
                                    DetailsCollapsedLabel = "Show Changelog",
                                    Text = "An update is available. Do you want to download it?\nYour version: " +
                                           Application.ProductVersion + "\nUpdated version: " + json.Payload.LatestVersion,
                                    DetailsExpandedText =
                                        new WebClientWithTimeout().DownloadString(Self.mainserver + "/launcher/changelog"),
                                    ExpansionMode = TaskDialogExpandedDetailsLocation.ExpandFooter
                                };

                                var update = new TaskDialogCommandLink("update", "Yes", "Launcher will be updated to " + json.Payload.LatestVersion + ".");
                                var cancel = new TaskDialogCommandLink("cancel", "No", "Launcher will ask you to update on the next launch.");
                                var skipupdate = new TaskDialogCommandLink("skipupdate", "Ignore", "This update will be skipped. A new prompt will apear as soon as a newer update is available.");

                                update.UseElevationIcon = true;

                                skipupdate.Click += (sender3, e3) =>
                                {
                                    settingFile.Write("IgnoreUpdateVersion", json.Payload.LatestVersion);
                                    dia.Close();
                                };

                                cancel.Click += (sender3, e3) =>
                                {
                                    dia.Close();
                                };

                                update.Click += (sender3, e3) =>
                                {
                                    if (File.Exists("GameLauncherUpdater.exe"))
                                    {
                                        Process.Start(@"GameLauncherUpdater.exe", Process.GetCurrentProcess().Id.ToString());
                                    }
                                    else
                                    {
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
                        }
                        else
                        {
                            try
                            {
                                if (((Form)sender).Name == "mainScreen") { }
                            }
                            catch
                            {
                                MessageBox.Show("Your launcher is up-to-date");
                            }
                        }
                    }
                    catch
                    {
                        try
                        {
                            if (((Form)sender).Name == "mainScreen") { }
                        }
                        catch
                        {
                            MessageBox.Show("Failed to check for update!");
                        }
                    }
                };
            }
            catch
            {
                MessageBox.Show("Failed to check for update!");
            }
        }
    }
}
