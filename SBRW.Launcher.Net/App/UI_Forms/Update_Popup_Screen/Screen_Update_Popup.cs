﻿using Newtonsoft.Json;
using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.LauncherUpdater;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Update_Popup_Screen
{
    public partial class Screen_Update_Popup : Form
    {
        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            TextBox_Changelog.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            GroupBox_Changelog.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Update.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Text_Update.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Skip.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Ignore.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            ForeColor = Color_Winform.Text_Fore_Color;
            BackColor = Color_Winform.BG_Fore_Color;

            GroupBox_Changelog.ForeColor = Color_Winform.Text_Fore_Color;
            TextBox_Changelog.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            TextBox_Changelog.BackColor = Color_Winform.BG_Darker_Fore_Color;

            Label_Text_Update.ForeColor = Color_Winform.Text_Fore_Color;

            Button_Update.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Update.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Update.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Update.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Ignore.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Ignore.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Ignore.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Ignore.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Skip.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Skip.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Skip.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Skip.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;
        }

        public Screen_Update_Popup()
        {
            Presence_Launcher.Status(0, "New Version Is Available: " + LauncherUpdateCheck.LatestLauncherBuild);
            InitializeComponent();
            Icon = FormsIcon.Retrive_Icon();
            SetVisuals();

            if (VisualsAPIChecker.GitHubAPI)
            {
                try
                {
                    if (Is_Json.Valid(LauncherUpdateCheck.VersionJSON))
                    {
#pragma warning disable CS8602 // Null Safe Check Done Above
                        if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                        {
                            TextBox_Changelog.Text = JsonConvert.DeserializeObject<List<GitHubRelease>>(LauncherUpdateCheck.VersionJSON)[0].Body.Replace("\r", Environment.NewLine);
                        }
                        else
                        {
                            TextBox_Changelog.Text = JsonConvert.DeserializeObject<GitHubRelease>(LauncherUpdateCheck.VersionJSON).Body.Replace("\r", Environment.NewLine);
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    else
                    {
                        TextBox_Changelog.Text = "\nUnable to Phrase Changelog";
                        GroupBox_Changelog.Text = "Changelog Error:";
                    }
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
