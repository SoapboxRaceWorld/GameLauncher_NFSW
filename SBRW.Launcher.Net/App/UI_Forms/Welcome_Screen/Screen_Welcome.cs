using SBRW.Launcher.App.Classes.LauncherCore.APICheckers;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
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
                GC.Collect();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Load CDN List                /
            /*******************************/

            Log.Checking("API: Test #3");
            /* Check If Launcher Failed to Connect to any APIs */
            if (!VisualsAPIChecker.CarbonAPITwo())
            {
                MessageBox.Show(null, "Unable to Connect to any CDN List API. Please check your connection." +
                "\n\nCDN Dropdown List will not be available on Welcome Screen",
                "GameLauncher has Paused, Failed To Connect to any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            Log.Checking("API: Test #3 Done");

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

            Label_Version.Text = "Version: v" + Application.ProductVersion;

            if (UnixOS.Detected())
            {
                Button_Save.Text = "Save Settings and Game Language";
            }

            /*******************************/
            /* Set Font                     /
            /*******************************/

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = UnixOS.Detected() ? 10f : 10f * 96f / CreateGraphics().DpiY;
            float FourthFontSize = UnixOS.Detected() ? 14f : 14f * 96f / CreateGraphics().DpiY;

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Label_Introduction.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            Label_CDN_Source.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Game_Language.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            ComboBox_Game_Language.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_Save.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_CDN_Status_List.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Button_CDN_Sources.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Version.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            BackColor = Color_Winform.BG_Fore_Color;
            ForeColor = Color_Winform.Text_Fore_Color;

            Label_CDN_Status_List.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Introduction.ForeColor = Color_Winform.Secondary_Text_Fore_Color;

            Label_CDN_Source.ForeColor = Color_Winform.Text_Fore_Color;
            Label_Game_Language.ForeColor = Color_Winform.Text_Fore_Color;

            Button_CDN_Sources.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_CDN_Sources.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_CDN_Sources.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_CDN_Sources.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Button_Save.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            Button_Save.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            Button_Save.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            Button_Save.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Label_Version.ForeColor = Color_Winform.Text_Fore_Color;

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
                        Label_CDN_Status_List.Text = "API Lists Connection - Error";
                        StatusCheck = true;
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
            }

            Button_Save.Visible = true;
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
