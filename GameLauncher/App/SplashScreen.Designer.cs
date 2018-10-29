namespace GameLauncher.App
{
    partial class SplashScreen
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loadingBar = new GameLauncherReborn.ProgressBarEx();
            this.SuspendLayout();
            // 
            // loadingBar
            // 
            this.loadingBar.BackColor = System.Drawing.Color.Transparent;
            this.loadingBar.Border = false;
            this.loadingBar.GradiantPosition = GameLauncherReborn.ProgressBarEx.GradiantArea.None;
            this.loadingBar.Image = null;
            this.loadingBar.Location = new System.Drawing.Point(0, 295);
            this.loadingBar.Name = "loadingBar";
            this.loadingBar.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.loadingBar.RoundedCorners = false;
            this.loadingBar.Size = new System.Drawing.Size(300, 5);
            this.loadingBar.Text = "progressBarEx1";
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(19)))), ((int)(((byte)(19)))));
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.loadingBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private GameLauncherReborn.ProgressBarEx loadingBar;
    }
}