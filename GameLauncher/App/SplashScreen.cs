using GameLauncher.App.Classes;
using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App {
    public partial class SplashScreen : Form {
        public SplashScreen() {
            InitializeComponent();
            Shown += new EventHandler((q, w) => { Console.WriteLine("Shown"); });
            Load += new EventHandler((q, w) => { Console.WriteLine("Load"); });

            Load += new EventHandler(SplashScreen_Load);
        }

        private void SplashScreen_Load(object sender, EventArgs e) {
            this.Show();

            for (int x = 0; x <= 600; x++) {
                Thread.Sleep(1);
                this.Refresh();
            }
        }

    }
}
