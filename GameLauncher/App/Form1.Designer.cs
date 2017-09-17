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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mainScreen));
            this.closebtn = new System.Windows.Forms.PictureBox();
            this.minimizebtn = new System.Windows.Forms.PictureBox();
            this.email = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.serverStatus = new System.Windows.Forms.Label();
            this.serverStatusImg = new System.Windows.Forms.PictureBox();
            this.serverPick = new System.Windows.Forms.ComboBox();
            this.currentWindowInfo = new System.Windows.Forms.Label();
            this.consoleLog = new System.Windows.Forms.RichTextBox();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this.Timeout = new System.Windows.Forms.Timer(this.components);
            this.onlineCount = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.registerText = new System.Windows.Forms.LinkLabel();
            this.emailLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.troubleLabel = new System.Windows.Forms.Label();
            this.githubLink = new System.Windows.Forms.LinkLabel();
            this.registerButton = new System.Windows.Forms.Button();
            this.selectServerLabel = new System.Windows.Forms.Label();
            this.settingsButton = new System.Windows.Forms.PictureBox();
            this.moveWindow = new System.Windows.Forms.Panel();
            this.playLoggedInAs = new System.Windows.Forms.Label();
            this.verticalBanner = new System.Windows.Forms.PictureBox();
            this.settingsSave = new System.Windows.Forms.Button();
            this.settingsLanguage = new System.Windows.Forms.ComboBox();
            this.settingsLanguageText = new System.Windows.Forms.Label();
            this.settingsQuality = new System.Windows.Forms.ComboBox();
            this.settingsQualityText = new System.Windows.Forms.Label();
            this.settingsLanguageDesc = new System.Windows.Forms.Label();
            this.settingsQualityDesc = new System.Windows.Forms.Label();
            this.registerEmail = new System.Windows.Forms.TextBox();
            this.registerTicket = new System.Windows.Forms.TextBox();
            this.registerPassword = new System.Windows.Forms.TextBox();
            this.registerConfirmPassword = new System.Windows.Forms.TextBox();
            this.registerEmailText = new System.Windows.Forms.Label();
            this.registerPasswordText = new System.Windows.Forms.Label();
            this.registerTicketText = new System.Windows.Forms.Label();
            this.registerConfirmPasswordText = new System.Windows.Forms.Label();
            this.registerAgree = new System.Windows.Forms.CheckBox();
            this.playProgress = new System.Windows.Forms.ProgressBar();
            this.playProgressText = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.Notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.forgotPassword = new System.Windows.Forms.LinkLabel();
            this.playProgressTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).BeginInit();
            this.moveWindow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).BeginInit();
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
            this.email.Location = new System.Drawing.Point(33, 183);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(210, 20);
            this.email.TabIndex = 1;
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(276, 183);
            this.password.Name = "password";
            this.password.PasswordChar = '•';
            this.password.Size = new System.Drawing.Size(210, 20);
            this.password.TabIndex = 2;
            // 
            // serverStatus
            // 
            this.serverStatus.AutoSize = true;
            this.serverStatus.BackColor = System.Drawing.Color.Transparent;
            this.serverStatus.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.serverStatus.Location = new System.Drawing.Point(44, 322);
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
            this.serverPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.serverPick.FormattingEnabled = true;
            this.serverPick.Location = new System.Drawing.Point(548, 47);
            this.serverPick.Name = "serverPick";
            this.serverPick.Size = new System.Drawing.Size(188, 21);
            this.serverPick.TabIndex = 5;
            // 
            // currentWindowInfo
            // 
            this.currentWindowInfo.AutoSize = true;
            this.currentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentWindowInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.currentWindowInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.currentWindowInfo.Location = new System.Drawing.Point(69, 116);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(437, 25);
            this.currentWindowInfo.TabIndex = 7;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN:";
            this.currentWindowInfo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.currentWindowInfo.UseCompatibleTextRendering = true;
            // 
            // consoleLog
            // 
            this.consoleLog.BackColor = System.Drawing.Color.Black;
            this.consoleLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.consoleLog.ForeColor = System.Drawing.Color.White;
            this.consoleLog.Location = new System.Drawing.Point(35, 389);
            this.consoleLog.Name = "consoleLog";
            this.consoleLog.ReadOnly = true;
            this.consoleLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.consoleLog.Size = new System.Drawing.Size(719, 87);
            this.consoleLog.TabIndex = 9;
            this.consoleLog.Text = "";
            // 
            // rememberMe
            // 
            this.rememberMe.AutoSize = true;
            this.rememberMe.BackColor = System.Drawing.Color.Transparent;
            this.rememberMe.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rememberMe.ForeColor = System.Drawing.Color.White;
            this.rememberMe.Location = new System.Drawing.Point(33, 209);
            this.rememberMe.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(268, 21);
            this.rememberMe.TabIndex = 3;
            this.rememberMe.Text = "REMEMBER MY EMAIL ADDRESS";
            this.rememberMe.UseVisualStyleBackColor = false;
            // 
            // Timeout
            // 
            this.Timeout.Interval = 3000;
            // 
            // onlineCount
            // 
            this.onlineCount.AutoSize = true;
            this.onlineCount.BackColor = System.Drawing.Color.Transparent;
            this.onlineCount.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.onlineCount.Location = new System.Drawing.Point(44, 337);
            this.onlineCount.Name = "onlineCount";
            this.onlineCount.Size = new System.Drawing.Size(127, 13);
            this.onlineCount.TabIndex = 11;
            this.onlineCount.Text = "Retrieving server status...";
            this.onlineCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Image = global::GameLauncher.Properties.Resources.button_enable;
            this.loginButton.Location = new System.Drawing.Point(340, 250);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(149, 37);
            this.loginButton.TabIndex = 12;
            this.loginButton.Text = "LOG IN";
            this.loginButton.UseVisualStyleBackColor = false;
            // 
            // registerText
            // 
            this.registerText.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(255)))), ((int)(((byte)(33)))));
            this.registerText.AutoSize = true;
            this.registerText.BackColor = System.Drawing.Color.Transparent;
            this.registerText.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registerText.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(255)))), ((int)(((byte)(33)))));
            this.registerText.Location = new System.Drawing.Point(527, 260);
            this.registerText.Name = "registerText";
            this.registerText.Size = new System.Drawing.Size(83, 15);
            this.registerText.TabIndex = 13;
            this.registerText.TabStop = true;
            this.registerText.Text = "regsiterText";
            this.registerText.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(255)))), ((int)(((byte)(33)))));
            this.registerText.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.registerText_LinkClicked);
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.BackColor = System.Drawing.Color.Transparent;
            this.emailLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.emailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.emailLabel.Location = new System.Drawing.Point(30, 161);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(131, 18);
            this.emailLabel.TabIndex = 14;
            this.emailLabel.Text = "EMAIL ADDRESS:";
            this.emailLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.BackColor = System.Drawing.Color.Transparent;
            this.passwordLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.passwordLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.passwordLabel.Location = new System.Drawing.Point(276, 161);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(100, 18);
            this.passwordLabel.TabIndex = 15;
            this.passwordLabel.Text = "PASSWORD:";
            this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // troubleLabel
            // 
            this.troubleLabel.AutoSize = true;
            this.troubleLabel.BackColor = System.Drawing.Color.Transparent;
            this.troubleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.troubleLabel.ForeColor = System.Drawing.Color.White;
            this.troubleLabel.Location = new System.Drawing.Point(30, 255);
            this.troubleLabel.Name = "troubleLabel";
            this.troubleLabel.Size = new System.Drawing.Size(148, 16);
            this.troubleLabel.TabIndex = 16;
            this.troubleLabel.Text = "HAVING TROUBLE?";
            // 
            // githubLink
            // 
            this.githubLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.githubLink.AutoSize = true;
            this.githubLink.BackColor = System.Drawing.Color.Transparent;
            this.githubLink.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.githubLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.githubLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.githubLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.githubLink.Location = new System.Drawing.Point(30, 271);
            this.githubLink.Name = "githubLink";
            this.githubLink.Size = new System.Drawing.Size(186, 16);
            this.githubLink.TabIndex = 17;
            this.githubLink.TabStop = true;
            this.githubLink.Text = "Visit our \"GitHub Issues\" page";
            this.githubLink.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.BorderSize = 0;
            this.registerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.Image = global::GameLauncher.Properties.Resources.button_enable;
            this.registerButton.Location = new System.Drawing.Point(623, 319);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(149, 37);
            this.registerButton.TabIndex = 19;
            this.registerButton.Text = "REGISTER";
            this.registerButton.UseVisualStyleBackColor = false;
            // 
            // selectServerLabel
            // 
            this.selectServerLabel.BackColor = System.Drawing.Color.Transparent;
            this.selectServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.selectServerLabel.ForeColor = System.Drawing.Color.White;
            this.selectServerLabel.Location = new System.Drawing.Point(370, 50);
            this.selectServerLabel.Name = "selectServerLabel";
            this.selectServerLabel.Size = new System.Drawing.Size(171, 15);
            this.selectServerLabel.TabIndex = 20;
            this.selectServerLabel.Text = "SELECT SERVER:";
            this.selectServerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.BackgroundImage = global::GameLauncher.Properties.Resources.settingsbtn;
            this.settingsButton.Location = new System.Drawing.Point(741, 44);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(31, 29);
            this.settingsButton.TabIndex = 21;
            this.settingsButton.TabStop = false;
            // 
            // moveWindow
            // 
            this.moveWindow.BackColor = System.Drawing.Color.Transparent;
            this.moveWindow.Controls.Add(this.playLoggedInAs);
            this.moveWindow.Controls.Add(this.selectServerLabel);
            this.moveWindow.Location = new System.Drawing.Point(1, 1);
            this.moveWindow.Name = "moveWindow";
            this.moveWindow.Size = new System.Drawing.Size(788, 87);
            this.moveWindow.TabIndex = 1;
            // 
            // playLoggedInAs
            // 
            this.playLoggedInAs.AutoSize = true;
            this.playLoggedInAs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playLoggedInAs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.playLoggedInAs.Location = new System.Drawing.Point(354, 51);
            this.playLoggedInAs.Name = "playLoggedInAs";
            this.playLoggedInAs.Size = new System.Drawing.Size(95, 13);
            this.playLoggedInAs.TabIndex = 0;
            this.playLoggedInAs.Text = "LOGGED IN AS";
            // 
            // verticalBanner
            // 
            this.verticalBanner.BackColor = System.Drawing.Color.Transparent;
            this.verticalBanner.Location = new System.Drawing.Point(526, 105);
            this.verticalBanner.Name = "verticalBanner";
            this.verticalBanner.Size = new System.Drawing.Size(249, 118);
            this.verticalBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.verticalBanner.TabIndex = 22;
            this.verticalBanner.TabStop = false;
            // 
            // settingsSave
            // 
            this.settingsSave.BackColor = System.Drawing.Color.Transparent;
            this.settingsSave.FlatAppearance.BorderSize = 0;
            this.settingsSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.settingsSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.settingsSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsSave.ForeColor = System.Drawing.Color.White;
            this.settingsSave.Image = global::GameLauncher.Properties.Resources.button_enable;
            this.settingsSave.Location = new System.Drawing.Point(613, 420);
            this.settingsSave.Name = "settingsSave";
            this.settingsSave.Size = new System.Drawing.Size(149, 37);
            this.settingsSave.TabIndex = 23;
            this.settingsSave.Text = "SAVE";
            this.settingsSave.UseVisualStyleBackColor = false;
            // 
            // settingsLanguage
            // 
            this.settingsLanguage.BackColor = System.Drawing.Color.White;
            this.settingsLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsLanguage.FormattingEnabled = true;
            this.settingsLanguage.Location = new System.Drawing.Point(77, 198);
            this.settingsLanguage.Name = "settingsLanguage";
            this.settingsLanguage.Size = new System.Drawing.Size(185, 21);
            this.settingsLanguage.TabIndex = 24;
            // 
            // settingsLanguageText
            // 
            this.settingsLanguageText.AutoSize = true;
            this.settingsLanguageText.BackColor = System.Drawing.Color.Transparent;
            this.settingsLanguageText.ForeColor = System.Drawing.Color.White;
            this.settingsLanguageText.Location = new System.Drawing.Point(74, 180);
            this.settingsLanguageText.Name = "settingsLanguageText";
            this.settingsLanguageText.Size = new System.Drawing.Size(113, 13);
            this.settingsLanguageText.TabIndex = 25;
            this.settingsLanguageText.Text = "SELECT LANGUAGE:";
            // 
            // settingsQuality
            // 
            this.settingsQuality.BackColor = System.Drawing.Color.White;
            this.settingsQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsQuality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsQuality.FormattingEnabled = true;
            this.settingsQuality.Location = new System.Drawing.Point(77, 269);
            this.settingsQuality.Name = "settingsQuality";
            this.settingsQuality.Size = new System.Drawing.Size(185, 21);
            this.settingsQuality.TabIndex = 26;
            // 
            // settingsQualityText
            // 
            this.settingsQualityText.AutoSize = true;
            this.settingsQualityText.BackColor = System.Drawing.Color.Transparent;
            this.settingsQualityText.ForeColor = System.Drawing.Color.White;
            this.settingsQualityText.Location = new System.Drawing.Point(74, 251);
            this.settingsQualityText.Name = "settingsQualityText";
            this.settingsQualityText.Size = new System.Drawing.Size(145, 13);
            this.settingsQualityText.TabIndex = 27;
            this.settingsQualityText.Text = "SELECT DOWNLOAD SIZE:";
            // 
            // settingsLanguageDesc
            // 
            this.settingsLanguageDesc.BackColor = System.Drawing.Color.Transparent;
            this.settingsLanguageDesc.ForeColor = System.Drawing.Color.White;
            this.settingsLanguageDesc.Location = new System.Drawing.Point(314, 179);
            this.settingsLanguageDesc.Name = "settingsLanguageDesc";
            this.settingsLanguageDesc.Size = new System.Drawing.Size(427, 56);
            this.settingsLanguageDesc.TabIndex = 28;
            this.settingsLanguageDesc.Text = "Select the language that the game text and audio should be displayed in. This wil" +
    "l not affect your chat or server options.";
            // 
            // settingsQualityDesc
            // 
            this.settingsQualityDesc.BackColor = System.Drawing.Color.Transparent;
            this.settingsQualityDesc.ForeColor = System.Drawing.Color.White;
            this.settingsQualityDesc.Location = new System.Drawing.Point(313, 242);
            this.settingsQualityDesc.Name = "settingsQualityDesc";
            this.settingsQualityDesc.Size = new System.Drawing.Size(405, 85);
            this.settingsQualityDesc.TabIndex = 29;
            this.settingsQualityDesc.Text = resources.GetString("settingsQualityDesc.Text");
            // 
            // registerEmail
            // 
            this.registerEmail.Location = new System.Drawing.Point(33, 183);
            this.registerEmail.Name = "registerEmail";
            this.registerEmail.Size = new System.Drawing.Size(210, 20);
            this.registerEmail.TabIndex = 30;
            // 
            // registerTicket
            // 
            this.registerTicket.Location = new System.Drawing.Point(33, 245);
            this.registerTicket.Name = "registerTicket";
            this.registerTicket.Size = new System.Drawing.Size(210, 20);
            this.registerTicket.TabIndex = 31;
            // 
            // registerPassword
            // 
            this.registerPassword.Location = new System.Drawing.Point(276, 183);
            this.registerPassword.Name = "registerPassword";
            this.registerPassword.PasswordChar = '•';
            this.registerPassword.Size = new System.Drawing.Size(210, 20);
            this.registerPassword.TabIndex = 32;
            // 
            // registerConfirmPassword
            // 
            this.registerConfirmPassword.Location = new System.Drawing.Point(276, 245);
            this.registerConfirmPassword.Name = "registerConfirmPassword";
            this.registerConfirmPassword.PasswordChar = '•';
            this.registerConfirmPassword.Size = new System.Drawing.Size(210, 20);
            this.registerConfirmPassword.TabIndex = 33;
            // 
            // registerEmailText
            // 
            this.registerEmailText.AutoSize = true;
            this.registerEmailText.BackColor = System.Drawing.Color.Transparent;
            this.registerEmailText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registerEmailText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.registerEmailText.Location = new System.Drawing.Point(30, 161);
            this.registerEmailText.Name = "registerEmailText";
            this.registerEmailText.Size = new System.Drawing.Size(131, 18);
            this.registerEmailText.TabIndex = 34;
            this.registerEmailText.Text = "EMAIL ADDRESS:";
            this.registerEmailText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // registerPasswordText
            // 
            this.registerPasswordText.AutoSize = true;
            this.registerPasswordText.BackColor = System.Drawing.Color.Transparent;
            this.registerPasswordText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registerPasswordText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.registerPasswordText.Location = new System.Drawing.Point(276, 161);
            this.registerPasswordText.Name = "registerPasswordText";
            this.registerPasswordText.Size = new System.Drawing.Size(100, 18);
            this.registerPasswordText.TabIndex = 35;
            this.registerPasswordText.Text = "PASSWORD:";
            this.registerPasswordText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // registerTicketText
            // 
            this.registerTicketText.AutoSize = true;
            this.registerTicketText.BackColor = System.Drawing.Color.Transparent;
            this.registerTicketText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registerTicketText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.registerTicketText.Location = new System.Drawing.Point(30, 224);
            this.registerTicketText.Name = "registerTicketText";
            this.registerTicketText.Size = new System.Drawing.Size(64, 18);
            this.registerTicketText.TabIndex = 36;
            this.registerTicketText.Text = "TICKET:";
            this.registerTicketText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // registerConfirmPasswordText
            // 
            this.registerConfirmPasswordText.AutoSize = true;
            this.registerConfirmPasswordText.BackColor = System.Drawing.Color.Transparent;
            this.registerConfirmPasswordText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.registerConfirmPasswordText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.registerConfirmPasswordText.Location = new System.Drawing.Point(276, 224);
            this.registerConfirmPasswordText.Name = "registerConfirmPasswordText";
            this.registerConfirmPasswordText.Size = new System.Drawing.Size(174, 18);
            this.registerConfirmPasswordText.TabIndex = 37;
            this.registerConfirmPasswordText.Text = "CONFIRM PASSWORD:";
            this.registerConfirmPasswordText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // registerAgree
            // 
            this.registerAgree.AutoSize = true;
            this.registerAgree.BackColor = System.Drawing.Color.Transparent;
            this.registerAgree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.registerAgree.Location = new System.Drawing.Point(33, 330);
            this.registerAgree.Name = "registerAgree";
            this.registerAgree.Size = new System.Drawing.Size(333, 17);
            this.registerAgree.TabIndex = 38;
            this.registerAgree.Text = "BY REGISTERING YOU AGREE TO THE TERMS OF SERVICE";
            this.registerAgree.UseVisualStyleBackColor = false;
            // 
            // playProgress
            // 
            this.playProgress.Location = new System.Drawing.Point(20, 436);
            this.playProgress.Name = "playProgress";
            this.playProgress.Size = new System.Drawing.Size(580, 18);
            this.playProgress.TabIndex = 39;
            // 
            // playProgressText
            // 
            this.playProgressText.AutoSize = true;
            this.playProgressText.BackColor = System.Drawing.Color.Transparent;
            this.playProgressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playProgressText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.playProgressText.Location = new System.Drawing.Point(17, 457);
            this.playProgressText.Name = "playProgressText";
            this.playProgressText.Size = new System.Drawing.Size(90, 13);
            this.playProgressText.TabIndex = 21;
            this.playProgressText.Text = "PLEASE WAIT";
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.ForeColor = System.Drawing.Color.White;
            this.playButton.Image = global::GameLauncher.Properties.Resources.playButton_enable;
            this.playButton.Location = new System.Drawing.Point(615, 422);
            this.playButton.Margin = new System.Windows.Forms.Padding(0);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(168, 58);
            this.playButton.TabIndex = 40;
            this.playButton.Text = "LAUNCH";
            this.playButton.UseVisualStyleBackColor = false;
            // 
            // Notification
            // 
            this.Notification.Text = "notifyIcon1";
            this.Notification.Visible = true;
            // 
            // forgotPassword
            // 
            this.forgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.forgotPassword.BackColor = System.Drawing.Color.Transparent;
            this.forgotPassword.DisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.forgotPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.forgotPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.forgotPassword.LinkArea = new System.Windows.Forms.LinkArea(0, 100);
            this.forgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.forgotPassword.Location = new System.Drawing.Point(276, 206);
            this.forgotPassword.Name = "forgotPassword";
            this.forgotPassword.Size = new System.Drawing.Size(210, 21);
            this.forgotPassword.TabIndex = 0;
            this.forgotPassword.TabStop = true;
            this.forgotPassword.Text = "I forgot my password.";
            this.forgotPassword.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.forgotPassword.UseCompatibleTextRendering = true;
            this.forgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            // 
            // playProgressTime
            // 
            this.playProgressTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playProgressTime.BackColor = System.Drawing.Color.Transparent;
            this.playProgressTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.playProgressTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playProgressTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(178)))), ((int)(((byte)(255)))));
            this.playProgressTime.Location = new System.Drawing.Point(506, 457);
            this.playProgressTime.Name = "playProgressTime";
            this.playProgressTime.Size = new System.Drawing.Size(94, 13);
            this.playProgressTime.TabIndex = 42;
            this.playProgressTime.Text = "CALCULATING";
            this.playProgressTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.loginbg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(790, 490);
            this.Controls.Add(this.playProgressTime);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.playProgressText);
            this.Controls.Add(this.playProgress);
            this.Controls.Add(this.registerAgree);
            this.Controls.Add(this.registerConfirmPasswordText);
            this.Controls.Add(this.registerTicketText);
            this.Controls.Add(this.registerPasswordText);
            this.Controls.Add(this.registerEmailText);
            this.Controls.Add(this.registerConfirmPassword);
            this.Controls.Add(this.registerPassword);
            this.Controls.Add(this.registerTicket);
            this.Controls.Add(this.registerEmail);
            this.Controls.Add(this.settingsQualityDesc);
            this.Controls.Add(this.settingsQualityText);
            this.Controls.Add(this.settingsQuality);
            this.Controls.Add(this.settingsLanguageText);
            this.Controls.Add(this.settingsLanguage);
            this.Controls.Add(this.verticalBanner);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.forgotPassword);
            this.Controls.Add(this.githubLink);
            this.Controls.Add(this.troubleLabel);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.registerText);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.onlineCount);
            this.Controls.Add(this.rememberMe);
            this.Controls.Add(this.consoleLog);
            this.Controls.Add(this.currentWindowInfo);
            this.Controls.Add(this.serverPick);
            this.Controls.Add(this.serverStatusImg);
            this.Controls.Add(this.serverStatus);
            this.Controls.Add(this.password);
            this.Controls.Add(this.email);
            this.Controls.Add(this.minimizebtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.moveWindow);
            this.Controls.Add(this.settingsSave);
            this.Controls.Add(this.settingsLanguageDesc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "mainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.mainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverStatusImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).EndInit();
            this.moveWindow.ResumeLayout(false);
            this.moveWindow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).EndInit();
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
        private System.Windows.Forms.Label currentWindowInfo;
        private System.Windows.Forms.RichTextBox consoleLog;
        private System.Windows.Forms.CheckBox rememberMe;
        private System.Windows.Forms.Timer Timeout;
        private System.Windows.Forms.Label onlineCount;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.LinkLabel registerText;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Label troubleLabel;
        private System.Windows.Forms.LinkLabel githubLink;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Label selectServerLabel;
        private System.Windows.Forms.PictureBox settingsButton;
        private System.Windows.Forms.Panel moveWindow;
        private System.Windows.Forms.PictureBox verticalBanner;
        private System.Windows.Forms.Button settingsSave;
        private System.Windows.Forms.ComboBox settingsLanguage;
        private System.Windows.Forms.Label settingsLanguageText;
        private System.Windows.Forms.ComboBox settingsQuality;
        private System.Windows.Forms.Label settingsQualityText;
        private System.Windows.Forms.Label settingsLanguageDesc;
        private System.Windows.Forms.Label settingsQualityDesc;
        private System.Windows.Forms.TextBox registerEmail;
        private System.Windows.Forms.TextBox registerTicket;
        private System.Windows.Forms.TextBox registerPassword;
        private System.Windows.Forms.TextBox registerConfirmPassword;
        private System.Windows.Forms.Label registerEmailText;
        private System.Windows.Forms.Label registerPasswordText;
        private System.Windows.Forms.Label registerTicketText;
        private System.Windows.Forms.Label registerConfirmPasswordText;
        private System.Windows.Forms.CheckBox registerAgree;
        private System.Windows.Forms.Label playLoggedInAs;
        private System.Windows.Forms.ProgressBar playProgress;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.NotifyIcon Notification;
        private System.Windows.Forms.LinkLabel forgotPassword;
        internal System.Windows.Forms.Label playProgressText;
        internal System.Windows.Forms.Label playProgressTime;
    }
}

