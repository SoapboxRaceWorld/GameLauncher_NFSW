using GameLauncher.App.Classes.LauncherCore.Visuals;
using System.Windows.Forms;

namespace GameLauncher.App.Classes
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();

            BackColor = Theming.SplashScreenTransparencyKey;
            TransparencyKey = Theming.SplashScreenTransparencyKey;
            BackgroundImage = Theming.Logo;
        }
    }
}
