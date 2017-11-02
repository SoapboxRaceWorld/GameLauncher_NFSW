namespace GameLauncher.App.Classes.Events
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.closeAbout = new System.Windows.Forms.Button();
            this.aboutInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // closeAbout
            // 
            this.closeAbout.Location = new System.Drawing.Point(12, 394);
            this.closeAbout.Name = "closeAbout";
            this.closeAbout.Size = new System.Drawing.Size(493, 32);
            this.closeAbout.TabIndex = 0;
            this.closeAbout.Text = "Close";
            this.closeAbout.UseVisualStyleBackColor = true;
            // 
            // aboutInfo
            // 
            this.aboutInfo.AutoSize = true;
            this.aboutInfo.BackColor = System.Drawing.Color.Transparent;
            this.aboutInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.aboutInfo.ForeColor = System.Drawing.Color.White;
            this.aboutInfo.Location = new System.Drawing.Point(123, 198);
            this.aboutInfo.Name = "aboutInfo";
            this.aboutInfo.Size = new System.Drawing.Size(276, 20);
            this.aboutInfo.TabIndex = 1;
            this.aboutInfo.Text = "Close this form before its too late";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 438);
            this.Controls.Add(this.aboutInfo);
            this.Controls.Add(this.closeAbout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About GameLauncher";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Red;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button closeAbout;
        private System.Windows.Forms.Label aboutInfo;
    }
}