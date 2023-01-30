using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.LauncherCore.Visuals;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
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
        private static string AboutXMLRevision { get; set; } = "2.1.10.A";
        private static string AboutXML { get; set; } = "/Launcher/SBRW/Official/" + AboutXMLRevision + "/about.xml";

        public static void OpenScreen()
        {
            if (Application.OpenForms["Screen_About"] != null || IsAboutOpen)
            {
                Application.OpenForms["Screen_About"]?.Activate();
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
            SetVisuals();
            this.Closing += (x, y) =>
            {
                PatchNoteBlocks = new List<AboutNoteBlock>();
                IsAboutOpen = false;
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }

        private void SetVisuals()
        {
            Icon = FormsIcon.Retrive_Icon();

            /*******************************/
            /* Set Font                     /
            /*******************************/

#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 10f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = 15f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = 26f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 10f;
            float SecondaryFontSize = 15f;
            float ThirdFontSize = 26f;
#endif

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
#if NETFRAMEWORK
                            Process.Start(PatchNoteBlocks[0].Link);
#else
                            Process.Start(new ProcessStartInfo { FileName = PatchNoteBlocks[0].Link, UseShellExecute = true });
#endif
                        }
                        break;
                    case nameof(PatchButton2):
                        if (!string.IsNullOrWhiteSpace(PatchNoteBlocks[1].Link))
                        {
#if NETFRAMEWORK
                            Process.Start(PatchNoteBlocks[1].Link);
#else
                            Process.Start(new ProcessStartInfo { FileName = PatchNoteBlocks[1].Link, UseShellExecute = true });
#endif
                        }
                        break;
                    case nameof(PatchButton3):
                        if (!string.IsNullOrWhiteSpace(PatchNoteBlocks[2].Link))
                        {
#if NETFRAMEWORK
                            Process.Start(PatchNoteBlocks[2].Link);
#else
                            Process.Start(new ProcessStartInfo { FileName = PatchNoteBlocks[2].Link, UseShellExecute = true });
#endif
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                            block.Title = node.ChildNodes[i].InnerText;
                                            break;
                                        case 1:
                                            block.Text = node.ChildNodes[i].InnerText.Replace("\\n", Environment.NewLine.ToString());
                                            break;
                                        case 2:
                                            block.Link = node.ChildNodes[i].InnerText;
                                            break;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
