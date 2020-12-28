using System.Net;
using GameLauncher.App.Classes.Events;
using GameLauncherReborn;
using GameLauncher.Resources;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes;

namespace GameLauncher.App
{
    public partial class UpdatePopup : Form
    {
        public UpdatePopup(string LatestLauncherBuild)
        {
            IniFile _settingFile = new IniFile("Settings.ini");

            if (_settingFile.Read("IgnoreUpdateVersion") == LatestLauncherBuild)
            {
                //No Update Popup
            }
            else
            {
                InitializeComponent();
                ApplyEmbeddedFonts();

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

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");
            ChangelogBox.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            ChangelogText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            UpdateButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            UpdateText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            SkipButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
            IgnoreButton.Font = new Font(DejaVuSansBold, 9f, FontStyle.Bold);
        }
    }
}
