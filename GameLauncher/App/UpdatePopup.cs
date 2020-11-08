using System.Net;
using GameLauncher.App.Classes.Events;
using GameLauncherReborn;
using System.Drawing;
using System.Windows.Forms;
using GameLauncher.App.Classes;

namespace GameLauncher.App
{
    public partial class UpdatePopup : Form {
        public UpdatePopup(UpdateCheckResponse updater) {
            IniFile _settingFile = new IniFile("Settings.ini");

            if (_settingFile.Read("IgnoreUpdateVersion") == updater.Payload.LatestVersion)
            {
                //No Update Popup
            }
            else
            {
                InitializeComponent();

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
    }
}
