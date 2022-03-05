using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.LauncherCore.Support;
using SBRW.Launcher.App.Classes.LauncherCore.Visuals;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace SBRW.Launcher.App.UI_Forms.About_Screen
{
    public partial class Screen_About : Form
    {
        private static XmlDocument? XML_Doc { get; set; }
        private static bool IsAboutOpen { get; set; }
        private static List<AboutNoteBlock> PatchNoteBlocks { get; set; } = new List<AboutNoteBlock>();
        private static string AboutXMLRevision { get; set; } = "2.1.8.A";
        private static string AboutXML { get; set; } = "/Launcher/SBRW/Official/" + AboutXMLRevision + "/about.xml";

        public static void OpenScreen()
        {
            if (Application.OpenForms["Screen_About"] != null || IsAboutOpen)
            {
                if (Application.OpenForms["Screen_About"] != null) { Application.OpenForms["Screen_About"].Activate(); }
            }
            else
            {
                try { new Screen_About().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "About Screen Encountered an Error";
                    LogToFileAddons.OpenLog("About Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_About()
        {
            IsAboutOpen = true;
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location);
            SetVisuals();
            this.Closing += (x, y) =>
            {
                PatchNoteBlocks = new List<AboutNoteBlock>();
                IsAboutOpen = false;
                GC.Collect();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            float MainFontSize = UnixOS.Detected() ? 10f : 10f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 15f : 15f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = UnixOS.Detected() ? 26f : 26f * 96f / CreateGraphics().DpiY;

            Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            AboutText.Font = new Font(FormsFont.Primary_Bold(), ThirdFontSize, FontStyle.Bold);
            PatchTitle1.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            PatchText1.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            PatchButton1.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            PatchTitle2.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            PatchText2.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            PatchButton2.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            PatchTitle3.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);
            PatchText3.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            PatchButton3.Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            AboutText.ForeColor = Color_Winform.Secondary_Text_Fore_Color;

            BackColor = Color_Winform.BG_Fore_Color;
            ForeColor = Color_Winform.Text_Fore_Color;

            PatchContainerPanel.BackColor = Color_Winform.BG_Fore_Color;
            PatchContainerPanel.ForeColor = Color_Winform.Text_Fore_Color;

            PatchTitle1.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchTitle1.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchTitle2.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchTitle2.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchTitle3.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchTitle3.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchText1.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchText1.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchText2.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchText2.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchText3.BackColor = Color_Winform_About.BG_Fore_Color;
            PatchText3.ForeColor = Color_Winform_About.Text_Fore_Color;

            PatchButton1.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            PatchButton1.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            PatchButton1.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            PatchButton1.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            PatchButton2.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            PatchButton2.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            PatchButton2.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            PatchButton2.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            PatchButton3.ForeColor = Color_Winform_Buttons.Blue_Fore_Color;
            PatchButton3.BackColor = Color_Winform_Buttons.Blue_Back_Color;
            PatchButton3.FlatAppearance.BorderColor = Color_Winform_Buttons.Blue_Border_Color;
            PatchButton3.FlatAppearance.MouseOverBackColor = Color_Winform_Buttons.Blue_Mouse_Over_Back_Color;

            Shown += new EventHandler(PatchNotes_Shown);
        }

        public void OnClickButton(object sender, EventArgs e)
        {
            if ((sender != null) && (e != null))
            {
                Button button = (Button)sender;
                switch (button.Name)
                {
                    case nameof(PatchButton1):
                        if (!string.IsNullOrWhiteSpace(PatchNoteBlocks[0].Link))
                        {
                            Process.Start(PatchNoteBlocks[0].Link);
                        }
                        break;
                    case nameof(PatchButton2):
                        if (!string.IsNullOrWhiteSpace(PatchNoteBlocks[1].Link))
                        {
                            Process.Start(PatchNoteBlocks[1].Link);
                        }
                        break;
                    case nameof(PatchButton3):
                        if (!string.IsNullOrWhiteSpace(PatchNoteBlocks[2].Link))
                        {
                            Process.Start(PatchNoteBlocks[2].Link);
                        }
                        break;
                }
            }
        }

        private void PatchNotes_Shown(object sender, EventArgs e)
        {
            try
            {
                FunctionStatus.CenterParent(this);
                Application.OpenForms[this.Name].Activate();
                this.BringToFront();

                if (XML_Doc == null)
                {
                    XML_Doc = new XmlDocument();
                    XML_Doc.Load(URLs.Static_Alt + AboutXML);
                }

                if (XML_Doc.DocumentElement != null && IsAboutOpen)
                {
                    foreach (XmlNode node in XML_Doc.DocumentElement)
                    {
                        if (!(node.ChildNodes.Count < 0))
                        {
                            AboutNoteBlock block = new AboutNoteBlock();

                            for (int i = 0; i < node.ChildNodes.Count; i++)
                            {
                                if (IsAboutOpen)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                            block.Title = node.ChildNodes[i].InnerText;
                                            break;
                                        case 1:
                                            block.Text = node.ChildNodes[i].InnerText;
                                            break;
                                        case 2:
                                            block.Link = node.ChildNodes[i].InnerText;
                                            break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }

                            if (IsAboutOpen)
                            {
                                PatchNoteBlocks.Add(block);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (IsAboutOpen && PatchNoteBlocks.Count >= 0)
                    {
                        Label[] PatchTitleObjects = { PatchTitle1, PatchTitle2, PatchTitle3 };
                        Label[] PatchTextObjects = { PatchText1, PatchText2, PatchText3 };

                        for (int i = 0; i < PatchNoteBlocks.Count; i++)
                        {
                            if (IsAboutOpen)
                            {
                                PatchTitleObjects[i].Text = PatchNoteBlocks[i].Title;
                                PatchTextObjects[i].Text = PatchNoteBlocks[i].Text;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {

                }
            }
            catch
            {
                PatchContainerPanel.Visible = false;
                MessageBox.Show("The launcher was unable to retrieve 'About' info from the server!");
            }
        }
    }
}
