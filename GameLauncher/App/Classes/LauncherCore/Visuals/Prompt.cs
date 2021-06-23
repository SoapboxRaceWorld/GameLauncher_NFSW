using GameLauncher.App.Classes.SystemPlatform.Linux;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption, string Mode)
        {
            Form prompt = new Form()
            {
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

            if (Mode == "GameFiles")
            {
                MessageBox.Show(null, "Due to a Unknown File Browser Dialog Bug Crash on " + DetectLinux.Distro() +
                                    ".The Launcher currently requires you to manually input (type or paste) the Game Files Path", "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var CustomPrompt = prompt.ShowDialog();

            return CustomPrompt == DialogResult.OK ? textBox.Text : string.Empty;
        }
    }
}
