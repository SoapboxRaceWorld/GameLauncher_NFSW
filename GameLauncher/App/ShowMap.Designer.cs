namespace GameLauncher.App
{
    partial class ShowMap
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
            this.srvInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // srvInfo
            // 
            this.srvInfo.BackColor = System.Drawing.Color.Transparent;
            this.srvInfo.ForeColor = System.Drawing.Color.White;
            this.srvInfo.Location = new System.Drawing.Point(12, 512);
            this.srvInfo.Name = "srvInfo";
            this.srvInfo.Size = new System.Drawing.Size(1000, 41);
            this.srvInfo.TabIndex = 0;
            this.srvInfo.Text = "ServerName: X";
            this.srvInfo.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // ShowMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.map;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1024, 562);
            this.Controls.Add(this.srvInfo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ShowMap";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ShowMap";
            this.Load += new System.EventHandler(this.ShowMap_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label srvInfo;
    }
}