using System.Windows.Forms;
namespace GameLauncher.App.UI_Forms.Welcome_Screen
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
            this.ButtonSave = new System.Windows.Forms.Button();
            this.ListStatusText = new System.Windows.Forms.Label();
            this.ButtonAPIError = new System.Windows.Forms.Button();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.GameLangSource = new System.Windows.Forms.ComboBox();
            this.GameLangText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // WelcomeText
            // 
            this.WelcomeText.BackColor = System.Drawing.Color.Transparent;
            this.WelcomeText.Font = new System.Drawing.Font("DejaVu Sans", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.WelcomeText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.WelcomeText.Location = new System.Drawing.Point(9, 6);
            this.WelcomeText.Name = "WelcomeText";
            this.WelcomeText.Size = new System.Drawing.Size(392, 80);
            this.WelcomeText.TabIndex = 0;
            this.WelcomeText.Text = "Checking API Status";
            this.WelcomeText.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DownloadSourceText
            // 
            this.DownloadSourceText.BackColor = System.Drawing.Color.Transparent;
            this.DownloadSourceText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadSourceText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DownloadSourceText.Location = new System.Drawing.Point(116, 187);
            this.DownloadSourceText.Name = "DownloadSourceText";
            this.DownloadSourceText.Size = new System.Drawing.Size(177, 14);
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
            this.CDNSource.Font = new System.Drawing.Font("DejaVu Sans", 9F);
            this.CDNSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.CDNSource.FormattingEnabled = true;
            this.CDNSource.ItemHeight = 13;
            this.CDNSource.Location = new System.Drawing.Point(95, 210);
            this.CDNSource.MaxDropDownItems = 21;
            this.CDNSource.Name = "CDNSource";
            this.CDNSource.Size = new System.Drawing.Size(220, 19);
            this.CDNSource.TabIndex = 4;
            // 
            // ButtonSave
            // 
            this.ButtonSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ButtonSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ButtonSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.ButtonSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.ButtonSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSave.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonSave.Location = new System.Drawing.Point(5, 287);
            this.ButtonSave.Name = "ButtonSave";
            this.ButtonSave.Size = new System.Drawing.Size(400, 32);
            this.ButtonSave.TabIndex = 5;
            this.ButtonSave.Text = "Save Settings and Select Download / Install Location";
            this.ButtonSave.UseVisualStyleBackColor = false;
            this.ButtonSave.Click += new System.EventHandler(this.Save_Click);
            // 
            // ListStatusText
            // 
            this.ListStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ListStatusText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ListStatusText.Location = new System.Drawing.Point(140, 95);
            this.ListStatusText.Name = "ListStatusText";
            this.ListStatusText.Size = new System.Drawing.Size(129, 14);
            this.ListStatusText.TabIndex = 7;
            this.ListStatusText.Text = "API Status - Pinging";
            this.ListStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonAPIError
            // 
            this.ButtonAPIError.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.ButtonAPIError.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.ButtonAPIError.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.ButtonAPIError.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonAPIError.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonAPIError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonAPIError.Location = new System.Drawing.Point(143, 255);
            this.ButtonAPIError.Name = "ButtonAPIError";
            this.ButtonAPIError.Size = new System.Drawing.Size(126, 26);
            this.ButtonAPIError.TabIndex = 8;
            this.ButtonAPIError.Text = "Manual Bypass";
            this.ButtonAPIError.UseVisualStyleBackColor = false;
            this.ButtonAPIError.Click += new System.EventHandler(this.APIErrorButton_Click);
            // 
            // VersionLabel
            // 
            this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
            this.VersionLabel.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(123, 337);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(164, 14);
            this.VersionLabel.TabIndex = 9;
            this.VersionLabel.Text = "Version: vXX.XX.XX.XXXX";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GameLangSource
            // 
            this.GameLangSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.GameLangSource.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.GameLangSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GameLangSource.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.GameLangSource.Font = new System.Drawing.Font("DejaVu Sans", 9F);
            this.GameLangSource.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.GameLangSource.FormattingEnabled = true;
            this.GameLangSource.ItemHeight = 13;
            this.GameLangSource.Location = new System.Drawing.Point(95, 150);
            this.GameLangSource.MaxDropDownItems = 21;
            this.GameLangSource.Name = "GameLangSource";
            this.GameLangSource.Size = new System.Drawing.Size(220, 19);
            this.GameLangSource.TabIndex = 11;
            // 
            // GameLangText
            // 
            this.GameLangText.BackColor = System.Drawing.Color.Transparent;
            this.GameLangText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GameLangText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.GameLangText.Location = new System.Drawing.Point(117, 127);
            this.GameLangText.Name = "GameLangText";
            this.GameLangText.Size = new System.Drawing.Size(175, 14);
            this.GameLangText.TabIndex = 10;
            this.GameLangText.Text = "Select Game Language:";
            this.GameLangText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // WelcomeScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(36)))), ((int)(((byte)(45)))));
            this.ClientSize = new System.Drawing.Size(410, 360);
            this.Controls.Add(this.GameLangSource);
            this.Controls.Add(this.GameLangText);
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.ButtonAPIError);
            this.Controls.Add(this.ListStatusText);
            this.Controls.Add(this.ButtonSave);
            this.Controls.Add(this.CDNSource);
            this.Controls.Add(this.DownloadSourceText);
            this.Controls.Add(this.WelcomeText);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WelcomeScreen";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Welcome - SBRW Laucher";
            this.Load += new System.EventHandler(this.WelcomeScreen_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label WelcomeText;
        private System.Windows.Forms.Label DownloadSourceText;
        private System.Windows.Forms.ComboBox CDNSource;
        private System.Windows.Forms.Button ButtonSave;
        private System.Windows.Forms.Label ListStatusText;
        private System.Windows.Forms.Button ButtonAPIError;
        private System.Windows.Forms.Label VersionLabel;
        private ComboBox GameLangSource;
        private Label GameLangText;
    }
}