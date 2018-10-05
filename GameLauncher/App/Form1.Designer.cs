namespace GameLauncher
{
    sealed partial class MainScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
            this.closebtn = new System.Windows.Forms.PictureBox();
            this.minimizebtn = new System.Windows.Forms.PictureBox();
            this.email = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.serverPick = new System.Windows.Forms.ComboBox();
            this.currentWindowInfo = new System.Windows.Forms.Label();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this.Timeout = new System.Windows.Forms.Timer(this.components);
            this.onlineCount = new System.Windows.Forms.Label();
            this.loginButton = new System.Windows.Forms.Button();
            this.registerText = new System.Windows.Forms.PictureBox();
            this.emailLabel = new System.Windows.Forms.Label();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.registerButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.PictureBox();
            this.verticalBanner = new System.Windows.Forms.PictureBox();
            this.settingsSave = new System.Windows.Forms.Button();
            this.settingsLanguage = new System.Windows.Forms.ComboBox();
            this.settingsLanguageText = new System.Windows.Forms.Label();
            this.settingsQuality = new System.Windows.Forms.ComboBox();
            this.settingsQualityText = new System.Windows.Forms.Label();
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
            this.playProgressText = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.Notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.forgotPassword = new System.Windows.Forms.LinkLabel();
            this.playProgressTime = new System.Windows.Forms.Label();
            this.launcherVersion = new System.Windows.Forms.Label();
            this.imageServerName = new System.Windows.Forms.Label();
            this.registerCancel = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.welcomeBack = new System.Windows.Forms.Label();
            this.settingsLanguageDesc = new System.Windows.Forms.Label();
            this.settingsUILang = new System.Windows.Forms.ComboBox();
            this.settingsUILangText = new System.Windows.Forms.Label();
            this.moreLanguages = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.translatedBy = new System.Windows.Forms.Label();
            this.settingsGameFiles = new System.Windows.Forms.Button();
            this.settingsGameFilesCurrent = new System.Windows.Forms.LinkLabel();
            this.settingsGamePathText = new System.Windows.Forms.Label();
            this.errorTicket = new System.Windows.Forms.Label();
            this.errorConfirm = new System.Windows.Forms.Label();
            this.errorPassword = new System.Windows.Forms.Label();
            this.errorEmail = new System.Windows.Forms.Label();
            this.errorTOS = new System.Windows.Forms.Label();
            this.addServer = new System.Windows.Forms.Button();
            this.showmap = new System.Windows.Forms.Label();
            this.inputeditor = new System.Windows.Forms.Button();
            this.legacyLaunch = new System.Windows.Forms.CheckBox();
            this.allowedCountriesLabel = new System.Windows.Forms.Label();
            this.settingsUILangDesc = new System.Windows.Forms.Label();
            this.playProgress = new GameLauncherReborn.ProgressBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.registerText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.Transparent;
            this.closebtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("closebtn.BackgroundImage")));
            this.closebtn.Location = new System.Drawing.Point(918, 2);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(52, 26);
            this.closebtn.TabIndex = 0;
            this.closebtn.TabStop = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // minimizebtn
            // 
            this.minimizebtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizebtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("minimizebtn.BackgroundImage")));
            this.minimizebtn.Location = new System.Drawing.Point(886, 2);
            this.minimizebtn.Name = "minimizebtn";
            this.minimizebtn.Size = new System.Drawing.Size(26, 26);
            this.minimizebtn.TabIndex = 0;
            this.minimizebtn.TabStop = false;
            this.minimizebtn.Click += new System.EventHandler(this.minimizebtn_Click);
            // 
            // email
            // 
            this.email.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(35)))), ((int)(((byte)(81)))));
            this.email.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.email.Font = new System.Drawing.Font("Arial", 14F);
            this.email.ForeColor = System.Drawing.Color.White;
            this.email.Location = new System.Drawing.Point(473, 285);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(244, 22);
            this.email.TabIndex = 2;
            // 
            // password
            // 
            this.password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(35)))), ((int)(((byte)(81)))));
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Font = new System.Drawing.Font("Arial", 14F);
            this.password.ForeColor = System.Drawing.Color.White;
            this.password.Location = new System.Drawing.Point(761, 285);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(182, 22);
            this.password.TabIndex = 4;
            this.password.UseSystemPasswordChar = true;
            this.password.WordWrap = false;
            // 
            // serverPick
            // 
            this.serverPick.BackColor = System.Drawing.Color.White;
            this.serverPick.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.serverPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.serverPick.FormattingEnabled = true;
            this.serverPick.Location = new System.Drawing.Point(720, 33);
            this.serverPick.Name = "serverPick";
            this.serverPick.Size = new System.Drawing.Size(223, 21);
            this.serverPick.TabIndex = 5;
            // 
            // currentWindowInfo
            // 
            this.currentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentWindowInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.currentWindowInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.currentWindowInfo.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.currentWindowInfo.ForeColor = System.Drawing.Color.White;
            this.currentWindowInfo.Location = new System.Drawing.Point(479, 140);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(222, 117);
            this.currentWindowInfo.TabIndex = 16;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN";
            this.currentWindowInfo.UseCompatibleTextRendering = true;
            this.currentWindowInfo.UseMnemonic = false;
            // 
            // rememberMe
            // 
            this.rememberMe.AutoSize = true;
            this.rememberMe.BackColor = System.Drawing.Color.Transparent;
            this.rememberMe.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rememberMe.ForeColor = System.Drawing.Color.White;
            this.rememberMe.Location = new System.Drawing.Point(463, 322);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(210, 19);
            this.rememberMe.TabIndex = 5;
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
            this.onlineCount.Location = new System.Drawing.Point(34, 373);
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
            this.loginButton.Image = global::GameLauncher.Properties.Resources.smallbutton_enabled;
            this.loginButton.Location = new System.Drawing.Point(822, 336);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(130, 50);
            this.loginButton.TabIndex = 12;
            this.loginButton.Text = "LOG IN";
            this.loginButton.UseVisualStyleBackColor = false;
            // 
            // registerText
            // 
            this.registerText.BackColor = System.Drawing.Color.Transparent;
            this.registerText.Image = global::GameLauncher.Properties.Resources.registerButton_en;
            this.registerText.Location = new System.Drawing.Point(720, 126);
            this.registerText.Margin = new System.Windows.Forms.Padding(0);
            this.registerText.Name = "registerText";
            this.registerText.Size = new System.Drawing.Size(250, 60);
            this.registerText.TabIndex = 7;
            this.registerText.TabStop = false;
            // 
            // emailLabel
            // 
            this.emailLabel.AutoSize = true;
            this.emailLabel.BackColor = System.Drawing.Color.Transparent;
            this.emailLabel.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.emailLabel.ForeColor = System.Drawing.Color.White;
            this.emailLabel.Location = new System.Drawing.Point(460, 238);
            this.emailLabel.Name = "emailLabel";
            this.emailLabel.Size = new System.Drawing.Size(148, 19);
            this.emailLabel.TabIndex = 2;
            this.emailLabel.Text = "E-MAIL ADDRESS";
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.BackColor = System.Drawing.Color.Transparent;
            this.passwordLabel.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.passwordLabel.ForeColor = System.Drawing.Color.White;
            this.passwordLabel.Location = new System.Drawing.Point(747, 238);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(106, 19);
            this.passwordLabel.TabIndex = 3;
            this.passwordLabel.Text = "PASSWORD";
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.BorderSize = 0;
            this.registerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.ForeColor = System.Drawing.Color.White;
            this.registerButton.Image = global::GameLauncher.Properties.Resources.smallbutton_enabled;
            this.registerButton.Location = new System.Drawing.Point(822, 453);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(130, 50);
            this.registerButton.TabIndex = 19;
            this.registerButton.Text = "REGISTER";
            this.registerButton.UseVisualStyleBackColor = false;
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("settingsButton.BackgroundImage")));
            this.settingsButton.Location = new System.Drawing.Point(856, 2);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(26, 26);
            this.settingsButton.TabIndex = 21;
            this.settingsButton.TabStop = false;
            // 
            // verticalBanner
            // 
            this.verticalBanner.BackColor = System.Drawing.Color.Transparent;
            this.verticalBanner.Location = new System.Drawing.Point(31, 126);
            this.verticalBanner.Name = "verticalBanner";
            this.verticalBanner.Size = new System.Drawing.Size(371, 273);
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
            this.settingsSave.Image = global::GameLauncher.Properties.Resources.smallbutton_enabled;
            this.settingsSave.Location = new System.Drawing.Point(822, 453);
            this.settingsSave.Name = "settingsSave";
            this.settingsSave.Size = new System.Drawing.Size(130, 50);
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
            this.settingsLanguage.Location = new System.Drawing.Point(59, 225);
            this.settingsLanguage.Name = "settingsLanguage";
            this.settingsLanguage.Size = new System.Drawing.Size(210, 21);
            this.settingsLanguage.TabIndex = 24;
            // 
            // settingsLanguageText
            // 
            this.settingsLanguageText.AutoSize = true;
            this.settingsLanguageText.BackColor = System.Drawing.Color.Transparent;
            this.settingsLanguageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsLanguageText.ForeColor = System.Drawing.Color.White;
            this.settingsLanguageText.Location = new System.Drawing.Point(56, 200);
            this.settingsLanguageText.Name = "settingsLanguageText";
            this.settingsLanguageText.Size = new System.Drawing.Size(142, 18);
            this.settingsLanguageText.TabIndex = 25;
            this.settingsLanguageText.Text = "GAME LANGUAGE:";
            // 
            // settingsQuality
            // 
            this.settingsQuality.BackColor = System.Drawing.Color.White;
            this.settingsQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsQuality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsQuality.FormattingEnabled = true;
            this.settingsQuality.Location = new System.Drawing.Point(59, 288);
            this.settingsQuality.Name = "settingsQuality";
            this.settingsQuality.Size = new System.Drawing.Size(210, 21);
            this.settingsQuality.TabIndex = 26;
            // 
            // settingsQualityText
            // 
            this.settingsQualityText.AutoSize = true;
            this.settingsQualityText.BackColor = System.Drawing.Color.Transparent;
            this.settingsQualityText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsQualityText.ForeColor = System.Drawing.Color.White;
            this.settingsQualityText.Location = new System.Drawing.Point(56, 264);
            this.settingsQualityText.Name = "settingsQualityText";
            this.settingsQualityText.Size = new System.Drawing.Size(137, 18);
            this.settingsQualityText.TabIndex = 27;
            this.settingsQualityText.Text = "DOWNLOAD SIZE:";
            // 
            // settingsQualityDesc
            // 
            this.settingsQualityDesc.BackColor = System.Drawing.Color.Transparent;
            this.settingsQualityDesc.ForeColor = System.Drawing.Color.White;
            this.settingsQualityDesc.Location = new System.Drawing.Point(296, 268);
            this.settingsQualityDesc.Name = "settingsQualityDesc";
            this.settingsQualityDesc.Size = new System.Drawing.Size(427, 85);
            this.settingsQualityDesc.TabIndex = 29;
            // 
            // registerEmail
            // 
            this.registerEmail.Location = new System.Drawing.Point(59, 224);
            this.registerEmail.Name = "registerEmail";
            this.registerEmail.Size = new System.Drawing.Size(210, 20);
            this.registerEmail.TabIndex = 30;
            // 
            // registerTicket
            // 
            this.registerTicket.Location = new System.Drawing.Point(59, 416);
            this.registerTicket.Name = "registerTicket";
            this.registerTicket.Size = new System.Drawing.Size(210, 20);
            this.registerTicket.TabIndex = 31;
            // 
            // registerPassword
            // 
            this.registerPassword.Location = new System.Drawing.Point(59, 288);
            this.registerPassword.Name = "registerPassword";
            this.registerPassword.PasswordChar = '•';
            this.registerPassword.Size = new System.Drawing.Size(210, 20);
            this.registerPassword.TabIndex = 32;
            // 
            // registerConfirmPassword
            // 
            this.registerConfirmPassword.Location = new System.Drawing.Point(59, 352);
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
            this.registerEmailText.ForeColor = System.Drawing.Color.White;
            this.registerEmailText.Location = new System.Drawing.Point(56, 200);
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
            this.registerPasswordText.ForeColor = System.Drawing.Color.White;
            this.registerPasswordText.Location = new System.Drawing.Point(56, 264);
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
            this.registerTicketText.ForeColor = System.Drawing.Color.White;
            this.registerTicketText.Location = new System.Drawing.Point(56, 392);
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
            this.registerConfirmPasswordText.ForeColor = System.Drawing.Color.White;
            this.registerConfirmPasswordText.Location = new System.Drawing.Point(56, 328);
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
            this.registerAgree.ForeColor = System.Drawing.Color.White;
            this.registerAgree.Location = new System.Drawing.Point(59, 464);
            this.registerAgree.Name = "registerAgree";
            this.registerAgree.Size = new System.Drawing.Size(333, 17);
            this.registerAgree.TabIndex = 38;
            this.registerAgree.Text = "BY REGISTERING YOU AGREE TO THE TERMS OF SERVICE";
            this.registerAgree.UseVisualStyleBackColor = false;
            // 
            // playProgressText
            // 
            this.playProgressText.AutoSize = true;
            this.playProgressText.BackColor = System.Drawing.Color.Transparent;
            this.playProgressText.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.playProgressText.ForeColor = System.Drawing.Color.White;
            this.playProgressText.Location = new System.Drawing.Point(33, 471);
            this.playProgressText.Name = "playProgressText";
            this.playProgressText.Size = new System.Drawing.Size(120, 19);
            this.playProgressText.TabIndex = 10;
            this.playProgressText.Text = "PLEASE WAIT";
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.Transparent;
            this.playButton.BackgroundImage = global::GameLauncher.Properties.Resources.largebutton_enabled;
            this.playButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playButton.ForeColor = System.Drawing.Color.Transparent;
            this.playButton.Location = new System.Drawing.Point(761, 431);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(219, 86);
            this.playButton.TabIndex = 15;
            this.playButton.Text = "PLAY NOW";
            this.playButton.UseVisualStyleBackColor = false;
            // 
            // Notification
            // 
            this.Notification.Text = "notifyIcon1";
            this.Notification.Visible = true;
            // 
            // forgotPassword
            // 
            this.forgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.forgotPassword.AutoSize = true;
            this.forgotPassword.BackColor = System.Drawing.Color.Transparent;
            this.forgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.forgotPassword.Location = new System.Drawing.Point(460, 360);
            this.forgotPassword.Name = "forgotPassword";
            this.forgotPassword.Size = new System.Drawing.Size(143, 13);
            this.forgotPassword.TabIndex = 6;
            this.forgotPassword.TabStop = true;
            this.forgotPassword.Text = "I FORGOT MY PASSWORD";
            this.forgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            // 
            // playProgressTime
            // 
            this.playProgressTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.playProgressTime.BackColor = System.Drawing.Color.Transparent;
            this.playProgressTime.Cursor = System.Windows.Forms.Cursors.Default;
            this.playProgressTime.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.playProgressTime.ForeColor = System.Drawing.Color.White;
            this.playProgressTime.Location = new System.Drawing.Point(667, 471);
            this.playProgressTime.Name = "playProgressTime";
            this.playProgressTime.Size = new System.Drawing.Size(72, 19);
            this.playProgressTime.TabIndex = 42;
            this.playProgressTime.Text = "0:00:00";
            this.playProgressTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // launcherVersion
            // 
            this.launcherVersion.BackColor = System.Drawing.Color.Transparent;
            this.launcherVersion.ForeColor = System.Drawing.Color.DarkGray;
            this.launcherVersion.Location = new System.Drawing.Point(535, 520);
            this.launcherVersion.Name = "launcherVersion";
            this.launcherVersion.Size = new System.Drawing.Size(453, 33);
            this.launcherVersion.TabIndex = 2;
            this.launcherVersion.Text = "v0.0.0.0";
            this.launcherVersion.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // imageServerName
            // 
            this.imageServerName.BackColor = System.Drawing.Color.Transparent;
            this.imageServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.imageServerName.ForeColor = System.Drawing.Color.White;
            this.imageServerName.Location = new System.Drawing.Point(31, 185);
            this.imageServerName.Name = "imageServerName";
            this.imageServerName.Size = new System.Drawing.Size(370, 188);
            this.imageServerName.TabIndex = 19;
            this.imageServerName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.imageServerName.UseCompatibleTextRendering = true;
            this.imageServerName.UseMnemonic = false;
            // 
            // registerCancel
            // 
            this.registerCancel.BackColor = System.Drawing.Color.Transparent;
            this.registerCancel.FlatAppearance.BorderSize = 0;
            this.registerCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.registerCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.registerCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerCancel.ForeColor = System.Drawing.Color.White;
            this.registerCancel.Image = global::GameLauncher.Properties.Resources.cancelbutton_enabled;
            this.registerCancel.Location = new System.Drawing.Point(667, 453);
            this.registerCancel.Name = "registerCancel";
            this.registerCancel.Size = new System.Drawing.Size(130, 50);
            this.registerCancel.TabIndex = 43;
            this.registerCancel.Text = "CANCEL";
            this.registerCancel.UseVisualStyleBackColor = false;
            // 
            // logoutButton
            // 
            this.logoutButton.BackColor = System.Drawing.Color.Transparent;
            this.logoutButton.FlatAppearance.BorderSize = 0;
            this.logoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.logoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logoutButton.ForeColor = System.Drawing.Color.White;
            this.logoutButton.Image = global::GameLauncher.Properties.Resources.smallbutton_enabled;
            this.logoutButton.Location = new System.Drawing.Point(822, 336);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(130, 50);
            this.logoutButton.TabIndex = 44;
            this.logoutButton.Text = "LOG OUT";
            this.logoutButton.UseVisualStyleBackColor = false;
            // 
            // welcomeBack
            // 
            this.welcomeBack.AutoSize = true;
            this.welcomeBack.BackColor = System.Drawing.Color.Transparent;
            this.welcomeBack.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.welcomeBack.ForeColor = System.Drawing.Color.White;
            this.welcomeBack.Location = new System.Drawing.Point(460, 238);
            this.welcomeBack.Name = "welcomeBack";
            this.welcomeBack.Size = new System.Drawing.Size(158, 19);
            this.welcomeBack.TabIndex = 46;
            this.welcomeBack.Text = "WELCOME BACK, ";
            // 
            // settingsLanguageDesc
            // 
            this.settingsLanguageDesc.BackColor = System.Drawing.Color.Transparent;
            this.settingsLanguageDesc.ForeColor = System.Drawing.Color.White;
            this.settingsLanguageDesc.Location = new System.Drawing.Point(296, 204);
            this.settingsLanguageDesc.Name = "settingsLanguageDesc";
            this.settingsLanguageDesc.Size = new System.Drawing.Size(427, 56);
            this.settingsLanguageDesc.TabIndex = 28;
            this.settingsLanguageDesc.Text = "Select the language that the game text and audio should be displayed in. This wil" +
    "l not affect your chat or server options.";
            // 
            // settingsUILang
            // 
            this.settingsUILang.BackColor = System.Drawing.Color.White;
            this.settingsUILang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsUILang.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsUILang.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsUILang.FormattingEnabled = true;
            this.settingsUILang.Location = new System.Drawing.Point(59, 353);
            this.settingsUILang.Name = "settingsUILang";
            this.settingsUILang.Size = new System.Drawing.Size(210, 21);
            this.settingsUILang.TabIndex = 48;
            // 
            // settingsUILangText
            // 
            this.settingsUILangText.AutoSize = true;
            this.settingsUILangText.BackColor = System.Drawing.Color.Transparent;
            this.settingsUILangText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsUILangText.ForeColor = System.Drawing.Color.White;
            this.settingsUILangText.Location = new System.Drawing.Point(56, 328);
            this.settingsUILangText.Name = "settingsUILangText";
            this.settingsUILangText.Size = new System.Drawing.Size(180, 18);
            this.settingsUILangText.TabIndex = 49;
            this.settingsUILangText.Text = "LAUNCHER LANGUAGE:";
            // 
            // moreLanguages
            // 
            this.moreLanguages.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.moreLanguages.AutoSize = true;
            this.moreLanguages.BackColor = System.Drawing.Color.Transparent;
            this.moreLanguages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.moreLanguages.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.moreLanguages.LinkColor = System.Drawing.Color.White;
            this.moreLanguages.Location = new System.Drawing.Point(56, 376);
            this.moreLanguages.Name = "moreLanguages";
            this.moreLanguages.Size = new System.Drawing.Size(101, 13);
            this.moreLanguages.TabIndex = 53;
            this.moreLanguages.TabStop = true;
            this.moreLanguages.Text = "More languages!";
            this.moreLanguages.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::GameLauncher.Properties.Resources.SBRW512;
            this.pictureBox1.Location = new System.Drawing.Point(31, 7);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(231, 72);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // translatedBy
            // 
            this.translatedBy.AutoSize = true;
            this.translatedBy.BackColor = System.Drawing.Color.Transparent;
            this.translatedBy.ForeColor = System.Drawing.Color.DarkGray;
            this.translatedBy.Location = new System.Drawing.Point(12, 538);
            this.translatedBy.Name = "translatedBy";
            this.translatedBy.Size = new System.Drawing.Size(105, 13);
            this.translatedBy.TabIndex = 55;
            this.translatedBy.Text = "Translated by: meme";
            // 
            // settingsGameFiles
            // 
            this.settingsGameFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGameFiles.Location = new System.Drawing.Point(59, 426);
            this.settingsGameFiles.Name = "settingsGameFiles";
            this.settingsGameFiles.Size = new System.Drawing.Size(210, 23);
            this.settingsGameFiles.TabIndex = 57;
            this.settingsGameFiles.Text = "Change GameFiles Path";
            this.settingsGameFiles.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settingsGameFiles.UseVisualStyleBackColor = true;
            // 
            // settingsGameFilesCurrent
            // 
            this.settingsGameFilesCurrent.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsGameFilesCurrent.AutoSize = true;
            this.settingsGameFilesCurrent.BackColor = System.Drawing.Color.Transparent;
            this.settingsGameFilesCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGameFilesCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.settingsGameFilesCurrent.LinkColor = System.Drawing.Color.White;
            this.settingsGameFilesCurrent.Location = new System.Drawing.Point(296, 428);
            this.settingsGameFilesCurrent.Name = "settingsGameFilesCurrent";
            this.settingsGameFilesCurrent.Size = new System.Drawing.Size(86, 13);
            this.settingsGameFilesCurrent.TabIndex = 58;
            this.settingsGameFilesCurrent.TabStop = true;
            this.settingsGameFilesCurrent.Text = "Current Path: ";
            this.settingsGameFilesCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // settingsGamePathText
            // 
            this.settingsGamePathText.AutoSize = true;
            this.settingsGamePathText.BackColor = System.Drawing.Color.Transparent;
            this.settingsGamePathText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGamePathText.ForeColor = System.Drawing.Color.White;
            this.settingsGamePathText.Location = new System.Drawing.Point(56, 402);
            this.settingsGamePathText.Name = "settingsGamePathText";
            this.settingsGamePathText.Size = new System.Drawing.Size(139, 18);
            this.settingsGamePathText.TabIndex = 60;
            this.settingsGamePathText.Text = "GAMEFILES PATH:";
            this.settingsGamePathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // errorTicket
            // 
            this.errorTicket.AutoSize = true;
            this.errorTicket.BackColor = System.Drawing.Color.Transparent;
            this.errorTicket.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.errorTicket.ForeColor = System.Drawing.Color.Red;
            this.errorTicket.Location = new System.Drawing.Point(296, 419);
            this.errorTicket.Name = "errorTicket";
            this.errorTicket.Size = new System.Drawing.Size(0, 13);
            this.errorTicket.TabIndex = 62;
            // 
            // errorConfirm
            // 
            this.errorConfirm.AutoSize = true;
            this.errorConfirm.BackColor = System.Drawing.Color.Transparent;
            this.errorConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.errorConfirm.ForeColor = System.Drawing.Color.Red;
            this.errorConfirm.Location = new System.Drawing.Point(296, 355);
            this.errorConfirm.Name = "errorConfirm";
            this.errorConfirm.Size = new System.Drawing.Size(0, 13);
            this.errorConfirm.TabIndex = 63;
            // 
            // errorPassword
            // 
            this.errorPassword.AutoSize = true;
            this.errorPassword.BackColor = System.Drawing.Color.Transparent;
            this.errorPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.errorPassword.ForeColor = System.Drawing.Color.Red;
            this.errorPassword.Location = new System.Drawing.Point(296, 291);
            this.errorPassword.Name = "errorPassword";
            this.errorPassword.Size = new System.Drawing.Size(0, 13);
            this.errorPassword.TabIndex = 64;
            // 
            // errorEmail
            // 
            this.errorEmail.AutoSize = true;
            this.errorEmail.BackColor = System.Drawing.Color.Transparent;
            this.errorEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.errorEmail.ForeColor = System.Drawing.Color.Red;
            this.errorEmail.Location = new System.Drawing.Point(296, 228);
            this.errorEmail.Name = "errorEmail";
            this.errorEmail.Size = new System.Drawing.Size(0, 13);
            this.errorEmail.TabIndex = 65;
            // 
            // errorTOS
            // 
            this.errorTOS.AutoSize = true;
            this.errorTOS.BackColor = System.Drawing.Color.Transparent;
            this.errorTOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.errorTOS.ForeColor = System.Drawing.Color.Red;
            this.errorTOS.Location = new System.Drawing.Point(56, 484);
            this.errorTOS.Name = "errorTOS";
            this.errorTOS.Size = new System.Drawing.Size(0, 13);
            this.errorTOS.TabIndex = 66;
            // 
            // addServer
            // 
            this.addServer.Location = new System.Drawing.Point(947, 32);
            this.addServer.Name = "addServer";
            this.addServer.Size = new System.Drawing.Size(23, 23);
            this.addServer.TabIndex = 68;
            this.addServer.Text = "+";
            this.addServer.UseVisualStyleBackColor = true;
            // 
            // showmap
            // 
            this.showmap.BackColor = System.Drawing.Color.Transparent;
            this.showmap.ForeColor = System.Drawing.Color.DarkGray;
            this.showmap.Location = new System.Drawing.Point(829, 58);
            this.showmap.Name = "showmap";
            this.showmap.Size = new System.Drawing.Size(141, 15);
            this.showmap.TabIndex = 70;
            this.showmap.Text = "[ SHOW MAP ]";
            this.showmap.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.showmap.Visible = false;
            // 
            // inputeditor
            // 
            this.inputeditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.inputeditor.Location = new System.Drawing.Point(-10, -30);
            this.inputeditor.Name = "inputeditor";
            this.inputeditor.Size = new System.Drawing.Size(210, 23);
            this.inputeditor.TabIndex = 72;
            this.inputeditor.Text = "Edit Game Controls";
            this.inputeditor.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.inputeditor.UseVisualStyleBackColor = true;
            // 
            // legacyLaunch
            // 
            this.legacyLaunch.AutoSize = true;
            this.legacyLaunch.BackColor = System.Drawing.Color.Transparent;
            this.legacyLaunch.ForeColor = System.Drawing.Color.White;
            this.legacyLaunch.Location = new System.Drawing.Point(59, 455);
            this.legacyLaunch.Name = "legacyLaunch";
            this.legacyLaunch.Size = new System.Drawing.Size(189, 17);
            this.legacyLaunch.TabIndex = 74;
            this.legacyLaunch.Text = "Use pre-1.9.1.9 launching function";
            this.legacyLaunch.UseVisualStyleBackColor = false;
            // 
            // allowedCountriesLabel
            // 
            this.allowedCountriesLabel.BackColor = System.Drawing.Color.Transparent;
            this.allowedCountriesLabel.ForeColor = System.Drawing.Color.Yellow;
            this.allowedCountriesLabel.Location = new System.Drawing.Point(675, -11);
            this.allowedCountriesLabel.Name = "allowedCountriesLabel";
            this.allowedCountriesLabel.Size = new System.Drawing.Size(10, 10);
            this.allowedCountriesLabel.TabIndex = 76;
            // 
            // settingsUILangDesc
            // 
            this.settingsUILangDesc.BackColor = System.Drawing.Color.Transparent;
            this.settingsUILangDesc.ForeColor = System.Drawing.Color.White;
            this.settingsUILangDesc.Location = new System.Drawing.Point(296, 332);
            this.settingsUILangDesc.Name = "settingsUILangDesc";
            this.settingsUILangDesc.Size = new System.Drawing.Size(427, 56);
            this.settingsUILangDesc.TabIndex = 51;
            this.settingsUILangDesc.Text = "Select the launcher language that the game text and audio should be displayed in." +
    " Remember to restart your launcher.";
            // 
            // playProgress
            // 
            this.playProgress.BackColor = System.Drawing.Color.Transparent;
            this.playProgress.BackgroundColor = System.Drawing.Color.Black;
            this.playProgress.Border = false;
            this.playProgress.GradiantPosition = GameLauncherReborn.ProgressBarEx.GradiantArea.None;
            this.playProgress.Image = null;
            this.playProgress.Location = new System.Drawing.Point(37, 494);
            this.playProgress.Name = "playProgress";
            this.playProgress.Size = new System.Drawing.Size(700, 5);
            this.playProgress.Text = "downloadProgress";
            // 
            // MainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1000, 562);
            this.Controls.Add(this.allowedCountriesLabel);
            this.Controls.Add(this.legacyLaunch);
            this.Controls.Add(this.inputeditor);
            this.Controls.Add(this.showmap);
            this.Controls.Add(this.addServer);
            this.Controls.Add(this.errorTOS);
            this.Controls.Add(this.errorEmail);
            this.Controls.Add(this.errorPassword);
            this.Controls.Add(this.errorConfirm);
            this.Controls.Add(this.errorTicket);
            this.Controls.Add(this.settingsGamePathText);
            this.Controls.Add(this.settingsGameFilesCurrent);
            this.Controls.Add(this.translatedBy);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.moreLanguages);
            this.Controls.Add(this.settingsUILangDesc);
            this.Controls.Add(this.settingsUILangText);
            this.Controls.Add(this.welcomeBack);
            this.Controls.Add(this.playProgress);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.launcherVersion);
            this.Controls.Add(this.playProgressTime);
            this.Controls.Add(this.playProgressText);
            this.Controls.Add(this.registerAgree);
            this.Controls.Add(this.registerConfirmPasswordText);
            this.Controls.Add(this.registerTicketText);
            this.Controls.Add(this.registerTicket);
            this.Controls.Add(this.settingsQualityText);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.forgotPassword);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.emailLabel);
            this.Controls.Add(this.registerText);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.onlineCount);
            this.Controls.Add(this.rememberMe);
            this.Controls.Add(this.currentWindowInfo);
            this.Controls.Add(this.serverPick);
            this.Controls.Add(this.password);
            this.Controls.Add(this.email);
            this.Controls.Add(this.minimizebtn);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.settingsSave);
            this.Controls.Add(this.settingsLanguageDesc);
            this.Controls.Add(this.registerEmail);
            this.Controls.Add(this.settingsQualityDesc);
            this.Controls.Add(this.registerCancel);
            this.Controls.Add(this.settingsLanguageText);
            this.Controls.Add(this.registerEmailText);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.logoutButton);
            this.Controls.Add(this.registerPassword);
            this.Controls.Add(this.registerConfirmPassword);
            this.Controls.Add(this.registerPasswordText);
            this.Controls.Add(this.imageServerName);
            this.Controls.Add(this.settingsGameFiles);
            this.Controls.Add(this.settingsUILang);
            this.Controls.Add(this.settingsQuality);
            this.Controls.Add(this.settingsLanguage);
            this.Controls.Add(this.verticalBanner);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Load += new System.EventHandler(this.mainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.registerText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox closebtn;
        private System.Windows.Forms.PictureBox minimizebtn;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.ComboBox serverPick;
        private System.Windows.Forms.Label currentWindowInfo;
        private System.Windows.Forms.CheckBox rememberMe;
        private System.Windows.Forms.Timer Timeout;
        private System.Windows.Forms.Label onlineCount;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.PictureBox registerText;
        private System.Windows.Forms.Label emailLabel;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.PictureBox settingsButton;
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
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.NotifyIcon Notification;
        private System.Windows.Forms.LinkLabel forgotPassword;
        internal System.Windows.Forms.Label playProgressText;
        internal System.Windows.Forms.Label playProgressTime;
        private System.Windows.Forms.Label launcherVersion;
        private System.Windows.Forms.Label imageServerName;
        private System.Windows.Forms.Button registerCancel;
        private GameLauncherReborn.ProgressBarEx playProgress;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Label welcomeBack;
        private System.Windows.Forms.ComboBox settingsUILang;
        private System.Windows.Forms.Label settingsUILangText;
        private System.Windows.Forms.LinkLabel moreLanguages;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label translatedBy;
        private System.Windows.Forms.Button settingsGameFiles;
        private System.Windows.Forms.LinkLabel settingsGameFilesCurrent;
        private System.Windows.Forms.Label settingsGamePathText;
        private System.Windows.Forms.Label errorTicket;
        private System.Windows.Forms.Label errorConfirm;
        private System.Windows.Forms.Label errorPassword;
        private System.Windows.Forms.Label errorEmail;
        private System.Windows.Forms.Label errorTOS;
        private System.Windows.Forms.Button addServer;
        private System.Windows.Forms.Label showmap;
        private System.Windows.Forms.Button inputeditor;
        private System.Windows.Forms.CheckBox legacyLaunch;
        private System.Windows.Forms.Label allowedCountriesLabel;
        private System.Windows.Forms.Label settingsUILangDesc;
    }
}
