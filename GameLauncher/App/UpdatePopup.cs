using System.Net;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using System;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using System.Text;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.RPC;

namespace GameLauncher.App
{
    public partial class UpdatePopup : Form
    {
        public UpdatePopup()
        {
            DiscordLauncherPresence.Status("Start Up", "New Version Is Available: " + LauncherUpdateCheck.LatestLauncherBuild);
            InitializeComponent();
            SetVisuals();

            if (VisualsAPIChecker.UnitedAPI())
            {
                try
                {
                    FunctionStatus.TLS();
                    Uri URLCall = new Uri(URLs.Main + "/launcher/changelog");
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                    else
                    {
                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    try
                    {
                        /* Download Up to Date Certificate Status */
                        ChangelogText.Text = Client.DownloadString(URLCall);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Update Popup", null, Error, null, true);
                        ChangelogText.Text = "\n" + Error.Message;
                        ChangelogBox.Text = "Changelog Error:";
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Update Popup", null, Error, null, true);
                    ChangelogText.Text = "\n" + Error.Message;
                    ChangelogBox.Text = "Changelog Error:";
                }
            }
            else
            {
                ChangelogText.Text = "\nUnited API Is Currently Down. Unable to Retrieve Changelog";
                ChangelogBox.Text = "Changelog Error:";
            }

            ChangelogText.Select(0, 0);
            ChangelogText.SelectionLength = 0;
            ChangelogText.TabStop = false;

            Bitmap bitmap1 = Bitmap.FromHicon(SystemIcons.Information.Handle);
            UpdateIcon.Image = bitmap1;

            UpdateText.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion +
                "\nUpdated version: " + LauncherUpdateCheck.LatestLauncherBuild;

            this.UpdateButton.DialogResult = DialogResult.OK;
            this.IgnoreButton.DialogResult = DialogResult.Cancel;
            this.SkipButton.DialogResult = DialogResult.Ignore;
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected()? 9f : 9f * 100f / CreateGraphics().DpiY;

            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ChangelogBox.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            ChangelogText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            UpdateButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            UpdateText.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            SkipButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            IgnoreButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            ForeColor = Theming.WinFormTextForeColor;
            BackColor = Theming.WinFormTBGForeColor;

            ChangelogText.ForeColor = Theming.WinFormSecondaryTextForeColor;
            ChangelogBox.ForeColor = Theming.WinFormTextForeColor;

            UpdateText.ForeColor = Theming.WinFormTextForeColor;

            UpdateButton.ForeColor = Theming.BlueForeColorButton;
            UpdateButton.BackColor = Theming.BlueBackColorButton;
            UpdateButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            UpdateButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            IgnoreButton.ForeColor = Theming.BlueForeColorButton;
            IgnoreButton.BackColor = Theming.BlueBackColorButton;
            IgnoreButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            IgnoreButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            SkipButton.ForeColor = Theming.BlueForeColorButton;
            SkipButton.BackColor = Theming.BlueBackColorButton;
            SkipButton.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            SkipButton.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;
        }
    }
}
