using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Linux;

namespace GameLauncher.App
{
    public partial class USXEditor : Form
    {
        public static bool FileReadOnly = false;
        public USXEditor()
        {
            if (File.Exists(FileGameSettings.UserSettingsLocation))
            {
                Log.Debug("UXE: Success, a UserSettings.xml file was found!");
                if (new FileInfo(FileGameSettings.UserSettingsLocation).IsReadOnly == true)
                {
                    FileReadOnly = true;
                    Log.Warning("UXE: UserSettings.xml is ReadOnly!");
                }
                else
                {
                    Log.Debug("UXE: UserSettings.xml can be modified!");
                }

                FileGameSettings.Read();
                InitializeComponent();
                SetVisuals();
            }
            else
            {
                MessageBox.Show(null, "How is this even possible? There is no UserSettings.xml file found!", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Warning("UXE: No UserSettings.xml file was found!");
                return;
            }
        }

        /* Settings Cancel */
        private void SettingsCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBoxPerformanceLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPerformanceLevel.SelectedIndex == 5)
            {
                this.Size = new Size(608, 866);
                this.CenterToScreen();

                comboBoxBaseTextureFilter.SelectedIndex = CheckValidRange("BaseTextureFilter", "0-2", FileGameSettingsData.BaseTextureFilter);
                comboBoxAnisotropicLevel.SelectedIndex = CheckValidRange("AnisotropicLevel", "0-4", FileGameSettingsData.BaseTextureMaxAni);
                comboBoxCarEnvironmentDetail.SelectedIndex = CheckValidRange("CarEnvironmentDetail", "0-1", FileGameSettingsData.CarLODLevel);
                comboBoxCarReflection.SelectedIndex = CheckValidRange("CarReflection", "0-4", FileGameSettingsData.CarEnvironmentMapEnable);
                comboBoxWorldGlobalDetail.SelectedIndex = CheckValidRange("WorldGlobalDetail", "0-4", FileGameSettingsData.GlobalDetailLevel);
                comboBoxWorldRoadReflection.SelectedIndex = CheckValidRange("WorldRoadReflection", "0-2", FileGameSettingsData.RoadReflectionEnable);
                comboBoxWorldRoadTexture.SelectedIndex = CheckValidRange("WorldRoadTexture", "0-2", FileGameSettingsData.RoadTextureFilter);
                comboBoxWorldRoadAntialiasing.SelectedIndex = CheckValidRange("WorldRoadAntialiasing", "0-4", FileGameSettingsData.RoadTextureMaxAni);
                comboBoxShaderFSAA.SelectedIndex = CheckValidRange("ShaderFSAA", "0-3", FileGameSettingsData.FSAALevel);
                comboBoxShadowDetail.SelectedIndex = CheckValidRange("ShadowDetail", "0-2", FileGameSettingsData.ShadowDetail);
                comboBoxShaderDetail.SelectedIndex = CheckValidRange("ShaderDetail", "0-3", FileGameSettingsData.ShaderDetail);

                if (FileGameSettingsData.BaseTextureLODBias == "0")
                {
                    radioBaseTextureLODOff.Checked = true;
                }
                else
                {
                    radioBaseTextureLODOn.Checked = true;
                }

                if (FileGameSettingsData.CarEnvironmentMapEnable == "0")
                {
                    radioCarDetailLODOff.Checked = true;
                }
                else
                {
                    radioCarDetailLODOn.Checked = true;
                }

                if (FileGameSettingsData.MaxSkidMarks == "0")
                {
                    radioMaxSkidMarksZero.Checked = true;
                }
                else if (FileGameSettingsData.MaxSkidMarks == "1")
                {
                    radioMaxSkidMarksOne.Checked = true;
                }
                else
                {
                    radioMaxSkidMarksTwo.Checked = true;
                }

                if (FileGameSettingsData.RoadTextureLODBias == "0")
                {
                    radioRoadLODBiasOff.Checked = true;
                }
                else
                {
                    radioRoadLODBiasOn.Checked = true;
                }

                if (FileGameSettingsData.MotionBlur == "False")
                {
                    radioMotionBlurOff.Checked = true;
                }
                else
                {
                    radioMotionBlurOn.Checked = true;
                }

                if (FileGameSettingsData.OverBrightEnable == "0")
                {
                    radioOverBrightOff.Checked = true;
                }
                else
                {
                    radioOverBrightOn.Checked = true;
                }

                if (FileGameSettingsData.ParticleSystemEnable == "0")
                {
                    radioParticleSysOff.Checked = true;
                }
                else
                {
                    radioParticleSysOn.Checked = true;
                }

                if (FileGameSettingsData.VisualTreatment == "0")
                {
                    radioVisualTreatOff.Checked = true;
                }
                else
                {
                    radioVisualTreatOn.Checked = true;
                }

                if (FileGameSettingsData.WaterSimEnable == "0")
                {
                    radioWaterSimulationOff.Checked = true;
                }
                else
                {
                    radioWaterSimulationOn.Checked = true;
                }

                if (FileGameSettingsData.PostProcessingEnable == "0")
                {
                    radioPostProcOff.Checked = true;
                }
                else
                {
                    radioPostProcOn.Checked = true;
                }
            }
            else
            {
                this.Size = new Size(298, 866);
                this.CenterToScreen();
            }
        }

        private void USXEditor_Load(object sender, EventArgs e)
        {
            numericResWidth.Value = Convert.ToInt32(FileGameSettingsData.ScreenWidth);
            numericResHeight.Value = Convert.ToInt32(FileGameSettingsData.ScreenHeight);
            numericBrightness.Value = Convert.ToDecimal(FileGameSettingsData.Brightness);
            numericMVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.MasterAudio);
            numericSFxVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.SFXAudio);
            numericCarVol.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.CarAudio);
            numericSpeech.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.SpeechAudio);
            numericGMusic.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.MusicAudio);
            numericFEMusic.Value = ConvertDecimalToWholeNumber(FileGameSettingsData.FreeroamAudio);

            comboBoxTransmisson.SelectedIndex = CheckValidRange("Transmission", "0-1", FileGameSettingsData.Transmission);
            comboAudioMode.SelectedIndex = CheckValidRange("AudioMode", "0-2", FileGameSettingsData.AudioMode);
            comboBoxCamera.SelectedIndex = CheckValidRange("Camera", "0-3", FileGameSettingsData.CameraPOV);
            comboBoxPerformanceLevel.SelectedIndex = CheckValidRange("PerformanceLevel", "0-5", FileGameSettingsData.PerformanceLevel);

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
            FileGameSettingsData.ScreenWidth = ValidWholeNumberRange("Resolution", numericResWidth.Value);
            FileGameSettingsData.ScreenHeight = ValidWholeNumberRange("Resolution", numericResHeight.Value);
            FileGameSettingsData.Brightness = ValidWholeNumberRange("Brightness", numericBrightness.Value);
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

            FileGameSettingsData.Transmission = comboBoxTransmisson.SelectedValue.ToString();
            FileGameSettingsData.AudioMode = comboAudioMode.SelectedValue.ToString();
            FileGameSettingsData.CameraPOV = comboBoxCamera.SelectedValue.ToString();

            if (comboBoxPerformanceLevel.SelectedValue.ToString() == "5")
            {
                FileGameSettingsData.MotionBlur = (radioMotionBlurOff.Checked == true) ? "False" : "True";
                FileGameSettingsData.MotionBlurEnable = (FileGameSettingsData.MotionBlur == "False") ? "0" : "1";
                FileGameSettingsData.RoadTextureLODBias = (radioRoadLODBiasOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.BaseTextureLODBias = (radioBaseTextureLODOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.CarEnvironmentMapEnable = (radioCarDetailLODOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.OverBrightEnable = (radioOverBrightOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.ParticleSystemEnable = (radioParticleSysOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.VisualTreatment = (radioVisualTreatOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.WaterSimEnable = (radioWaterSimulationOff.Checked == true) ? "0" : "1";
                FileGameSettingsData.MaxSkidMarks = SelectedElement("MaxSkidMarks");
                FileGameSettingsData.PostProcessingEnable = (radioPostProcOff.Checked == true) ? "0" : "1";

                FileGameSettingsData.PerformanceLevel = comboBoxPerformanceLevel.SelectedValue.ToString();
                FileGameSettingsData.BaseTextureFilter = comboBoxBaseTextureFilter.SelectedIndex.ToString();
                FileGameSettingsData.BaseTextureMaxAni = comboBoxAnisotropicLevel.SelectedIndex.ToString();
                FileGameSettingsData.CarLODLevel = comboBoxCarEnvironmentDetail.SelectedIndex.ToString();
                FileGameSettingsData.CarEnvironmentMapEnable = comboBoxCarReflection.SelectedIndex.ToString();
                FileGameSettingsData.GlobalDetailLevel = comboBoxWorldGlobalDetail.SelectedIndex.ToString();
                FileGameSettingsData.RoadReflectionEnable = comboBoxWorldRoadReflection.SelectedIndex.ToString();
                FileGameSettingsData.RoadTextureFilter = comboBoxWorldRoadTexture.SelectedIndex.ToString();
                FileGameSettingsData.RoadTextureMaxAni = comboBoxWorldRoadAntialiasing.SelectedIndex.ToString();
                FileGameSettingsData.FSAALevel = comboBoxShaderFSAA.SelectedIndex.ToString();
                FileGameSettingsData.ShadowDetail = comboBoxShadowDetail.SelectedIndex.ToString();
                FileGameSettingsData.ShaderDetail = comboBoxShaderDetail.SelectedIndex.ToString();
            }

            FileGameSettings.Save();
        }

        private int CheckValidRange(string Type, string Range, string Value)
        {
            int ConvertedValue = Convert.ToInt32(Value);

            if (Range == "0-1")
            {
                if (ConvertedValue <= 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else if (Range == "0-2")
            {
                if (Type == "AudioMode")
                {
                    if (ConvertedValue <= 1)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (ConvertedValue <= 0)
                    {
                        return 0;
                    }
                    else if (ConvertedValue >= 2)
                    {
                        return 2;
                    }
                    else
                    {
                        return ConvertedValue;
                    }
                }
            }
            else if (Range == "0-3")
            {
                if (Type == "Camera" && (ConvertedValue < 0 || ConvertedValue > 3))
                {
                    return 2;
                }
                else if (ConvertedValue <= 0)
                {
                    return 0;
                }
                else if (ConvertedValue >= 3)
                {
                    return 3;
                }
                else
                {
                    return ConvertedValue;
                }
            }
            else if (Range == "0-4")
            {
                if (ConvertedValue <= 0)
                {
                    return 0;
                }
                else if (ConvertedValue >= 4)
                {
                    return 4;
                }
                else
                {
                    return ConvertedValue;
                }
            }
            else if (Range == "0-5")
            {
                if (ConvertedValue <= 0)
                {
                    return 0;
                }
                else if (ConvertedValue >= 5)
                {
                    return 5;
                }
                else
                {
                    return ConvertedValue;
                }
            }
            else
            {
                return 0;
            }
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

        /* Check User Inputed Value and Keep in Within the Value Range of 0-100 or Round to the Nearest Whole Number*/
        private string ValidWholeNumberRange(string Type, decimal UIName)
        {
            decimal Value = Math.Round(UIName, MidpointRounding.ToEven);

            if (Type == "Brightness")
            {
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
            else
            {
                return Value.ToString();
            }
        }

        private string SelectedElement(string Type)
        {
            if (Type == "MaxSkidMarks")
            {
                if (radioMaxSkidMarksZero.Checked == true)
                {
                    return "0";
                }
                else if (radioMaxSkidMarksOne.Checked == true)
                {
                    return "1";
                }
                else
                {
                    return "2";
                }
            }
            else
            {
                return "0";
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

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "SBRW UserSettings XML Editor";

            /*******************************/
            /* Set Background Image         /
            /*******************************/

            BackgroundImage = Theming.USXEEditor;
            TransparencyKey = Theming.USXEETransparencyKey;

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            labelLauncherVersion.Text = "Version: v" + Application.ProductVersion;
            labelOverRideAspect.Text = FileGameSettingsData.PostProcessingEnable;

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
            labelVideoOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Regular);
            SettingsSave.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            SettingsCancel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            /* Titles */
            labelVideoOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelAudioOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelVolumeControl.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelGameplayOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelShaderDetails.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelWorldDetails.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelCarDetail.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            labelBaseTextures.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold | FontStyle.Underline);
            /* Sub-Titles */
            labelPerfLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelResolution.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelBrightness.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelWindowed.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelEnableAero.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelVSync.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelPixelAspect.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelAudioMode.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelAudioQuality.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelMasterVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelSFXVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelCarVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelSpeechVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGameMusicVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelFEMusicVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGPOCamera.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGPOTrans.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGPODamage.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGPOUnits.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelFSAA.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelMotionBlur.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelOverbright.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelPostProc.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelPartSys.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelShadowDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelShaderDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelVisTreat.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelWaterSimulation.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelGlobalDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelSkidMarks.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelRoadReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelRoadTexture.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelRoadLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelRoadAnti.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelCarModel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelCarReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelCDLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelFilterLvl.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelBTLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelAntialiasing.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            labelLauncherVersion.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            /* Radio Buttons */
            radioWindowedOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioWindowedOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioAeroOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioAeroOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioVSyncOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioVSyncOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioAQLow.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioAQHigh.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioDamageOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioDamageOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioKPH.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMPH.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMotionBlurOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMotionBlurOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioOverBrightOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioOverBrightOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioPostProcOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioPostProcOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioParticleSysOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioParticleSysOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioVisualTreatOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioVisualTreatOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioWaterSimulationOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioWaterSimulationOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMaxSkidMarksZero.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMaxSkidMarksOne.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioMaxSkidMarksTwo.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioRoadLODBiasOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioRoadLODBiasOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioCarDetailLODOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioCarDetailLODOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioBaseTextureLODOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            radioBaseTextureLODOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            /* Input Boxes */
            numericResWidth.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericResHeight.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericBrightness.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericMVol.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericSFxVol.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericCarVol.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericSpeech.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericGMusic.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            numericFEMusic.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            /* DropDown Menus */
            comboBoxPerformanceLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboAudioMode.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxCamera.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxTransmisson.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxShaderFSAA.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxShadowDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxShaderDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxWorldGlobalDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxWorldRoadReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxWorldRoadTexture.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxWorldRoadAntialiasing.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxCarEnvironmentDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxCarReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxBaseTextureFilter.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);
            comboBoxAnisotropicLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            if (FileReadOnly == true)
            {
                SettingsSave.Text = "Read-Only";
                SettingsSave.ForeColor = Theming.WinFormErrorTextForeColor;
            }
            else
            {
                SettingsSave.ForeColor = Theming.SeventhTextForeColor;
            }

            SettingsSave.Image = Theming.GreenButton;
            SettingsCancel.Image = Theming.GrayButton;
            /* Titles */
            labelVideoOptions.ForeColor = Theming.SecondaryTextForeColor;
            labelAudioOptions.ForeColor = Theming.SecondaryTextForeColor;
            labelVolumeControl.ForeColor = Theming.SecondaryTextForeColor;
            labelGameplayOptions.ForeColor = Theming.SecondaryTextForeColor;
            labelShaderDetails.ForeColor = Theming.SecondaryTextForeColor;
            labelWorldDetails.ForeColor = Theming.SecondaryTextForeColor;
            labelCarDetail.ForeColor = Theming.SecondaryTextForeColor;
            labelBaseTextures.ForeColor = Theming.SecondaryTextForeColor;
            /* Sub-Titles */
            labelPerfLevel.ForeColor = Theming.Link;
            labelResolution.ForeColor = Theming.MainTextForeColor;
            labelBrightness.ForeColor = Theming.MainTextForeColor;
            labelWindowed.ForeColor = Theming.MainTextForeColor;
            labelEnableAero.ForeColor = Theming.MainTextForeColor;
            labelVSync.ForeColor = Theming.MainTextForeColor;
            labelPixelAspect.ForeColor = Theming.MainTextForeColor;
            labelAudioMode.ForeColor = Theming.MainTextForeColor;
            labelAudioQuality.ForeColor = Theming.MainTextForeColor;
            labelMasterVol.ForeColor = Theming.MainTextForeColor;
            labelSFXVol.ForeColor = Theming.MainTextForeColor;
            labelCarVol.ForeColor = Theming.MainTextForeColor;
            labelSpeechVol.ForeColor = Theming.MainTextForeColor;
            labelGameMusicVol.ForeColor = Theming.MainTextForeColor;
            labelFEMusicVol.ForeColor = Theming.MainTextForeColor;
            labelGPOCamera.ForeColor = Theming.MainTextForeColor;
            labelGPOTrans.ForeColor = Theming.MainTextForeColor;
            labelGPODamage.ForeColor = Theming.MainTextForeColor;
            labelGPOUnits.ForeColor = Theming.MainTextForeColor;
            labelFSAA.ForeColor = Theming.MainTextForeColor;
            labelMotionBlur.ForeColor = Theming.MainTextForeColor;
            labelOverbright.ForeColor = Theming.MainTextForeColor;
            labelPostProc.ForeColor = Theming.MainTextForeColor;
            labelPartSys.ForeColor = Theming.MainTextForeColor;
            labelShadowDetail.ForeColor = Theming.MainTextForeColor;
            labelShaderDetail.ForeColor = Theming.MainTextForeColor;
            labelVisTreat.ForeColor = Theming.MainTextForeColor;
            labelWaterSimulation.ForeColor = Theming.MainTextForeColor;
            labelGlobalDetail.ForeColor = Theming.MainTextForeColor;
            labelSkidMarks.ForeColor = Theming.MainTextForeColor;
            labelRoadReflection.ForeColor = Theming.MainTextForeColor;
            labelRoadTexture.ForeColor = Theming.MainTextForeColor;
            labelRoadLODBias.ForeColor = Theming.MainTextForeColor;
            labelRoadAnti.ForeColor = Theming.MainTextForeColor;
            labelCarModel.ForeColor = Theming.MainTextForeColor;
            labelCarReflection.ForeColor = Theming.MainTextForeColor;
            labelCDLODBias.ForeColor = Theming.MainTextForeColor;
            labelFilterLvl.ForeColor = Theming.MainTextForeColor;
            labelBTLODBias.ForeColor = Theming.MainTextForeColor;
            labelAntialiasing.ForeColor = Theming.MainTextForeColor;
            labelLauncherVersion.ForeColor = Theming.MainTextForeColor;
            /* Radio Buttons */
            radioWindowedOn.ForeColor = Theming.MainTextForeColor;
            radioWindowedOff.ForeColor = Theming.MainTextForeColor;
            radioAeroOn.ForeColor = Theming.MainTextForeColor;
            radioAeroOff.ForeColor = Theming.MainTextForeColor;
            radioVSyncOn.ForeColor = Theming.MainTextForeColor;
            radioVSyncOff.ForeColor = Theming.MainTextForeColor;
            radioAQLow.ForeColor = Theming.MainTextForeColor;
            radioAQHigh.ForeColor = Theming.MainTextForeColor;
            radioDamageOn.ForeColor = Theming.MainTextForeColor;
            radioDamageOff.ForeColor = Theming.MainTextForeColor;
            radioKPH.ForeColor = Theming.MainTextForeColor;
            radioMPH.ForeColor = Theming.MainTextForeColor;
            radioMotionBlurOn.ForeColor = Theming.MainTextForeColor;
            radioMotionBlurOff.ForeColor = Theming.MainTextForeColor;
            radioOverBrightOn.ForeColor = Theming.MainTextForeColor;
            radioOverBrightOff.ForeColor = Theming.MainTextForeColor;
            radioPostProcOn.ForeColor = Theming.MainTextForeColor;
            radioPostProcOff.ForeColor = Theming.MainTextForeColor;
            radioParticleSysOn.ForeColor = Theming.MainTextForeColor;
            radioParticleSysOff.ForeColor = Theming.MainTextForeColor;
            radioVisualTreatOn.ForeColor = Theming.MainTextForeColor;
            radioVisualTreatOff.ForeColor = Theming.MainTextForeColor;
            radioWaterSimulationOn.ForeColor = Theming.MainTextForeColor;
            radioWaterSimulationOff.ForeColor = Theming.MainTextForeColor;
            radioMaxSkidMarksZero.ForeColor = Theming.MainTextForeColor;
            radioMaxSkidMarksOne.ForeColor = Theming.MainTextForeColor;
            radioMaxSkidMarksTwo.ForeColor = Theming.MainTextForeColor;
            radioRoadLODBiasOn.ForeColor = Theming.MainTextForeColor;
            radioRoadLODBiasOff.ForeColor = Theming.MainTextForeColor;
            radioCarDetailLODOn.ForeColor = Theming.MainTextForeColor;
            radioCarDetailLODOff.ForeColor = Theming.MainTextForeColor;
            radioBaseTextureLODOn.ForeColor = Theming.MainTextForeColor;
            radioBaseTextureLODOff.ForeColor = Theming.MainTextForeColor;
            /* Input Boxes */
            numericResWidth.ForeColor = Theming.CDNMenuTextForeColor;
            numericResWidth.BackColor = Theming.CDNMenuBGForeColor;
            numericResHeight.ForeColor = Theming.CDNMenuTextForeColor;
            numericResHeight.BackColor = Theming.CDNMenuBGForeColor;
            numericBrightness.ForeColor = Theming.CDNMenuTextForeColor;
            numericBrightness.BackColor = Theming.CDNMenuBGForeColor;
            numericMVol.ForeColor = Theming.CDNMenuTextForeColor;
            numericMVol.BackColor = Theming.CDNMenuBGForeColor;
            numericSFxVol.ForeColor = Theming.CDNMenuTextForeColor;
            numericSFxVol.BackColor = Theming.CDNMenuBGForeColor;
            numericCarVol.ForeColor = Theming.CDNMenuTextForeColor;
            numericCarVol.BackColor = Theming.CDNMenuBGForeColor;
            numericSpeech.ForeColor = Theming.CDNMenuTextForeColor;
            numericSpeech.BackColor = Theming.CDNMenuBGForeColor;
            numericGMusic.ForeColor = Theming.CDNMenuTextForeColor;
            numericGMusic.BackColor = Theming.CDNMenuBGForeColor;
            numericFEMusic.ForeColor = Theming.CDNMenuTextForeColor;
            numericFEMusic.BackColor = Theming.CDNMenuBGForeColor;
            /* DropDown Menus */
            comboBoxPerformanceLevel.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxPerformanceLevel.BackColor = Theming.CDNMenuBGForeColor;
            comboAudioMode.ForeColor = Theming.CDNMenuTextForeColor;
            comboAudioMode.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxCamera.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxCamera.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxTransmisson.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxTransmisson.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxShaderFSAA.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxShaderFSAA.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxShadowDetail.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxShadowDetail.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxShaderDetail.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxShaderDetail.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxWorldGlobalDetail.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxWorldGlobalDetail.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxWorldRoadReflection.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxWorldRoadReflection.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxWorldRoadTexture.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxWorldRoadTexture.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxWorldRoadAntialiasing.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxWorldRoadAntialiasing.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxCarEnvironmentDetail.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxCarEnvironmentDetail.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxCarReflection.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxCarReflection.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxBaseTextureFilter.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxBaseTextureFilter.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxAnisotropicLevel.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxAnisotropicLevel.BackColor = Theming.CDNMenuBGForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            SettingsSave.MouseEnter += new EventHandler(Greenbutton_hover_MouseEnter);
            SettingsSave.MouseLeave += new EventHandler(Greenbutton_MouseLeave);
            SettingsSave.MouseUp += new MouseEventHandler(Greenbutton_hover_MouseUp);
            SettingsSave.MouseDown += new MouseEventHandler(Greenbutton_click_MouseDown);

            SettingsCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            SettingsCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            SettingsCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            SettingsCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);

            /*******************************/
            /* Comboboxes                   /
            /*******************************/

            /* Transmisson ComboBox */
            var TransmissonList = new[] {
                new { SaveTheManuals = "Automatic", Value = "0" },
                new { SaveTheManuals = "Manual", Value = "1" }
            };
            comboBoxTransmisson.DisplayMember = "SaveTheManuals";
            comboBoxTransmisson.ValueMember = "Value";
            comboBoxTransmisson.DataSource = TransmissonList;

            /* AudioMode ComboBox */
            var AudioModeList = new[] {
                new { Sound = "Stero", Value = "1" },
                new { Sound = "Surround", Value = "2" }
            };
            comboAudioMode.DisplayMember = "Sound";
            comboAudioMode.ValueMember = "Value";
            comboAudioMode.DataSource = AudioModeList;

            /* CameraPOV ComboBox */
            var CameraPOVList = new[] {
                new { CameraPOV = "Bumper", Value = "0" },
                new { CameraPOV = "Hood", Value = "1" },
                new { CameraPOV = "Near", Value = "2" },
                new { CameraPOV = "Far", Value = "3" }
            };
            comboBoxCamera.DisplayMember = "CameraPOV";
            comboBoxCamera.ValueMember = "Value";
            comboBoxCamera.DataSource = CameraPOVList;

            /* PerformanceLevel ComboBox */
            var PerformanceLevelList = new[] {
                new { PerformanceLevel = "Minimum", Value = "0" },
                new { PerformanceLevel = "Low", Value = "1" },
                new { PerformanceLevel = "Medium", Value = "2" },
                new { PerformanceLevel = "High", Value = "3" },
                new { PerformanceLevel = "Maximum", Value = "4" },
                new { PerformanceLevel = "Custom", Value = "5" }
            };
            comboBoxPerformanceLevel.DisplayMember = "PerformanceLevel";
            comboBoxPerformanceLevel.ValueMember = "Value";
            comboBoxPerformanceLevel.DataSource = PerformanceLevelList;

            /* BaseTextureFilter ComboBox */
            var BaseTextureFilterList = new[] {
                new { BaseTextureFilter = "Bilinear", Value = "0" },
                new { BaseTextureFilter = "Trilinear", Value = "1" },
                new { BaseTextureFilter = "Anisotropic", Value = "2" }
            };
            comboBoxBaseTextureFilter.DisplayMember = "BaseTextureFilter";
            comboBoxBaseTextureFilter.ValueMember = "Value";
            comboBoxBaseTextureFilter.DataSource = BaseTextureFilterList;

            /* AnisotropicLevel ComboBox */
            var AnisotropicLevelList = new[] {
                new { AnisotropicLevel = "None", Value = "0" },
                new { AnisotropicLevel = "2x", Value = "1" },
                new { AnisotropicLevel = "4x", Value = "2" },
                new { AnisotropicLevel = "8x", Value = "3" },
                new { AnisotropicLevel = "16x", Value = "4" }
            };
            comboBoxAnisotropicLevel.DisplayMember = "AnisotropicLevel";
            comboBoxAnisotropicLevel.ValueMember = "Value";
            comboBoxAnisotropicLevel.DataSource = AnisotropicLevelList;

            /* CarEnvironmentDetail ComboBox */
            var CarEnvironmentDetailList = new[] {
                new { CarEnvironmentDetail = "Low", Value = "0" },
                new { CarEnvironmentDetail = "High", Value = "1" }
            };
            comboBoxCarEnvironmentDetail.DisplayMember = "CarEnvironmentDetail";
            comboBoxCarEnvironmentDetail.ValueMember = "Value";
            comboBoxCarEnvironmentDetail.DataSource = CarEnvironmentDetailList;

            /* CarReflection ComboBox */
            var CarReflectionList = new[] {
                new { CarReflection = "Minimum", Value = "0" },
                new { CarReflection = "Low", Value = "1" },
                new { CarReflection = "Medium", Value = "2" },
                new { CarReflection = "High", Value = "3" },
                new { CarReflection = "Maximum", Value = "4" }
            };
            comboBoxCarReflection.DisplayMember = "CarReflection";
            comboBoxCarReflection.ValueMember = "Value";
            comboBoxCarReflection.DataSource = CarReflectionList;

            /* WorldGlobalDetail ComboBox */
            var WorldGlobalDetailList = new[] {
                new { WorldGlobalDetail = "Minimum", Value = "0" },
                new { WorldGlobalDetail = "Low", Value = "1" },
                new { WorldGlobalDetail = "Medium", Value = "2" },
                new { WorldGlobalDetail = "High", Value = "3" },
                new { WorldGlobalDetail = "Ultra", Value = "4" }
            };
            comboBoxWorldGlobalDetail.DisplayMember = "WorldGlobalDetail";
            comboBoxWorldGlobalDetail.ValueMember = "Value";
            comboBoxWorldGlobalDetail.DataSource = WorldGlobalDetailList;

            /* WorldRoadReflection ComboBox */
            var WorldRoadReflectionList = new[] {
                new { WorldRoadReflection = "Minimum", Value = "0" },
                new { WorldRoadReflection = "Medium", Value = "1" },
                new { WorldRoadReflection = "Maximum", Value = "2" }
            };
            comboBoxWorldRoadReflection.DisplayMember = "WorldRoadReflection";
            comboBoxWorldRoadReflection.ValueMember = "Value";
            comboBoxWorldRoadReflection.DataSource = WorldRoadReflectionList;

            /* WorldRoadTexture ComboBox */
            var WorldRoadTextureList = new[] {
                new { WorldRoadTexture = "Minimum", Value = "0" },
                new { WorldRoadTexture = "Medium", Value = "1" },
                new { WorldRoadTexture = "Maximum", Value = "2" }
            };
            comboBoxWorldRoadTexture.DisplayMember = "WorldRoadTexture";
            comboBoxWorldRoadTexture.ValueMember = "Value";
            comboBoxWorldRoadTexture.DataSource = WorldRoadTextureList;

            /* WorldRoadAntialiasing ComboBox */
            var WorldRoadAntialiasingList = new[] {
                new { WorldRoadAntialiasing = "None", Value = "0" },
                new { WorldRoadAntialiasing = "2x", Value = "1" },
                new { WorldRoadAntialiasing = "4x", Value = "2" },
                new { WorldRoadAntialiasing = "8x", Value = "3" },
                new { WorldRoadAntialiasing = "16x", Value = "4" }
            };
            comboBoxWorldRoadAntialiasing.DisplayMember = "WorldRoadAntialiasing";
            comboBoxWorldRoadAntialiasing.ValueMember = "Value";
            comboBoxWorldRoadAntialiasing.DataSource = WorldRoadAntialiasingList;

            /* ShaderFSAA ComboBox */
            var ShaderFSAAList = new[] {
                new { ShaderFSAA = "None", Value = "0" },
                new { ShaderFSAA = "Low", Value = "1" },
                new { ShaderFSAA = "Medium", Value = "2" },
                new { ShaderFSAA = "High", Value = "3" },
                new { ShaderFSAA = "Ultra", Value = "4" }
            };
            comboBoxShaderFSAA.DisplayMember = "ShaderFSAA";
            comboBoxShaderFSAA.ValueMember = "Value";
            comboBoxShaderFSAA.DataSource = ShaderFSAAList;

            /* ShadowDetail ComboBox */
            var ShadowDetailList = new[] {
                new { ShadowDetail = "Low", Value = "0" },
                new { ShadowDetail = "Medium", Value = "1" },
                new { ShadowDetail = "High", Value = "2" }
            };
            comboBoxShadowDetail.DisplayMember = "ShadowDetail";
            comboBoxShadowDetail.ValueMember = "Value";
            comboBoxShadowDetail.DataSource = ShadowDetailList;

            /* ShaderDetail ComboBox */
            var ShaderDetailList = new[] {
                new { ShaderDetail = "Minimum", Value = "0" },
                new { ShaderDetail = "Low", Value = "1" },
                new { ShaderDetail = "Medium", Value = "2" },
                new { ShaderDetail = "High", Value = "3" },
                new { ShaderDetail = "Maximum", Value = "4" }
            };
            comboBoxShaderDetail.DisplayMember = "ShaderDetail";
            comboBoxShaderDetail.ValueMember = "Value";
            comboBoxShaderDetail.DataSource = ShaderDetailList;
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonHover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonHover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsSave.Image = Theming.GreenButtonClick;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonClick;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonHover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            SettingsCancel.Image = Theming.GrayButtonHover;
        }
    }
}
