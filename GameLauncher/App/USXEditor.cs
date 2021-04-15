using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.SystemPlatform.Windows;

namespace GameLauncher.App
{
    public partial class USXEditor : Form
    {
        public string _userSettings = Environment.GetEnvironmentVariable("AppData") + "/Need for Speed World/Settings/UserSettings.xml";

        public USXEditor()
        {
            if (File.Exists(_userSettings))
            {
                Log.Debug("UXE: Success, a UserSettings.xml file was found!");
                if (new FileInfo(_userSettings).IsReadOnly == true)
                {
                    Log.Warning("UXE: UserSettings.xml is ReadOnly!");
                }
                else
                {
                    Log.Debug("UXE: UserSettings.xml can be modified!");
                }

            }
            else
            {
                MessageBox.Show(null, "How is this even possible? There is no UserSettings.xml file found!", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Warning("UXE: No UserSettings.xml file was found!");
            }

            InitializeComponent();
            SetVisuals();
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "SBRW UserSettings XML Editor";

            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/
            // This oddly seems to not do this as it already centers itself
            //FunctionStatus.CenterParent(this);

            /*******************************/
            /* Set Background Image         /
            /*******************************/

            BackgroundImage = Theming.USXEEditor;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            labelLauncherVersion.Text = "Version: v" + Application.ProductVersion;

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                SecondaryFontSize = 8f;
            }
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            labelVideoOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold  | System.Drawing.FontStyle.Underline);
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
