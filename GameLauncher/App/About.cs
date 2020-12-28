using GameLauncher.App.Classes;
using GameLauncherReborn;
using GameLauncher.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher.App
{
    public partial class About : Form
    {
        private List<AboutNoteBlock> patchNoteBlocks = new List<AboutNoteBlock>();

        public About()
        {
            InitializeComponent();
            ApplyEmbeddedFonts();
        }

        private void ApplyEmbeddedFonts()
        {
            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");
            AboutText.Font = new Font(DejaVuSansBold, 26.25f, FontStyle.Bold);
            PatchTitle1.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
            PatchText1.Font = new Font(DejaVuSans, 9.75f, FontStyle.Regular);
            PatchButton1.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
            PatchTitle2.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
            PatchText2.Font = new Font(DejaVuSans, 9.75f, FontStyle.Regular);
            PatchButton2.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
            PatchTitle3.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
            PatchText3.Font = new Font(DejaVuSans, 9.75f, FontStyle.Regular);
            PatchButton3.Font = new Font(DejaVuSans, 15f, FontStyle.Regular);
        }

        private void FetchPatchNotes()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Self.secondstaticapiserver + "/about.xml");

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
