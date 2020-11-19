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
            this.Timeout = new System.Windows.Forms.Timer(this.components);
            this.Notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.logo = new System.Windows.Forms.PictureBox();
            this.closebtn = new System.Windows.Forms.PictureBox();
            this.SelectServerBtn = new System.Windows.Forms.Button();
            this.translatedBy = new System.Windows.Forms.Label();
            this.settingsButton = new System.Windows.Forms.PictureBox();
            this.serverPick = new System.Windows.Forms.ComboBox();
            this.addServer = new System.Windows.Forms.Button();
            this.imageServerName = new System.Windows.Forms.Label();
            this.verticalBanner = new System.Windows.Forms.PictureBox();
            this.playProgressText = new System.Windows.Forms.Label();
            this.launcherIconStatus = new System.Windows.Forms.PictureBox();
            this.launcherStatusText = new System.Windows.Forms.Label();
            this.launcherStatusDesc = new System.Windows.Forms.Label();
            this.ServerStatusIcon = new System.Windows.Forms.PictureBox();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.ServerStatusDesc = new System.Windows.Forms.Label();
            this.APIStatusIcon = new System.Windows.Forms.PictureBox();
            this.APIStatusText = new System.Windows.Forms.Label();
            this.APIStatusDesc = new System.Windows.Forms.Label();
            this.currentWindowInfo = new System.Windows.Forms.Label();
            this.email = new System.Windows.Forms.TextBox();
            this.MainEmailBorder = new System.Windows.Forms.PictureBox();
            this.password = new System.Windows.Forms.TextBox();
            this.MainPasswordBorder = new System.Windows.Forms.PictureBox();
            this.rememberMe = new System.Windows.Forms.CheckBox();
            this.forgotPassword = new System.Windows.Forms.LinkLabel();
            this.loginButton = new System.Windows.Forms.Button();
            this.registerText = new System.Windows.Forms.Button();
            this.ServerPingStatusText = new System.Windows.Forms.Label();
            this.logoutButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.playProgressTextTimer = new System.Windows.Forms.Label();
            this.registerEmail = new System.Windows.Forms.TextBox();
            this.RegisterEmailBorder = new System.Windows.Forms.PictureBox();
            this.registerPassword = new System.Windows.Forms.TextBox();
            this.RegisterPasswordBorder = new System.Windows.Forms.PictureBox();
            this.registerConfirmPassword = new System.Windows.Forms.TextBox();
            this.RegisterPasswordValidateBorder = new System.Windows.Forms.PictureBox();
            this.registerTicket = new System.Windows.Forms.TextBox();
            this.RegisterTicketBorder = new System.Windows.Forms.PictureBox();
            this.registerAgree = new System.Windows.Forms.CheckBox();
            this.registerButton = new System.Windows.Forms.Button();
            this.registerCancel = new System.Windows.Forms.Button();
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.settingsLauncherVersion = new System.Windows.Forms.Label();
            this.settingsAboutButton = new System.Windows.Forms.Button();
            this.settingsGameFiles = new System.Windows.Forms.Button();
            this.SettingsClearCommunicationLogButton = new System.Windows.Forms.Button();
            this.SettingsClearCrashLogsButton = new System.Windows.Forms.Button();
            this.settingsVFilesButton = new System.Windows.Forms.Button();
            this.settingsGamePathText = new System.Windows.Forms.Label();
            this.settingsSave = new System.Windows.Forms.Button();
            this.settingsCancel = new System.Windows.Forms.Button();
            this.settingsCDNText = new System.Windows.Forms.Label();
            this.settingsCDNPick = new System.Windows.Forms.ComboBox();
            this.settingsLanguageText = new System.Windows.Forms.Label();
            this.settingsLanguage = new System.Windows.Forms.ComboBox();
            this.settingsWordFilterCheck = new System.Windows.Forms.CheckBox();
            this.settingsProxyCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsDiscordRPCCheckbox = new System.Windows.Forms.CheckBox();
            this.settingsGameFilesCurrentText = new System.Windows.Forms.Label();
            this.settingsGameFilesCurrent = new System.Windows.Forms.LinkLabel();
            this.settingsCDNCurrentText = new System.Windows.Forms.Label();
            this.settingsCDNCurrent = new System.Windows.Forms.LinkLabel();
            this.settingsLauncherPathText = new System.Windows.Forms.Label();
            this.settingsLauncherPathCurrent = new System.Windows.Forms.LinkLabel();
            this.settingsNetworkText = new System.Windows.Forms.Label();
            this.settingsMainSrvText = new System.Windows.Forms.Label();
            this.settingsMainCDNText = new System.Windows.Forms.Label();
            this.settingsBkupSrvText = new System.Windows.Forms.Label();
            this.settingsBkupCDNText = new System.Windows.Forms.Label();
            this.ShowPlayPanel = new System.Windows.Forms.Panel();
            this.RegisterPanel = new System.Windows.Forms.Panel();
            this.DiscordInviteLink = new System.Windows.Forms.LinkLabel();
            this.ServerShutDown = new System.Windows.Forms.Label();
            this.ServerInfoPanel = new System.Windows.Forms.Panel();
            this.HomePageIcon = new System.Windows.Forms.PictureBox();
            this.DiscordIcon = new System.Windows.Forms.PictureBox();
            this.FacebookIcon = new System.Windows.Forms.PictureBox();
            this.TwitterAccountLink = new System.Windows.Forms.LinkLabel();
            this.TwitterIcon = new System.Windows.Forms.PictureBox();
            this.FacebookGroupLink = new System.Windows.Forms.LinkLabel();
            this.HomePageLink = new System.Windows.Forms.LinkLabel();
            this.SceneryGroupText = new System.Windows.Forms.Label();
            this.extractingProgress = new GameLauncherReborn.ProgressBarEx();
            this.playProgress = new GameLauncherReborn.ProgressBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterEmailBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordValidateBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterTicketBorder)).BeginInit();
            this.SettingsPanel.SuspendLayout();
            this.ShowPlayPanel.SuspendLayout();
            this.RegisterPanel.SuspendLayout();
            this.ServerInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HomePageIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscordIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacebookIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TwitterIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // Timeout
            // 
            this.Timeout.Interval = 3000;
            // 
            // Notification
            // 
            this.Notification.Text = "notifyIcon1";
            this.Notification.Visible = true;
            // 
            // logo
            // 
            this.logo.BackColor = System.Drawing.Color.Transparent;
            this.logo.Image = global::GameLauncher.Properties.Resources.logo;
            this.logo.Location = new System.Drawing.Point(27, 9);
            this.logo.Name = "logo";
            this.logo.Size = new System.Drawing.Size(227, 60);
            this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logo.TabIndex = 0;
            this.logo.TabStop = false;
            // 
            // closebtn
            // 
            this.closebtn.BackColor = System.Drawing.Color.Transparent;
            this.closebtn.BackgroundImage = global::GameLauncher.Properties.Resources.close;
            this.closebtn.Location = new System.Drawing.Point(841, 15);
            this.closebtn.Name = "closebtn";
            this.closebtn.Size = new System.Drawing.Size(24, 24);
            this.closebtn.TabIndex = 0;
            this.closebtn.TabStop = false;
            this.closebtn.Click += new System.EventHandler(this.Closebtn_Click);
            // 
            // SelectServerBtn
            // 
            this.SelectServerBtn.Location = new System.Drawing.Point(888, 15);
            this.SelectServerBtn.Name = "SelectServerBtn";
            this.SelectServerBtn.Size = new System.Drawing.Size(228, 24);
            this.SelectServerBtn.TabIndex = 1;
            this.SelectServerBtn.Text = "Select Server";
            this.SelectServerBtn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SelectServerBtn.UseVisualStyleBackColor = true;
            this.SelectServerBtn.Click += new System.EventHandler(this.SelectServerBtn_Click);
            // 
            // translatedBy
            // 
            this.translatedBy.AutoSize = true;
            this.translatedBy.BackColor = System.Drawing.Color.Transparent;
            this.translatedBy.ForeColor = System.Drawing.Color.DarkGray;
            this.translatedBy.Location = new System.Drawing.Point(12, -1);
            this.translatedBy.Name = "translatedBy";
            this.translatedBy.Size = new System.Drawing.Size(105, 13);
            this.translatedBy.TabIndex = 55;
            this.translatedBy.Text = "Translated by: meme";
            // 
            // settingsButton
            // 
            this.settingsButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("settingsButton.BackgroundImage")));
            this.settingsButton.Location = new System.Drawing.Point(807, 15);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(24, 24);
            this.settingsButton.TabIndex = 21;
            this.settingsButton.TabStop = false;
            // 
            // serverPick
            // 
            this.serverPick.BackColor = System.Drawing.Color.White;
            this.serverPick.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.serverPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.serverPick.ForeColor = System.Drawing.Color.Black;
            this.serverPick.FormattingEnabled = true;
            this.serverPick.Location = new System.Drawing.Point(586, 50);
            this.serverPick.Name = "serverPick";
            this.serverPick.Size = new System.Drawing.Size(241, 22);
            this.serverPick.TabIndex = 2;
            // 
            // addServer
            // 
            this.addServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.addServer.Location = new System.Drawing.Point(833, 49);
            this.addServer.Name = "addServer";
            this.addServer.Size = new System.Drawing.Size(24, 24);
            this.addServer.TabIndex = 3;
            this.addServer.Text = "+";
            this.addServer.UseVisualStyleBackColor = true;
            // 
            // imageServerName
            // 
            this.imageServerName.BackColor = System.Drawing.Color.Transparent;
            this.imageServerName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.imageServerName.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.imageServerName.ForeColor = System.Drawing.Color.Transparent;
            this.imageServerName.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.imageServerName.Location = new System.Drawing.Point(30, 301);
            this.imageServerName.Name = "imageServerName";
            this.imageServerName.Size = new System.Drawing.Size(519, 43);
            this.imageServerName.TabIndex = 19;
            this.imageServerName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.imageServerName.UseCompatibleTextRendering = true;
            this.imageServerName.UseMnemonic = false;
            // 
            // verticalBanner
            // 
            this.verticalBanner.BackColor = System.Drawing.Color.Transparent;
            this.verticalBanner.Location = new System.Drawing.Point(28, 81);
            this.verticalBanner.Name = "verticalBanner";
            this.verticalBanner.Size = new System.Drawing.Size(523, 223);
            this.verticalBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.verticalBanner.TabIndex = 22;
            this.verticalBanner.TabStop = false;
            // 
            // playProgressText
            // 
            this.playProgressText.AutoSize = true;
            this.playProgressText.BackColor = System.Drawing.Color.Transparent;
            this.playProgressText.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.playProgressText.ForeColor = System.Drawing.Color.White;
            this.playProgressText.Location = new System.Drawing.Point(43, 398);
            this.playProgressText.Name = "playProgressText";
            this.playProgressText.Size = new System.Drawing.Size(120, 19);
            this.playProgressText.TabIndex = 10;
            this.playProgressText.Text = "PLEASE WAIT";
            // 
            // launcherIconStatus
            // 
            this.launcherIconStatus.BackColor = System.Drawing.Color.Transparent;
            this.launcherIconStatus.Image = global::GameLauncher.Properties.Resources.ac_success;
            this.launcherIconStatus.Location = new System.Drawing.Point(27, 456);
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
            this.launcherStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.launcherStatusText.Location = new System.Drawing.Point(53, 453);
            this.launcherStatusText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.launcherStatusText.Name = "launcherStatusText";
            this.launcherStatusText.Size = new System.Drawing.Size(173, 15);
            this.launcherStatusText.TabIndex = 4;
            this.launcherStatusText.Text = "Anti-Cheat System - Activated";
            this.launcherStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.launcherStatusText.UseCompatibleTextRendering = true;
            // 
            // launcherStatusDesc
            // 
            this.launcherStatusDesc.AutoSize = true;
            this.launcherStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.launcherStatusDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.launcherStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.launcherStatusDesc.Location = new System.Drawing.Point(54, 469);
            this.launcherStatusDesc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.launcherStatusDesc.Name = "launcherStatusDesc";
            this.launcherStatusDesc.Size = new System.Drawing.Size(90, 13);
            this.launcherStatusDesc.TabIndex = 5;
            this.launcherStatusDesc.Text = "Version : v2.0.0.0";
            this.launcherStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerStatusIcon
            // 
            this.ServerStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("ServerStatusIcon.Image")));
            this.ServerStatusIcon.Location = new System.Drawing.Point(204, 456);
            this.ServerStatusIcon.Name = "ServerStatusIcon";
            this.ServerStatusIcon.Size = new System.Drawing.Size(24, 24);
            this.ServerStatusIcon.TabIndex = 6;
            this.ServerStatusIcon.TabStop = false;
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ServerStatusText.Location = new System.Drawing.Point(235, 453);
            this.ServerStatusText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ServerStatusText.Name = "ServerStatusText";
            this.ServerStatusText.Size = new System.Drawing.Size(162, 15);
            this.ServerStatusText.TabIndex = 7;
            this.ServerStatusText.Text = "Server Status - Pinging";
            this.ServerStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerStatusDesc
            // 
            this.ServerStatusDesc.AutoSize = true;
            this.ServerStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusDesc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ServerStatusDesc.Location = new System.Drawing.Point(235, 469);
            this.ServerStatusDesc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ServerStatusDesc.Name = "ServerStatusDesc";
            this.ServerStatusDesc.Size = new System.Drawing.Size(85, 13);
            this.ServerStatusDesc.TabIndex = 8;
            this.ServerStatusDesc.Text = "Checking Status";
            this.ServerStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // APIStatusIcon
            // 
            this.APIStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("APIStatusIcon.Image")));
            this.APIStatusIcon.Location = new System.Drawing.Point(398, 455);
            this.APIStatusIcon.Margin = new System.Windows.Forms.Padding(2);
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
            this.APIStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.APIStatusText.Location = new System.Drawing.Point(426, 453);
            this.APIStatusText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
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
            this.APIStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.APIStatusDesc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.APIStatusDesc.Location = new System.Drawing.Point(427, 469);
            this.APIStatusDesc.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.APIStatusDesc.Name = "APIStatusDesc";
            this.APIStatusDesc.Size = new System.Drawing.Size(85, 13);
            this.APIStatusDesc.TabIndex = 120;
            this.APIStatusDesc.Text = "Checking Status";
            this.APIStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // currentWindowInfo
            // 
            this.currentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.currentWindowInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.currentWindowInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.currentWindowInfo.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.currentWindowInfo.ForeColor = System.Drawing.Color.White;
            this.currentWindowInfo.Location = new System.Drawing.Point(640, 82);
            this.currentWindowInfo.Name = "currentWindowInfo";
            this.currentWindowInfo.Size = new System.Drawing.Size(180, 60);
            this.currentWindowInfo.TabIndex = 16;
            this.currentWindowInfo.Text = "ENTER ACCOUNT INFORMATION\n TO LOG IN";
            this.currentWindowInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.currentWindowInfo.UseCompatibleTextRendering = true;
            this.currentWindowInfo.UseMnemonic = false;
            // 
            // email
            // 
            this.email.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.email.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.email.Font = new System.Drawing.Font("Arial", 14F);
            this.email.ForeColor = System.Drawing.Color.White;
            this.email.Location = new System.Drawing.Point(645, 172);
            this.email.Name = "email";
            this.email.Size = new System.Drawing.Size(180, 22);
            this.email.TabIndex = 4;
            this.email.TextChanged += new System.EventHandler(this.Email_TextChanged);
            // 
            // MainEmailBorder
            // 
            this.MainEmailBorder.BackColor = System.Drawing.Color.Transparent;
            this.MainEmailBorder.Image = global::GameLauncher.Properties.Resources.email_text_border;
            this.MainEmailBorder.Location = new System.Drawing.Point(606, 161);
            this.MainEmailBorder.Name = "MainEmailBorder";
            this.MainEmailBorder.Size = new System.Drawing.Size(231, 37);
            this.MainEmailBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MainEmailBorder.TabIndex = 144;
            this.MainEmailBorder.TabStop = false;
            this.MainEmailBorder.Visible = false;
            // 
            // password
            // 
            this.password.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.password.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.password.Font = new System.Drawing.Font("Arial", 14F);
            this.password.ForeColor = System.Drawing.Color.White;
            this.password.Location = new System.Drawing.Point(645, 221);
            this.password.Name = "password";
            this.password.Size = new System.Drawing.Size(180, 22);
            this.password.TabIndex = 5;
            this.password.UseSystemPasswordChar = true;
            this.password.WordWrap = false;
            this.password.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // MainPasswordBorder
            // 
            this.MainPasswordBorder.BackColor = System.Drawing.Color.Transparent;
            this.MainPasswordBorder.Image = global::GameLauncher.Properties.Resources.password_text_border;
            this.MainPasswordBorder.Location = new System.Drawing.Point(606, 210);
            this.MainPasswordBorder.Name = "MainPasswordBorder";
            this.MainPasswordBorder.Size = new System.Drawing.Size(231, 37);
            this.MainPasswordBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MainPasswordBorder.TabIndex = 145;
            this.MainPasswordBorder.TabStop = false;
            this.MainPasswordBorder.Visible = false;
            // 
            // rememberMe
            // 
            this.rememberMe.AutoSize = true;
            this.rememberMe.BackColor = System.Drawing.Color.Transparent;
            this.rememberMe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.rememberMe.ForeColor = System.Drawing.Color.White;
            this.rememberMe.Location = new System.Drawing.Point(645, 260);
            this.rememberMe.Name = "rememberMe";
            this.rememberMe.Size = new System.Drawing.Size(159, 19);
            this.rememberMe.TabIndex = 6;
            this.rememberMe.Text = "REMEMBER MY LOGIN";
            this.rememberMe.UseVisualStyleBackColor = false;
            // 
            // forgotPassword
            // 
            this.forgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.forgotPassword.AutoSize = true;
            this.forgotPassword.BackColor = System.Drawing.Color.Transparent;
            this.forgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.forgotPassword.Location = new System.Drawing.Point(659, 281);
            this.forgotPassword.Name = "forgotPassword";
            this.forgotPassword.Size = new System.Drawing.Size(143, 13);
            this.forgotPassword.TabIndex = 7;
            this.forgotPassword.TabStop = true;
            this.forgotPassword.Text = "I FORGOT MY PASSWORD";
            this.forgotPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.forgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
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
            this.loginButton.Location = new System.Drawing.Point(605, 362);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(230, 35);
            this.loginButton.TabIndex = 8;
            this.loginButton.Text = "LOG ON";
            this.loginButton.UseVisualStyleBackColor = false;
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
            this.registerText.Location = new System.Drawing.Point(606, 409);
            this.registerText.Name = "registerText";
            this.registerText.Size = new System.Drawing.Size(231, 35);
            this.registerText.TabIndex = 10;
            this.registerText.Text = "REGISTER";
            this.registerText.UseVisualStyleBackColor = false;
            // 
            // ServerPingStatusText
            // 
            this.ServerPingStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerPingStatusText.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerPingStatusText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ServerPingStatusText.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.ServerPingStatusText.ForeColor = System.Drawing.Color.White;
            this.ServerPingStatusText.Location = new System.Drawing.Point(625, 290);
            this.ServerPingStatusText.Name = "ServerPingStatusText";
            this.ServerPingStatusText.Size = new System.Drawing.Size(194, 61);
            this.ServerPingStatusText.TabIndex = 148;
            this.ServerPingStatusText.Text = "Your Ping to the Server";
            this.ServerPingStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ServerPingStatusText.UseCompatibleTextRendering = true;
            this.ServerPingStatusText.UseMnemonic = false;
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
            this.logoutButton.Location = new System.Drawing.Point(6, 60);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(231, 35);
            this.logoutButton.TabIndex = 9;
            this.logoutButton.Text = "LOG OUT";
            this.logoutButton.UseVisualStyleBackColor = false;
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
            this.playButton.Location = new System.Drawing.Point(7, 94);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(230, 63);
            this.playButton.TabIndex = 15;
            this.playButton.Text = "PLAY NOW";
            this.playButton.UseVisualStyleBackColor = false;
            // 
            // playProgressTextTimer
            // 
            this.playProgressTextTimer.BackColor = System.Drawing.Color.Transparent;
            this.playProgressTextTimer.Font = new System.Drawing.Font("Arial", 17F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(238)));
            this.playProgressTextTimer.ForeColor = System.Drawing.Color.White;
            this.playProgressTextTimer.Location = new System.Drawing.Point(431, 399);
            this.playProgressTextTimer.Name = "playProgressTextTimer";
            this.playProgressTextTimer.Size = new System.Drawing.Size(120, 19);
            this.playProgressTextTimer.TabIndex = 135;
            this.playProgressTextTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.playProgressTextTimer.Visible = false;
            // 
            // registerEmail
            // 
            this.registerEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.registerEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerEmail.Font = new System.Drawing.Font("Arial", 14F);
            this.registerEmail.ForeColor = System.Drawing.Color.White;
            this.registerEmail.Location = new System.Drawing.Point(58, 79);
            this.registerEmail.Name = "registerEmail";
            this.registerEmail.Size = new System.Drawing.Size(180, 22);
            this.registerEmail.TabIndex = 12;
            this.registerEmail.TextChanged += new System.EventHandler(this.RegisterEmail_TextChanged);
            // 
            // RegisterEmailBorder
            // 
            this.RegisterEmailBorder.BackColor = System.Drawing.Color.Transparent;
            this.RegisterEmailBorder.Image = global::GameLauncher.Properties.Resources.email_text_border;
            this.RegisterEmailBorder.Location = new System.Drawing.Point(19, 68);
            this.RegisterEmailBorder.Name = "RegisterEmailBorder";
            this.RegisterEmailBorder.Size = new System.Drawing.Size(231, 37);
            this.RegisterEmailBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RegisterEmailBorder.TabIndex = 138;
            this.RegisterEmailBorder.TabStop = false;
            // 
            // registerPassword
            // 
            this.registerPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.registerPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerPassword.Font = new System.Drawing.Font("Arial", 14F);
            this.registerPassword.ForeColor = System.Drawing.Color.White;
            this.registerPassword.Location = new System.Drawing.Point(58, 128);
            this.registerPassword.Name = "registerPassword";
            this.registerPassword.Size = new System.Drawing.Size(180, 22);
            this.registerPassword.TabIndex = 13;
            this.registerPassword.UseSystemPasswordChar = true;
            this.registerPassword.TextChanged += new System.EventHandler(this.RegisterPassword_TextChanged);
            // 
            // RegisterPasswordBorder
            // 
            this.RegisterPasswordBorder.BackColor = System.Drawing.Color.Transparent;
            this.RegisterPasswordBorder.Image = global::GameLauncher.Properties.Resources.password_text_border;
            this.RegisterPasswordBorder.Location = new System.Drawing.Point(19, 117);
            this.RegisterPasswordBorder.Name = "RegisterPasswordBorder";
            this.RegisterPasswordBorder.Size = new System.Drawing.Size(231, 37);
            this.RegisterPasswordBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RegisterPasswordBorder.TabIndex = 139;
            this.RegisterPasswordBorder.TabStop = false;
            // 
            // registerConfirmPassword
            // 
            this.registerConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.registerConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerConfirmPassword.Font = new System.Drawing.Font("Arial", 14F);
            this.registerConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.registerConfirmPassword.Location = new System.Drawing.Point(58, 177);
            this.registerConfirmPassword.Name = "registerConfirmPassword";
            this.registerConfirmPassword.Size = new System.Drawing.Size(180, 22);
            this.registerConfirmPassword.TabIndex = 14;
            this.registerConfirmPassword.UseSystemPasswordChar = true;
            this.registerConfirmPassword.TextChanged += new System.EventHandler(this.RegisterConfirmPassword_TextChanged);
            // 
            // RegisterPasswordValidateBorder
            // 
            this.RegisterPasswordValidateBorder.BackColor = System.Drawing.Color.Transparent;
            this.RegisterPasswordValidateBorder.Image = global::GameLauncher.Properties.Resources.password_text_border;
            this.RegisterPasswordValidateBorder.Location = new System.Drawing.Point(19, 166);
            this.RegisterPasswordValidateBorder.Name = "RegisterPasswordValidateBorder";
            this.RegisterPasswordValidateBorder.Size = new System.Drawing.Size(231, 37);
            this.RegisterPasswordValidateBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RegisterPasswordValidateBorder.TabIndex = 140;
            this.RegisterPasswordValidateBorder.TabStop = false;
            // 
            // registerTicket
            // 
            this.registerTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.registerTicket.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.registerTicket.Font = new System.Drawing.Font("Arial", 14F);
            this.registerTicket.ForeColor = System.Drawing.Color.White;
            this.registerTicket.Location = new System.Drawing.Point(58, 225);
            this.registerTicket.Name = "registerTicket";
            this.registerTicket.Size = new System.Drawing.Size(180, 22);
            this.registerTicket.TabIndex = 15;
            this.registerTicket.TextChanged += new System.EventHandler(this.RegisterTicket_TextChanged);
            // 
            // RegisterTicketBorder
            // 
            this.RegisterTicketBorder.BackColor = System.Drawing.Color.Transparent;
            this.RegisterTicketBorder.Image = global::GameLauncher.Properties.Resources.ticket_text_border;
            this.RegisterTicketBorder.Location = new System.Drawing.Point(19, 215);
            this.RegisterTicketBorder.Name = "RegisterTicketBorder";
            this.RegisterTicketBorder.Size = new System.Drawing.Size(230, 37);
            this.RegisterTicketBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RegisterTicketBorder.TabIndex = 141;
            this.RegisterTicketBorder.TabStop = false;
            // 
            // registerAgree
            // 
            this.registerAgree.BackColor = System.Drawing.Color.Transparent;
            this.registerAgree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.registerAgree.Location = new System.Drawing.Point(27, 257);
            this.registerAgree.Name = "registerAgree";
            this.registerAgree.Size = new System.Drawing.Size(232, 35);
            this.registerAgree.TabIndex = 16;
            this.registerAgree.Text = "BY REGISTERING YOU AGREE TO THE TERMS OF SERVICE";
            this.registerAgree.UseVisualStyleBackColor = false;
            this.registerAgree.CheckedChanged += new System.EventHandler(this.RegisterAgree_CheckedChanged);
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
            this.registerButton.Location = new System.Drawing.Point(18, 296);
            this.registerButton.Name = "registerButton";
            this.registerButton.Size = new System.Drawing.Size(230, 35);
            this.registerButton.TabIndex = 17;
            this.registerButton.Text = "REGISTER";
            this.registerButton.UseVisualStyleBackColor = false;
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
            this.registerCancel.Location = new System.Drawing.Point(19, 336);
            this.registerCancel.Name = "registerCancel";
            this.registerCancel.Size = new System.Drawing.Size(231, 35);
            this.registerCancel.TabIndex = 18;
            this.registerCancel.Text = "CANCEL";
            this.registerCancel.UseVisualStyleBackColor = false;
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SettingsPanel.BackColor = System.Drawing.Color.Transparent;
            this.SettingsPanel.Controls.Add(this.settingsLauncherVersion);
            this.SettingsPanel.Controls.Add(this.settingsAboutButton);
            this.SettingsPanel.Controls.Add(this.settingsGameFiles);
            this.SettingsPanel.Controls.Add(this.SettingsClearCommunicationLogButton);
            this.SettingsPanel.Controls.Add(this.SettingsClearCrashLogsButton);
            this.SettingsPanel.Controls.Add(this.settingsVFilesButton);
            this.SettingsPanel.Controls.Add(this.settingsGamePathText);
            this.SettingsPanel.Controls.Add(this.settingsSave);
            this.SettingsPanel.Controls.Add(this.settingsCancel);
            this.SettingsPanel.Controls.Add(this.settingsCDNText);
            this.SettingsPanel.Controls.Add(this.settingsCDNPick);
            this.SettingsPanel.Controls.Add(this.settingsLanguageText);
            this.SettingsPanel.Controls.Add(this.settingsLanguage);
            this.SettingsPanel.Controls.Add(this.settingsWordFilterCheck);
            this.SettingsPanel.Controls.Add(this.settingsProxyCheckbox);
            this.SettingsPanel.Controls.Add(this.settingsDiscordRPCCheckbox);
            this.SettingsPanel.Controls.Add(this.settingsGameFilesCurrentText);
            this.SettingsPanel.Controls.Add(this.settingsGameFilesCurrent);
            this.SettingsPanel.Controls.Add(this.settingsCDNCurrentText);
            this.SettingsPanel.Controls.Add(this.settingsCDNCurrent);
            this.SettingsPanel.Controls.Add(this.settingsLauncherPathText);
            this.SettingsPanel.Controls.Add(this.settingsLauncherPathCurrent);
            this.SettingsPanel.Controls.Add(this.settingsNetworkText);
            this.SettingsPanel.Controls.Add(this.settingsMainSrvText);
            this.SettingsPanel.Controls.Add(this.settingsMainCDNText);
            this.SettingsPanel.Controls.Add(this.settingsBkupSrvText);
            this.SettingsPanel.Controls.Add(this.settingsBkupCDNText);
            this.SettingsPanel.ForeColor = System.Drawing.Color.Transparent;
            this.SettingsPanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.SettingsPanel.Location = new System.Drawing.Point(27, 50);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(837, 452);
            this.SettingsPanel.TabIndex = 150;
            // 
            // settingsLauncherVersion
            // 
            this.settingsLauncherVersion.AutoSize = true;
            this.settingsLauncherVersion.BackColor = System.Drawing.Color.Transparent;
            this.settingsLauncherVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsLauncherVersion.Location = new System.Drawing.Point(28, 416);
            this.settingsLauncherVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.settingsLauncherVersion.Name = "settingsLauncherVersion";
            this.settingsLauncherVersion.Size = new System.Drawing.Size(84, 13);
            this.settingsLauncherVersion.TabIndex = 160;
            this.settingsLauncherVersion.Text = "Version v2.0.0.0";
            this.settingsLauncherVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // settingsAboutButton
            // 
            this.settingsAboutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.settingsAboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.settingsAboutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.settingsAboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsAboutButton.Location = new System.Drawing.Point(756, 5);
            this.settingsAboutButton.Name = "settingsAboutButton";
            this.settingsAboutButton.Size = new System.Drawing.Size(75, 23);
            this.settingsAboutButton.TabIndex = 159;
            this.settingsAboutButton.Text = "About";
            this.settingsAboutButton.UseVisualStyleBackColor = true;
            this.settingsAboutButton.Click += new System.EventHandler(this.PatchNotes_Click);
            // 
            // settingsGameFiles
            // 
            this.settingsGameFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.settingsGameFiles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.settingsGameFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.settingsGameFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsGameFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGameFiles.ForeColor = System.Drawing.Color.Silver;
            this.settingsGameFiles.Location = new System.Drawing.Point(26, 89);
            this.settingsGameFiles.Margin = new System.Windows.Forms.Padding(0);
            this.settingsGameFiles.Name = "settingsGameFiles";
            this.settingsGameFiles.Size = new System.Drawing.Size(220, 23);
            this.settingsGameFiles.TabIndex = 130;
            this.settingsGameFiles.Text = "Change GameFiles Path";
            this.settingsGameFiles.UseVisualStyleBackColor = false;
            // 
            // SettingsClearCommunicationLogButton
            // 
            this.SettingsClearCommunicationLogButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsClearCommunicationLogButton.Enabled = false;
            this.SettingsClearCommunicationLogButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsClearCommunicationLogButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsClearCommunicationLogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsClearCommunicationLogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SettingsClearCommunicationLogButton.ForeColor = System.Drawing.Color.Silver;
            this.SettingsClearCommunicationLogButton.Location = new System.Drawing.Point(26, 253);
            this.SettingsClearCommunicationLogButton.Margin = new System.Windows.Forms.Padding(0);
            this.SettingsClearCommunicationLogButton.Name = "SettingsClearCommunicationLogButton";
            this.SettingsClearCommunicationLogButton.Size = new System.Drawing.Size(131, 25);
            this.SettingsClearCommunicationLogButton.TabIndex = 134;
            this.SettingsClearCommunicationLogButton.Text = "Clear NFSWO Log";
            this.SettingsClearCommunicationLogButton.UseVisualStyleBackColor = false;
            this.SettingsClearCommunicationLogButton.Click += new System.EventHandler(this.SettingsClearCommunicationLogButton_Click);
            // 
            // SettingsClearCrashLogsButton
            // 
            this.SettingsClearCrashLogsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsClearCrashLogsButton.Enabled = false;
            this.SettingsClearCrashLogsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsClearCrashLogsButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsClearCrashLogsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsClearCrashLogsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SettingsClearCrashLogsButton.ForeColor = System.Drawing.Color.Silver;
            this.SettingsClearCrashLogsButton.Location = new System.Drawing.Point(26, 216);
            this.SettingsClearCrashLogsButton.Margin = new System.Windows.Forms.Padding(0);
            this.SettingsClearCrashLogsButton.Name = "SettingsClearCrashLogsButton";
            this.SettingsClearCrashLogsButton.Size = new System.Drawing.Size(131, 25);
            this.SettingsClearCrashLogsButton.TabIndex = 133;
            this.SettingsClearCrashLogsButton.Text = "Clear Crash Logs";
            this.SettingsClearCrashLogsButton.UseVisualStyleBackColor = false;
            this.SettingsClearCrashLogsButton.Click += new System.EventHandler(this.SettingsClearCrashLogsButton_Click);
            // 
            // settingsVFilesButton
            // 
            this.settingsVFilesButton.AutoSize = true;
            this.settingsVFilesButton.BackColor = System.Drawing.Color.Transparent;
            this.settingsVFilesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsVFilesButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.settingsVFilesButton.Location = new System.Drawing.Point(26, 354);
            this.settingsVFilesButton.Name = "settingsVFilesButton";
            this.settingsVFilesButton.Size = new System.Drawing.Size(131, 23);
            this.settingsVFilesButton.TabIndex = 141;
            this.settingsVFilesButton.Text = "Validate Game Files";
            this.settingsVFilesButton.UseVisualStyleBackColor = false;
            this.settingsVFilesButton.Visible = false;
            // 
            // settingsGamePathText
            // 
            this.settingsGamePathText.AutoSize = true;
            this.settingsGamePathText.BackColor = System.Drawing.Color.Transparent;
            this.settingsGamePathText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsGamePathText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsGamePathText.Location = new System.Drawing.Point(24, 67);
            this.settingsGamePathText.Name = "settingsGamePathText";
            this.settingsGamePathText.Size = new System.Drawing.Size(135, 18);
            this.settingsGamePathText.TabIndex = 135;
            this.settingsGamePathText.Text = "GAMEFILES PATH";
            this.settingsGamePathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
            this.settingsSave.Location = new System.Drawing.Point(561, 392);
            this.settingsSave.Name = "settingsSave";
            this.settingsSave.Size = new System.Drawing.Size(130, 50);
            this.settingsSave.TabIndex = 151;
            this.settingsSave.Text = "SAVE";
            this.settingsSave.UseVisualStyleBackColor = false;
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
            this.settingsCancel.Location = new System.Drawing.Point(697, 392);
            this.settingsCancel.Name = "settingsCancel";
            this.settingsCancel.Size = new System.Drawing.Size(130, 50);
            this.settingsCancel.TabIndex = 152;
            this.settingsCancel.Text = "CANCEL";
            this.settingsCancel.UseVisualStyleBackColor = false;
            // 
            // settingsCDNText
            // 
            this.settingsCDNText.AutoSize = true;
            this.settingsCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsCDNText.Location = new System.Drawing.Point(24, 111);
            this.settingsCDNText.Name = "settingsCDNText";
            this.settingsCDNText.Size = new System.Drawing.Size(111, 18);
            this.settingsCDNText.TabIndex = 137;
            this.settingsCDNText.Text = "CDN: PINGING";
            this.settingsCDNText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsCDNPick
            // 
            this.settingsCDNPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.settingsCDNPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsCDNPick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsCDNPick.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsCDNPick.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.settingsCDNPick.FormattingEnabled = true;
            this.settingsCDNPick.Location = new System.Drawing.Point(26, 133);
            this.settingsCDNPick.Name = "settingsCDNPick";
            this.settingsCDNPick.Size = new System.Drawing.Size(220, 21);
            this.settingsCDNPick.TabIndex = 133;
            this.settingsCDNPick.SelectedIndexChanged += new System.EventHandler(this.SettingsCDNPick_SelectedIndexChanged);
            // 
            // settingsLanguageText
            // 
            this.settingsLanguageText.AutoSize = true;
            this.settingsLanguageText.BackColor = System.Drawing.Color.Transparent;
            this.settingsLanguageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsLanguageText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsLanguageText.Location = new System.Drawing.Point(24, 158);
            this.settingsLanguageText.Name = "settingsLanguageText";
            this.settingsLanguageText.Size = new System.Drawing.Size(138, 18);
            this.settingsLanguageText.TabIndex = 131;
            this.settingsLanguageText.Text = "GAME LANGUAGE";
            this.settingsLanguageText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsLanguage
            // 
            this.settingsLanguage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.settingsLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.settingsLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingsLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.settingsLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.settingsLanguage.FormattingEnabled = true;
            this.settingsLanguage.Location = new System.Drawing.Point(26, 180);
            this.settingsLanguage.Name = "settingsLanguage";
            this.settingsLanguage.Size = new System.Drawing.Size(131, 21);
            this.settingsLanguage.TabIndex = 133;
            // 
            // settingsWordFilterCheck
            // 
            this.settingsWordFilterCheck.AutoSize = true;
            this.settingsWordFilterCheck.BackColor = System.Drawing.Color.Transparent;
            this.settingsWordFilterCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsWordFilterCheck.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsWordFilterCheck.Location = new System.Drawing.Point(26, 288);
            this.settingsWordFilterCheck.Name = "settingsWordFilterCheck";
            this.settingsWordFilterCheck.Size = new System.Drawing.Size(235, 17);
            this.settingsWordFilterCheck.TabIndex = 135;
            this.settingsWordFilterCheck.Text = "Disable Word Filtering on Game Chat";
            this.settingsWordFilterCheck.UseVisualStyleBackColor = false;
            // 
            // settingsProxyCheckbox
            // 
            this.settingsProxyCheckbox.AutoSize = true;
            this.settingsProxyCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.settingsProxyCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsProxyCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsProxyCheckbox.Location = new System.Drawing.Point(26, 308);
            this.settingsProxyCheckbox.Name = "settingsProxyCheckbox";
            this.settingsProxyCheckbox.Size = new System.Drawing.Size(103, 17);
            this.settingsProxyCheckbox.TabIndex = 136;
            this.settingsProxyCheckbox.Text = "Disable Proxy";
            this.settingsProxyCheckbox.UseVisualStyleBackColor = false;
            // 
            // settingsDiscordRPCCheckbox
            // 
            this.settingsDiscordRPCCheckbox.AutoSize = true;
            this.settingsDiscordRPCCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.settingsDiscordRPCCheckbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsDiscordRPCCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.settingsDiscordRPCCheckbox.Location = new System.Drawing.Point(26, 328);
            this.settingsDiscordRPCCheckbox.Name = "settingsDiscordRPCCheckbox";
            this.settingsDiscordRPCCheckbox.Size = new System.Drawing.Size(144, 17);
            this.settingsDiscordRPCCheckbox.TabIndex = 137;
            this.settingsDiscordRPCCheckbox.Text = "Disable Discord RPC";
            this.settingsDiscordRPCCheckbox.UseVisualStyleBackColor = false;
            // 
            // settingsGameFilesCurrentText
            // 
            this.settingsGameFilesCurrentText.AutoSize = true;
            this.settingsGameFilesCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.settingsGameFilesCurrentText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsGameFilesCurrentText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsGameFilesCurrentText.Location = new System.Drawing.Point(313, 82);
            this.settingsGameFilesCurrentText.Name = "settingsGameFilesCurrentText";
            this.settingsGameFilesCurrentText.Size = new System.Drawing.Size(147, 13);
            this.settingsGameFilesCurrentText.TabIndex = 149;
            this.settingsGameFilesCurrentText.Text = "CURRENT DIRECTORY:";
            this.settingsGameFilesCurrentText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsGameFilesCurrent
            // 
            this.settingsGameFilesCurrent.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsGameFilesCurrent.AutoSize = true;
            this.settingsGameFilesCurrent.BackColor = System.Drawing.Color.Transparent;
            this.settingsGameFilesCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsGameFilesCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.settingsGameFilesCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.settingsGameFilesCurrent.Location = new System.Drawing.Point(312, 99);
            this.settingsGameFilesCurrent.Name = "settingsGameFilesCurrent";
            this.settingsGameFilesCurrent.Size = new System.Drawing.Size(181, 13);
            this.settingsGameFilesCurrent.TabIndex = 138;
            this.settingsGameFilesCurrent.TabStop = true;
            this.settingsGameFilesCurrent.Text = "C:\\Soapbox Race World\\Game Files";
            this.settingsGameFilesCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.settingsGameFilesCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // settingsCDNCurrentText
            // 
            this.settingsCDNCurrentText.AutoSize = true;
            this.settingsCDNCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrentText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsCDNCurrentText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsCDNCurrentText.Location = new System.Drawing.Point(313, 120);
            this.settingsCDNCurrentText.Name = "settingsCDNCurrentText";
            this.settingsCDNCurrentText.Size = new System.Drawing.Size(101, 13);
            this.settingsCDNCurrentText.TabIndex = 150;
            this.settingsCDNCurrentText.Text = "CURRENT CDN:";
            this.settingsCDNCurrentText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsCDNCurrent
            // 
            this.settingsCDNCurrent.ActiveLinkColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrent.AutoSize = true;
            this.settingsCDNCurrent.BackColor = System.Drawing.Color.Transparent;
            this.settingsCDNCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsCDNCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.settingsCDNCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.settingsCDNCurrent.Location = new System.Drawing.Point(313, 137);
            this.settingsCDNCurrent.Name = "settingsCDNCurrent";
            this.settingsCDNCurrent.Size = new System.Drawing.Size(80, 13);
            this.settingsCDNCurrent.TabIndex = 139;
            this.settingsCDNCurrent.TabStop = true;
            this.settingsCDNCurrent.Text = "http://localhost";
            this.settingsCDNCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.settingsCDNCurrent.VisitedLinkColor = System.Drawing.Color.White;
            this.settingsCDNCurrent.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SettingsCDNCurrent_LinkClicked);
            // 
            // settingsLauncherPathText
            // 
            this.settingsLauncherPathText.AutoSize = true;
            this.settingsLauncherPathText.BackColor = System.Drawing.Color.Transparent;
            this.settingsLauncherPathText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.settingsLauncherPathText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsLauncherPathText.Location = new System.Drawing.Point(312, 158);
            this.settingsLauncherPathText.Name = "settingsLauncherPathText";
            this.settingsLauncherPathText.Size = new System.Drawing.Size(131, 13);
            this.settingsLauncherPathText.TabIndex = 136;
            this.settingsLauncherPathText.Text = "LAUNCHER FOLDER:";
            this.settingsLauncherPathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsLauncherPathCurrent
            // 
            this.settingsLauncherPathCurrent.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsLauncherPathCurrent.AutoSize = true;
            this.settingsLauncherPathCurrent.BackColor = System.Drawing.Color.Transparent;
            this.settingsLauncherPathCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsLauncherPathCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.settingsLauncherPathCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.settingsLauncherPathCurrent.Location = new System.Drawing.Point(311, 176);
            this.settingsLauncherPathCurrent.Name = "settingsLauncherPathCurrent";
            this.settingsLauncherPathCurrent.Size = new System.Drawing.Size(174, 13);
            this.settingsLauncherPathCurrent.TabIndex = 140;
            this.settingsLauncherPathCurrent.TabStop = true;
            this.settingsLauncherPathCurrent.Text = "C:\\Soapbox Race World\\Launcher";
            this.settingsLauncherPathCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.settingsLauncherPathCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // settingsNetworkText
            // 
            this.settingsNetworkText.AutoSize = true;
            this.settingsNetworkText.BackColor = System.Drawing.Color.Transparent;
            this.settingsNetworkText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
            this.settingsNetworkText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.settingsNetworkText.Location = new System.Drawing.Point(313, 206);
            this.settingsNetworkText.Name = "settingsNetworkText";
            this.settingsNetworkText.Size = new System.Drawing.Size(175, 18);
            this.settingsNetworkText.TabIndex = 142;
            this.settingsNetworkText.Text = "CONNECTION STATUS:";
            this.settingsNetworkText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // settingsMainSrvText
            // 
            this.settingsMainSrvText.AutoSize = true;
            this.settingsMainSrvText.BackColor = System.Drawing.Color.Transparent;
            this.settingsMainSrvText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsMainSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsMainSrvText.Location = new System.Drawing.Point(312, 228);
            this.settingsMainSrvText.Name = "settingsMainSrvText";
            this.settingsMainSrvText.Size = new System.Drawing.Size(154, 13);
            this.settingsMainSrvText.TabIndex = 143;
            this.settingsMainSrvText.Text = "Main Server List API: PINGING";
            // 
            // settingsMainCDNText
            // 
            this.settingsMainCDNText.AutoSize = true;
            this.settingsMainCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsMainCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsMainCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsMainCDNText.Location = new System.Drawing.Point(312, 248);
            this.settingsMainCDNText.Name = "settingsMainCDNText";
            this.settingsMainCDNText.Size = new System.Drawing.Size(146, 13);
            this.settingsMainCDNText.TabIndex = 146;
            this.settingsMainCDNText.Text = "Main CDN List API: PINGING";
            // 
            // settingsBkupSrvText
            // 
            this.settingsBkupSrvText.AutoSize = true;
            this.settingsBkupSrvText.BackColor = System.Drawing.Color.Transparent;
            this.settingsBkupSrvText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsBkupSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsBkupSrvText.Location = new System.Drawing.Point(312, 268);
            this.settingsBkupSrvText.Name = "settingsBkupSrvText";
            this.settingsBkupSrvText.Size = new System.Drawing.Size(168, 13);
            this.settingsBkupSrvText.TabIndex = 144;
            this.settingsBkupSrvText.Text = "Backup Server List API: PINGING";
            // 
            // settingsBkupCDNText
            // 
            this.settingsBkupCDNText.AutoSize = true;
            this.settingsBkupCDNText.BackColor = System.Drawing.Color.Transparent;
            this.settingsBkupCDNText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.settingsBkupCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.settingsBkupCDNText.Location = new System.Drawing.Point(312, 288);
            this.settingsBkupCDNText.Name = "settingsBkupCDNText";
            this.settingsBkupCDNText.Size = new System.Drawing.Size(160, 13);
            this.settingsBkupCDNText.TabIndex = 145;
            this.settingsBkupCDNText.Text = "Backup CDN List API: PINGING";
            // 
            // ShowPlayPanel
            // 
            this.ShowPlayPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ShowPlayPanel.BackColor = System.Drawing.Color.Transparent;
            this.ShowPlayPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ShowPlayPanel.Controls.Add(this.logoutButton);
            this.ShowPlayPanel.Controls.Add(this.playButton);
            this.ShowPlayPanel.ForeColor = System.Drawing.Color.Transparent;
            this.ShowPlayPanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.ShowPlayPanel.Location = new System.Drawing.Point(600, 290);
            this.ShowPlayPanel.Name = "ShowPlayPanel";
            this.ShowPlayPanel.Size = new System.Drawing.Size(237, 164);
            this.ShowPlayPanel.TabIndex = 153;
            // 
            // RegisterPanel
            // 
            this.RegisterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RegisterPanel.BackColor = System.Drawing.Color.Transparent;
            this.RegisterPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RegisterPanel.Controls.Add(this.registerEmail);
            this.RegisterPanel.Controls.Add(this.RegisterEmailBorder);
            this.RegisterPanel.Controls.Add(this.registerPassword);
            this.RegisterPanel.Controls.Add(this.RegisterPasswordBorder);
            this.RegisterPanel.Controls.Add(this.registerConfirmPassword);
            this.RegisterPanel.Controls.Add(this.RegisterPasswordValidateBorder);
            this.RegisterPanel.Controls.Add(this.registerTicket);
            this.RegisterPanel.Controls.Add(this.RegisterTicketBorder);
            this.RegisterPanel.Controls.Add(this.registerAgree);
            this.RegisterPanel.Controls.Add(this.registerButton);
            this.RegisterPanel.Controls.Add(this.registerCancel);
            this.RegisterPanel.ForeColor = System.Drawing.Color.Transparent;
            this.RegisterPanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.RegisterPanel.Location = new System.Drawing.Point(587, 93);
            this.RegisterPanel.Name = "RegisterPanel";
            this.RegisterPanel.Size = new System.Drawing.Size(263, 374);
            this.RegisterPanel.TabIndex = 156;
            // 
            // DiscordInviteLink
            // 
            this.DiscordInviteLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DiscordInviteLink.AutoSize = true;
            this.DiscordInviteLink.BackColor = System.Drawing.Color.Transparent;
            this.DiscordInviteLink.ForeColor = System.Drawing.Color.Transparent;
            this.DiscordInviteLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.DiscordInviteLink.Location = new System.Drawing.Point(106, 11);
            this.DiscordInviteLink.Name = "DiscordInviteLink";
            this.DiscordInviteLink.Size = new System.Drawing.Size(43, 26);
            this.DiscordInviteLink.TabIndex = 163;
            this.DiscordInviteLink.TabStop = true;
            this.DiscordInviteLink.Text = "Discord\r\nInvite";
            this.DiscordInviteLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DiscordInviteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DiscordInviteLink_LinkClicked);
            // 
            // ServerShutDown
            // 
            this.ServerShutDown.BackColor = System.Drawing.Color.Transparent;
            this.ServerShutDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ServerShutDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerShutDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ServerShutDown.Location = new System.Drawing.Point(155, 58);
            this.ServerShutDown.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ServerShutDown.Name = "ServerShutDown";
            this.ServerShutDown.Size = new System.Drawing.Size(162, 15);
            this.ServerShutDown.TabIndex = 169;
            this.ServerShutDown.Text = "Restart Timer: 0 Hours";
            this.ServerShutDown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ServerInfoPanel
            // 
            this.ServerInfoPanel.BackColor = System.Drawing.Color.Transparent;
            this.ServerInfoPanel.BackgroundImage = global::GameLauncher.Properties.Resources.eyecatcher;
            this.ServerInfoPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ServerInfoPanel.Controls.Add(this.HomePageIcon);
            this.ServerInfoPanel.Controls.Add(this.DiscordIcon);
            this.ServerInfoPanel.Controls.Add(this.FacebookIcon);
            this.ServerInfoPanel.Controls.Add(this.TwitterAccountLink);
            this.ServerInfoPanel.Controls.Add(this.TwitterIcon);
            this.ServerInfoPanel.Controls.Add(this.FacebookGroupLink);
            this.ServerInfoPanel.Controls.Add(this.HomePageLink);
            this.ServerInfoPanel.Controls.Add(this.DiscordInviteLink);
            this.ServerInfoPanel.Controls.Add(this.ServerShutDown);
            this.ServerInfoPanel.Controls.Add(this.SceneryGroupText);
            this.ServerInfoPanel.Location = new System.Drawing.Point(142, 301);
            this.ServerInfoPanel.Name = "ServerInfoPanel";
            this.ServerInfoPanel.Size = new System.Drawing.Size(325, 88);
            this.ServerInfoPanel.TabIndex = 172;
            // 
            // HomePageIcon
            // 
            this.HomePageIcon.BackColor = System.Drawing.Color.Transparent;
            this.HomePageIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_home_page;
            this.HomePageIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.HomePageIcon.Location = new System.Drawing.Point(10, 11);
            this.HomePageIcon.Margin = new System.Windows.Forms.Padding(2);
            this.HomePageIcon.Name = "HomePageIcon";
            this.HomePageIcon.Size = new System.Drawing.Size(24, 24);
            this.HomePageIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.HomePageIcon.TabIndex = 178;
            this.HomePageIcon.TabStop = false;
            // 
            // DiscordIcon
            // 
            this.DiscordIcon.BackColor = System.Drawing.Color.Transparent;
            this.DiscordIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_discord;
            this.DiscordIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.DiscordIcon.Location = new System.Drawing.Point(79, 13);
            this.DiscordIcon.Margin = new System.Windows.Forms.Padding(2);
            this.DiscordIcon.Name = "DiscordIcon";
            this.DiscordIcon.Size = new System.Drawing.Size(24, 24);
            this.DiscordIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.DiscordIcon.TabIndex = 177;
            this.DiscordIcon.TabStop = false;
            // 
            // FacebookIcon
            // 
            this.FacebookIcon.BackColor = System.Drawing.Color.Transparent;
            this.FacebookIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_facebook;
            this.FacebookIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.FacebookIcon.Location = new System.Drawing.Point(157, 13);
            this.FacebookIcon.Margin = new System.Windows.Forms.Padding(2);
            this.FacebookIcon.Name = "FacebookIcon";
            this.FacebookIcon.Size = new System.Drawing.Size(24, 24);
            this.FacebookIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.FacebookIcon.TabIndex = 176;
            this.FacebookIcon.TabStop = false;
            // 
            // TwitterAccountLink
            // 
            this.TwitterAccountLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TwitterAccountLink.AutoSize = true;
            this.TwitterAccountLink.BackColor = System.Drawing.Color.Transparent;
            this.TwitterAccountLink.ForeColor = System.Drawing.Color.Transparent;
            this.TwitterAccountLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.TwitterAccountLink.Location = new System.Drawing.Point(274, 11);
            this.TwitterAccountLink.Name = "TwitterAccountLink";
            this.TwitterAccountLink.Size = new System.Drawing.Size(39, 26);
            this.TwitterAccountLink.TabIndex = 175;
            this.TwitterAccountLink.TabStop = true;
            this.TwitterAccountLink.Text = "Twitter\r\nPage";
            this.TwitterAccountLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TwitterAccountLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TwitterAccountLink_LinkClicked);
            // 
            // TwitterIcon
            // 
            this.TwitterIcon.BackColor = System.Drawing.Color.Transparent;
            this.TwitterIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_twitter;
            this.TwitterIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TwitterIcon.Location = new System.Drawing.Point(246, 11);
            this.TwitterIcon.Margin = new System.Windows.Forms.Padding(2);
            this.TwitterIcon.Name = "TwitterIcon";
            this.TwitterIcon.Size = new System.Drawing.Size(24, 24);
            this.TwitterIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.TwitterIcon.TabIndex = 174;
            this.TwitterIcon.TabStop = false;
            // 
            // FacebookGroupLink
            // 
            this.FacebookGroupLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FacebookGroupLink.AutoSize = true;
            this.FacebookGroupLink.BackColor = System.Drawing.Color.Transparent;
            this.FacebookGroupLink.ForeColor = System.Drawing.Color.Transparent;
            this.FacebookGroupLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.FacebookGroupLink.Location = new System.Drawing.Point(186, 11);
            this.FacebookGroupLink.Name = "FacebookGroupLink";
            this.FacebookGroupLink.Size = new System.Drawing.Size(55, 26);
            this.FacebookGroupLink.TabIndex = 173;
            this.FacebookGroupLink.TabStop = true;
            this.FacebookGroupLink.Text = "Facebook\r\nGroup";
            this.FacebookGroupLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.FacebookGroupLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FacebookGroupLink_LinkClicked);
            // 
            // HomePageLink
            // 
            this.HomePageLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HomePageLink.AutoSize = true;
            this.HomePageLink.BackColor = System.Drawing.Color.Transparent;
            this.HomePageLink.ForeColor = System.Drawing.Color.Transparent;
            this.HomePageLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.HomePageLink.Location = new System.Drawing.Point(39, 11);
            this.HomePageLink.Name = "HomePageLink";
            this.HomePageLink.Size = new System.Drawing.Size(35, 26);
            this.HomePageLink.TabIndex = 171;
            this.HomePageLink.TabStop = true;
            this.HomePageLink.Text = "Home\r\nPage";
            this.HomePageLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HomePageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HomePageLink_LinkClicked);
            // 
            // SceneryGroupText
            // 
            this.SceneryGroupText.BackColor = System.Drawing.Color.Transparent;
            this.SceneryGroupText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SceneryGroupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SceneryGroupText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SceneryGroupText.Location = new System.Drawing.Point(8, 59);
            this.SceneryGroupText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SceneryGroupText.Name = "SceneryGroupText";
            this.SceneryGroupText.Size = new System.Drawing.Size(140, 14);
            this.SceneryGroupText.TabIndex = 179;
            this.SceneryGroupText.Text = "Scenery: Normal";
            this.SceneryGroupText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // extractingProgress
            // 
            this.extractingProgress.BackColor = System.Drawing.Color.Transparent;
            this.extractingProgress.BackgroundColor = System.Drawing.Color.Black;
            this.extractingProgress.Border = false;
            this.extractingProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.extractingProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.extractingProgress.Image = global::GameLauncher.Properties.Resources.progress_success;
            this.extractingProgress.Location = new System.Drawing.Point(30, 425);
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
            this.playProgress.Image = global::GameLauncher.Properties.Resources.progress_preload;
            this.playProgress.Location = new System.Drawing.Point(30, 425);
            this.playProgress.Name = "playProgress";
            this.playProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.playProgress.RoundedCorners = false;
            this.playProgress.Size = new System.Drawing.Size(519, 13);
            this.playProgress.Text = "downloadProgress";
            // 
            // MainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.mainbackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(891, 529);
            this.Controls.Add(this.currentWindowInfo);
            this.Controls.Add(this.ServerPingStatusText);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.RegisterPanel);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.ShowPlayPanel);
            this.Controls.Add(this.closebtn);
            this.Controls.Add(this.SettingsPanel);
            this.Controls.Add(this.SelectServerBtn);
            this.Controls.Add(this.translatedBy);
            this.Controls.Add(this.serverPick);
            this.Controls.Add(this.addServer);
            this.Controls.Add(this.playProgressText);
            this.Controls.Add(this.extractingProgress);
            this.Controls.Add(this.launcherIconStatus);
            this.Controls.Add(this.APIStatusIcon);
            this.Controls.Add(this.ServerStatusIcon);
            this.Controls.Add(this.launcherStatusText);
            this.Controls.Add(this.launcherStatusDesc);
            this.Controls.Add(this.ServerStatusText);
            this.Controls.Add(this.ServerStatusDesc);
            this.Controls.Add(this.APIStatusText);
            this.Controls.Add(this.APIStatusDesc);
            this.Controls.Add(this.email);
            this.Controls.Add(this.MainEmailBorder);
            this.Controls.Add(this.password);
            this.Controls.Add(this.MainPasswordBorder);
            this.Controls.Add(this.rememberMe);
            this.Controls.Add(this.forgotPassword);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.registerText);
            this.Controls.Add(this.playProgress);
            this.Controls.Add(this.playProgressTextTimer);
            this.Controls.Add(this.verticalBanner);
            this.Controls.Add(this.ServerInfoPanel);
            this.Controls.Add(this.imageServerName);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closebtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.launcherIconStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterEmailBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordValidateBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterTicketBorder)).EndInit();
            this.SettingsPanel.ResumeLayout(false);
            this.SettingsPanel.PerformLayout();
            this.ShowPlayPanel.ResumeLayout(false);
            this.RegisterPanel.ResumeLayout(false);
            this.RegisterPanel.PerformLayout();
            this.ServerInfoPanel.ResumeLayout(false);
            this.ServerInfoPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HomePageIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscordIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacebookIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TwitterIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer Timeout;
        private System.Windows.Forms.NotifyIcon Notification;
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.PictureBox closebtn;
        private System.Windows.Forms.Button SelectServerBtn;
        private System.Windows.Forms.Label translatedBy;
        private System.Windows.Forms.PictureBox settingsButton;
        private System.Windows.Forms.ComboBox serverPick;
        private System.Windows.Forms.Button addServer;
        private System.Windows.Forms.Label imageServerName;
        private System.Windows.Forms.PictureBox verticalBanner;
        internal System.Windows.Forms.Label playProgressText;
        private GameLauncherReborn.ProgressBarEx extractingProgress;
        private GameLauncherReborn.ProgressBarEx playProgress;
        private System.Windows.Forms.PictureBox launcherIconStatus;
        private System.Windows.Forms.Label launcherStatusText;
        private System.Windows.Forms.Label launcherStatusDesc;
        private System.Windows.Forms.PictureBox ServerStatusIcon;
        private System.Windows.Forms.Label ServerStatusText;
        private System.Windows.Forms.Label ServerStatusDesc;
        private System.Windows.Forms.PictureBox APIStatusIcon;
        private System.Windows.Forms.Label APIStatusText;
        private System.Windows.Forms.Label APIStatusDesc;
        private System.Windows.Forms.Label currentWindowInfo;
        private System.Windows.Forms.TextBox email;
        private System.Windows.Forms.PictureBox MainEmailBorder;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.PictureBox MainPasswordBorder;
        private System.Windows.Forms.CheckBox rememberMe;
        private System.Windows.Forms.LinkLabel forgotPassword;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button registerText;
        private System.Windows.Forms.Label ServerPingStatusText;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Button playButton;
        internal System.Windows.Forms.Label playProgressTextTimer;
        private System.Windows.Forms.TextBox registerEmail;
        private System.Windows.Forms.PictureBox RegisterEmailBorder;
        private System.Windows.Forms.TextBox registerPassword;
        private System.Windows.Forms.PictureBox RegisterPasswordBorder;
        private System.Windows.Forms.TextBox registerConfirmPassword;
        private System.Windows.Forms.PictureBox RegisterPasswordValidateBorder;
        private System.Windows.Forms.TextBox registerTicket;
        private System.Windows.Forms.PictureBox RegisterTicketBorder;
        private System.Windows.Forms.CheckBox registerAgree;
        private System.Windows.Forms.Button registerButton;
        private System.Windows.Forms.Button registerCancel;
        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Button settingsSave;
        private System.Windows.Forms.Button settingsCancel;
        private System.Windows.Forms.Label settingsGamePathText;
        private System.Windows.Forms.Button settingsGameFiles;
        private System.Windows.Forms.Label settingsCDNText;
        private System.Windows.Forms.ComboBox settingsCDNPick;
        private System.Windows.Forms.Label settingsLanguageText;
        private System.Windows.Forms.ComboBox settingsLanguage;
        private System.Windows.Forms.CheckBox settingsWordFilterCheck;
        private System.Windows.Forms.CheckBox settingsProxyCheckbox;
        private System.Windows.Forms.CheckBox settingsDiscordRPCCheckbox;
        private System.Windows.Forms.Button SettingsClearCrashLogsButton;
        private System.Windows.Forms.Label settingsGameFilesCurrentText;
        private System.Windows.Forms.LinkLabel settingsGameFilesCurrent;
        private System.Windows.Forms.Label settingsCDNCurrentText;
        private System.Windows.Forms.LinkLabel settingsCDNCurrent;
        private System.Windows.Forms.Label settingsLauncherPathText;
        private System.Windows.Forms.LinkLabel settingsLauncherPathCurrent;
        private System.Windows.Forms.Label settingsNetworkText;
        private System.Windows.Forms.Label settingsMainSrvText;
        private System.Windows.Forms.Label settingsMainCDNText;
        private System.Windows.Forms.Label settingsBkupSrvText;
        private System.Windows.Forms.Label settingsBkupCDNText;
        private System.Windows.Forms.Panel ShowPlayPanel;
        private System.Windows.Forms.Panel RegisterPanel;
        private System.Windows.Forms.Button settingsVFilesButton;
        private System.Windows.Forms.Button SettingsClearCommunicationLogButton;
        private System.Windows.Forms.Button settingsAboutButton;
        private System.Windows.Forms.Label settingsLauncherVersion;
        private System.Windows.Forms.LinkLabel DiscordInviteLink;
        private System.Windows.Forms.Label ServerShutDown;
        private System.Windows.Forms.Panel ServerInfoPanel;
        private System.Windows.Forms.LinkLabel TwitterAccountLink;
        private System.Windows.Forms.PictureBox TwitterIcon;
        private System.Windows.Forms.LinkLabel FacebookGroupLink;
        private System.Windows.Forms.LinkLabel HomePageLink;
        private System.Windows.Forms.PictureBox HomePageIcon;
        private System.Windows.Forms.PictureBox DiscordIcon;
        private System.Windows.Forms.PictureBox FacebookIcon;
        private System.Windows.Forms.Label SceneryGroupText;
    }
}
