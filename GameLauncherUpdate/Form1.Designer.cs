namespace GameLauncherUpdater
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.downloadProgress = new System.Windows.Forms.ProgressBar();
            this.information = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // downloadProgress
            // 
            this.downloadProgress.Location = new System.Drawing.Point(12, 41);
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.Size = new System.Drawing.Size(331, 10);
            this.downloadProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.downloadProgress.TabIndex = 0;
            // 
            // information
            // 
            this.information.AutoEllipsis = true;
            this.information.BackColor = System.Drawing.Color.Transparent;
            this.information.ForeColor = System.Drawing.Color.Snow;
            this.information.Location = new System.Drawing.Point(12, 1);
            this.information.Name = "information";
            this.information.Size = new System.Drawing.Size(331, 37);
            this.information.TabIndex = 1;
            this.information.Text = "Checking for latest update";
            this.information.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::GameLauncherUpdater.Properties.Resources.Background;
            this.ClientSize = new System.Drawing.Size(355, 63);
            this.Controls.Add(this.information);
            this.Controls.Add(this.downloadProgress);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Update";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar downloadProgress;
        private System.Windows.Forms.Label information;
    }
}

