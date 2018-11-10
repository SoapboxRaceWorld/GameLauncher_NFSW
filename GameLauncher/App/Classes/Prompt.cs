using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.App.Classes {
    public static class Prompt {
        public static string ShowDialog(string text, string caption) {
            Form prompt = new Form() {
                Width = 415,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 12, Top = 9, Text = text, AutoSize = false, Size = new System.Drawing.Size(375, 68) };
            TextBox textBox = new TextBox() { Left = 15, Top = 80, Width = 290 };
            Button confirmation = new Button() { Text = "Ok", Left = 312, Width = 75, Top = 78, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
