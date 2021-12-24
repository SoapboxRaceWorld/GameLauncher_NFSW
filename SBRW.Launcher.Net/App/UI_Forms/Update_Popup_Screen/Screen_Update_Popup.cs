using Newtonsoft.Json;
using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.LauncherUpdater;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Update_Popup_Screen
{
    public partial class Screen_Update_Popup : Form
    {
        public Screen_Update_Popup()
        {
            Presence_Launcher.Status("Start Up", "New Version Is Available: " + LauncherUpdateCheck.LatestLauncherBuild);
            InitializeComponent();
            //SetVisuals();

            if (VisualsAPIChecker.GitHubAPI)
            {
                try
                {
                    TextBox_Changelog.Text = (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed()) ?
                    JsonConvert.DeserializeObject<List<GitHubRelease>>(LauncherUpdateCheck.VersionJSON)[0].Body.Replace("\r", Environment.NewLine) :
                    JsonConvert.DeserializeObject<GitHubRelease>(LauncherUpdateCheck.VersionJSON).Body.Replace("\r", Environment.NewLine);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Update Popup", string.Empty, Error, string.Empty, true);
                    TextBox_Changelog.Text = "\n" + Error.Message;
                    GroupBox_Changelog.Text = "Changelog Error:";
                }
            }
            else
            {
                TextBox_Changelog.Text = "\nUnable to Retrieve Changelog";
                GroupBox_Changelog.Text = "Changelog Error:";
            }

            TextBox_Changelog.Select(0, 0);
            TextBox_Changelog.SelectionLength = 0;
            TextBox_Changelog.TabStop = false;

            Bitmap Icon_Handle = Bitmap.FromHicon(SystemIcons.Information.Handle);
            Icon_Information.Image = Icon_Handle;

            Label_Text_Update.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion +
                "\nUpdated version: " + LauncherUpdateCheck.LatestLauncherBuild;

            this.Button_Update.DialogResult = DialogResult.OK;
            this.Button_Ignore.DialogResult = DialogResult.Cancel;
            this.Button_Skip.DialogResult = DialogResult.Ignore;
        }
    }
}
