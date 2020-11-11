using GameLauncher.App.Classes;
using GameLauncherReborn;
using System;
using System.Collections.Generic;
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
                patchContainerPanel.Visible = false;
                MessageBox.Show("The launcher was unable to retrieve about info from the server!");
            }

            Label[] patchTitleObjects = { patchTitle1, patchTitle2, patchTitle3 };
            Label[] patchTextObjects = { patchText1, patchText2, patchText3 };

            for (int i = 0; i < patchNoteBlocks.Count; i++)
            {
                patchTitleObjects[i].Text = patchNoteBlocks[i].Title;
                patchTextObjects[i].Text = patchNoteBlocks[i].Text;
            }
        }

        public void OnClickButton(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            switch (button.Name)
            {
                case nameof(patchButton1):
                    Process.Start(patchNoteBlocks[0].Link);
                    break;
                case nameof(patchButton2):
                    Process.Start(patchNoteBlocks[1].Link);
                    break;
                case nameof(patchButton3):
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
