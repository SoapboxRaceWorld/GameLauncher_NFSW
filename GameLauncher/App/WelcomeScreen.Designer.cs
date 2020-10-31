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
            this.downloadSourceText = new System.Windows.Forms.Label();
            this.CDNSource = new System.Windows.Forms.ComboBox();
            this.Save = new System.Windows.Forms.Button();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.CDNStatusText = new System.Windows.Forms.Label();
            this.apiErrorButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WelcomeText
            // 
            this.WelcomeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomeText.Location = new System.Drawing.Point(12, 9);
            this.WelcomeText.Name = "WelcomeText";
            this.WelcomeText.Size = new System.Drawing.Size(357, 69);
            this.WelcomeText.TabIndex = 0;
            this.WelcomeText.Text = "Checking API Status";
            this.WelcomeText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // downloadSourceText
            // 
            this.downloadSourceText.AutoSize = true;
            this.downloadSourceText.Location = new System.Drawing.Point(12, 117);
            this.downloadSourceText.Name = "downloadSourceText";
            this.downloadSourceText.Size = new System.Drawing.Size(95, 13);
            this.downloadSourceText.TabIndex = 2;
            this.downloadSourceText.Text = "Download Source:";
            this.downloadSourceText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CDNSource
            // 
            this.CDNSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CDNSource.FormattingEnabled = true;
            this.CDNSource.Location = new System.Drawing.Point(132, 114);
            this.CDNSource.Name = "CDNSource";
            this.CDNSource.Size = new System.Drawing.Size(237, 21);
            this.CDNSource.TabIndex = 4;
            // 
            // Save
            // 
            this.Save.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Save.Location = new System.Drawing.Point(15, 158);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(354, 37);
            this.Save.TabIndex = 5;
            this.Save.Text = "Save Settings and Select Download Location";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.Location = new System.Drawing.Point(12, 78);
            this.ServerStatusText.Name = "ServerStatusText";
            this.ServerStatusText.Size = new System.Drawing.Size(164, 13);
            this.ServerStatusText.TabIndex = 6;
            this.ServerStatusText.Text = "Server API Status - Pinging";
            this.ServerStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CDNStatusText
            // 
            this.CDNStatusText.Location = new System.Drawing.Point(205, 78);
            this.CDNStatusText.Name = "CDNStatusText";
            this.CDNStatusText.Size = new System.Drawing.Size(164, 13);
            this.CDNStatusText.TabIndex = 7;
            this.CDNStatusText.Text = "CDN API Status - Pinging";
            this.CDNStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // apiErrorButton
            // 
            this.apiErrorButton.AutoSize = true;
            this.apiErrorButton.Location = new System.Drawing.Point(152, 141);
            this.apiErrorButton.Name = "apiErrorButton";
            this.apiErrorButton.Size = new System.Drawing.Size(89, 23);
            this.apiErrorButton.TabIndex = 8;
            this.apiErrorButton.Text = "Manual Bypass";
            this.apiErrorButton.UseVisualStyleBackColor = true;
            this.apiErrorButton.Click += new System.EventHandler(this.APIErrorButton_Click);
            // 
            // WelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 206);
            this.Controls.Add(this.apiErrorButton);
            this.Controls.Add(this.CDNStatusText);
            this.Controls.Add(this.ServerStatusText);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.CDNSource);
            this.Controls.Add(this.downloadSourceText);
            this.Controls.Add(this.WelcomeText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeScreen";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label WelcomeText;
        private System.Windows.Forms.Label downloadSourceText;
        private System.Windows.Forms.ComboBox CDNSource;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Label ServerStatusText;
        private System.Windows.Forms.Label CDNStatusText;
        private System.Windows.Forms.Button apiErrorButton;
    }
}