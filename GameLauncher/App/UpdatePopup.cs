using GameLauncher.App.Classes.Events;
using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class UpdatePopup : Form {
        public UpdatePopup(UpdateCheckResponse updater) {
            InitializeComponent();

            changelogText.Text = new WebClientWithTimeout().DownloadString(Self.mainserver + "/launcher/changelog");
            changelogText.Select(0, 0);
            changelogText.SelectionLength = 0;
            changelogText.TabStop = false;

            Bitmap bitmap1 = Bitmap.FromHicon(SystemIcons.Information.Handle);
            icon.Image = bitmap1;

            updateLabel.Text = "An update is available. Would you like to update?\nYour version: " + Application.ProductVersion + "\nUpdated version: " + updater.Payload.LatestVersion;

            this.update.DialogResult = DialogResult.OK;
        }
    }
}
