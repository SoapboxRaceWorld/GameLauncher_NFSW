using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher.App.UI_Forms.About_Screen
{
    public partial class About : Form
    {
        private static bool IsAboutOpen = false;
        private readonly List<AboutNoteBlock> patchNoteBlocks = new List<AboutNoteBlock>();
        private static readonly string AboutXMLRevision = "2.1.8.A";
        private static readonly string AboutXML = "/Launcher/SBRW/Official/" + AboutXMLRevision + "/about.xml";

        public static void OpenScreen()
        {
            if (Application.OpenForms["About"] != null || IsAboutOpen)
            {
                if (Application.OpenForms["About"] != null) { Application.OpenForms["About"].Activate(); }
            }
            else
            {
                try { new About().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "About Screen Encountered an Error";
                    LogToFileAddons.OpenLog("About Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public About()
        {
            IsAboutOpen = true;
            InitializeComponent();
            SetVisuals();
            this.Closing += (x, y) =>
            {
                IsAboutOpen = false;
                GC.Collect();
            };
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            float MainFontSize = UnixOS.Detected() ? 10f : 10f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 15f : 15f * 96f / CreateGraphics().DpiY;
            float ThirdFontSize = UnixOS.Detected() ? 26f : 26f * 96f / CreateGraphics().DpiY;

            Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            AboutText.Font = new Font(DejaVuSansBold, ThirdFontSize, FontStyle.Bold);
            PatchTitle1.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            PatchText1.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            PatchButton1.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            PatchTitle2.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            PatchText2.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            PatchButton2.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            PatchTitle3.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);
            PatchText3.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            PatchButton3.Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);

            /********************************/
            /* Set Theme Colors              /
            /********************************/

            AboutText.ForeColor = Theming.WinFormTextForeColor;

            BackColor = Theming.WinFormTBGForeColor;
            ForeColor = Theming.WinFormTextForeColor;

            PatchContainerPanel.BackColor = Theming.WinFormTBGForeColor;
            PatchContainerPanel.ForeColor = Theming.WinFormTextForeColor;

            PatchTitle1.BackColor = Theming.AboutBGForeColor;
            PatchTitle1.ForeColor = Theming.WinFormTextForeColor;

            PatchTitle2.BackColor = Theming.AboutBGForeColor;
            PatchTitle2.ForeColor = Theming.WinFormTextForeColor;

            PatchTitle3.BackColor = Theming.AboutBGForeColor;
            PatchTitle3.ForeColor = Theming.WinFormTextForeColor;

            PatchText1.BackColor = Theming.AboutBGForeColor;
            PatchText1.ForeColor = Theming.AboutTextForeColor;

            PatchText2.BackColor = Theming.AboutBGForeColor;
            PatchText2.ForeColor = Theming.AboutTextForeColor;

            PatchText3.BackColor = Theming.AboutBGForeColor;
            PatchText3.ForeColor = Theming.AboutTextForeColor;

            PatchButton1.ForeColor = Theming.BlueForeColorButton;
            PatchButton1.BackColor = Theming.BlueBackColorButton;
            PatchButton1.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            PatchButton1.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            PatchButton2.ForeColor = Theming.BlueForeColorButton;
            PatchButton2.BackColor = Theming.BlueBackColorButton;
            PatchButton2.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            PatchButton2.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

            PatchButton3.ForeColor = Theming.BlueForeColorButton;
            PatchButton3.BackColor = Theming.BlueBackColorButton;
            PatchButton3.FlatAppearance.BorderColor = Theming.BlueBorderColorButton;
            PatchButton3.FlatAppearance.MouseOverBackColor = Theming.BlueMouseOverBackColorButton;

        }

        private void FetchPatchNotes()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(URLs.Static_Alt + AboutXML);

                foreach (XmlNode node in doc.DocumentElement)
                {
                    AboutNoteBlock block = new AboutNoteBlock();
                    for (int i = 0; i < node.ChildNodes.Count; i++)
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
                    patchNoteBlocks.Add(block);
                }
            }
            catch
            {
                PatchContainerPanel.Visible = false;
                MessageBox.Show("The launcher was unable to retrieve 'About' info from the server!");
            }

            Label[] PatchTitleObjects = { PatchTitle1, PatchTitle2, PatchTitle3 };
            Label[] PatchTextObjects = { PatchText1, PatchText2, PatchText3 };

            for (int i = 0; i < patchNoteBlocks.Count; i++)
            {
                PatchTitleObjects[i].Text = patchNoteBlocks[i].Title;
                PatchTextObjects[i].Text = patchNoteBlocks[i].Text;
            }
        }

        public void OnClickButton(object sender, EventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                switch (button.Name)
                {
                    case nameof(PatchButton1):
                        Process.Start(patchNoteBlocks[0].Link);
                        break;
                    case nameof(PatchButton2):
                        Process.Start(patchNoteBlocks[1].Link);
                        break;
                    case nameof(PatchButton3):
                        Process.Start(patchNoteBlocks[2].Link);
                        break;
                }
            }
            catch { }
        }

        private void PatchNotes_Load(object sender, System.EventArgs e)
        {
            FetchPatchNotes();
        }
    }
}
