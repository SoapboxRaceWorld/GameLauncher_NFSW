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
            this.email = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.serverPick = new System.Windows.Forms.ComboBox();
            this.currentWindowInfo = new System.Windows.Forms.Label();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this.Timeout = new System.Windows.Forms.Timer(this.components);
            this.loginButton = new System.Windows.Forms.Button();
            this.registerButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.PictureBox();
            this.verticalBanner = new System.Windows.Forms.PictureBox();
            this.settingsSave = new System.Windows.Forms.Button();
            this.settingsLanguage = new System.Windows.Forms.ComboBox();
            this.settingsLanguageText = new System.Windows.Forms.Label();
            this.settingsQuality = new System.Windows.Forms.ComboBox();
            this.settingsQualityText = new System.Windows.Forms.Label();
            this.registerEmail = new System.Windows.Forms.TextBox();
            this.registerTicket = new System.Windows.Forms.TextBox();
            this.registerPassword = new System.Windows.Forms.TextBox();
            this.registerConfirmPassword = new System.Windows.Forms.TextBox();
            this.registerAgree = new System.Windows.Forms.CheckBox();
            this.playProgressText = new System.Windows.Forms.Label();
            this.playButton = new System.Windows.Forms.Button();
            this.Notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.forgotPassword = new System.Windows.Forms.LinkLabel();
            this.launcherStatusDesc = new System.Windows.Forms.Label();
            this.imageServerName = new System.Windows.Forms.Label();
            this.registerCancel = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.logo = new System.Windows.Forms.PictureBox();
            this.translatedBy = new System.Windows.Forms.Label();
            this.settingsGameFiles = new System.Windows.Forms.Button();
            this.settingsGameFilesCurrent = new System.Windows.Forms.LinkLabel();
            this.settingsGamePathText = new System.Windows.Forms.Label();
            this.addServer = new System.Windows.Forms.Button();
            this.launcherIconStatus = new System.Windows.Forms.PictureBox();
            this.launcherStatusText = new System.Windows.Forms.Label();
            this.ServerStatusDesc = new System.Windows.Forms.Label();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.ServerStatusIcon = new System.Windows.Forms.PictureBox();
            this.registerText = new System.Windows.Forms.Button();
            this.cdnText = new System.Windows.Forms.Label();
            this.cdnPick = new System.Windows.Forms.ComboBox();
            this.vfilesButton = new System.Windows.Forms.Button();
            this.wordFilterCheck = new System.Windows.Forms.CheckBox();
            this.SelectServerBtn = new System.Windows.Forms.Button();
            this.proxyCheckbox = new System.Windows.Forms.CheckBox();
            this.extractingProgress = new GameLauncherReborn.ProgressBarEx();
            this.playProgress = new GameLauncherReborn.ProgressBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.Transparent;
            this.closebtn.BackgroundImage = global::GameLauncher.Properties.Resources.close;
            this.closebtn.Location = new System.Drawing.Point(838, 24);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(24, 24);
            this.closebtn.TabIndex = 0;
            this.closebtn.TabStop = false;
            this.closebtn.Click += new System.EventHandler(this.closebtn_Click);
            // 
            // email
            // 
            this.email.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.email.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.email.Font = new System.Drawing.Font("Arial", 14F);
            this.email.ForeColor = System.Drawing.Color.White;
            this.email.Location = new System.Drawing.Point(650, 195);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(187, 22);
            this.email.TabIndex = 2;
            // 
            // password
            // 
            this.password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Font = new System.Drawing.Font("Arial", 14F);
            this.password.ForeColor = System.Drawing.Color.White;
            this.password.Location = new System.Drawing.Point(650, 247);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(187, 22);
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
            this.serverPick.Location = new System.Drawing.Point(599, 65);
            this.serverPick.Name = "serverPick";
            this.serverPick.Size = new System.Drawing.Size(233, 21);
            this.serverPick.TabIndex = 5;
            // 
            // currentWindowInfo
            // 
            this.currentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentWindowInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.currentWindowInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.currentWindowInfo.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.currentWindowInfo.ForeColor = System.Drawing.Color.White;
            this.currentWindowInfo.Location = new System.Drawing.Point(653, 106);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(194, 61);
            this.currentWindowInfo.TabIndex = 16;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN";
            this.currentWindowInfo.UseCompatibleTextRendering = true;
            this.currentWindowInfo.UseMnemonic = false;
            // 
            // rememberMe
            // 
            this.rememberMe.AutoSize = true;
            this.rememberMe.BackColor = System.Drawing.Color.Transparent;
            this.rememberMe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rememberMe.ForeColor = System.Drawing.Color.White;
            this.rememberMe.Location = new System.Drawing.Point(615, 285);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(159, 19);
            this.rememberMe.TabIndex = 5;
            this.rememberMe.Text = "REMEMBER MY LOGIN";
            this.rememberMe.UseVisualStyleBackColor = false;
            // 
            // Timeout
            // 
            this.Timeout.Interval = 3000;
            // 
            // loginButton
            // 
            this.loginButton.BackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatAppearance.BorderSize = 0;
            this.loginButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.loginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginButton.ForeColor = System.Drawing.Color.White;
            this.loginButton.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.loginButton.Location = new System.Drawing.Point(615, 395);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(231, 35);
            this.loginButton.TabIndex = 12;
            this.loginButton.Text = "LOG IN";
            this.loginButton.UseVisualStyleBackColor = false;
            // 
            // registerButton
            // 
            this.registerButton.BackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.BorderSize = 0;
            this.registerButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.registerButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.registerButton.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.registerButton.Location = new System.Drawing.Point(614, 400);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(231, 35);
            this.registerButton.TabIndex = 19;
            this.registerButton.Text = "REGISTER";
            this.registerButton.UseVisualStyleBackColor = false;
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.BackgroundImage = global::GameLauncher.Properties.Resources.settingsbtn1;
            this.settingsButton.Location = new System.Drawing.Point(805, 24);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(24, 24);
            this.settingsButton.TabIndex = 21;
            this.settingsButton.TabStop = false;
            // 
            // verticalBanner
            // 
            this.verticalBanner.BackColor = System.Drawing.Color.Transparent;
            this.verticalBanner.Location = new System.Drawing.Point(39, 144);
            this.verticalBanner.Name = "verticalBanner";
            this.verticalBanner.Size = new System.Drawing.Size(523, 223);
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
            this.settingsSave.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.settingsSave.Location = new System.Drawing.Point(720, 437);
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
            this.settingsLanguage.Location = new System.Drawing.Point(46, 154);
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
            this.settingsLanguageText.Location = new System.Drawing.Point(43, 130);
            this.settingsLanguageText.Name = "settingsLanguageText";
            this.settingsLanguageText.Size = new System.Drawing.Size(142, 18);
            this.settingsLanguageText.TabIndex = 25;
            this.settingsLanguageText.Text = "GAME LANGUAGE:";
            // 
            // settingsQuality
            // 
            this.settingsQuality.BackColor = System.Drawing.Color.White;
            this.settingsQuality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsQuality.Enabled = false;
            this.settingsQuality.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsQuality.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsQuality.FormattingEnabled = true;
            this.settingsQuality.Location = new System.Drawing.Point(299, 153);
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
            this.settingsQualityText.Location = new System.Drawing.Point(296, 130);
            this.settingsQualityText.Name = "settingsQualityText";
            this.settingsQualityText.Size = new System.Drawing.Size(137, 18);
            this.settingsQualityText.TabIndex = 27;
            this.settingsQualityText.Text = "DOWNLOAD SIZE:";
            // 
            // registerEmail
            // 
            this.registerEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.registerEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerEmail.Font = new System.Drawing.Font("Arial", 14F);
            this.registerEmail.ForeColor = System.Drawing.Color.White;
            this.registerEmail.Location = new System.Drawing.Point(650, 172);
            this.registerEmail.Name = "registerEmail";
            this.registerEmail.Size = new System.Drawing.Size(187, 22);
            this.registerEmail.TabIndex = 30;
            // 
            // registerTicket
            // 
            this.registerTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.registerTicket.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerTicket.Font = new System.Drawing.Font("Arial", 14F);
            this.registerTicket.ForeColor = System.Drawing.Color.White;
            this.registerTicket.Location = new System.Drawing.Point(650, 319);
            this.registerTicket.Name = "registerTicket";
            this.registerTicket.Size = new System.Drawing.Size(187, 22);
            this.registerTicket.TabIndex = 31;
            // 
            // registerPassword
            // 
            this.registerPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.registerPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerPassword.Font = new System.Drawing.Font("Arial", 14F);
            this.registerPassword.ForeColor = System.Drawing.Color.White;
            this.registerPassword.Location = new System.Drawing.Point(650, 221);
            this.registerPassword.Name = "registerPassword";
            this.registerPassword.Size = new System.Drawing.Size(187, 22);
            this.registerPassword.TabIndex = 32;
            this.registerPassword.UseSystemPasswordChar = true;
            // 
            // registerConfirmPassword
            // 
            this.registerConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(28)))), ((int)(((byte)(36)))));
            this.registerConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerConfirmPassword.Font = new System.Drawing.Font("Arial", 14F);
            this.registerConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.registerConfirmPassword.Location = new System.Drawing.Point(650, 270);
            this.registerConfirmPassword.Name = "registerConfirmPassword";
            this.registerConfirmPassword.Size = new System.Drawing.Size(187, 22);
            this.registerConfirmPassword.TabIndex = 33;
            this.registerConfirmPassword.UseSystemPasswordChar = true;
            // 
            // registerAgree
            // 
            this.registerAgree.BackColor = System.Drawing.Color.Transparent;
            this.registerAgree.ForeColor = System.Drawing.Color.White;
            this.registerAgree.Location = new System.Drawing.Point(615, 354);
            this.registerAgree.Name = "registerAgree";
            this.registerAgree.Size = new System.Drawing.Size(232, 35);
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
            this.playProgressText.Location = new System.Drawing.Point(54, 422);
            this.playProgressText.Name = "playProgressText";
            this.playProgressText.Size = new System.Drawing.Size(120, 19);
            this.playProgressText.TabIndex = 10;
            this.playProgressText.Text = "PLEASE WAIT";
            // 
            // playButton
            // 
            this.playButton.BackColor = System.Drawing.Color.Transparent;
            this.playButton.BackgroundImage = global::GameLauncher.Properties.Resources.playbutton;
            this.playButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.playButton.FlatAppearance.BorderSize = 0;
            this.playButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.playButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.playButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.playButton.ForeColor = System.Drawing.Color.Transparent;
            this.playButton.Location = new System.Drawing.Point(616, 406);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(230, 63);
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
            this.forgotPassword.Location = new System.Drawing.Point(612, 307);
            this.forgotPassword.Name = "forgotPassword";
            this.forgotPassword.Size = new System.Drawing.Size(143, 13);
            this.forgotPassword.TabIndex = 6;
            this.forgotPassword.TabStop = true;
            this.forgotPassword.Text = "I FORGOT MY PASSWORD";
            this.forgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            // 
            // launcherStatusDesc
            // 
            this.launcherStatusDesc.AutoSize = true;
            this.launcherStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.launcherStatusDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.launcherStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.launcherStatusDesc.Location = new System.Drawing.Point(65, 486);
            this.launcherStatusDesc.Name = "launcherStatusDesc";
            this.launcherStatusDesc.Size = new System.Drawing.Size(146, 13);
            this.launcherStatusDesc.TabIndex = 5;
            this.launcherStatusDesc.Text = "Version : 2.0.0.0-Build123456";
            this.launcherStatusDesc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // imageServerName
            // 
            this.imageServerName.BackColor = System.Drawing.Color.Transparent;
            this.imageServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.imageServerName.ForeColor = System.Drawing.Color.White;
            this.imageServerName.Location = new System.Drawing.Point(41, 148);
            this.imageServerName.Name = "imageServerName";
            this.imageServerName.Size = new System.Drawing.Size(519, 259);
            this.imageServerName.TabIndex = 19;
            this.imageServerName.TextAlign = System.Drawing.ContentAlignment.BottomRight;
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
            this.registerCancel.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.registerCancel.Location = new System.Drawing.Point(614, 440);
            this.registerCancel.Name = "registerCancel";
            this.registerCancel.Size = new System.Drawing.Size(231, 35);
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
            this.logoutButton.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.logoutButton.Location = new System.Drawing.Point(615, 364);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(231, 35);
            this.logoutButton.TabIndex = 44;
            this.logoutButton.Text = "LOG OUT";
            this.logoutButton.UseVisualStyleBackColor = false;
            // 
            // logo
            // 
            this.logo.BackColor = System.Drawing.Color.Transparent;
            this.logo.Image = global::GameLauncher.Properties.Resources.logo;
            this.logo.Location = new System.Drawing.Point(38, 34);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(227, 60);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logo.TabIndex = 0;
            this.logo.TabStop = false;
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
            this.settingsGameFiles.Location = new System.Drawing.Point(46, 216);
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
            this.settingsGameFilesCurrent.Location = new System.Drawing.Point(46, 242);
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
            this.settingsGamePathText.Location = new System.Drawing.Point(46, 194);
            this.settingsGamePathText.Name = "settingsGamePathText";
            this.settingsGamePathText.Size = new System.Drawing.Size(139, 18);
            this.settingsGamePathText.TabIndex = 60;
            this.settingsGamePathText.Text = "GAMEFILES PATH:";
            this.settingsGamePathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // addServer
            // 
            this.addServer.Location = new System.Drawing.Point(838, 64);
            this.addServer.Name = "addServer";
            this.addServer.Size = new System.Drawing.Size(24, 23);
            this.addServer.TabIndex = 68;
            this.addServer.Text = "+";
            this.addServer.UseVisualStyleBackColor = true;
            // 
            // launcherIconStatus
            // 
            this.launcherIconStatus.BackColor = System.Drawing.Color.Transparent;
            this.launcherIconStatus.Image = global::GameLauncher.Properties.Resources.ac_success;
            this.launcherIconStatus.Location = new System.Drawing.Point(38, 478);
            this.launcherIconStatus.Name = "launcherIconStatus";
            this.launcherIconStatus.Size = new System.Drawing.Size(21, 24);
            this.launcherIconStatus.TabIndex = 79;
            this.launcherIconStatus.TabStop = false;
            // 
            // launcherStatusText
            // 
            this.launcherStatusText.BackColor = System.Drawing.Color.Transparent;
            this.launcherStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.launcherStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.launcherStatusText.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.launcherStatusText.Location = new System.Drawing.Point(65, 476);
            this.launcherStatusText.Name = "launcherStatusText";
            this.launcherStatusText.Size = new System.Drawing.Size(199, 15);
            this.launcherStatusText.TabIndex = 4;
            this.launcherStatusText.Text = "Anti-Cheat System - Activated";
            this.launcherStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.launcherStatusText.UseCompatibleTextRendering = true;
            // 
            // ServerStatusDesc
            // 
            this.ServerStatusDesc.AutoSize = true;
            this.ServerStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.ServerStatusDesc.Location = new System.Drawing.Point(326, 486);
            this.ServerStatusDesc.Name = "ServerStatusDesc";
            this.ServerStatusDesc.Size = new System.Drawing.Size(0, 13);
            this.ServerStatusDesc.TabIndex = 8;
            this.ServerStatusDesc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerStatusText.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ServerStatusText.Location = new System.Drawing.Point(326, 476);
            this.ServerStatusText.Name = "ServerStatusText";
            this.ServerStatusText.Size = new System.Drawing.Size(234, 15);
            this.ServerStatusText.TabIndex = 7;
            this.ServerStatusText.Text = "Server Status - Pinging";
            this.ServerStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerStatusIcon
            // 
            this.ServerStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusIcon.Image = global::GameLauncher.Properties.Resources.webicon;
            this.ServerStatusIcon.Location = new System.Drawing.Point(299, 478);
            this.ServerStatusIcon.Name = "ServerStatusIcon";
            this.ServerStatusIcon.Size = new System.Drawing.Size(21, 24);
            this.ServerStatusIcon.TabIndex = 6;
            this.ServerStatusIcon.TabStop = false;
            // 
            // registerText
            // 
            this.registerText.BackColor = System.Drawing.Color.Transparent;
            this.registerText.FlatAppearance.BorderSize = 0;
            this.registerText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.registerText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.registerText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.registerText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.registerText.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.registerText.Location = new System.Drawing.Point(615, 442);
            this.registerText.Name = "registerText";
            this.registerText.Size = new System.Drawing.Size(231, 35);
            this.registerText.TabIndex = 82;
            this.registerText.Text = "REGISTER";
            this.registerText.UseVisualStyleBackColor = false;
            // 
            // cdnText
            // 
            this.cdnText.AutoSize = true;
            this.cdnText.BackColor = System.Drawing.Color.Transparent;
            this.cdnText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cdnText.ForeColor = System.Drawing.Color.White;
            this.cdnText.Location = new System.Drawing.Point(296, 194);
            this.cdnText.Name = "cdnText";
            this.cdnText.Size = new System.Drawing.Size(45, 18);
            this.cdnText.TabIndex = 83;
            this.cdnText.Text = "CDN:";
            this.cdnText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // cdnPick
            // 
            this.cdnPick.BackColor = System.Drawing.Color.White;
            this.cdnPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cdnPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.cdnPick.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.cdnPick.FormattingEnabled = true;
            this.cdnPick.Location = new System.Drawing.Point(299, 218);
            this.cdnPick.Name = "cdnPick";
            this.cdnPick.Size = new System.Drawing.Size(210, 21);
            this.cdnPick.TabIndex = 84;
            // 
            // vfilesButton
            // 
            this.vfilesButton.AutoSize = true;
            this.vfilesButton.BackColor = System.Drawing.Color.Transparent;
            this.vfilesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.vfilesButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.vfilesButton.Location = new System.Drawing.Point(49, 269);
            this.vfilesButton.Name = "vfilesButton";
            this.vfilesButton.Size = new System.Drawing.Size(207, 23);
            this.vfilesButton.TabIndex = 96;
            this.vfilesButton.Text = "Validate Game Files";
            this.vfilesButton.UseVisualStyleBackColor = false;
            // 
            // wordFilterCheck
            // 
            this.wordFilterCheck.AutoSize = true;
            this.wordFilterCheck.BackColor = System.Drawing.Color.Transparent;
            this.wordFilterCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.wordFilterCheck.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.wordFilterCheck.Location = new System.Drawing.Point(49, 303);
            this.wordFilterCheck.Name = "wordFilterCheck";
            this.wordFilterCheck.Size = new System.Drawing.Size(235, 17);
            this.wordFilterCheck.TabIndex = 93;
            this.wordFilterCheck.Text = "Disable Word Filtering on Game Chat";
            this.wordFilterCheck.UseVisualStyleBackColor = false;
            // 
            // SelectServerBtn
            // 
            this.SelectServerBtn.Location = new System.Drawing.Point(889, 12);
            this.SelectServerBtn.Name = "SelectServerBtn";
            this.SelectServerBtn.Size = new System.Drawing.Size(228, 23);
            this.SelectServerBtn.TabIndex = 99;
            this.SelectServerBtn.Text = "Select Server";
            this.SelectServerBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SelectServerBtn.UseVisualStyleBackColor = true;
            this.SelectServerBtn.Click += new System.EventHandler(this.SelectServerBtn_Click);
            // 
            // proxyCheckbox
            // 
            this.proxyCheckbox.AutoSize = true;
            this.proxyCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.proxyCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.proxyCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.proxyCheckbox.Location = new System.Drawing.Point(49, 324);
            this.proxyCheckbox.Name = "proxyCheckbox";
            this.proxyCheckbox.Size = new System.Drawing.Size(200, 17);
            this.proxyCheckbox.TabIndex = 102;
            this.proxyCheckbox.Text = "Disable DiscordRPC and Proxy";
            this.proxyCheckbox.UseVisualStyleBackColor = false;
            // 
            // extractingProgress
            // 
            this.extractingProgress.BackColor = System.Drawing.Color.Transparent;
            this.extractingProgress.BackgroundColor = System.Drawing.Color.Black;
            this.extractingProgress.Border = false;
            this.extractingProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.extractingProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.extractingProgress.GradiantPosition = GameLauncherReborn.ProgressBarEx.GradiantArea.None;
            this.extractingProgress.Image = global::GameLauncher.Properties.Resources.progress;
            this.extractingProgress.Location = new System.Drawing.Point(41, 448);
            this.extractingProgress.Name = "extractingProgress";
            this.extractingProgress.ProgressColor = System.Drawing.Color.Green;
            this.extractingProgress.RoundedCorners = false;
            this.extractingProgress.Size = new System.Drawing.Size(519, 13);
            this.extractingProgress.Text = "downloadProgress";
            // 
            // playProgress
            // 
            this.playProgress.BackColor = System.Drawing.Color.Transparent;
            this.playProgress.BackgroundColor = System.Drawing.Color.Black;
            this.playProgress.Border = false;
            this.playProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.playProgress.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.playProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.playProgress.GradiantPosition = GameLauncherReborn.ProgressBarEx.GradiantArea.None;
            this.playProgress.Image = global::GameLauncher.Properties.Resources.progressgrayscale;
            this.playProgress.Location = new System.Drawing.Point(41, 448);
            this.playProgress.Name = "playProgress";
            this.playProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.playProgress.RoundedCorners = false;
            this.playProgress.Size = new System.Drawing.Size(519, 13);
            this.playProgress.Text = "downloadProgress";
            // 
            // MainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.loginbg;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(891, 529);
            this.Controls.Add(this.verticalBanner);
            this.Controls.Add(this.proxyCheckbox);
            this.Controls.Add(this.SelectServerBtn);
            this.Controls.Add(this.vfilesButton);
            this.Controls.Add(this.wordFilterCheck);
            this.Controls.Add(this.cdnPick);
            this.Controls.Add(this.cdnText);
            this.Controls.Add(this.ServerStatusIcon);
            this.Controls.Add(this.ServerStatusText);
            this.Controls.Add(this.ServerStatusDesc);
            this.Controls.Add(this.launcherStatusText);
            this.Controls.Add(this.launcherIconStatus);
            this.Controls.Add(this.addServer);
            this.Controls.Add(this.settingsGameFilesCurrent);
            this.Controls.Add(this.translatedBy);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.launcherStatusDesc);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.forgotPassword);
            this.Controls.Add(this.rememberMe);
            this.Controls.Add(this.currentWindowInfo);
            this.Controls.Add(this.serverPick);
            this.Controls.Add(this.password);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.logoutButton);
            this.Controls.Add(this.settingsQuality);
            this.Controls.Add(this.settingsQualityText);
            this.Controls.Add(this.registerEmail);
            this.Controls.Add(this.registerPassword);
            this.Controls.Add(this.registerConfirmPassword);
            this.Controls.Add(this.registerTicket);
            this.Controls.Add(this.registerAgree);
            this.Controls.Add(this.playProgressText);
            this.Controls.Add(this.settingsGamePathText);
            this.Controls.Add(this.settingsLanguageText);
            this.Controls.Add(this.settingsLanguage);
            this.Controls.Add(this.settingsGameFiles);
            this.Controls.Add(this.extractingProgress);
            this.Controls.Add(this.playProgress);
            this.Controls.Add(this.email);
            this.Controls.Add(this.settingsSave);
            this.Controls.Add(this.registerCancel);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.registerButton);
            this.Controls.Add(this.playButton);
            this.Controls.Add(this.registerText);
            this.Controls.Add(this.imageServerName);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.Red;
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox closebtn;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.ComboBox serverPick;
        private System.Windows.Forms.Label currentWindowInfo;
        private System.Windows.Forms.CheckBox rememberMe;
        private System.Windows.Forms.Timer Timeout;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.PictureBox settingsButton;
        private System.Windows.Forms.PictureBox verticalBanner;
        private System.Windows.Forms.Button settingsSave;
        private System.Windows.Forms.ComboBox settingsLanguage;
        private System.Windows.Forms.Label settingsLanguageText;
        private System.Windows.Forms.ComboBox settingsQuality;
        private System.Windows.Forms.Label settingsQualityText;
        private System.Windows.Forms.TextBox registerEmail;
        private System.Windows.Forms.TextBox registerTicket;
        private System.Windows.Forms.TextBox registerPassword;
        private System.Windows.Forms.TextBox registerConfirmPassword;
        private System.Windows.Forms.CheckBox registerAgree;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.NotifyIcon Notification;
        private System.Windows.Forms.LinkLabel forgotPassword;
        internal System.Windows.Forms.Label playProgressText;
        private System.Windows.Forms.Label launcherStatusDesc;
        private System.Windows.Forms.Label imageServerName;
        private System.Windows.Forms.Button registerCancel;
        private GameLauncherReborn.ProgressBarEx playProgress;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.Label translatedBy;
        private System.Windows.Forms.Button settingsGameFiles;
        private System.Windows.Forms.LinkLabel settingsGameFilesCurrent;
        private System.Windows.Forms.Label settingsGamePathText;
        private System.Windows.Forms.Button addServer;
        private GameLauncherReborn.ProgressBarEx extractingProgress;
        private System.Windows.Forms.PictureBox launcherIconStatus;
        private System.Windows.Forms.Label launcherStatusText;
        private System.Windows.Forms.Label ServerStatusDesc;
        private System.Windows.Forms.Label ServerStatusText;
        private System.Windows.Forms.PictureBox ServerStatusIcon;
        private System.Windows.Forms.Button registerText;
        private System.Windows.Forms.Label cdnText;
        private System.Windows.Forms.ComboBox cdnPick;
        private System.Windows.Forms.Button vfilesButton;
        private System.Windows.Forms.CheckBox wordFilterCheck;
        private System.Windows.Forms.Button SelectServerBtn;
        private System.Windows.Forms.CheckBox proxyCheckbox;
    }
}
