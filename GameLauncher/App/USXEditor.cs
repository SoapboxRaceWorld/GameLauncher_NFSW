using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using System.Diagnostics;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.RPC;

namespace GameLauncher.App
{
    public partial class USXEditor : Form
    {
        public static bool FileReadOnly = false;
        public static int AmountofCenterTimes = 0;
        public static bool ResolutionsListLoaded = false;
        public USXEditor()
        {
            if (File.Exists(FileGameSettings.UserSettingsLocation))
            {
                DiscordLauncherPresense.Status("User XML Editor", null);
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

                FileGameSettings.Read("Full File");
                ResolutionsListUpdater.Get();
                InitializeComponent();
                SetVisuals();
                this.Closing += (x, y) =>
                {
                    DiscordLauncherPresense.Status("Settings", null);
                };
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
            try
            {
                if (comboBoxPerformanceLevel.SelectedIndex == 0)
                {
                    /* Minimal */
                    comboBoxBaseTextureFilter.SelectedIndex = 0;
                    comboBoxAnisotropicLevel.SelectedIndex = 0;
                    comboBoxCarEnvironmentDetail.SelectedIndex = 0;
                    comboBoxWorldGlobalDetail.SelectedIndex = 0;
                    comboBoxWorldRoadReflection.SelectedIndex = 0;
                    comboBoxWorldRoadTexture.SelectedIndex = 0;
                    comboBoxWorldRoadAniso.SelectedIndex = 0;
                    comboBoxShaderFSAA.SelectedIndex = 0;
                    comboBoxShadowDetail.SelectedIndex = 0;
                    comboBoxShaderDetail.SelectedIndex = 0;

                    SetCorrectElementValues("BaseTextureLODBias", "0");
                    SetCorrectElementValues("CarEnvironmentMapEnable", "0");
                    SetCorrectElementValues("MaxSkidMarks", "0");
                    SetCorrectElementValues("RoadTextureLODBias", "0");
                    SetCorrectElementValues("MotionBlurEnable", "0");
                    SetCorrectElementValues("OverBrightEnable", "0");
                    SetCorrectElementValues("ParticleSystemEnable", "0");
                    SetCorrectElementValues("VisualTreatment", "0");
                    SetCorrectElementValues("WaterSimEnable", "0");
                    SetCorrectElementValues("PostProcessingEnable", "0");

                    Log.Info("USXE: Selected Minimum Preset");
                }
                else if (comboBoxPerformanceLevel.SelectedIndex == 1)
                {
                    /* Low */
                    comboBoxBaseTextureFilter.SelectedIndex = 0;
                    comboBoxAnisotropicLevel.SelectedIndex = 0;
                    comboBoxCarEnvironmentDetail.SelectedIndex = 1;
                    comboBoxWorldGlobalDetail.SelectedIndex = 1;
                    comboBoxWorldRoadReflection.SelectedIndex = 0;
                    comboBoxWorldRoadTexture.SelectedIndex = 0;
                    comboBoxWorldRoadAniso.SelectedIndex = 0;
                    comboBoxShaderFSAA.SelectedIndex = 0;
                    comboBoxShadowDetail.SelectedIndex = 0;
                    comboBoxShaderDetail.SelectedIndex = 1;

                    SetCorrectElementValues("BaseTextureLODBias", "0");
                    SetCorrectElementValues("CarEnvironmentMapEnable", "1");
                    SetCorrectElementValues("MaxSkidMarks", "0");
                    SetCorrectElementValues("RoadTextureLODBias", "0");
                    SetCorrectElementValues("MotionBlurEnable", "0");
                    SetCorrectElementValues("OverBrightEnable", "0");
                    SetCorrectElementValues("ParticleSystemEnable", "1");
                    SetCorrectElementValues("VisualTreatment", "0");
                    SetCorrectElementValues("WaterSimEnable", "0");
                    SetCorrectElementValues("PostProcessingEnable", "0");

                    Log.Info("USXE: Selected Low Preset");
                }
                else if (comboBoxPerformanceLevel.SelectedIndex == 2)
                {
                    /* Medium */
                    comboBoxBaseTextureFilter.SelectedIndex = 1;
                    comboBoxAnisotropicLevel.SelectedIndex = 0;
                    comboBoxCarEnvironmentDetail.SelectedIndex = 2;
                    comboBoxWorldGlobalDetail.SelectedIndex = 3;
                    comboBoxWorldRoadReflection.SelectedIndex = 1;
                    comboBoxWorldRoadTexture.SelectedIndex = 1;
                    comboBoxWorldRoadAniso.SelectedIndex = 0;
                    comboBoxShaderFSAA.SelectedIndex = 1;
                    comboBoxShadowDetail.SelectedIndex = 1;
                    comboBoxShaderDetail.SelectedIndex = 1;

                    SetCorrectElementValues("BaseTextureLODBias", "0");
                    SetCorrectElementValues("CarEnvironmentMapEnable", "2");
                    SetCorrectElementValues("MaxSkidMarks", "1");
                    SetCorrectElementValues("RoadTextureLODBias", "0");
                    SetCorrectElementValues("MotionBlurEnable", "0");
                    SetCorrectElementValues("OverBrightEnable", "1");
                    SetCorrectElementValues("ParticleSystemEnable", "1");
                    SetCorrectElementValues("VisualTreatment", "0");
                    SetCorrectElementValues("WaterSimEnable", "0");
                    SetCorrectElementValues("PostProcessingEnable", "0");

                    Log.Info("USXE: Selected Medium Preset");
                }
                else if (comboBoxPerformanceLevel.SelectedIndex == 3)
                {
                    /* High */
                    comboBoxBaseTextureFilter.SelectedIndex = 2;
                    comboBoxAnisotropicLevel.SelectedIndex = 3;
                    comboBoxCarEnvironmentDetail.SelectedIndex = 3;
                    comboBoxWorldGlobalDetail.SelectedIndex = 2;
                    comboBoxWorldRoadReflection.SelectedIndex = 1;
                    comboBoxWorldRoadTexture.SelectedIndex = 1;
                    comboBoxWorldRoadAniso.SelectedIndex = 3;
                    comboBoxShaderFSAA.SelectedIndex = 2;
                    comboBoxShadowDetail.SelectedIndex = 2;
                    comboBoxShaderDetail.SelectedIndex = 3;

                    SetCorrectElementValues("BaseTextureLODBias", "0");
                    SetCorrectElementValues("CarEnvironmentMapEnable", "2");
                    SetCorrectElementValues("MaxSkidMarks", "2");
                    SetCorrectElementValues("RoadTextureLODBias", "0");
                    SetCorrectElementValues("MotionBlurEnable", "0");
                    SetCorrectElementValues("OverBrightEnable", "1");
                    SetCorrectElementValues("ParticleSystemEnable", "1");
                    SetCorrectElementValues("VisualTreatment", "1");
                    SetCorrectElementValues("WaterSimEnable", "1");
                    SetCorrectElementValues("PostProcessingEnable", "0");

                    Log.Info("USXE: Selected High Preset");
                }
                else if (comboBoxPerformanceLevel.SelectedIndex == 4)
                {
                    /* Maximum */
                    comboBoxBaseTextureFilter.SelectedIndex = 2;
                    comboBoxAnisotropicLevel.SelectedIndex = 4;
                    comboBoxCarEnvironmentDetail.SelectedIndex = 4;
                    comboBoxWorldGlobalDetail.SelectedIndex = 4;
                    comboBoxWorldRoadReflection.SelectedIndex = 2;
                    comboBoxWorldRoadTexture.SelectedIndex = 2;
                    comboBoxWorldRoadAniso.SelectedIndex = 4;
                    comboBoxShaderFSAA.SelectedIndex = 2;
                    comboBoxShadowDetail.SelectedIndex = 2;
                    comboBoxShaderDetail.SelectedIndex = 4;

                    SetCorrectElementValues("BaseTextureLODBias", "0");
                    SetCorrectElementValues("CarEnvironmentMapEnable", "3");
                    SetCorrectElementValues("MaxSkidMarks", "2");
                    SetCorrectElementValues("RoadTextureLODBias", "0");
                    SetCorrectElementValues("MotionBlurEnable", "0");
                    SetCorrectElementValues("OverBrightEnable", "1");
                    SetCorrectElementValues("ParticleSystemEnable", "1");
                    SetCorrectElementValues("VisualTreatment", "1");
                    SetCorrectElementValues("WaterSimEnable", "1");
                    SetCorrectElementValues("PostProcessingEnable", "1");

                    Log.Info("USXE: Selected Maxium Preset");
                }
                else
                {
                    /* Custom */
                    comboBoxBaseTextureFilter.SelectedIndex = CheckValidRange("BaseTextureFilter", "0-2", FileGameSettingsData.BaseTextureFilter);
                    comboBoxAnisotropicLevel.SelectedIndex = CheckValidRange("AnisotropicLevel", "0-4", FileGameSettingsData.BaseTextureMaxAni);
                    comboBoxCarEnvironmentDetail.SelectedIndex = CheckValidRange("CarEnvironmentDetail", "0-4", FileGameSettingsData.CarEnvironmentMapEnable);
                    comboBoxWorldGlobalDetail.SelectedIndex = CheckValidRange("WorldGlobalDetail", "0-4", FileGameSettingsData.GlobalDetailLevel);
                    comboBoxWorldRoadReflection.SelectedIndex = CheckValidRange("WorldRoadReflection", "0-2", FileGameSettingsData.RoadReflectionEnable);
                    comboBoxWorldRoadTexture.SelectedIndex = CheckValidRange("WorldRoadTexture", "0-2", FileGameSettingsData.RoadTextureFilter);
                    comboBoxWorldRoadAniso.SelectedIndex = CheckValidRange("WorldRoadAniso", "0-4", FileGameSettingsData.RoadTextureMaxAni);
                    comboBoxShaderFSAA.SelectedIndex = CheckValidRange("ShaderFSAA", "0-2", FileGameSettingsData.FSAALevel);
                    comboBoxShadowDetail.SelectedIndex = CheckValidRange("ShadowDetail", "0-2", FileGameSettingsData.ShadowDetail);
                    comboBoxShaderDetail.SelectedIndex = CheckValidRange("ShaderDetail", "0-3", FileGameSettingsData.ShaderDetail);

                    SetCorrectElementValues("BaseTextureLODBias", FileGameSettingsData.BaseTextureLODBias);
                    SetCorrectElementValues("CarEnvironmentMapEnable", FileGameSettingsData.CarEnvironmentMapEnable);
                    SetCorrectElementValues("MaxSkidMarks", FileGameSettingsData.MaxSkidMarks);
                    SetCorrectElementValues("RoadTextureLODBias", FileGameSettingsData.RoadTextureLODBias);
                    SetCorrectElementValues("MotionBlurEnable", FileGameSettingsData.MotionBlurEnable);
                    SetCorrectElementValues("OverBrightEnable", FileGameSettingsData.OverBrightEnable);
                    SetCorrectElementValues("ParticleSystemEnable", FileGameSettingsData.ParticleSystemEnable);
                    SetCorrectElementValues("VisualTreatment", FileGameSettingsData.VisualTreatment);
                    SetCorrectElementValues("WaterSimEnable", FileGameSettingsData.WaterSimEnable);
                    SetCorrectElementValues("PostProcessingEnable", FileGameSettingsData.PostProcessingEnable);

                    Log.Info("USXE: Selected Custom Preset");
                }

                if (comboBoxPerformanceLevel.SelectedIndex == 5)
                {
                    if (WindowsProductVersion.GetWindowsNumber() >= 10)
                    {
                        Size = new Size(564, 726);
                    }
                    else
                    {
                        Size = new Size(554, 712);
                    }
                    comboResolutions.Visible = false;

                    if (AmountofCenterTimes == 0)
                    {
                        AmountofCenterTimes++;
                        CenterToScreen();
                    }
                }
                else
                {
                    if (WindowsProductVersion.GetWindowsNumber() >= 10)
                    {
                        Size = new Size(292, 726);
                    }
                    else
                    {
                        Size = new Size(282, 712);
                    }


                    if (ResolutionsListLoaded == true)
                    {
                        comboResolutions.Visible = true;
                    }
                    else
                    {
                        comboResolutions.Visible = false;
                    }
                }
            }
            catch (Exception Error)
            {
                Log.Error("USXE: " + Error.Message);
            }
        }

        private void SettingsSave_Click(object sender, EventArgs e)
        {
            FileGameSettingsData.ScreenWidth = ValidWholeNumberRange("Resolution", (comboBoxPerformanceLevel.SelectedValue.ToString() == "5" || ResolutionsListLoaded == false) ? 
                                               numericResWidth.Value : Convert.ToDecimal(((JsonResolutions)comboResolutions.SelectedItem).Width));
            FileGameSettingsData.ScreenHeight = ValidWholeNumberRange("Resolution", (comboBoxPerformanceLevel.SelectedValue.ToString() == "5" || ResolutionsListLoaded == false) ?
                                                numericResHeight.Value : Convert.ToDecimal(((JsonResolutions)comboResolutions.SelectedItem).Height));
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
            FileGameSettingsData.SpeedUnits = (radioKmH.Checked == true) ? "0" : "1";

            FileGameSettingsData.Transmission = comboBoxTransmisson.SelectedValue.ToString();
            FileGameSettingsData.AudioMode = comboAudioMode.SelectedValue.ToString();
            FileGameSettingsData.CameraPOV = comboBoxCamera.SelectedValue.ToString();

            FileGameSettingsData.MotionBlurEnable = (radioMotionBlurOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.RoadTextureLODBias = (radioRoadLODBiasOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.BaseTextureLODBias = (radioBaseTextureLODOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.CarLODLevel = (radioCarDetailLODOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.OverBrightEnable = (radioOverBrightOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.ParticleSystemEnable = (radioParticleSysOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.VisualTreatment = (radioVisualTreatOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.WaterSimEnable = (radioWaterSimulationOff.Checked == true) ? "0" : "1";
            FileGameSettingsData.MaxSkidMarks = SelectedElement("MaxSkidMarks");
            FileGameSettingsData.PostProcessingEnable = (radioPostProcOff.Checked == true) ? "0" : "1";

            FileGameSettingsData.PerformanceLevel = comboBoxPerformanceLevel.SelectedValue.ToString();
            FileGameSettingsData.BaseTextureFilter = comboBoxBaseTextureFilter.SelectedValue.ToString();
            FileGameSettingsData.BaseTextureMaxAni = comboBoxAnisotropicLevel.SelectedValue.ToString();
            FileGameSettingsData.CarEnvironmentMapEnable = comboBoxCarEnvironmentDetail.SelectedValue.ToString();
            FileGameSettingsData.GlobalDetailLevel = comboBoxWorldGlobalDetail.SelectedValue.ToString();
            FileGameSettingsData.RoadReflectionEnable = comboBoxWorldRoadReflection.SelectedValue.ToString();
            FileGameSettingsData.RoadTextureFilter = comboBoxWorldRoadTexture.SelectedValue.ToString();
            FileGameSettingsData.RoadTextureMaxAni = comboBoxWorldRoadAniso.SelectedValue.ToString();
            FileGameSettingsData.FSAALevel = comboBoxShaderFSAA.SelectedValue.ToString();
            FileGameSettingsData.ShadowDetail = comboBoxShadowDetail.SelectedValue.ToString();
            FileGameSettingsData.ShaderDetail = comboBoxShaderDetail.SelectedValue.ToString();

            FileGameSettings.Save("Display", "Full File");
        }

        private int CheckValidRange(string Type, string Range, string Value)
        {
            int ConvertedValue;

            try
            {
                ConvertedValue = Convert.ToInt32(Value);
            }
            catch 
            {
                ConvertedValue = 0;
            }

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
                if (Type == "ShaderFSAA")
                {
                    if (ConvertedValue == 0)
                    {
                        return 0;
                    }
                    else if (ConvertedValue == 2)
                    {
                        return 1;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else
                {
                    if (ConvertedValue <= 0)
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
            }
            else if (Range == "0-4")
            {
                if (Type == "WorldRoadAniso" || Type == "AnisotropicLevel")
                {
                    if (ConvertedValue == 0)
                    {
                        return 0;
                    }
                    if (ConvertedValue == 2)
                    {
                        return 1;
                    }
                    else if (ConvertedValue == 4)
                    {
                        return 2;
                    }
                    else if (ConvertedValue == 8)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else
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
            decimal Value;

            try
            {
                Value = Math.Round(Convert.ToDecimal(UIName), 2) * 100;
            }
            catch
            {
                Value = 52;
            }

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
            decimal Value;

            try
            {
                Value = Math.Round(UIName, MidpointRounding.ToEven);
            }
            catch
            {
                Value = 52;
            }

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
            decimal Value;

            try
            {
                Value = Math.Round(UIName, 1);
            }
            catch
            {
                Value = 100;
            }

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

        private void SetCorrectElementValues(string Element, string ComparisonValue)
        {
            try
            {
                if (Element == "BaseTextureLODBias")
                {
                    if (ComparisonValue == "0")
                    {
                        radioBaseTextureLODOff.Checked = true;
                    }
                    else
                    {
                        radioBaseTextureLODOn.Checked = true;
                    }
                }
                else if (Element == "CarEnvironmentMapEnable")
                {
                    if (ComparisonValue == "0")
                    {
                        radioCarDetailLODOff.Checked = true;
                    }
                    else
                    {
                        radioCarDetailLODOn.Checked = true;
                    }
                }
                else if (Element == "MaxSkidMarks")
                {
                    if (ComparisonValue == "0")
                    {
                        radioMaxSkidMarksZero.Checked = true;
                    }
                    else if (ComparisonValue == "1")
                    {
                        radioMaxSkidMarksOne.Checked = true;
                    }
                    else
                    {
                        radioMaxSkidMarksTwo.Checked = true;
                    }
                }
                else if (Element == "RoadTextureLODBias")
                {
                    if (ComparisonValue == "0")
                    {
                        radioRoadLODBiasOff.Checked = true;
                    }
                    else
                    {
                        radioRoadLODBiasOn.Checked = true;
                    }
                }
                else if (Element == "MotionBlurEnable")
                {
                    if (ComparisonValue == "0")
                    {
                        radioMotionBlurOff.Checked = true;
                    }
                    else
                    {
                        radioMotionBlurOn.Checked = true;
                    }
                }
                else if (Element == "OverBrightEnable")
                {
                    if (ComparisonValue == "0")
                    {
                        radioOverBrightOff.Checked = true;
                    }
                    else
                    {
                        radioOverBrightOn.Checked = true;
                    }
                }
                else if (Element == "ParticleSystemEnable")
                {
                    if (ComparisonValue == "0")
                    {
                        radioParticleSysOff.Checked = true;
                    }
                    else
                    {
                        radioParticleSysOn.Checked = true;
                    }
                }
                else if (Element == "VisualTreatment")
                {
                    if (ComparisonValue == "0")
                    {
                        radioVisualTreatOff.Checked = true;
                    }
                    else
                    {
                        radioVisualTreatOn.Checked = true;
                    }
                }
                else if (Element == "WaterSimEnable")
                {
                    if (ComparisonValue == "0")
                    {
                        radioWaterSimulationOff.Checked = true;
                    }
                    else
                    {
                        radioWaterSimulationOn.Checked = true;
                    }
                }
                else if (Element == "PostProcessingEnable")
                {
                    if (ComparisonValue == "0")
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
                    Log.Error("USXE: Unknown Function Call [Element: '" + Element + "' ComparisonValue: '" + ComparisonValue + "']");
                }
            }
            catch (Exception Error)
            {
                Log.Error("USXE: " + Error.Message);
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

            /********************************/
            /* Set Hardcoded Text and Values /
            /********************************/

            labelLauncherVersion.Text = "Version: v" + Application.ProductVersion;
            labelOverRideAspect.Text = FileGameSettingsData.PostProcessingEnable;
            AmountofCenterTimes = 0;

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
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Bold);
            labelVideoOptions.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
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
            labelPerfLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelResolution.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelBrightness.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelWindowed.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelEnableAero.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelVSync.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelPixelAspect.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelOverRideAspect.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelAudioMode.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelAudioQuality.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelMasterVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelSFXVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelCarVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelSpeechVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGameMusicVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelFEMusicVol.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGPOCamera.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGPOTrans.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGPODamage.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGPOUnits.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelFSAA.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelMotionBlur.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelOverbright.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelPostProc.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelPartSys.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelShadowDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelShaderDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelVisTreat.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelWaterSimulation.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelGlobalDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelSkidMarks.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelRoadReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelRoadTexture.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelRoadLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelRoadAniso.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelCarEnvMap.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelCDLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelFilterLvl.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelBTLODBias.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelAntialiasing.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            labelLauncherVersion.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            /* Radio Buttons */
            radioWindowedOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioWindowedOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioAeroOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioAeroOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioVSyncOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioVSyncOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioAQLow.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioAQHigh.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioDamageOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioDamageOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioKmH.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMpH.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMotionBlurOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMotionBlurOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioOverBrightOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioOverBrightOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioPostProcOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioPostProcOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioParticleSysOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioParticleSysOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioVisualTreatOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioVisualTreatOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioWaterSimulationOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioWaterSimulationOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMaxSkidMarksZero.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMaxSkidMarksOne.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioMaxSkidMarksTwo.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioRoadLODBiasOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioRoadLODBiasOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioCarDetailLODOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioCarDetailLODOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioBaseTextureLODOn.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            radioBaseTextureLODOff.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
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
            comboBoxPerformanceLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboAudioMode.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxCamera.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxTransmisson.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxShaderFSAA.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxShadowDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxShaderDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxWorldGlobalDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxWorldRoadReflection.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxWorldRoadTexture.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxWorldRoadAniso.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxCarEnvironmentDetail.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxBaseTextureFilter.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);
            comboBoxAnisotropicLevel.Font = new Font(DejaVuSansBold, SecondaryFontSize, FontStyle.Bold);

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
            labelRoadAniso.ForeColor = Theming.MainTextForeColor;
            labelCarEnvMap.ForeColor = Theming.MainTextForeColor;
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
            radioKmH.ForeColor = Theming.MainTextForeColor;
            radioMpH.ForeColor = Theming.MainTextForeColor;
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
            comboResolutions.ForeColor = Theming.CDNMenuTextForeColor;
            comboResolutions.BackColor = Theming.CDNMenuBGForeColor;
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
            comboBoxWorldRoadAniso.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxWorldRoadAniso.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxCarEnvironmentDetail.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxCarEnvironmentDetail.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxBaseTextureFilter.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxBaseTextureFilter.BackColor = Theming.CDNMenuBGForeColor;
            comboBoxAnisotropicLevel.ForeColor = Theming.CDNMenuTextForeColor;
            comboBoxAnisotropicLevel.BackColor = Theming.CDNMenuBGForeColor;

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
                new { Sound = "Stereo", Value = "1" },
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
                new { AnisotropicLevel = "2x", Value = "2" },
                new { AnisotropicLevel = "4x", Value = "4" },
                new { AnisotropicLevel = "8x", Value = "8" },
                new { AnisotropicLevel = "16x", Value = "16" }
            };
            comboBoxAnisotropicLevel.DisplayMember = "AnisotropicLevel";
            comboBoxAnisotropicLevel.ValueMember = "Value";
            comboBoxAnisotropicLevel.DataSource = AnisotropicLevelList;

            /* CarEnvironmentDetail ComboBox */
            var CarEnvironmentDetailList = new[] {
                new { CarEnvironmentDetail = "Minimum", Value = "0" },
                new { CarEnvironmentDetail = "Low", Value = "1" },
                new { CarEnvironmentDetail = "Medium", Value = "2" },
                new { CarEnvironmentDetail = "High", Value = "3" },
                new { CarEnvironmentDetail = "Maximum", Value = "4" }
            };
            comboBoxCarEnvironmentDetail.DisplayMember = "CarEnvironmentDetail";
            comboBoxCarEnvironmentDetail.ValueMember = "Value";
            comboBoxCarEnvironmentDetail.DataSource = CarEnvironmentDetailList;

            /* WorldGlobalDetail ComboBox */
            var WorldGlobalDetailList = new[] {
                new { WorldGlobalDetail = "Minimum", Value = "0" },
                new { WorldGlobalDetail = "Low", Value = "1" },
                new { WorldGlobalDetail = "Medium", Value = "2" },
                new { WorldGlobalDetail = "High", Value = "3" },
                new { WorldGlobalDetail = "Maximum", Value = "4" }
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

            /* WorldRoadAniso ComboBox */
            var WorldRoadAnisoList = new[] {
                new { WorldRoadAniso = "None", Value = "0" },
                new { WorldRoadAniso = "2x", Value = "2" },
                new { WorldRoadAniso = "4x", Value = "4" },
                new { WorldRoadAniso = "8x", Value = "8" },
                new { WorldRoadAniso = "16x", Value = "16" }
            };
            comboBoxWorldRoadAniso.DisplayMember = "WorldRoadAniso";
            comboBoxWorldRoadAniso.ValueMember = "Value";
            comboBoxWorldRoadAniso.DataSource = WorldRoadAnisoList;

            /* ShaderFSAA ComboBox */
            var ShaderFSAAList = new[] {
                new { ShaderFSAA = "Off", Value = "0" },
                new { ShaderFSAA = "2x", Value = "2" },
                new { ShaderFSAA = "4x", Value = "4" }
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
                new { ShaderDetail = "High", Value = "4" }
            };
            comboBoxShaderDetail.DisplayMember = "ShaderDetail";
            comboBoxShaderDetail.ValueMember = "Value";
            comboBoxShaderDetail.DataSource = ShaderDetailList;

            try
            {
                ResolutionsListLoaded = true;
                comboResolutions.DisplayMember = "Resolution";
                comboResolutions.DataSource = ResolutionsListUpdater.List;
            }
            catch (Exception Error)
            {
                Log.Error("Resolution: " + Error.Message);
            }

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

            comboBoxPerformanceLevel.SelectedIndexChanged += new EventHandler(comboBoxPerformanceLevel_SelectedIndexChanged);

            /*********************************************************************/
            /* Set Drop Down, Radio, Input Boxes, and Set Window Size and Postion /
            /*********************************************************************/

            /* Bugfix: Set the Size Ahead of Time and it will change after setting the PerformanceLevel Index */
            Size = new Size(290, 842);

            if (ResolutionsListLoaded == true)
            {
                comboResolutions.Visible = true;
            }
            else
            {
                comboResolutions.Visible = false;
            }

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
                radioDamageOff.Checked = true;
            }
            else
            {
                radioDamageOn.Checked = true;
            }

            if (FileGameSettingsData.SpeedUnits == "0")
            {
                radioKmH.Checked = true;
            }
            else
            {
                radioMpH.Checked = true;
            }

            string SavedResolution = FileGameSettingsData.ScreenWidth + "x" + FileGameSettingsData.ScreenHeight;
            if (!string.IsNullOrWhiteSpace(SavedResolution))
            {
                try
                {
                    if (ResolutionsListUpdater.List.FindIndex(i => string.Equals(i.Resolution, SavedResolution)) != 0)
                    {
                        int Index = ResolutionsListUpdater.List.FindIndex(i => string.Equals(i.Resolution, SavedResolution));

                        if (Index >= 0)
                        {
                            comboResolutions.SelectedIndex = Index;
                        }
                    }
                    else
                    {
                        comboResolutions.SelectedIndex = 1;
                    }
                }
                catch (Exception Error)
                {
                    Log.Error("USXE Resolution" + Error.Message);
                }
            }
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

        private void labelLauncherVersion_Click(object sender, EventArgs e)
        {
            DialogResult Alert = MessageBox.Show(null, "Do you want my Super Honk?", "FunkyWacky", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

            if (Alert == DialogResult.No)
            {
                MessageBox.Show(null, "**Sad Turbo Noises**", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Alert == DialogResult.Yes)
            {
                Process.Start("https://www.youtube.com/watch?v=2aL6D8tj2wk");
            }
            else
            {
                Log.Error("USXE: User Broke the Honk!");
            }
        }
    }
}