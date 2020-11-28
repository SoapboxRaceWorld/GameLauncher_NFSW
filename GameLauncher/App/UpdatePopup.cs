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
        public UpdatePopup(UpdateCheckResponse updater)
        {
            IniFile _settingFile = new IniFile("Settings.ini");

            if (_settingFile.Read("IgnoreUpdateVersion") == updater.Payload.LatestVersion)
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

                UpdateText.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + updater.Payload.LatestVersion;

                this.UpdateButton.DialogResult = DialogResult.OK;
                this.IgnoreButton.DialogResult = DialogResult.Cancel;
                this.SkipButton.DialogResult = DialogResult.Ignore;
            }
        }

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansCondensed = FontWrapper.Instance.GetFontFamily("DejaVuSansCondensed.ttf");
            ChangelogText.Font = new Font(DejaVuSans, 9f, FontStyle.Regular);
            UpdateButton.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            UpdateText.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            SkipButton.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
            IgnoreButton.Font = new Font(DejaVuSansCondensed, 9f, FontStyle.Bold);
        }
    }
}
