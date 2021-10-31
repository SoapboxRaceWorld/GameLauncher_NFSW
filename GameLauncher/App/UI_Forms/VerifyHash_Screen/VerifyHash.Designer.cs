namespace GameLauncher.App.UI_Forms.VerifyHash_Screen
{
    partial class VerifyHash
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VerifyHash));
            this.ScanProgressBar = new System.Windows.Forms.ProgressBar();
            this.ScanProgressText = new System.Windows.Forms.Label();
            this.DownloadProgressText = new System.Windows.Forms.Label();
            this.DownloadProgressBar = new System.Windows.Forms.ProgressBar();
            this.StartScanner = new System.Windows.Forms.Button();
            this.StopScanner = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.VerifyHashWelcome = new System.Windows.Forms.Label();
            this.VerifyHashText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScanProgressBar
            // 
            this.ScanProgressBar.Location = new System.Drawing.Point(13, 150);
            this.ScanProgressBar.Name = "ScanProgressBar";
            this.ScanProgressBar.Size = new System.Drawing.Size(384, 23);
            this.ScanProgressBar.TabIndex = 0;
            // 
            // ScanProgressText
            // 
            this.ScanProgressText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanProgressText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.ScanProgressText.Location = new System.Drawing.Point(6, 128);
            this.ScanProgressText.Margin = new System.Windows.Forms.Padding(0);
            this.ScanProgressText.Name = "ScanProgressText";
            this.ScanProgressText.Size = new System.Drawing.Size(396, 18);
            this.ScanProgressText.TabIndex = 2;
            this.ScanProgressText.Text = "Scanning Progress:";
            this.ScanProgressText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DownloadProgressText
            // 
            this.DownloadProgressText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadProgressText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.DownloadProgressText.Location = new System.Drawing.Point(1, 176);
            this.DownloadProgressText.Margin = new System.Windows.Forms.Padding(0);
            this.DownloadProgressText.Name = "DownloadProgressText";
            this.DownloadProgressText.Size = new System.Drawing.Size(408, 53);
            this.DownloadProgressText.TabIndex = 7;
            this.DownloadProgressText.Text = "\r\nDownload Progress:";
            this.DownloadProgressText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DownloadProgressBar
            // 
            this.DownloadProgressBar.Location = new System.Drawing.Point(13, 232);
            this.DownloadProgressBar.Name = "DownloadProgressBar";
            this.DownloadProgressBar.Size = new System.Drawing.Size(384, 23);
            this.DownloadProgressBar.TabIndex = 6;
            // 
            // StartScanner
            // 
            this.StartScanner.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.StartScanner.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.StartScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartScanner.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.StartScanner.Location = new System.Drawing.Point(159, 260);
            this.StartScanner.Name = "StartScanner";
            this.StartScanner.Size = new System.Drawing.Size(91, 24);
            this.StartScanner.TabIndex = 3;
            this.StartScanner.Text = "Start Scan";
            this.StartScanner.UseVisualStyleBackColor = true;
            // 
            // StopScanner
            // 
            this.StopScanner.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.StopScanner.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.StopScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopScanner.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.StopScanner.Location = new System.Drawing.Point(159, 260);
            this.StopScanner.Name = "StopScanner";
            this.StopScanner.Size = new System.Drawing.Size(91, 24);
            this.StopScanner.TabIndex = 4;
            this.StopScanner.Text = "Stop Scan";
            this.StopScanner.UseVisualStyleBackColor = true;
            this.StopScanner.Visible = false;
            // 
            // VersionLabel
            // 
            this.VersionLabel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.VersionLabel.Location = new System.Drawing.Point(13, 375);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(164, 14);
            this.VersionLabel.TabIndex = 5;
            this.VersionLabel.Text = "Version: vXX.XX.XX.XXXX";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VerifyHashWelcome
            // 
            this.VerifyHashWelcome.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.VerifyHashWelcome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.VerifyHashWelcome.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VerifyHashWelcome.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.VerifyHashWelcome.Location = new System.Drawing.Point(16, 6);
            this.VerifyHashWelcome.Name = "VerifyHashWelcome";
            this.VerifyHashWelcome.Size = new System.Drawing.Size(381, 106);
            this.VerifyHashWelcome.TabIndex = 8;
            this.VerifyHashWelcome.Text = "Welcome!\r\n\r\nThe scanning process is pretty quick,\r\nbut may still take a while.\r\nD" +
    "epending on your connection,\r\nre-downloading will take the longest.\r\nPlease allo" +
    "w it to complete fully!";
            this.VerifyHashWelcome.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // VerifyHashText
            // 
            this.VerifyHashText.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.VerifyHashText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VerifyHashText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.VerifyHashText.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.VerifyHashText.Location = new System.Drawing.Point(13, 297);
            this.VerifyHashText.Name = "VerifyHashText";
            this.VerifyHashText.Size = new System.Drawing.Size(384, 66);
            this.VerifyHashText.TabIndex = 9;
            this.VerifyHashText.Text = "Please select \"Start Scan\"\r\nTo begin Validating Gamefiles";
            this.VerifyHashText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // VerifyHash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(410, 391);
            this.Controls.Add(this.VerifyHashText);
            this.Controls.Add(this.VerifyHashWelcome);
            this.Controls.Add(this.ScanProgressText);
            this.Controls.Add(this.ScanProgressBar);
            this.Controls.Add(this.DownloadProgressText);
            this.Controls.Add(this.DownloadProgressBar);
            this.Controls.Add(this.StartScanner);
            this.Controls.Add(this.StopScanner);
            this.Controls.Add(this.VersionLabel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VerifyHash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "VerifyHash - SBRW Launcher";
            this.Load += new System.EventHandler(this.VerifyHash_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ScanProgressText;
        private System.Windows.Forms.ProgressBar ScanProgressBar;
        private System.Windows.Forms.Label DownloadProgressText;
        private System.Windows.Forms.ProgressBar DownloadProgressBar;
        private System.Windows.Forms.Button StartScanner;
        private System.Windows.Forms.Button StopScanner;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label VerifyHashWelcome;
        private System.Windows.Forms.Label VerifyHashText;
    }
}