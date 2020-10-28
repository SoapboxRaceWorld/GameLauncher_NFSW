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
            this.settingsLanguage = new System.Windows.Forms.ComboBox();
            this.settingsLanguageText = new System.Windows.Forms.Label();
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
            this.settingsCDNText = new System.Windows.Forms.Label();
            this.settingsCDNPick = new System.Windows.Forms.ComboBox();
            this.settingsVFilesButton = new System.Windows.Forms.Button();
            this.settingsWordFilterCheck = new System.Windows.Forms.CheckBox();
            this.SelectServerBtn = new System.Windows.Forms.Button();
            this.settingsProxyCheckbox = new System.Windows.Forms.CheckBox();
            this.verticalBanner = new System.Windows.Forms.PictureBox();
            this.settingsNetworkText = new System.Windows.Forms.Label();
            this.settingsMainSrvText = new System.Windows.Forms.Label();
            this.settingsBkupSrvText = new System.Windows.Forms.Label();
            this.settingsBkupCDNText = new System.Windows.Forms.Label();
            this.settingsMainCDNText = new System.Windows.Forms.Label();
            this.settingsCDNCurrent = new System.Windows.Forms.Label();
            this.APIStatusIcon = new System.Windows.Forms.PictureBox();
            this.APIStatusText = new System.Windows.Forms.Label();
            this.APIStatusDesc = new System.Windows.Forms.Label();
            this.settingsDiscordRPCCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsGameFilesCurrentText = new System.Windows.Forms.Label();
            this.settingsCDNCurrentText = new System.Windows.Forms.Label();
            this.settingsCancel = new System.Windows.Forms.Button();
            this.settingsSave = new System.Windows.Forms.Button();
            this.playProgressTextTimer = new System.Windows.Forms.Label();
            this.extractingProgress = new GameLauncherReborn.ProgressBarEx();
            this.playProgress = new GameLauncherReborn.ProgressBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).BeginInit();
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
            this.currentWindowInfo.Location = new System.Drawing.Point(650, 99);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(194, 61);
            this.currentWindowInfo.TabIndex = 16;
            this.currentWindowInfo.Text = "ENTER YOUR ACCOUNT INFORMATION TO LOG IN";
            this.currentWindowInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.rememberMe.Size = new System.Drawing.Size(158, 19);
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
            this.settingsButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("settingsButton.BackgroundImage")));
            this.settingsButton.Location = new System.Drawing.Point(805, 24);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(24, 24);
            this.settingsButton.TabIndex = 21;
            this.settingsButton.TabStop = false;
            // 
            // settingsLanguage
            // 
            this.settingsLanguage.BackColor = System.Drawing.Color.White;
            this.settingsLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsLanguage.FormattingEnabled = true;
            this.settingsLanguage.Location = new System.Drawing.Point(46, 245);
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
            this.settingsLanguageText.Location = new System.Drawing.Point(46, 221);
            this.settingsLanguageText.Name = "settingsLanguageText";
            this.settingsLanguageText.Size = new System.Drawing.Size(142, 18);
            this.settingsLanguageText.TabIndex = 25;
            this.settingsLanguageText.Text = "GAME LANGUAGE:";
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
            this.forgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
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
            this.launcherStatusDesc.Location = new System.Drawing.Point(65, 489);
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
            this.settingsGameFiles.Location = new System.Drawing.Point(46, 152);
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
            this.settingsGameFilesCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsGameFilesCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.settingsGameFilesCurrent.LinkColor = System.Drawing.Color.White;
            this.settingsGameFilesCurrent.Location = new System.Drawing.Point(46, 195);
            this.settingsGameFilesCurrent.Name = "settingsGameFilesCurrent";
            this.settingsGameFilesCurrent.Size = new System.Drawing.Size(181, 13);
            this.settingsGameFilesCurrent.TabIndex = 58;
            this.settingsGameFilesCurrent.TabStop = true;
            this.settingsGameFilesCurrent.Text = "C:\\Soapbox Race World\\Game Files";
            this.settingsGameFilesCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.settingsGameFilesCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // settingsGamePathText
            // 
            this.settingsGamePathText.AutoSize = true;
            this.settingsGamePathText.BackColor = System.Drawing.Color.Transparent;
            this.settingsGamePathText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGamePathText.ForeColor = System.Drawing.Color.White;
            this.settingsGamePathText.Location = new System.Drawing.Point(46, 130);
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
            this.launcherStatusText.Size = new System.Drawing.Size(173, 15);
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
            this.ServerStatusDesc.Location = new System.Drawing.Point(260, 489);
            this.ServerStatusDesc.Name = "ServerStatusDesc";
            this.ServerStatusDesc.Size = new System.Drawing.Size(85, 13);
            this.ServerStatusDesc.TabIndex = 8;
            this.ServerStatusDesc.Text = "Checking Status";
            this.ServerStatusDesc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerStatusText.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ServerStatusText.Location = new System.Drawing.Point(260, 476);
            this.ServerStatusText.Name = "ServerStatusText";
            this.ServerStatusText.Size = new System.Drawing.Size(162, 15);
            this.ServerStatusText.TabIndex = 7;
            this.ServerStatusText.Text = "Server Status - Pinging";
            this.ServerStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerStatusIcon
            // 
            this.ServerStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("ServerStatusIcon.Image")));
            this.ServerStatusIcon.Location = new System.Drawing.Point(232, 478);
            this.ServerStatusIcon.Name = "ServerStatusIcon";
            this.ServerStatusIcon.Size = new System.Drawing.Size(24, 24);
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
            // settingsCDNText
            // 
            this.settingsCDNText.AutoSize = true;
            this.settingsCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsCDNText.ForeColor = System.Drawing.Color.White;
            this.settingsCDNText.Location = new System.Drawing.Point(404, 130);
            this.settingsCDNText.Name = "settingsCDNText";
            this.settingsCDNText.Size = new System.Drawing.Size(45, 18);
            this.settingsCDNText.TabIndex = 83;
            this.settingsCDNText.Text = "CDN:";
            this.settingsCDNText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsCDNPick
            // 
            this.settingsCDNPick.BackColor = System.Drawing.Color.White;
            this.settingsCDNPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsCDNPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsCDNPick.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.settingsCDNPick.FormattingEnabled = true;
            this.settingsCDNPick.Location = new System.Drawing.Point(407, 154);
            this.settingsCDNPick.Name = "settingsCDNPick";
            this.settingsCDNPick.Size = new System.Drawing.Size(210, 21);
            this.settingsCDNPick.TabIndex = 84;
            // 
            // settingsVFilesButton
            // 
            this.settingsVFilesButton.AutoSize = true;
            this.settingsVFilesButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsVFilesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsVFilesButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.settingsVFilesButton.Location = new System.Drawing.Point(46, 358);
            this.settingsVFilesButton.Name = "settingsVFilesButton";
            this.settingsVFilesButton.Size = new System.Drawing.Size(207, 23);
            this.settingsVFilesButton.TabIndex = 96;
            this.settingsVFilesButton.Text = "Validate Game Files";
            this.settingsVFilesButton.UseVisualStyleBackColor = false;
            this.settingsVFilesButton.Visible = false;
            this.settingsVFilesButton.Click += new System.EventHandler(this.vfilesButton_Click);
            // 
            // settingsWordFilterCheck
            // 
            this.settingsWordFilterCheck.AutoSize = true;
            this.settingsWordFilterCheck.BackColor = System.Drawing.Color.Transparent;
            this.settingsWordFilterCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsWordFilterCheck.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsWordFilterCheck.Location = new System.Drawing.Point(46, 282);
            this.settingsWordFilterCheck.Name = "settingsWordFilterCheck";
            this.settingsWordFilterCheck.Size = new System.Drawing.Size(234, 17);
            this.settingsWordFilterCheck.TabIndex = 93;
            this.settingsWordFilterCheck.Text = "Disable Word Filtering on Game Chat";
            this.settingsWordFilterCheck.UseVisualStyleBackColor = false;
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
            // settingsProxyCheckbox
            // 
            this.settingsProxyCheckbox.AutoSize = true;
            this.settingsProxyCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.settingsProxyCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsProxyCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsProxyCheckbox.Location = new System.Drawing.Point(46, 303);
            this.settingsProxyCheckbox.Name = "settingsProxyCheckbox";
            this.settingsProxyCheckbox.Size = new System.Drawing.Size(102, 17);
            this.settingsProxyCheckbox.TabIndex = 102;
            this.settingsProxyCheckbox.Text = "Disable Proxy";
            this.settingsProxyCheckbox.UseVisualStyleBackColor = false;
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
            // settingsNetworkText
            // 
            this.settingsNetworkText.AutoSize = true;
            this.settingsNetworkText.BackColor = System.Drawing.Color.Transparent;
            this.settingsNetworkText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsNetworkText.ForeColor = System.Drawing.Color.White;
            this.settingsNetworkText.Location = new System.Drawing.Point(404, 221);
            this.settingsNetworkText.Name = "settingsNetworkText";
            this.settingsNetworkText.Size = new System.Drawing.Size(175, 18);
            this.settingsNetworkText.TabIndex = 105;
            this.settingsNetworkText.Text = "CONNECTION STATUS:";
            // 
            // settingsMainSrvText
            // 
            this.settingsMainSrvText.AutoSize = true;
            this.settingsMainSrvText.BackColor = System.Drawing.Color.Transparent;
            this.settingsMainSrvText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsMainSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsMainSrvText.Location = new System.Drawing.Point(404, 243);
            this.settingsMainSrvText.Name = "settingsMainSrvText";
            this.settingsMainSrvText.Size = new System.Drawing.Size(154, 13);
            this.settingsMainSrvText.TabIndex = 106;
            this.settingsMainSrvText.Text = "Main Server List API: PINGING";
            // 
            // settingsBkupSrvText
            // 
            this.settingsBkupSrvText.AutoSize = true;
            this.settingsBkupSrvText.BackColor = System.Drawing.Color.Transparent;
            this.settingsBkupSrvText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsBkupSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsBkupSrvText.Location = new System.Drawing.Point(404, 281);
            this.settingsBkupSrvText.Name = "settingsBkupSrvText";
            this.settingsBkupSrvText.Size = new System.Drawing.Size(168, 13);
            this.settingsBkupSrvText.TabIndex = 107;
            this.settingsBkupSrvText.Text = "Backup Server List API: PINGING";
            // 
            // settingsBkupCDNText
            // 
            this.settingsBkupCDNText.AutoSize = true;
            this.settingsBkupCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsBkupCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsBkupCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsBkupCDNText.Location = new System.Drawing.Point(404, 301);
            this.settingsBkupCDNText.Name = "settingsBkupCDNText";
            this.settingsBkupCDNText.Size = new System.Drawing.Size(160, 13);
            this.settingsBkupCDNText.TabIndex = 108;
            this.settingsBkupCDNText.Text = "Backup CDN List API: PINGING";
            // 
            // settingsMainCDNText
            // 
            this.settingsMainCDNText.AutoSize = true;
            this.settingsMainCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsMainCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsMainCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsMainCDNText.Location = new System.Drawing.Point(404, 262);
            this.settingsMainCDNText.Name = "settingsMainCDNText";
            this.settingsMainCDNText.Size = new System.Drawing.Size(146, 13);
            this.settingsMainCDNText.TabIndex = 109;
            this.settingsMainCDNText.Text = "Main CDN List API: PINGING";
            // 
            // settingsCDNCurrent
            // 
            this.settingsCDNCurrent.AutoSize = true;
            this.settingsCDNCurrent.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsCDNCurrent.ForeColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrent.Location = new System.Drawing.Point(404, 195);
            this.settingsCDNCurrent.Name = "settingsCDNCurrent";
            this.settingsCDNCurrent.Size = new System.Drawing.Size(80, 13);
            this.settingsCDNCurrent.TabIndex = 110;
            this.settingsCDNCurrent.Text = "http://localhost";
            this.settingsCDNCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // APIStatusIcon
            // 
            this.APIStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("APIStatusIcon.Image")));
            this.APIStatusIcon.Location = new System.Drawing.Point(428, 478);
            this.APIStatusIcon.Name = "APIStatusIcon";
            this.APIStatusIcon.Size = new System.Drawing.Size(24, 24);
            this.APIStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.APIStatusIcon.TabIndex = 113;
            this.APIStatusIcon.TabStop = false;
            // 
            // APIStatusText
            // 
            this.APIStatusText.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.APIStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.APIStatusText.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.APIStatusText.Location = new System.Drawing.Point(455, 476);
            this.APIStatusText.Name = "APIStatusText";
            this.APIStatusText.Size = new System.Drawing.Size(162, 15);
            this.APIStatusText.TabIndex = 116;
            this.APIStatusText.Text = "Main API - Pinging";
            this.APIStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // APIStatusDesc
            // 
            this.APIStatusDesc.AutoSize = true;
            this.APIStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.APIStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
            this.APIStatusDesc.Location = new System.Drawing.Point(455, 489);
            this.APIStatusDesc.Name = "APIStatusDesc";
            this.APIStatusDesc.Size = new System.Drawing.Size(85, 13);
            this.APIStatusDesc.TabIndex = 120;
            this.APIStatusDesc.Text = "Checking Status";
            this.APIStatusDesc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsDiscordRPCCheckbox
            // 
            this.settingsDiscordRPCCheckbox.AutoSize = true;
            this.settingsDiscordRPCCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.settingsDiscordRPCCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsDiscordRPCCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsDiscordRPCCheckbox.Location = new System.Drawing.Point(46, 326);
            this.settingsDiscordRPCCheckbox.Name = "settingsDiscordRPCCheckbox";
            this.settingsDiscordRPCCheckbox.Size = new System.Drawing.Size(143, 17);
            this.settingsDiscordRPCCheckbox.TabIndex = 123;
            this.settingsDiscordRPCCheckbox.Text = "Disable Discord RPC";
            this.settingsDiscordRPCCheckbox.UseVisualStyleBackColor = false;
            // 
            // settingsGameFilesCurrentText
            // 
            this.settingsGameFilesCurrentText.AutoSize = true;
            this.settingsGameFilesCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.settingsGameFilesCurrentText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsGameFilesCurrentText.ForeColor = System.Drawing.Color.White;
            this.settingsGameFilesCurrentText.Location = new System.Drawing.Point(46, 178);
            this.settingsGameFilesCurrentText.Name = "settingsGameFilesCurrentText";
            this.settingsGameFilesCurrentText.Size = new System.Drawing.Size(78, 13);
            this.settingsGameFilesCurrentText.TabIndex = 126;
            this.settingsGameFilesCurrentText.Text = "Current Path";
            this.settingsGameFilesCurrentText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsCDNCurrentText
            // 
            this.settingsCDNCurrentText.AutoSize = true;
            this.settingsCDNCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrentText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsCDNCurrentText.ForeColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrentText.Location = new System.Drawing.Point(404, 178);
            this.settingsCDNCurrentText.Name = "settingsCDNCurrentText";
            this.settingsCDNCurrentText.Size = new System.Drawing.Size(97, 13);
            this.settingsCDNCurrentText.TabIndex = 129;
            this.settingsCDNCurrentText.Text = "CURRENT CDN";
            this.settingsCDNCurrentText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // settingsCancel
            // 
            this.settingsCancel.BackColor = System.Drawing.Color.Transparent;
            this.settingsCancel.FlatAppearance.BorderSize = 0;
            this.settingsCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.settingsCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.settingsCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsCancel.ForeColor = System.Drawing.Color.White;
            this.settingsCancel.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.settingsCancel.Location = new System.Drawing.Point(720, 436);
            this.settingsCancel.Name = "settingsCancel";
            this.settingsCancel.Size = new System.Drawing.Size(130, 50);
            this.settingsCancel.TabIndex = 132;
            this.settingsCancel.Text = "CANCEL";
            this.settingsCancel.UseVisualStyleBackColor = false;
            this.settingsCancel.Click += new System.EventHandler(this.settingsCancel_Click);
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
            this.settingsSave.Location = new System.Drawing.Point(584, 436);
            this.settingsSave.Name = "settingsSave";
            this.settingsSave.Size = new System.Drawing.Size(130, 50);
            this.settingsSave.TabIndex = 23;
            this.settingsSave.Text = "SAVE";
            this.settingsSave.UseVisualStyleBackColor = false;
            // 
            // playProgressTextTimer
            // 
            this.playProgressTextTimer.BackColor = System.Drawing.Color.Transparent;
            this.playProgressTextTimer.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.playProgressTextTimer.ForeColor = System.Drawing.Color.White;
            this.playProgressTextTimer.Location = new System.Drawing.Point(442, 422);
            this.playProgressTextTimer.Name = "playProgressTextTimer";
            this.playProgressTextTimer.Size = new System.Drawing.Size(120, 19);
            this.playProgressTextTimer.TabIndex = 135;
            this.playProgressTextTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.playProgressTextTimer.Visible = false;
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
            this.Controls.Add(this.playProgressTextTimer);
            this.Controls.Add(this.settingsCancel);
            this.Controls.Add(this.settingsCDNCurrentText);
            this.Controls.Add(this.settingsGameFilesCurrentText);
            this.Controls.Add(this.settingsDiscordRPCCheckbox);
            this.Controls.Add(this.APIStatusText);
            this.Controls.Add(this.APIStatusDesc);
            this.Controls.Add(this.APIStatusIcon);
            this.Controls.Add(this.settingsCDNCurrent);
            this.Controls.Add(this.settingsMainCDNText);
            this.Controls.Add(this.settingsBkupCDNText);
            this.Controls.Add(this.settingsBkupSrvText);
            this.Controls.Add(this.settingsMainSrvText);
            this.Controls.Add(this.settingsNetworkText);
            this.Controls.Add(this.settingsProxyCheckbox);
            this.Controls.Add(this.SelectServerBtn);
            this.Controls.Add(this.settingsVFilesButton);
            this.Controls.Add(this.settingsWordFilterCheck);
            this.Controls.Add(this.settingsCDNPick);
            this.Controls.Add(this.settingsCDNText);
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
            this.Controls.Add(this.verticalBanner);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).EndInit();
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
        private System.Windows.Forms.ComboBox settingsLanguage;
        private System.Windows.Forms.Label settingsLanguageText;
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
        private System.Windows.Forms.Label settingsCDNText;
        private System.Windows.Forms.ComboBox settingsCDNPick;
        private System.Windows.Forms.Button settingsVFilesButton;
        private System.Windows.Forms.CheckBox settingsWordFilterCheck;
        private System.Windows.Forms.Button SelectServerBtn;
        private System.Windows.Forms.CheckBox settingsProxyCheckbox;
        private System.Windows.Forms.PictureBox verticalBanner;
        private System.Windows.Forms.Label settingsNetworkText;
        private System.Windows.Forms.Label settingsMainSrvText;
        private System.Windows.Forms.Label settingsBkupSrvText;
        private System.Windows.Forms.Label settingsBkupCDNText;
        private System.Windows.Forms.Label settingsMainCDNText;
        private System.Windows.Forms.Label settingsCDNCurrent;
        private System.Windows.Forms.PictureBox APIStatusIcon;
        private System.Windows.Forms.Label APIStatusText;
        private System.Windows.Forms.Label APIStatusDesc;
        private System.Windows.Forms.CheckBox settingsDiscordRPCCheckbox;
        private System.Windows.Forms.Label settingsGameFilesCurrentText;
        private System.Windows.Forms.Label settingsCDNCurrentText;
        private System.Windows.Forms.Button settingsCancel;
        private System.Windows.Forms.Button settingsSave;
        internal System.Windows.Forms.Label playProgressTextTimer;
    }
}
