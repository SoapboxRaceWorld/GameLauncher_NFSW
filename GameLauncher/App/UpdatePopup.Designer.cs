namespace GameLauncher.App
{
    partial class UpdatePopup
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
            this.changeLogURL = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SkipCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // changeLogURL
            // 
            this.changeLogURL.Location = new System.Drawing.Point(15, 96);
            this.changeLogURL.MinimumSize = new System.Drawing.Size(20, 20);
            this.changeLogURL.Name = "changeLogURL";
            this.changeLogURL.Size = new System.Drawing.Size(445, 217);
            this.changeLogURL.TabIndex = 0;
            this.changeLogURL.Url = new System.Uri("http://changeLogURL", System.UriKind.Absolute);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(448, 39);
            this.label1.TabIndex = 1;
            this.label1.Text = "An Update is available, do you wanna download it now?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(385, 67);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(75, 23);
            this.UpdateButton.TabIndex = 2;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(304, 67);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // SkipCheckBox
            // 
            this.SkipCheckBox.AutoSize = true;
            this.SkipCheckBox.Location = new System.Drawing.Point(15, 73);
            this.SkipCheckBox.Name = "SkipCheckBox";
            this.SkipCheckBox.Size = new System.Drawing.Size(103, 17);
            this.SkipCheckBox.TabIndex = 4;
            this.SkipCheckBox.Text = "Skip this version";
            this.SkipCheckBox.UseVisualStyleBackColor = true;
            // 
            // UpdatePopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 325);
            this.Controls.Add(this.SkipCheckBox);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.changeLogURL);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdatePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UpdatePopup";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.UpdatePopup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.WebBrowser changeLogURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button UpdateButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.CheckBox SkipCheckBox;
    }
}