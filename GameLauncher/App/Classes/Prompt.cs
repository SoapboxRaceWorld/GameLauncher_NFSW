using System.Windows.Forms;

namespace GameLauncher.App.Classes
{
    public static class Prompt {
        public static string ShowDialog(string text, string caption) {
            Form prompt = new Form() {
                Width = 415,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false
            };

            Label textLabel = new Label() { Left = 12, Top = 9, Text = text, AutoSize = false, Size = new System.Drawing.Size(375, 68) };
            TextBox textBox = new TextBox() { Left = 12, Top = 79, Width = 375 };
            Button confirmation = new Button() { Text = "OK", Left = 231, Width = 75, Top = 105, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancel", Left = 312, Width = 75, Top = 105, DialogResult = DialogResult.Cancel };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
