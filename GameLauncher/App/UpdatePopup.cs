using System.Net;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.SystemPlatform.Linux;

namespace GameLauncher.App
{
    public partial class UpdatePopup : Form
    {
        public UpdatePopup(string LatestLauncherBuild)
        {
            if (FileSettingsSave.IgnoreVersion == LatestLauncherBuild)
            {
                /* No Update Popup */
            }
            else
            {
                InitializeComponent();
                SetVisuals();

                FunctionStatus.TLS();
                ChangelogText.Text = new WebClient().DownloadString(URLs.Main + "/launcher/changelog");
                ChangelogText.Select(0, 0);
                ChangelogText.SelectionLength = 0;
                ChangelogText.TabStop = false;

                Bitmap bitmap1 = Bitmap.FromHicon(SystemIcons.Information.Handle);
                UpdateIcon.Image = bitmap1;

                UpdateText.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + LatestLauncherBuild;

                this.UpdateButton.DialogResult = DialogResult.OK;
                this.IgnoreButton.DialogResult = DialogResult.Cancel;
                this.SkipButton.DialogResult = DialogResult.Ignore;
            }
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
            }

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
