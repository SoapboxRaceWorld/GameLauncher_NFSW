using System.Windows.Forms;
namespace GameLauncher.App
{
    partial class WelcomeScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeScreen));
            this.WelcomeText = new System.Windows.Forms.Label();
            this.DownloadSourceText = new System.Windows.Forms.Label();
            this.CDNSource = new System.Windows.Forms.ComboBox();
            this.Save = new System.Windows.Forms.Button();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.CDNStatusText = new System.Windows.Forms.Label();
            this.APIErrorButton = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // WelcomeText
            // 
            this.WelcomeText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomeText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.WelcomeText.Location = new System.Drawing.Point(9, 9);
            this.WelcomeText.Name = "WelcomeText";
            this.WelcomeText.Size = new System.Drawing.Size(357, 69);
            this.WelcomeText.TabIndex = 0;
            this.WelcomeText.Text = "Checking API Status";
            this.WelcomeText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DownloadSourceText
            // 
            this.DownloadSourceText.AutoSize = true;
            this.DownloadSourceText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadSourceText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DownloadSourceText.Location = new System.Drawing.Point(12, 115);
            this.DownloadSourceText.Name = "DownloadSourceText";
            this.DownloadSourceText.Size = new System.Drawing.Size(163, 14);
            this.DownloadSourceText.TabIndex = 2;
            this.DownloadSourceText.Text = "CDN / Download Source:";
            this.DownloadSourceText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CDNSource
            // 
            this.CDNSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.CDNSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CDNSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CDNSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CDNSource.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CDNSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.CDNSource.FormattingEnabled = true;
            this.CDNSource.ItemHeight = 13;
            this.CDNSource.Location = new System.Drawing.Point(164, 114);
            this.CDNSource.MaxDropDownItems = 21;
            this.CDNSource.Name = "CDNSource";
            this.CDNSource.Size = new System.Drawing.Size(190, 19);
            this.CDNSource.TabIndex = 4;
            // 
            // Save
            // 
            this.Save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Save.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.Save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Save.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Save.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Save.Location = new System.Drawing.Point(15, 158);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(339, 32);
            this.Save.TabIndex = 5;
            this.Save.Text = "Save Settings and Select Download / Install Location";
            this.Save.UseVisualStyleBackColor = false;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ServerStatusText.Location = new System.Drawing.Point(12, 78);
            this.ServerStatusText.Name = "ServerStatusText";
            this.ServerStatusText.Size = new System.Drawing.Size(164, 13);
            this.ServerStatusText.TabIndex = 6;
            this.ServerStatusText.Text = "Server API Status - Pinging";
            this.ServerStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CDNStatusText
            // 
            this.CDNStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CDNStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CDNStatusText.Location = new System.Drawing.Point(196, 78);
            this.CDNStatusText.Name = "CDNStatusText";
            this.CDNStatusText.Size = new System.Drawing.Size(164, 13);
            this.CDNStatusText.TabIndex = 7;
            this.CDNStatusText.Text = "CDN API Status - Pinging";
            this.CDNStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // APIErrorButton
            // 
            this.APIErrorButton.AutoSize = true;
            this.APIErrorButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.APIErrorButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.APIErrorButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.APIErrorButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.APIErrorButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.APIErrorButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.APIErrorButton.Location = new System.Drawing.Point(148, 141);
            this.APIErrorButton.Name = "APIErrorButton";
            this.APIErrorButton.Size = new System.Drawing.Size(114, 26);
            this.APIErrorButton.TabIndex = 8;
            this.APIErrorButton.Text = "Manual Bypass";
            this.APIErrorButton.UseVisualStyleBackColor = false;
            this.APIErrorButton.Click += new System.EventHandler(this.APIErrorButton_Click);
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Location = new System.Drawing.Point(17, 190);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(156, 13);
            this.VersionLabel.TabIndex = 9;
            this.VersionLabel.Text = "Version : vX.X.X.X";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // WelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(379, 206);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.APIErrorButton);
            this.Controls.Add(this.CDNStatusText);
            this.Controls.Add(this.ServerStatusText);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.CDNSource);
            this.Controls.Add(this.DownloadSourceText);
            this.Controls.Add(this.WelcomeText);
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label WelcomeText;
        private System.Windows.Forms.Label DownloadSourceText;
        private System.Windows.Forms.ComboBox CDNSource;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Label ServerStatusText;
        private System.Windows.Forms.Label CDNStatusText;
        private System.Windows.Forms.Button APIErrorButton;
        private System.Windows.Forms.Label VersionLabel;
    }
}