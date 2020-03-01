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
            this.WelcomeText = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.QualityDownload = new System.Windows.Forms.ComboBox();
            this.CDNSource = new System.Windows.Forms.ComboBox();
            this.Save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WelcomeText
            // 
            this.WelcomeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomeText.Location = new System.Drawing.Point(12, 9);
            this.WelcomeText.Name = "WelcomeText";
            this.WelcomeText.Size = new System.Drawing.Size(357, 69);
            this.WelcomeText.TabIndex = 0;
            this.WelcomeText.Text = "Howdy! Looks like it\'s the first time this launcher is started. Please specify wh" +
    "ere you want to download all required game files ";
            this.WelcomeText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Download Type:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Download Source:";
            // 
            // QualityDownload
            // 
            this.QualityDownload.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QualityDownload.FormattingEnabled = true;
            this.QualityDownload.Location = new System.Drawing.Point(132, 81);
            this.QualityDownload.Name = "QualityDownload";
            this.QualityDownload.Size = new System.Drawing.Size(237, 21);
            this.QualityDownload.TabIndex = 3;
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
            // WelcomeScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 206);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.CDNSource);
            this.Controls.Add(this.QualityDownload);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WelcomeText);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label WelcomeText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox QualityDownload;
        private System.Windows.Forms.ComboBox CDNSource;
        private System.Windows.Forms.Button Save;
    }
}