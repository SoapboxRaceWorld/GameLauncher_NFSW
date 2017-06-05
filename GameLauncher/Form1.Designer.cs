namespace GameLauncher
{
    partial class mainScreen
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
        /// Wymagana metoda obsługi projektanta — nie należy modyfikować 
        /// zawartość tej metody z edytorem kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainScreen));
            this.closebtn = new System.Windows.Forms.PictureBox();
            this.minimizebtn = new System.Windows.Forms.PictureBox();
            this.email = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.serverStatus = new System.Windows.Forms.Label();
            this.serverStatusImg = new System.Windows.Forms.PictureBox();
            this.serverPick = new System.Windows.Forms.ComboBox();
            this.loginButton = new System.Windows.Forms.Label();
            this.currentWindowInfo = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).BeginInit();
            this.SuspendLayout();
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.Transparent;
            this.closebtn.BackgroundImage = global::GameLauncher.Properties.Resources.close;
            this.closebtn.Location = new System.Drawing.Point(742, 1);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(33, 26);
            this.closebtn.TabIndex = 0;
            this.closebtn.TabStop = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // minimizebtn
            // 
            this.minimizebtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizebtn.BackgroundImage = global::GameLauncher.Properties.Resources.minimize;
            this.minimizebtn.Location = new System.Drawing.Point(708, 1);
            this.minimizebtn.Name = "minimizebtn";
            this.minimizebtn.Size = new System.Drawing.Size(33, 26);
            this.minimizebtn.TabIndex = 0;
            this.minimizebtn.TabStop = false;
            this.minimizebtn.Click += new System.EventHandler(this.minimizebtn_Click);
            // 
            // email
            // 
            this.email.Location = new System.Drawing.Point(34, 182);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(209, 20);
            this.email.TabIndex = 1;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(277, 182);
            this.password.Name = "password";
            this.password.PasswordChar = '•';
            this.password.Size = new System.Drawing.Size(209, 20);
            this.password.TabIndex = 2;
            // 
            // serverStatus
            // 
            this.serverStatus.AutoSize = true;
            this.serverStatus.BackColor = System.Drawing.Color.Transparent;
            this.serverStatus.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.serverStatus.Location = new System.Drawing.Point(44, 329);
            this.serverStatus.Name = "serverStatus";
            this.serverStatus.Size = new System.Drawing.Size(127, 13);
            this.serverStatus.TabIndex = 3;
            this.serverStatus.Text = "Retrieving server status...";
            this.serverStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // serverStatusImg
            // 
            this.serverStatusImg.BackColor = System.Drawing.Color.Transparent;
            this.serverStatusImg.BackgroundImage = global::GameLauncher.Properties.Resources.server_online;
            this.serverStatusImg.Location = new System.Drawing.Point(20, 334);
            this.serverStatusImg.Name = "serverStatusImg";
            this.serverStatusImg.Size = new System.Drawing.Size(16, 16);
            this.serverStatusImg.TabIndex = 4;
            this.serverStatusImg.TabStop = false;
            // 
            // serverPick
            // 
            this.serverPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverPick.FormattingEnabled = true;
            this.serverPick.Location = new System.Drawing.Point(564, 49);
            this.serverPick.Name = "serverPick";
            this.serverPick.Size = new System.Drawing.Size(204, 21);
            this.serverPick.TabIndex = 5;
            // 
            // loginButton
            // 
            this.loginButton.AutoSize = true;
            this.loginButton.BackColor = System.Drawing.Color.Transparent;
            this.loginButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.loginButton.ForeColor = System.Drawing.Color.Silver;
            this.loginButton.Location = new System.Drawing.Point(340, 250);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(35, 13);
            this.loginButton.TabIndex = 6;
            this.loginButton.Text = "label1";
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // currentWindowInfo
            // 
            this.currentWindowInfo.AutoSize = true;
            this.currentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentWindowInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.currentWindowInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.currentWindowInfo.Location = new System.Drawing.Point(69, 116);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(419, 20);
            this.currentWindowInfo.TabIndex = 7;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
            this.currentWindowInfo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // mainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.loginbg;
            this.ClientSize = new System.Drawing.Size(790, 490);
            this.Controls.Add(this.currentWindowInfo);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.serverPick);
            this.Controls.Add(this.serverStatusImg);
            this.Controls.Add(this.serverStatus);
            this.Controls.Add(this.password);
            this.Controls.Add(this.email);
            this.Controls.Add(this.minimizebtn);
            this.Controls.Add(this.closebtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "mainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.mainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox closebtn;
        private System.Windows.Forms.PictureBox minimizebtn;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label serverStatus;
        private System.Windows.Forms.PictureBox serverStatusImg;
        private System.Windows.Forms.ComboBox serverPick;
        private System.Windows.Forms.Label loginButton;
        private System.Windows.Forms.Label currentWindowInfo;
    }
}

