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
        private readonly float _dpiDefaultScale = 96f;

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

                changelogText.Text = new WebClient().DownloadString(Self.mainserver + "/launcher/changelog");
                changelogText.Select(0, 0);
                changelogText.SelectionLength = 0;
                changelogText.TabStop = false;

                Bitmap bitmap1 = Bitmap.FromHicon(SystemIcons.Information.Handle);
                icon.Image = bitmap1;

                updateLabel.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + updater.Payload.LatestVersion;

                this.update.DialogResult = DialogResult.OK;
                this.ignore.DialogResult = DialogResult.Cancel;
                this.skip.DialogResult = DialogResult.Ignore;
            }
        }

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansCondensed = FontWrapper.Instance.GetFontFamily("DejaVuSansCondensed.ttf");
            Font = new Font(DejaVuSans, 8.25f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            changelogText.Font = new Font(DejaVuSans, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Regular);
            update.Font = new Font(DejaVuSansCondensed, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            updateLabel.Font = new Font(DejaVuSansCondensed, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            skip.Font = new Font(DejaVuSansCondensed, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
            ignore.Font = new Font(DejaVuSansCondensed, 9f * _dpiDefaultScale / CreateGraphics().DpiX, FontStyle.Bold);
        }
    }
}
