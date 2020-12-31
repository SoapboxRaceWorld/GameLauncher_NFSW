namespace GameLauncher.App
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
            this.InvalidProgressText = new System.Windows.Forms.Label();
            this.InvalidProgressBar = new System.Windows.Forms.ProgressBar();
            this.StartScanner = new System.Windows.Forms.Button();
            this.StopScanner = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ScanProgressBar
            // 
            this.ScanProgressBar.Location = new System.Drawing.Point(13, 100);
            this.ScanProgressBar.Name = "ScanProgressBar";
            this.ScanProgressBar.Size = new System.Drawing.Size(384, 23);
            this.ScanProgressBar.TabIndex = 0;
            // 
            // ScanProgressText
            // 
            this.ScanProgressText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanProgressText.Location = new System.Drawing.Point(13, 80);
            this.ScanProgressText.Name = "ScanProgressText";
            this.ScanProgressText.Size = new System.Drawing.Size(384, 18);
            this.ScanProgressText.TabIndex = 2;
            this.ScanProgressText.Text = "Scanning: I shall call this ... T(oUpper)im";
            // 
            // InvalidProgressText
            // 
            this.InvalidProgressText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvalidProgressText.Location = new System.Drawing.Point(13, 160);
            this.InvalidProgressText.Name = "InvalidProgressText";
            this.InvalidProgressText.Size = new System.Drawing.Size(384, 18);
            this.InvalidProgressText.TabIndex = 7;
            this.InvalidProgressText.Text = "Re-Downloading: I shall call this ... T(oLower)im";
            // 
            // InvalidProgressBar
            // 
            this.InvalidProgressBar.Location = new System.Drawing.Point(13, 180);
            this.InvalidProgressBar.Name = "InvalidProgressBar";
            this.InvalidProgressBar.Size = new System.Drawing.Size(384, 23);
            this.InvalidProgressBar.TabIndex = 6;
            // 
            // StartScanner
            // 
            this.StartScanner.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.StartScanner.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.StartScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartScanner.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.StartScanner.Location = new System.Drawing.Point(159, 233);
            this.StartScanner.Name = "StartScanner";
            this.StartScanner.Size = new System.Drawing.Size(91, 24);
            this.StartScanner.TabIndex = 3;
            this.StartScanner.Text = "Start Scan";
            this.StartScanner.UseVisualStyleBackColor = true;
            this.StartScanner.Click += new System.EventHandler(this.StartScanner_Click);
            // 
            // StopScanner
            // 
            this.StopScanner.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.StopScanner.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.StopScanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopScanner.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopScanner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.StopScanner.Location = new System.Drawing.Point(159, 256);
            this.StopScanner.Name = "StopScanner";
            this.StopScanner.Size = new System.Drawing.Size(91, 24);
            this.StopScanner.TabIndex = 4;
            this.StopScanner.Text = "Stop Scan";
            this.StopScanner.UseVisualStyleBackColor = true;
            this.StopScanner.Visible = false;
            this.StopScanner.Click += new System.EventHandler(this.StopScanner_Click);
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.VersionLabel.Location = new System.Drawing.Point(13, 375);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(107, 14);
            this.VersionLabel.TabIndex = 5;
            this.VersionLabel.Text = "Version: vX.X.X.X";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VerifyHash
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(410, 391);
            this.Controls.Add(this.ScanProgressText);
            this.Controls.Add(this.ScanProgressBar);
            this.Controls.Add(this.InvalidProgressText);
            this.Controls.Add(this.InvalidProgressBar);
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
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VerifyHash";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VerifyHash_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ScanProgressText;
        private System.Windows.Forms.ProgressBar ScanProgressBar;
        private System.Windows.Forms.Label InvalidProgressText;
        private System.Windows.Forms.ProgressBar InvalidProgressBar;
        private System.Windows.Forms.Button StartScanner;
        private System.Windows.Forms.Button StopScanner;
        private System.Windows.Forms.Label VersionLabel;
    }
}