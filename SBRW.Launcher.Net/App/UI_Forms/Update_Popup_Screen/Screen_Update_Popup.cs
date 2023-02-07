using Newtonsoft.Json;
using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
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
        /// <summary>
        /// 
        /// </summary>
        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
#endif

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

        public Screen_Update_Popup(bool Update_Mode = true)
        {
            if (Update_Mode)
            {
                Presence_Launcher.Status(0, "New Version Is Available: " + LauncherUpdateCheck.LatestLauncherBuild);
            }
            else
            {
                this.Text = "Storage Alert Check";
            }

            InitializeComponent();
            Icon = FormsIcon.Retrive_Icon();
            SetVisuals();

            if (Update_Mode)
            {
                if (VisualsAPIChecker.GitHubAPI)
                {
                    try
                    {
                        if (LauncherUpdateCheck.VersionJSON.Valid_Json())
                        {
#pragma warning disable CS8602 // Null Safe Check Done Above
                            if (EnableInsiderBetaTester.Allowed())
                            {
                                TextBox_Changelog.Text = JsonConvert.DeserializeObject<List<GitHubRelease>>(LauncherUpdateCheck.VersionJSON)[LauncherUpdateCheck.Version_JSON_Index].Body.Replace("\r", Environment.NewLine);
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
            }
            else
            {
                GroupBox_Changelog.Text = "Details:";
                TextBox_Changelog.Text = string.Empty;
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText("Click Ignore to Enable Storage Detection Bypass (Unix Builds Only) and Restarts the Downloader");
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText("Click Retry to temporary bypass the Storage Detection.");
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText(Environment.NewLine);
                TextBox_Changelog.AppendText("Click Ok, to Close this Message");
            }

            TextBox_Changelog.Select(0, 0);
            TextBox_Changelog.SelectionLength = 0;
            TextBox_Changelog.TabStop = false;

            Bitmap Icon_Handle = Bitmap.FromHicon(SystemIcons.Information.Handle);
            Icon_Information.Image = Icon_Handle;

            if (Update_Mode)
            {
                Label_Text_Update.Text = "An update is available. Would you like to update?\nYour version: " + LauncherUpdateCheck.CurrentLauncherBuild +
                "\nUpdated version: " + LauncherUpdateCheck.LatestLauncherBuild;

                this.Button_Update.DialogResult = DialogResult.OK;
                this.Button_Ignore.DialogResult = DialogResult.Cancel;
                this.Button_Skip.DialogResult = DialogResult.Ignore;
            }
            else
            {
                Label_Text_Update.Text = "Did the launcher correctly detect limited free space?";
                this.Button_Update.Text = "OK";
                this.Button_Update.DialogResult = DialogResult.OK;
                this.Button_Ignore.DialogResult = DialogResult.Ignore;
                this.Button_Skip.Text = "Retry";
                this.Button_Skip.DialogResult = DialogResult.Retry;
            }
        }
    }
}
