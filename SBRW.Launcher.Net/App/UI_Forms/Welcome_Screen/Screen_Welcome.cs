using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.App.UI_Forms.Selection_CDN_Screen;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extra.File_;
using SBRW.Launcher.Core.Extra.Ini_;
using SBRW.Launcher.Core.Extra.XML_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.Core.Cache;

namespace SBRW.Launcher.App.UI_Forms.Welcome_Screen
{
    public partial class Screen_Welcome : Form
    {
#pragma warning disable CS8618
        public static Screen_Welcome Screen_Instance { get; set; }
#pragma warning restore CS8618
        private bool StatusCheck { get; set; }

        public Screen_Welcome()
        {
            InitializeComponent();
            Icon = FormsIcon.Retrive_Icon();
            SetVisuals();
            this.Closing += (x, CloseForm) =>
            {
                /* This is for Mono Support */
                if (ToolTip_Hover.Active)
                {
                    ToolTip_Hover.RemoveAll();
                    ToolTip_Hover.Dispose();
                }

#if !(RELEASE_UNIX || DEBUG_UNIX)
                GC.Collect();
#endif
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Load CDN List                /
            /*******************************/

            /* Check If Launcher Failed to Connect to any APIs */
            if (!VisualsAPIChecker.Local_Cached_API())
            {
                MessageBox.Show(null, "Unable to Connect to any CDN List API. Please check your connection." +
                "\n\nCDN Selection List will not be available",
                "SBRW Launcher Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            /*******************************/
            /* Load CDN List                /
            /*******************************/

            if (!CDNListUpdater.LoadedList)
            {
                CDNListUpdater.GetList();
            }

            /*******************************/
            /* Set Hardcoded Text           /
            /*******************************/

            Label_Version.Text = "Version: " + Application.ProductVersion;
            bool Is_Game_Path_Empty = string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Game_Path);
            if (!Is_Game_Path_Empty)
            {
                Button_Save.Text = "Save Settings";
            }
            
            CheckBox_LZMA_Downloader.Checked = InformationCache.EnableLZMADownloader();
            CheckBox_Alt_WebCalls.Checked = InformationCache.EnableAltWebCalls();

            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = 10f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
            float ThirdFontSize = 10f;
#endif

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Alt_WebCalls.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_LZMA_Downloader.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Introduction.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            Label_CDN_Source.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Game_Language.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            ComboBox_Game_Language.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Save.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_CDN_Status_List.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_CDN_Sources.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Version.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            NumericUpDown_WebClient_Timeout.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_WebClient_Timeout.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Download_Method.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_WebClient_Settings.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Color_Winform.BG_Fore_Color;
            ForeColor = Color_Winform.Text_Fore_Color;

            Label_CDN_Status_List.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Introduction.ForeColor = Color_Winform.Secondary_Text_Fore_Color;
            Label_CDN_Source.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Game_Language.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Version.ForeColor = Color_Winform.Text_Fore_Color;
            Label_WebClient_Timeout.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Download_Method.ForeColor = Color_Winform.Text_Fore_Color;
            Label_WebClient_Settings.ForeColor = Color_Winform.Text_Fore_Color;

            Button_CDN_Sources.ForeColor = !Is_Game_Path_Empty ? Color_Winform_Buttons.Yellow_Fore_Color :
                Color_Winform_Buttons.Blue_Fore_Color;
            Button_CDN_Sources.BackColor = !Is_Game_Path_Empty ? Color_Winform_Buttons.Yellow_Back_Color :
                Color_Winform_Buttons.Blue_Back_Color;
            Button_CDN_Sources.FlatAppearance.BorderColor = !Is_Game_Path_Empty ? Color_Winform_Buttons.Yellow_Border_Color :
                Color_Winform_Buttons.Blue_Border_Color;
            Button_CDN_Sources.FlatAppearance.MouseOverBackColor = !Is_Game_Path_Empty ? Color_Winform_Buttons.Yellow_Mouse_Over_Back_Color :
                Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Save.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Save.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Save.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Save.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            CheckBox_LZMA_Downloader.ForeColor = Color_Winform_Other.CheckBoxes_Settings;
            CheckBox_Alt_WebCalls.ForeColor = Color_Winform_Other.CheckBoxes_Settings;

            /* Input Boxes */
            NumericUpDown_WebClient_Timeout.ForeColor = Color_Winform_Other.DropMenu_Text_ForeColor;
            NumericUpDown_WebClient_Timeout.BackColor = Color_Winform_Other.DropMenu_Background_ForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            ComboBox_Game_Language.DrawItem += new DrawItemEventHandler(GameLangSource_DrawItem);
            ComboBox_Game_Language.SelectedIndexChanged += new EventHandler(GameLangSource_SelectedIndexChanged);

            Load += new EventHandler(WelcomeScreen_Load);
            Shown += new EventHandler(WelcomeScreen_Shown);
            Button_Save.Click += new EventHandler(Save_Click);
            Button_CDN_Sources.Click += new EventHandler(Button_CDN_Selection_Click);
            Button_Save.DialogResult = DialogResult.OK;

            Screen_Instance = this;

            ToolTip_Hover.SetToolTip(Button_CDN_Sources, "Download Location for Fetching the base GameFiles\n" +
                "Can also be a Soruce for VerifyHash to get replacement files");
            ToolTip_Hover.SetToolTip(ComboBox_Game_Language, "Controls the In-Game Lanuguage setting\n" +
                "This also includes setting the Default Chat joined In-Game");
            ToolTip_Hover.SetToolTip(CheckBox_Alt_WebCalls, "Changes the internal method used by Launcher for Communications\n" +
            "Unchecked: Uses \'standard\' WebClient calls\n" +
            "Checked: Uses WebClientWithTimeout");
            ToolTip_Hover.SetToolTip(CheckBox_LZMA_Downloader, "Setting for LZMA Downloader:\n" +
                "If Checked, this enables the old LZMA Downloader\n" +
                "If Unchecked, enables the new SBRW Pack Downloader");
        }

        private void GameLangSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (((Json_List_Language)ComboBox_Game_Language.SelectedItem).IsSpecial)
                {
                    ComboBox_Game_Language.SelectedIndex = 1;
                    return;
                }
            }
            catch { }
        }

        private void CheckListStatus()
        {
            Label_CDN_Status_List.Text = "United List - Online";

            if (!VisualsAPIChecker.UnitedAPI())
            {
                Label_CDN_Status_List.Text = "Carbon List - Online";

                if (!VisualsAPIChecker.CarbonAPI())
                {
                    Label_CDN_Status_List.Text = "Carbon 2nd List - Online";
                        
                    if (!VisualsAPIChecker.CarbonAPITwo())
                    {
                        Label_CDN_Status_List.Text = "Local Cache List - Offline";

                        if (!VisualsAPIChecker.Local_Cached_API())
                        {
                            Label_CDN_Status_List.Text = "API Lists Connection - Error";
                            StatusCheck = true;
                        }
                    }
                }
            }

            Button_Save.Visible = false;
            Label_Introduction.Text = "Howdy!\n" +
                    "Looks like this is the first time this launcher has been started.\n" +
                    "Please select from the options below in order to continue this setup.";

            if (StatusCheck)
            {
                Button_CDN_Sources.Text = "Fail Safe CDN";
            }
        }

        private void WelcomeScreen_Load(object sender, EventArgs e)
        {
            ComboBox_Game_Language.DisplayMember = "Name";
            ComboBox_Game_Language.DataSource = LanguageListUpdater.CleanList;
        }

        private void WelcomeScreen_Shown(object sender, EventArgs e)
        {
            Application.OpenForms[this.Name].Activate();
            this.BringToFront();

            CheckListStatus();

            /********************************/
            /* Load XML (Only one Section)   /
            /********************************/

            XML_File.Read(1);
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(((Json_List_Language)ComboBox_Game_Language.SelectedItem).Value_Ini))
            {
                Save_Settings.Live_Data.Launcher_Language = ((Json_List_Language)ComboBox_Game_Language.SelectedItem).Value_Ini;
                XML_File.XML_Settings_Data.Language = ((Json_List_Language)ComboBox_Game_Language.SelectedItem).Value_XML;
            }

            if (Save_Settings.Live_Data.Launcher_LZMA_Downloader != (CheckBox_LZMA_Downloader.Checked ? "1" : "0"))
            {
                Save_Settings.Live_Data.Launcher_LZMA_Downloader = CheckBox_LZMA_Downloader.Checked ? "1" : "0";
            }

            if (Save_Settings.Live_Data.Launcher_WebCall_TimeOut_Time != NumericUpDown_WebClient_Timeout.Value.ToString())
            {
                Save_Settings.Live_Data.Launcher_WebCall_TimeOut_Time = NumericUpDown_WebClient_Timeout.Value.ToString();

                if (NumericUpDown_WebClient_Timeout.Value > 0)
                {
                    Launcher_Value.Launcher_WebCall_Timeout_Enable = true;
                }
                else
                {
                    Launcher_Value.Launcher_WebCall_Timeout_Enable = false;
                }
            }

            if (Save_Settings.Live_Data.Launcher_WebClient_Method != (CheckBox_Alt_WebCalls.Checked ? "WebClientWithTimeout" : "WebClient"))
            {
                Save_Settings.Live_Data.Launcher_WebClient_Method = CheckBox_Alt_WebCalls.Checked ? "WebClientWithTimeout" : "WebClient";
                Launcher_Value.Launcher_Alternative_Webcalls(Save_Settings.Live_Data.Launcher_WebClient_Method == "WebClient");
            }

            try
            {
                if (((Json_List_Language)ComboBox_Game_Language.SelectedItem).Category == "Custom")
                {
                    /* Create Custom Settings.ini for LangPicker.asi module */
                    if (!Directory.Exists(Save_Settings.Live_Data.Game_Path + "/scripts"))
                    {
                        Directory.CreateDirectory(Save_Settings.Live_Data.Game_Path + "/scripts");
                    }

                    Ini_File LanguagePickerFile = new Ini_File(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini");
                    LanguagePickerFile.Key_Write("Language", ((Json_List_Language)ComboBox_Game_Language.SelectedItem).Value_Ini);
                    MessageBox.Show(null, "Please Note: If a Server does not provide a Language Pack, it will fallback to English instead.",
                        "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (File.Exists(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini"))
                    {
                        File.Delete(Save_Settings.Live_Data.Game_Path + "/scripts/LangPicker.ini");
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WELCOME SCREEN", string.Empty, Error, string.Empty, true);
            }

            QuitWithoutSaving_Click(sender, e);
        }

        private void QuitWithoutSaving_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Button_CDN_Selection_Click(object sender, EventArgs e)
        {
            if (!StatusCheck)
            {
                Screen_CDN_Selection.OpenScreen(1);
            }
            else if (string.IsNullOrWhiteSpace(Save_Settings.Live_Data.Launcher_CDN))
            {
                Save_Settings.Live_Data.Launcher_CDN = "http://localhost";
                Button_Save.Visible = true;
            }
        }

        private void GameLangSource_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                string langListText = string.Empty;

                if (sender is ComboBox cb)
                {
                    if (e.Index != -1 && cb.Items != null)
                    {
                        if (cb.Items[e.Index] is Json_List_Language si)
                        {
                            langListText = si.Name;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(langListText) && sender != null)
                {
                    Font font = ((ComboBox)sender).Font;
                    Brush backgroundColor;
                    Brush textColor;

                    if (langListText.StartsWith("<GROUP>"))
                    {
                        font = new Font(font, FontStyle.Bold);
                        e.Graphics.FillRectangle(new SolidBrush(Color_Winform_Other.DropMenu_Category_Background_ForeColor), e.Bounds);
                        e.Graphics.DrawString(langListText.Replace("<GROUP>", string.Empty), font,
                            new SolidBrush(Color_Winform_Other.DropMenu_Category_Text_ForeColor), e.Bounds);
                    }
                    else
                    {
                        font = new Font(font, FontStyle.Bold);
                        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected && e.State != DrawItemState.ComboBoxEdit)
                        {
                            backgroundColor = SystemBrushes.Highlight;
                            textColor = SystemBrushes.HighlightText;
                        }
                        else
                        {
                            backgroundColor = new SolidBrush(Color_Winform_Other.DropMenu_Background_ForeColor);
                            textColor = new SolidBrush(Color_Winform_Other.DropMenu_Text_ForeColor);
                        }

                        e.Graphics.FillRectangle(backgroundColor, e.Bounds);
                        e.Graphics.DrawString("    " + langListText, font, textColor, e.Bounds);
                    }
                }
            }
            catch { }
        }
    }
}
