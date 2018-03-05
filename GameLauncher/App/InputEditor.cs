using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameLauncher;

namespace GameLauncher.App {
    public partial class InputEditor : Form {
        string gamePath;

        public InputEditor(string GetGamePath) {
            gamePath = GetGamePath;
            InitializeComponent();
        }

        void errorHandler(string errorText) {
            this.Close();
            MessageBox.Show(errorText);
        }

        private void InputEditor_Load(object sender, EventArgs e) {
            string InputFile = gamePath + "\\Settings\\SystemSettings.dat";

            if (!File.Exists(InputFile)) {
                errorHandler("Please wait until download finishes.");
            }

            BinaryReader n = new BinaryReader(new FileStream(InputFile, FileMode.Open));
            sbyte FileContentSbyte = n.ReadSByte();
            n.Close();
            uint FileContentInt = Convert.ToUInt32(File.ReadAllBytes(InputFile).Count());

            Decryptor.apply_cipher(FileContentSbyte, FileContentInt);

            debug.Text = n.ReadString();

        }
    }
}
