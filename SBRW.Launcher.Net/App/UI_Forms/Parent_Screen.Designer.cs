namespace SBRW.Launcher.App.UI_Forms
{
    partial class Parent_Screen
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
            this.components = new System.ComponentModel.Container();
            this.Panel_Form_Screens = new System.Windows.Forms.Panel();
            this.PictureBox_Screen_Splash = new System.Windows.Forms.PictureBox();
            this.GroupBox_Launcherlog = new System.Windows.Forms.GroupBox();
            this.TextBox_Live_Log = new System.Windows.Forms.TextBox();
            this.Panel_Splash_Screen = new System.Windows.Forms.Panel();
            this.Button_Close = new System.Windows.Forms.PictureBox();
            this.Clock = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Screen_Splash)).BeginInit();
            this.GroupBox_Launcherlog.SuspendLayout();
            this.Panel_Splash_Screen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Button_Close)).BeginInit();
            this.SuspendLayout();
            // 
            // Panel_Form_Screens
            // 
            this.Panel_Form_Screens.BackColor = System.Drawing.Color.Transparent;
            this.Panel_Form_Screens.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Panel_Form_Screens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Form_Screens.Location = new System.Drawing.Point(0, 0);
            this.Panel_Form_Screens.Name = "Panel_Form_Screens";
            this.Panel_Form_Screens.Size = new System.Drawing.Size(563, 404);
            this.Panel_Form_Screens.TabIndex = 0;
            this.Panel_Form_Screens.Visible = false;
            // 
            // PictureBox_Screen_Splash
            // 
            this.PictureBox_Screen_Splash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PictureBox_Screen_Splash.ErrorImage = null;
            this.PictureBox_Screen_Splash.InitialImage = null;
            this.PictureBox_Screen_Splash.Location = new System.Drawing.Point(25, 29);
            this.PictureBox_Screen_Splash.Name = "PictureBox_Screen_Splash";
            this.PictureBox_Screen_Splash.Size = new System.Drawing.Size(512, 181);
            this.PictureBox_Screen_Splash.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox_Screen_Splash.TabIndex = 0;
            this.PictureBox_Screen_Splash.TabStop = false;
            // 
            // GroupBox_Launcherlog
            // 
            this.GroupBox_Launcherlog.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GroupBox_Launcherlog.Controls.Add(this.TextBox_Live_Log);
            this.GroupBox_Launcherlog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GroupBox_Launcherlog.Location = new System.Drawing.Point(25, 216);
            this.GroupBox_Launcherlog.Name = "GroupBox_Launcherlog";
            this.GroupBox_Launcherlog.Size = new System.Drawing.Size(512, 157);
            this.GroupBox_Launcherlog.TabIndex = 28;
            this.GroupBox_Launcherlog.TabStop = false;
            this.GroupBox_Launcherlog.Text = "Log:";
            // 
            // TextBox_Live_Log
            // 
            this.TextBox_Live_Log.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox_Live_Log.Location = new System.Drawing.Point(6, 17);
            this.TextBox_Live_Log.Multiline = true;
            this.TextBox_Live_Log.Name = "TextBox_Live_Log";
            this.TextBox_Live_Log.ReadOnly = true;
            this.TextBox_Live_Log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TextBox_Live_Log.Size = new System.Drawing.Size(500, 135);
            this.TextBox_Live_Log.TabIndex = 35;
            // 
            // Panel_Splash_Screen
            // 
            this.Panel_Splash_Screen.BackColor = System.Drawing.Color.Transparent;
            this.Panel_Splash_Screen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Panel_Splash_Screen.Controls.Add(this.Button_Close);
            this.Panel_Splash_Screen.Controls.Add(this.PictureBox_Screen_Splash);
            this.Panel_Splash_Screen.Controls.Add(this.GroupBox_Launcherlog);
            this.Panel_Splash_Screen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel_Splash_Screen.Location = new System.Drawing.Point(0, 0);
            this.Panel_Splash_Screen.Name = "Panel_Splash_Screen";
            this.Panel_Splash_Screen.Size = new System.Drawing.Size(563, 404);
            this.Panel_Splash_Screen.TabIndex = 29;
            // 
            // Button_Close
            // 
            this.Button_Close.BackColor = System.Drawing.Color.Transparent;
            this.Button_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.Button_Close.InitialImage = null;
            this.Button_Close.Location = new System.Drawing.Point(526, 12);
            this.Button_Close.Name = "Button_Close";
            this.Button_Close.Size = new System.Drawing.Size(25, 25);
            this.Button_Close.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Button_Close.TabIndex = 6;
            this.Button_Close.TabStop = false;
            // 
            // Clock
            // 
            this.Clock.Enabled = true;
            this.Clock.Interval = 1200;
            // 
            // Parent_Screen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(563, 404);
            this.Controls.Add(this.Panel_Form_Screens);
            this.Controls.Add(this.Panel_Splash_Screen);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Parent_Screen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Parent_Screen";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox_Screen_Splash)).EndInit();
            this.GroupBox_Launcherlog.ResumeLayout(false);
            this.GroupBox_Launcherlog.PerformLayout();
            this.Panel_Splash_Screen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Button_Close)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Timer Clock;
        public System.Windows.Forms.TextBox TextBox_Live_Log;
        public System.Windows.Forms.Panel Panel_Form_Screens;
        public System.Windows.Forms.PictureBox PictureBox_Screen_Splash;
        public System.Windows.Forms.GroupBox GroupBox_Launcherlog;
        public System.Windows.Forms.Panel Panel_Splash_Screen;
        public System.Windows.Forms.PictureBox Button_Close;
    }
}