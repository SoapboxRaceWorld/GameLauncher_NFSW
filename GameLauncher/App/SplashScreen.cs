using GameLauncher.App.Classes;
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
            for(int x = 0; x <= 95; x++) {
                loadingBar.Value = x;
                Delay.WaitMSeconds(30);
                Application.DoEvents();
            }
        }
    }
}
