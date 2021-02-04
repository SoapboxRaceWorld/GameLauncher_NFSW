using System.Net;
using GameLauncherReborn;
using GameLauncher.Resources;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;

namespace GameLauncher.App
{
    public partial class UpdatePopup : Form
    {
        public UpdatePopup(string LatestLauncherBuild)
        {
            if (FileSettingsSave.IgnoreVersion == LatestLauncherBuild)
            {
                //No Update Popup
            }
            else
            {
                InitializeComponent();
                SetVisuals();

                ChangelogText.Text = new WebClient().DownloadString(Self.mainserver + "/launcher/changelog");
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
            ChangelogBox.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ChangelogText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            UpdateButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            UpdateText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SkipButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            IgnoreButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);

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
