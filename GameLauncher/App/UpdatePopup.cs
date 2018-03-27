using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class UpdatePopup : Form {
        public UpdatePopup() {
            InitializeComponent();
        }

        private void UpdatePopup_Load(object sender, EventArgs e) {
            changeLogURL.Url = new Uri("https://launcher.soapboxrace.world/changelog/?current_version=" + Application.ProductVersion);
        }
    }
}
