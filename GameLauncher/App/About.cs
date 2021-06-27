using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.SystemPlatform.Linux;

namespace GameLauncher.App
{
    public partial class About : Form
    {
        private readonly List<AboutNoteBlock> patchNoteBlocks = new List<AboutNoteBlock>();
        private static readonly string AboutXMLRevision = "2.1.8.A";
        private static readonly string AboutXML = "/Launcher/SBRW/Official/" + AboutXMLRevision + "/ about.xml";

        public About()
        {
            InitializeComponent();
            SetVisuals();
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 10f * 100f / CreateGraphics().DpiY;
            var SecondaryFontSize = 15f * 100f / CreateGraphics().DpiY;
            var ThirdFontSize = 26f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 10f;
                SecondaryFontSize = 15f;
                ThirdFontSize = 26f;
            }

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
                FunctionStatus.TLS();
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
                MessageBox.Show("The launcher was unable to retrieve about info from the server!");
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

        private void PatchNotes_Load(object sender, System.EventArgs e)
        {
            FetchPatchNotes();
        }
    }
}
