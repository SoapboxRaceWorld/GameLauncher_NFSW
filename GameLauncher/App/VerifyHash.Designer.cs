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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.StatusText = new System.Windows.Forms.Label();
            this.StartScanner = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(105, 103);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(592, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // StatusText
            // 
            this.StatusText.Location = new System.Drawing.Point(102, 60);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(595, 23);
            this.StatusText.TabIndex = 2;
            this.StatusText.Text = "label1";
            // 
            // StartScanner
            // 
            this.StartScanner.Location = new System.Drawing.Point(357, 254);
            this.StartScanner.Name = "StartScanner";
            this.StartScanner.Size = new System.Drawing.Size(75, 23);
            this.StartScanner.TabIndex = 3;
            this.StartScanner.Text = "Start Scan";
            this.StartScanner.UseVisualStyleBackColor = true;
            this.StartScanner.Click += new System.EventHandler(this.StartScanner_Click);
            // 
            // VerifyHash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StartScanner);
            this.Controls.Add(this.StatusText);
            this.Controls.Add(this.progressBar1);
            this.Name = "VerifyHash";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VerifyHash";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label StatusText;
        private System.Windows.Forms.Button StartScanner;
    }
}