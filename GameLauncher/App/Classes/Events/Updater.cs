using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

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

                        if(json.update.info == true) {
                            DialogResult reply = MessageBox.Show("An update is available. Do you wanna download it?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + json.github_build, "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if(reply == DialogResult.Yes) {
                                Process.Start(@"GL_Update.exe", Process.GetCurrentProcess().Id.ToString());
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
