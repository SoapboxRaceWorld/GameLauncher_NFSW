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
        public USXEditor()
        {
            if (File.Exists(FileGameSettings.UserSettingsLocation))
            {
                Log.Debug("UXE: Success, a UserSettings.xml file was found!");
                if (new FileInfo(FileGameSettings.UserSettingsLocation).IsReadOnly == true)
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
                return;
            }

            FileGameSettings.Read();
            /* This was used to check if the Values actualy changed (05-11-2021) - DavidCarbon
             * 
            Log.Debug(FileGameSettingsData.Transmission);
            FileGameSettingsData.Transmission = "4";
            FileGameSettings.Save();
            */

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

            /*******************************/
            /* Comboboxes                   /
            /*******************************/

            comboBox3.DisplayMember = "Text";
            comboBox3.ValueMember = "Value";
            var TransmissonList = new[] {
                new { Text = "Automatic", Value = "0" },
                new { Text = "Manual", Value = "1" },
            };
            comboBox3.DataSource = TransmissonList;
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void USXEditor_Load(object sender, EventArgs e)
        {
            numericBrightness.Value = Convert.ToDecimal(FileGameSettingsData.Brightness);
            numericMVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.MasterAudio);
            numericSFxVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.SFXAudio);
            numericCarVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.CarAudio);
            numericSpeech.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.SpeechAudio);
            numericGMusic.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.MusicAudio);
            numericFEMusic.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.FreeroamAudio);

            comboBox3.SelectedIndex = (FileGameSettingsData.Transmission == "1") ? 1 : 0;

            if (FileGameSettingsData.ScreenWindowed == "0")
            {
                radioWindowedOff.Checked = true;
            }
            else
            {
                radioWindowedOn.Checked = true;
            }

            if (FileGameSettingsData.EnableAero == "0")
            {
                radioAeroOff.Checked = true;
            }
            else
            {
                radioAeroOn.Checked = true;
            }

            if (FileGameSettingsData.VSyncOn == "0")
            {
                radioVSyncOff.Checked = true;
            }
            else
            {
                radioVSyncOn.Checked = true;
            }

            if (FileGameSettingsData.AudioQuality == "0")
            {
                radioAQLow.Checked = true;
            }
            else
            {
                radioAQHigh.Checked = true;
            }

            if (FileGameSettingsData.Damage == "0")
            {
                radioDamageOn.Checked = true;
            }
            else
            {
                radioDamageOff.Checked = true;
            }

            if (FileGameSettingsData.SpeedUnits == "0")
            {
                radioKPH.Checked = true;
            }
            else
            {
                radioMPH.Checked = true;
            }
        }

        private void SettingsSave_Click(object sender, EventArgs e)
        {
            FileGameSettingsData.Brightness = ValidWholeNumberRange(numericBrightness.Value);
            FileGameSettingsData.MasterAudio = ValidDecimalNumberRange(numericMVol.Value);
            FileGameSettingsData.SFXAudio = ValidDecimalNumberRange(numericSFxVol.Value);
            FileGameSettingsData.CarAudio = ValidDecimalNumberRange(numericCarVol.Value);
            FileGameSettingsData.SpeechAudio = ValidDecimalNumberRange(numericSpeech.Value);
            FileGameSettingsData.MusicAudio = ValidDecimalNumberRange(numericGMusic.Value);
            FileGameSettingsData.FreeroamAudio = ValidDecimalNumberRange(numericFEMusic.Value);

            FileGameSettingsData.AudioQuality = (radioAQLow.Checked == true) ? "0" : "1";
            FileGameSettingsData.VSyncOn = (radioVSyncOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.EnableAero = (radioAeroOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.ScreenWindowed = (radioWindowedOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.Damage = (radioDamageOn.Checked == true) ? "0" : "1";
            FileGameSettingsData.SpeedUnits = (radioKPH.Checked == true) ? "0" : "1";

            FileGameSettingsData.Transmission = comboBox3.SelectedValue.ToString();

            FileGameSettings.Save();
        }
        /* Converts Decimal Numbers (ex. 0.52) to 52 */
        private decimal ConvertDecimalToWholeNumber(string UIName)
        {
            decimal Value = Math.Round(Convert.ToDecimal(UIName), 2) * 100;

            if (Value >= 100)
            {
                return 100;
            }
            else if (Value <= 0)
            {
                return 0;
            }
            else
            {
                return Value;
            }
        }

        /* Check User Inputed Value and Keep in Within the Value Range of 0-100 */
        private string ValidWholeNumberRange(decimal UIName)
        {
            decimal Value = Math.Round(UIName, MidpointRounding.ToEven);

            if (Value >= 100)
            {
                return "100";
            }
            else if (Value <= 0)
            {
                return "0";
            }
            else
            {
                return Value.ToString();
            }
        }

        /* Check User Inputed Value and Keep in Within the Value Range of 0-1 with In-between Values */
        private string ValidDecimalNumberRange(decimal UIName)
        {
            decimal Value = Math.Round(UIName, 1);

            if (Value >= 100)
            {
                return "1";
            }
            else if (Value <= 0)
            {
                return "0";
            }
            else
            {
                decimal CleanValue = Value / 100;
                return CleanValue.ToString();
            }
        }
    }
}
