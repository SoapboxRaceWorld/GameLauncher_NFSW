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
            this.CloseBTN = new System.Windows.Forms.PictureBox();
            this.SelectServerBtn = new System.Windows.Forms.Button();
            this.translatedBy = new System.Windows.Forms.Label();
            this.SettingsButton = new System.Windows.Forms.PictureBox();
            this.ServerPick = new System.Windows.Forms.ComboBox();
            this.AddServer = new System.Windows.Forms.Button();
            this.ImageServerName = new System.Windows.Forms.Label();
            this.VerticalBanner = new System.Windows.Forms.PictureBox();
            this.PlayProgressText = new System.Windows.Forms.Label();
            this.LauncherIconStatus = new System.Windows.Forms.PictureBox();
            this.LauncherStatusText = new System.Windows.Forms.Label();
            this.LauncherStatusDesc = new System.Windows.Forms.Label();
            this.ServerStatusIcon = new System.Windows.Forms.PictureBox();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.ServerStatusDesc = new System.Windows.Forms.Label();
            this.APIStatusIcon = new System.Windows.Forms.PictureBox();
            this.APIStatusText = new System.Windows.Forms.Label();
            this.APIStatusDesc = new System.Windows.Forms.Label();
            this.CurrentWindowInfo = new System.Windows.Forms.Label();
            this.MainEmail = new System.Windows.Forms.TextBox();
            this.MainEmailBorder = new System.Windows.Forms.PictureBox();
            this.MainPassword = new System.Windows.Forms.TextBox();
            this.MainPasswordBorder = new System.Windows.Forms.PictureBox();
            this.RememberMe = new System.Windows.Forms.CheckBox();
            this.ForgotPassword = new System.Windows.Forms.LinkLabel();
            this.LoginButton = new System.Windows.Forms.Button();
            this.RegisterText = new System.Windows.Forms.Button();
            this.ServerPingStatusText = new System.Windows.Forms.Label();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.PlayProgressTextTimer = new System.Windows.Forms.Label();
            this.RegisterEmail = new System.Windows.Forms.TextBox();
            this.RegisterEmailBorder = new System.Windows.Forms.PictureBox();
            this.RegisterPassword = new System.Windows.Forms.TextBox();
            this.RegisterPasswordBorder = new System.Windows.Forms.PictureBox();
            this.RegisterConfirmPassword = new System.Windows.Forms.TextBox();
            this.RegisterConfirmPasswordBorder = new System.Windows.Forms.PictureBox();
            this.RegisterTicket = new System.Windows.Forms.TextBox();
            this.RegisterTicketBorder = new System.Windows.Forms.PictureBox();
            this.RegisterAgree = new System.Windows.Forms.CheckBox();
            this.RegisterButton = new System.Windows.Forms.Button();
            this.RegisterCancel = new System.Windows.Forms.Button();
            this.SettingsPanel = new System.Windows.Forms.Panel();
            this.SettingsClearServerModCacheButton = new System.Windows.Forms.Button();
            this.SettingsLauncherVersion = new System.Windows.Forms.Label();
            this.SettingsAboutButton = new System.Windows.Forms.Button();
            this.SettingsGameFiles = new System.Windows.Forms.Button();
            this.SettingsClearCommunicationLogButton = new System.Windows.Forms.Button();
            this.SettingsClearCrashLogsButton = new System.Windows.Forms.Button();
            this.SettingsVFilesButton = new System.Windows.Forms.Button();
            this.SettingsGamePathText = new System.Windows.Forms.Label();
            this.SettingsSave = new System.Windows.Forms.Button();
            this.SettingsCancel = new System.Windows.Forms.Button();
            this.SettingsCDNText = new System.Windows.Forms.Label();
            this.SettingsCDNPick = new System.Windows.Forms.ComboBox();
            this.SettingsLanguageText = new System.Windows.Forms.Label();
            this.SettingsLanguage = new System.Windows.Forms.ComboBox();
            this.SettingsWordFilterCheck = new System.Windows.Forms.CheckBox();
            this.SettingsProxyCheckbox = new System.Windows.Forms.CheckBox();
            this.SettingsDiscordRPCCheckbox = new System.Windows.Forms.CheckBox();
            this.SettingsGameFilesCurrentText = new System.Windows.Forms.Label();
            this.SettingsGameFilesCurrent = new System.Windows.Forms.LinkLabel();
            this.SettingsCDNCurrentText = new System.Windows.Forms.Label();
            this.SettingsCDNCurrent = new System.Windows.Forms.LinkLabel();
            this.SettingsLauncherPathText = new System.Windows.Forms.Label();
            this.SettingsLauncherPathCurrent = new System.Windows.Forms.LinkLabel();
            this.SettingsNetworkText = new System.Windows.Forms.Label();
            this.SettingsMainSrvText = new System.Windows.Forms.Label();
            this.SettingsMainCDNText = new System.Windows.Forms.Label();
            this.SettingsBkupSrvText = new System.Windows.Forms.Label();
            this.SettingsBkupCDNText = new System.Windows.Forms.Label();
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
            this.ExtractingProgress = new GameLauncherReborn.ProgressBarEx();
            this.PlayProgress = new GameLauncherReborn.ProgressBarEx();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBTN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VerticalBanner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LauncherIconStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterEmailBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterConfirmPasswordBorder)).BeginInit();
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
            // CloseBTN
            // 
            this.CloseBTN.BackColor = System.Drawing.Color.Transparent;
            this.CloseBTN.BackgroundImage = global::GameLauncher.Properties.Resources.close;
            this.CloseBTN.Location = new System.Drawing.Point(841, 15);
            this.CloseBTN.Name = "CloseBTN";
            this.CloseBTN.Size = new System.Drawing.Size(24, 24);
            this.CloseBTN.TabIndex = 0;
            this.CloseBTN.TabStop = false;
            this.CloseBTN.Click += new System.EventHandler(this.CloseBTN_Click);
            // 
            // SelectServerBtn
            // 
            this.SelectServerBtn.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.translatedBy.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.translatedBy.ForeColor = System.Drawing.Color.DarkGray;
            this.translatedBy.Location = new System.Drawing.Point(12, -1);
            this.translatedBy.Name = "translatedBy";
            this.translatedBy.Size = new System.Drawing.Size(126, 13);
            this.translatedBy.TabIndex = 55;
            this.translatedBy.Text = "Translated by: meme";
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackColor = System.Drawing.Color.Transparent;
            this.SettingsButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SettingsButton.BackgroundImage")));
            this.SettingsButton.Location = new System.Drawing.Point(807, 15);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(24, 24);
            this.SettingsButton.TabIndex = 21;
            this.SettingsButton.TabStop = false;
            // 
            // ServerPick
            // 
            this.ServerPick.BackColor = System.Drawing.Color.White;
            this.ServerPick.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ServerPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ServerPick.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerPick.ForeColor = System.Drawing.Color.Black;
            this.ServerPick.FormattingEnabled = true;
            this.ServerPick.Location = new System.Drawing.Point(586, 50);
            this.ServerPick.Name = "ServerPick";
            this.ServerPick.Size = new System.Drawing.Size(241, 22);
            this.ServerPick.TabIndex = 2;
            // 
            // AddServer
            // 
            this.AddServer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.AddServer.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddServer.Location = new System.Drawing.Point(833, 49);
            this.AddServer.Name = "AddServer";
            this.AddServer.Size = new System.Drawing.Size(24, 24);
            this.AddServer.TabIndex = 3;
            this.AddServer.Text = "+";
            this.AddServer.UseVisualStyleBackColor = true;
            // 
            // ImageServerName
            // 
            this.ImageServerName.BackColor = System.Drawing.Color.Transparent;
            this.ImageServerName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ImageServerName.Font = new System.Drawing.Font("DejaVu Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImageServerName.ForeColor = System.Drawing.Color.Transparent;
            this.ImageServerName.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ImageServerName.Location = new System.Drawing.Point(30, 301);
            this.ImageServerName.Name = "ImageServerName";
            this.ImageServerName.Size = new System.Drawing.Size(519, 43);
            this.ImageServerName.TabIndex = 19;
            this.ImageServerName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.ImageServerName.UseCompatibleTextRendering = true;
            this.ImageServerName.UseMnemonic = false;
            this.ImageServerName.Visible = false;
            // 
            // VerticalBanner
            // 
            this.VerticalBanner.BackColor = System.Drawing.Color.Transparent;
            this.VerticalBanner.Location = new System.Drawing.Point(28, 81);
            this.VerticalBanner.Name = "VerticalBanner";
            this.VerticalBanner.Size = new System.Drawing.Size(523, 223);
            this.VerticalBanner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.VerticalBanner.TabIndex = 22;
            this.VerticalBanner.TabStop = false;
            // 
            // PlayProgressText
            // 
            this.PlayProgressText.AutoSize = true;
            this.PlayProgressText.BackColor = System.Drawing.Color.Transparent;
            this.PlayProgressText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayProgressText.ForeColor = System.Drawing.Color.White;
            this.PlayProgressText.Location = new System.Drawing.Point(42, 401);
            this.PlayProgressText.Name = "PlayProgressText";
            this.PlayProgressText.Size = new System.Drawing.Size(96, 14);
            this.PlayProgressText.TabIndex = 10;
            this.PlayProgressText.Text = "PLEASE WAIT";
            // 
            // LauncherIconStatus
            // 
            this.LauncherIconStatus.BackColor = System.Drawing.Color.Transparent;
            this.LauncherIconStatus.Image = global::GameLauncher.Properties.Resources.ac_success;
            this.LauncherIconStatus.Location = new System.Drawing.Point(27, 456);
            this.LauncherIconStatus.Name = "LauncherIconStatus";
            this.LauncherIconStatus.Size = new System.Drawing.Size(21, 24);
            this.LauncherIconStatus.TabIndex = 79;
            this.LauncherIconStatus.TabStop = false;
            // 
            // LauncherStatusText
            // 
            this.LauncherStatusText.BackColor = System.Drawing.Color.Transparent;
            this.LauncherStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LauncherStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.LauncherStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LauncherStatusText.Location = new System.Drawing.Point(53, 453);
            this.LauncherStatusText.Name = "LauncherStatusText";
            this.LauncherStatusText.Size = new System.Drawing.Size(130, 16);
            this.LauncherStatusText.TabIndex = 4;
            this.LauncherStatusText.Text = "Anti-Cheat System";
            this.LauncherStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.LauncherStatusText.UseCompatibleTextRendering = true;
            // 
            // LauncherStatusDesc
            // 
            this.LauncherStatusDesc.AutoSize = true;
            this.LauncherStatusDesc.BackColor = System.Drawing.Color.Transparent;
            this.LauncherStatusDesc.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LauncherStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.LauncherStatusDesc.Location = new System.Drawing.Point(54, 469);
            this.LauncherStatusDesc.Name = "LauncherStatusDesc";
            this.LauncherStatusDesc.Size = new System.Drawing.Size(91, 13);
            this.LauncherStatusDesc.TabIndex = 5;
            this.LauncherStatusDesc.Text = "Version : v2.0.0.0";
            this.LauncherStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ServerStatusIcon
            // 
            this.ServerStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("ServerStatusIcon.Image")));
            this.ServerStatusIcon.Location = new System.Drawing.Point(189, 456);
            this.ServerStatusIcon.Name = "ServerStatusIcon";
            this.ServerStatusIcon.Size = new System.Drawing.Size(24, 24);
            this.ServerStatusIcon.TabIndex = 6;
            this.ServerStatusIcon.TabStop = false;
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ServerStatusText.Location = new System.Drawing.Point(220, 453);
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
            this.ServerStatusDesc.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ServerStatusDesc.Location = new System.Drawing.Point(220, 469);
            this.ServerStatusDesc.Name = "ServerStatusDesc";
            this.ServerStatusDesc.Size = new System.Drawing.Size(86, 13);
            this.ServerStatusDesc.TabIndex = 8;
            this.ServerStatusDesc.Text = "Checking Status";
            this.ServerStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // APIStatusIcon
            // 
            this.APIStatusIcon.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusIcon.Image = ((System.Drawing.Image)(resources.GetObject("APIStatusIcon.Image")));
            this.APIStatusIcon.Location = new System.Drawing.Point(398, 455);
            this.APIStatusIcon.Name = "APIStatusIcon";
            this.APIStatusIcon.Size = new System.Drawing.Size(24, 24);
            this.APIStatusIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.APIStatusIcon.TabIndex = 113;
            this.APIStatusIcon.TabStop = false;
            // 
            // APIStatusText
            // 
            this.APIStatusText.BackColor = System.Drawing.Color.Transparent;
            this.APIStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.APIStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.APIStatusText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.APIStatusText.Location = new System.Drawing.Point(426, 453);
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
            this.APIStatusDesc.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.APIStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.APIStatusDesc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.APIStatusDesc.Location = new System.Drawing.Point(427, 469);
            this.APIStatusDesc.Name = "APIStatusDesc";
            this.APIStatusDesc.Size = new System.Drawing.Size(86, 13);
            this.APIStatusDesc.TabIndex = 120;
            this.APIStatusDesc.Text = "Checking Status";
            this.APIStatusDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // CurrentWindowInfo
            // 
            this.CurrentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.CurrentWindowInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.CurrentWindowInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CurrentWindowInfo.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentWindowInfo.ForeColor = System.Drawing.Color.White;
            this.CurrentWindowInfo.Location = new System.Drawing.Point(638, 82);
            this.CurrentWindowInfo.Name = "CurrentWindowInfo";
            this.CurrentWindowInfo.Size = new System.Drawing.Size(184, 60);
            this.CurrentWindowInfo.TabIndex = 16;
            this.CurrentWindowInfo.Text = "ENTER ACCOUNT INFORMATION\n TO LOG IN";
            this.CurrentWindowInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CurrentWindowInfo.UseCompatibleTextRendering = true;
            this.CurrentWindowInfo.UseMnemonic = false;
            // 
            // MainEmail
            // 
            this.MainEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.MainEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MainEmail.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainEmail.ForeColor = System.Drawing.Color.White;
            this.MainEmail.Location = new System.Drawing.Point(645, 173);
            this.MainEmail.Name = "MainEmail";
            this.MainEmail.Size = new System.Drawing.Size(180, 13);
            this.MainEmail.TabIndex = 4;
            this.MainEmail.TextChanged += new System.EventHandler(this.Email_TextChanged);
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
            // MainPassword
            // 
            this.MainPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.MainPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.MainPassword.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainPassword.ForeColor = System.Drawing.Color.White;
            this.MainPassword.Location = new System.Drawing.Point(645, 221);
            this.MainPassword.Name = "MainPassword";
            this.MainPassword.Size = new System.Drawing.Size(180, 13);
            this.MainPassword.TabIndex = 5;
            this.MainPassword.UseSystemPasswordChar = true;
            this.MainPassword.WordWrap = false;
            this.MainPassword.TextChanged += new System.EventHandler(this.Password_TextChanged);
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
            // RememberMe
            // 
            this.RememberMe.AutoSize = true;
            this.RememberMe.BackColor = System.Drawing.Color.Transparent;
            this.RememberMe.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RememberMe.ForeColor = System.Drawing.Color.White;
            this.RememberMe.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.RememberMe.Location = new System.Drawing.Point(645, 260);
            this.RememberMe.Name = "RememberMe";
            this.RememberMe.Size = new System.Drawing.Size(163, 17);
            this.RememberMe.TabIndex = 6;
            this.RememberMe.Text = "REMEMBER MY LOGIN";
            this.RememberMe.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.RememberMe.UseVisualStyleBackColor = false;
            // 
            // ForgotPassword
            // 
            this.ForgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.ForgotPassword.AutoSize = true;
            this.ForgotPassword.BackColor = System.Drawing.Color.Transparent;
            this.ForgotPassword.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.ForgotPassword.Location = new System.Drawing.Point(659, 281);
            this.ForgotPassword.Name = "ForgotPassword";
            this.ForgotPassword.Size = new System.Drawing.Size(134, 13);
            this.ForgotPassword.TabIndex = 7;
            this.ForgotPassword.TabStop = true;
            this.ForgotPassword.Text = "I FORGOT MY PASSWORD";
            this.ForgotPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ForgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            // 
            // LoginButton
            // 
            this.LoginButton.BackColor = System.Drawing.Color.Transparent;
            this.LoginButton.FlatAppearance.BorderSize = 0;
            this.LoginButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LoginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LoginButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoginButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginButton.ForeColor = System.Drawing.Color.White;
            this.LoginButton.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.LoginButton.Location = new System.Drawing.Point(605, 362);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(231, 35);
            this.LoginButton.TabIndex = 8;
            this.LoginButton.Text = "LOG ON";
            this.LoginButton.UseVisualStyleBackColor = false;
            // 
            // RegisterText
            // 
            this.RegisterText.BackColor = System.Drawing.Color.Transparent;
            this.RegisterText.FlatAppearance.BorderSize = 0;
            this.RegisterText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.RegisterText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.RegisterText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RegisterText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.RegisterText.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.RegisterText.Location = new System.Drawing.Point(605, 409);
            this.RegisterText.Name = "RegisterText";
            this.RegisterText.Size = new System.Drawing.Size(231, 35);
            this.RegisterText.TabIndex = 10;
            this.RegisterText.Text = "REGISTER";
            this.RegisterText.UseVisualStyleBackColor = false;
            // 
            // ServerPingStatusText
            // 
            this.ServerPingStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerPingStatusText.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerPingStatusText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ServerPingStatusText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            // LogoutButton
            // 
            this.LogoutButton.BackColor = System.Drawing.Color.Transparent;
            this.LogoutButton.FlatAppearance.BorderSize = 0;
            this.LogoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LogoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LogoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LogoutButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogoutButton.ForeColor = System.Drawing.Color.White;
            this.LogoutButton.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.LogoutButton.Location = new System.Drawing.Point(6, 60);
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.Size = new System.Drawing.Size(231, 35);
            this.LogoutButton.TabIndex = 9;
            this.LogoutButton.Text = "LOG OUT";
            this.LogoutButton.UseVisualStyleBackColor = false;
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.Color.Transparent;
            this.PlayButton.BackgroundImage = global::GameLauncher.Properties.Resources.playbutton;
            this.PlayButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.PlayButton.FlatAppearance.BorderSize = 0;
            this.PlayButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PlayButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayButton.ForeColor = System.Drawing.Color.Transparent;
            this.PlayButton.Location = new System.Drawing.Point(7, 94);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(230, 63);
            this.PlayButton.TabIndex = 15;
            this.PlayButton.Text = "PLAY NOW";
            this.PlayButton.UseVisualStyleBackColor = false;
            // 
            // PlayProgressTextTimer
            // 
            this.PlayProgressTextTimer.BackColor = System.Drawing.Color.Transparent;
            this.PlayProgressTextTimer.Font = new System.Drawing.Font("DejaVu Sans", 9F);
            this.PlayProgressTextTimer.ForeColor = System.Drawing.Color.White;
            this.PlayProgressTextTimer.Location = new System.Drawing.Point(431, 399);
            this.PlayProgressTextTimer.Name = "PlayProgressTextTimer";
            this.PlayProgressTextTimer.Size = new System.Drawing.Size(120, 19);
            this.PlayProgressTextTimer.TabIndex = 135;
            this.PlayProgressTextTimer.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.PlayProgressTextTimer.Visible = false;
            // 
            // RegisterEmail
            // 
            this.RegisterEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.RegisterEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RegisterEmail.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterEmail.ForeColor = System.Drawing.Color.White;
            this.RegisterEmail.Location = new System.Drawing.Point(58, 80);
            this.RegisterEmail.Name = "RegisterEmail";
            this.RegisterEmail.Size = new System.Drawing.Size(180, 13);
            this.RegisterEmail.TabIndex = 12;
            this.RegisterEmail.TextChanged += new System.EventHandler(this.RegisterEmail_TextChanged);
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
            // RegisterPassword
            // 
            this.RegisterPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.RegisterPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RegisterPassword.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterPassword.ForeColor = System.Drawing.Color.White;
            this.RegisterPassword.Location = new System.Drawing.Point(58, 128);
            this.RegisterPassword.Name = "RegisterPassword";
            this.RegisterPassword.Size = new System.Drawing.Size(180, 13);
            this.RegisterPassword.TabIndex = 13;
            this.RegisterPassword.UseSystemPasswordChar = true;
            this.RegisterPassword.TextChanged += new System.EventHandler(this.RegisterPassword_TextChanged);
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
            // RegisterConfirmPassword
            // 
            this.RegisterConfirmPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.RegisterConfirmPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RegisterConfirmPassword.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterConfirmPassword.ForeColor = System.Drawing.Color.White;
            this.RegisterConfirmPassword.Location = new System.Drawing.Point(58, 177);
            this.RegisterConfirmPassword.Name = "RegisterConfirmPassword";
            this.RegisterConfirmPassword.Size = new System.Drawing.Size(180, 13);
            this.RegisterConfirmPassword.TabIndex = 14;
            this.RegisterConfirmPassword.UseSystemPasswordChar = true;
            this.RegisterConfirmPassword.TextChanged += new System.EventHandler(this.RegisterConfirmPassword_TextChanged);
            // 
            // RegisterConfirmPasswordBorder
            // 
            this.RegisterConfirmPasswordBorder.BackColor = System.Drawing.Color.Transparent;
            this.RegisterConfirmPasswordBorder.Image = global::GameLauncher.Properties.Resources.password_text_border;
            this.RegisterConfirmPasswordBorder.Location = new System.Drawing.Point(19, 166);
            this.RegisterConfirmPasswordBorder.Name = "RegisterConfirmPasswordBorder";
            this.RegisterConfirmPasswordBorder.Size = new System.Drawing.Size(231, 37);
            this.RegisterConfirmPasswordBorder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RegisterConfirmPasswordBorder.TabIndex = 140;
            this.RegisterConfirmPasswordBorder.TabStop = false;
            // 
            // RegisterTicket
            // 
            this.RegisterTicket.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.RegisterTicket.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.RegisterTicket.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterTicket.ForeColor = System.Drawing.Color.White;
            this.RegisterTicket.Location = new System.Drawing.Point(58, 228);
            this.RegisterTicket.Name = "RegisterTicket";
            this.RegisterTicket.Size = new System.Drawing.Size(180, 13);
            this.RegisterTicket.TabIndex = 15;
            this.RegisterTicket.TextChanged += new System.EventHandler(this.RegisterTicket_TextChanged);
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
            // RegisterAgree
            // 
            this.RegisterAgree.BackColor = System.Drawing.Color.Transparent;
            this.RegisterAgree.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterAgree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.RegisterAgree.Location = new System.Drawing.Point(27, 257);
            this.RegisterAgree.Name = "RegisterAgree";
            this.RegisterAgree.Size = new System.Drawing.Size(232, 35);
            this.RegisterAgree.TabIndex = 16;
            this.RegisterAgree.Text = "BY REGISTERING YOU AGREE TO THE TERMS OF SERVICE";
            this.RegisterAgree.UseVisualStyleBackColor = false;
            this.RegisterAgree.CheckedChanged += new System.EventHandler(this.RegisterAgree_CheckedChanged);
            // 
            // RegisterButton
            // 
            this.RegisterButton.BackColor = System.Drawing.Color.Transparent;
            this.RegisterButton.FlatAppearance.BorderSize = 0;
            this.RegisterButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.RegisterButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.RegisterButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RegisterButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.RegisterButton.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.RegisterButton.Location = new System.Drawing.Point(18, 296);
            this.RegisterButton.Name = "RegisterButton";
            this.RegisterButton.Size = new System.Drawing.Size(231, 35);
            this.RegisterButton.TabIndex = 17;
            this.RegisterButton.Text = "REGISTER";
            this.RegisterButton.UseVisualStyleBackColor = false;
            // 
            // RegisterCancel
            // 
            this.RegisterCancel.BackColor = System.Drawing.Color.Transparent;
            this.RegisterCancel.FlatAppearance.BorderSize = 0;
            this.RegisterCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.RegisterCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.RegisterCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RegisterCancel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RegisterCancel.ForeColor = System.Drawing.Color.White;
            this.RegisterCancel.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.RegisterCancel.Location = new System.Drawing.Point(18, 336);
            this.RegisterCancel.Name = "RegisterCancel";
            this.RegisterCancel.Size = new System.Drawing.Size(231, 35);
            this.RegisterCancel.TabIndex = 18;
            this.RegisterCancel.Text = "CANCEL";
            this.RegisterCancel.UseVisualStyleBackColor = false;
            // 
            // SettingsPanel
            // 
            this.SettingsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SettingsPanel.BackColor = System.Drawing.Color.Transparent;
            this.SettingsPanel.Controls.Add(this.SettingsClearServerModCacheButton);
            this.SettingsPanel.Controls.Add(this.SettingsLauncherVersion);
            this.SettingsPanel.Controls.Add(this.SettingsAboutButton);
            this.SettingsPanel.Controls.Add(this.SettingsGameFiles);
            this.SettingsPanel.Controls.Add(this.SettingsClearCommunicationLogButton);
            this.SettingsPanel.Controls.Add(this.SettingsClearCrashLogsButton);
            this.SettingsPanel.Controls.Add(this.SettingsVFilesButton);
            this.SettingsPanel.Controls.Add(this.SettingsGamePathText);
            this.SettingsPanel.Controls.Add(this.SettingsSave);
            this.SettingsPanel.Controls.Add(this.SettingsCancel);
            this.SettingsPanel.Controls.Add(this.SettingsCDNText);
            this.SettingsPanel.Controls.Add(this.SettingsCDNPick);
            this.SettingsPanel.Controls.Add(this.SettingsLanguageText);
            this.SettingsPanel.Controls.Add(this.SettingsLanguage);
            this.SettingsPanel.Controls.Add(this.SettingsWordFilterCheck);
            this.SettingsPanel.Controls.Add(this.SettingsProxyCheckbox);
            this.SettingsPanel.Controls.Add(this.SettingsDiscordRPCCheckbox);
            this.SettingsPanel.Controls.Add(this.SettingsGameFilesCurrentText);
            this.SettingsPanel.Controls.Add(this.SettingsGameFilesCurrent);
            this.SettingsPanel.Controls.Add(this.SettingsCDNCurrentText);
            this.SettingsPanel.Controls.Add(this.SettingsCDNCurrent);
            this.SettingsPanel.Controls.Add(this.SettingsLauncherPathText);
            this.SettingsPanel.Controls.Add(this.SettingsLauncherPathCurrent);
            this.SettingsPanel.Controls.Add(this.SettingsNetworkText);
            this.SettingsPanel.Controls.Add(this.SettingsMainSrvText);
            this.SettingsPanel.Controls.Add(this.SettingsMainCDNText);
            this.SettingsPanel.Controls.Add(this.SettingsBkupSrvText);
            this.SettingsPanel.Controls.Add(this.SettingsBkupCDNText);
            this.SettingsPanel.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsPanel.ForeColor = System.Drawing.Color.Transparent;
            this.SettingsPanel.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.SettingsPanel.Location = new System.Drawing.Point(27, 50);
            this.SettingsPanel.Name = "SettingsPanel";
            this.SettingsPanel.Size = new System.Drawing.Size(837, 452);
            this.SettingsPanel.TabIndex = 150;
            // 
            // SettingsClearServerModCacheButton
            // 
            this.SettingsClearServerModCacheButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsClearServerModCacheButton.Enabled = false;
            this.SettingsClearServerModCacheButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsClearServerModCacheButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsClearServerModCacheButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsClearServerModCacheButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsClearServerModCacheButton.ForeColor = System.Drawing.Color.Silver;
            this.SettingsClearServerModCacheButton.Location = new System.Drawing.Point(26, 291);
            this.SettingsClearServerModCacheButton.Name = "SettingsClearServerModCacheButton";
            this.SettingsClearServerModCacheButton.Size = new System.Drawing.Size(131, 25);
            this.SettingsClearServerModCacheButton.TabIndex = 161;
            this.SettingsClearServerModCacheButton.Text = "Clear Server Mods";
            this.SettingsClearServerModCacheButton.UseVisualStyleBackColor = false;
            this.SettingsClearServerModCacheButton.Click += new System.EventHandler(this.SettingsClearServerModCacheButton_Click);
            // 
            // SettingsLauncherVersion
            // 
            this.SettingsLauncherVersion.AutoSize = true;
            this.SettingsLauncherVersion.BackColor = System.Drawing.Color.Transparent;
            this.SettingsLauncherVersion.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsLauncherVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsLauncherVersion.Location = new System.Drawing.Point(27, 419);
            this.SettingsLauncherVersion.Name = "SettingsLauncherVersion";
            this.SettingsLauncherVersion.Size = new System.Drawing.Size(85, 13);
            this.SettingsLauncherVersion.TabIndex = 160;
            this.SettingsLauncherVersion.Text = "Version v2.0.0.0";
            this.SettingsLauncherVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingsAboutButton
            // 
            this.SettingsAboutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsAboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsAboutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsAboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsAboutButton.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsAboutButton.Location = new System.Drawing.Point(756, 5);
            this.SettingsAboutButton.Name = "SettingsAboutButton";
            this.SettingsAboutButton.Size = new System.Drawing.Size(75, 23);
            this.SettingsAboutButton.TabIndex = 159;
            this.SettingsAboutButton.Text = "About";
            this.SettingsAboutButton.UseVisualStyleBackColor = true;
            this.SettingsAboutButton.Click += new System.EventHandler(this.PatchNotes_Click);
            // 
            // SettingsGameFiles
            // 
            this.SettingsGameFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsGameFiles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsGameFiles.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsGameFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsGameFiles.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsGameFiles.ForeColor = System.Drawing.Color.Silver;
            this.SettingsGameFiles.Location = new System.Drawing.Point(26, 89);
            this.SettingsGameFiles.Name = "SettingsGameFiles";
            this.SettingsGameFiles.Size = new System.Drawing.Size(220, 23);
            this.SettingsGameFiles.TabIndex = 130;
            this.SettingsGameFiles.Text = "Change GameFiles Path";
            this.SettingsGameFiles.UseVisualStyleBackColor = false;
            // 
            // SettingsClearCommunicationLogButton
            // 
            this.SettingsClearCommunicationLogButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(29)))), ((int)(((byte)(38)))));
            this.SettingsClearCommunicationLogButton.Enabled = false;
            this.SettingsClearCommunicationLogButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(181)))), ((int)(((byte)(191)))));
            this.SettingsClearCommunicationLogButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsClearCommunicationLogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsClearCommunicationLogButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsClearCommunicationLogButton.ForeColor = System.Drawing.Color.Silver;
            this.SettingsClearCommunicationLogButton.Location = new System.Drawing.Point(26, 253);
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
            this.SettingsClearCrashLogsButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsClearCrashLogsButton.ForeColor = System.Drawing.Color.Silver;
            this.SettingsClearCrashLogsButton.Location = new System.Drawing.Point(26, 216);
            this.SettingsClearCrashLogsButton.Name = "SettingsClearCrashLogsButton";
            this.SettingsClearCrashLogsButton.Size = new System.Drawing.Size(131, 25);
            this.SettingsClearCrashLogsButton.TabIndex = 133;
            this.SettingsClearCrashLogsButton.Text = "Clear Crash Logs";
            this.SettingsClearCrashLogsButton.UseVisualStyleBackColor = false;
            this.SettingsClearCrashLogsButton.Click += new System.EventHandler(this.SettingsClearCrashLogsButton_Click);
            // 
            // SettingsVFilesButton
            // 
            this.SettingsVFilesButton.AutoSize = true;
            this.SettingsVFilesButton.BackColor = System.Drawing.Color.Transparent;
            this.SettingsVFilesButton.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsVFilesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsVFilesButton.Location = new System.Drawing.Point(26, 395);
            this.SettingsVFilesButton.Name = "SettingsVFilesButton";
            this.SettingsVFilesButton.Size = new System.Drawing.Size(131, 23);
            this.SettingsVFilesButton.TabIndex = 141;
            this.SettingsVFilesButton.Text = "Validate Game Files";
            this.SettingsVFilesButton.UseVisualStyleBackColor = false;
            this.SettingsVFilesButton.Visible = false;
            // 
            // SettingsGamePathText
            // 
            this.SettingsGamePathText.AutoSize = true;
            this.SettingsGamePathText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsGamePathText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsGamePathText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsGamePathText.Location = new System.Drawing.Point(24, 67);
            this.SettingsGamePathText.Name = "SettingsGamePathText";
            this.SettingsGamePathText.Size = new System.Drawing.Size(122, 14);
            this.SettingsGamePathText.TabIndex = 135;
            this.SettingsGamePathText.Text = "GAMEFILES PATH";
            this.SettingsGamePathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsSave
            // 
            this.SettingsSave.BackColor = System.Drawing.Color.Transparent;
            this.SettingsSave.FlatAppearance.BorderSize = 0;
            this.SettingsSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.SettingsSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.SettingsSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsSave.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsSave.ForeColor = System.Drawing.Color.White;
            this.SettingsSave.Image = global::GameLauncher.Properties.Resources.greenbutton;
            this.SettingsSave.Location = new System.Drawing.Point(561, 392);
            this.SettingsSave.Name = "SettingsSave";
            this.SettingsSave.Size = new System.Drawing.Size(130, 50);
            this.SettingsSave.TabIndex = 151;
            this.SettingsSave.Text = "SAVE";
            this.SettingsSave.UseVisualStyleBackColor = false;
            // 
            // SettingsCancel
            // 
            this.SettingsCancel.BackColor = System.Drawing.Color.Transparent;
            this.SettingsCancel.FlatAppearance.BorderSize = 0;
            this.SettingsCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.SettingsCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.SettingsCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsCancel.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsCancel.ForeColor = System.Drawing.Color.White;
            this.SettingsCancel.Image = global::GameLauncher.Properties.Resources.graybutton;
            this.SettingsCancel.Location = new System.Drawing.Point(697, 392);
            this.SettingsCancel.Name = "SettingsCancel";
            this.SettingsCancel.Size = new System.Drawing.Size(130, 50);
            this.SettingsCancel.TabIndex = 152;
            this.SettingsCancel.Text = "CANCEL";
            this.SettingsCancel.UseVisualStyleBackColor = false;
            // 
            // SettingsCDNText
            // 
            this.SettingsCDNText.AutoSize = true;
            this.SettingsCDNText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsCDNText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsCDNText.Location = new System.Drawing.Point(24, 111);
            this.SettingsCDNText.Name = "SettingsCDNText";
            this.SettingsCDNText.Size = new System.Drawing.Size(103, 14);
            this.SettingsCDNText.TabIndex = 137;
            this.SettingsCDNText.Text = "CDN: PINGING";
            this.SettingsCDNText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsCDNPick
            // 
            this.SettingsCDNPick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsCDNPick.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.SettingsCDNPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SettingsCDNPick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsCDNPick.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsCDNPick.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.SettingsCDNPick.FormattingEnabled = true;
            this.SettingsCDNPick.Location = new System.Drawing.Point(26, 133);
            this.SettingsCDNPick.Name = "SettingsCDNPick";
            this.SettingsCDNPick.Size = new System.Drawing.Size(220, 21);
            this.SettingsCDNPick.TabIndex = 133;
            this.SettingsCDNPick.SelectedIndexChanged += new System.EventHandler(this.SettingsCDNPick_SelectedIndexChanged);
            // 
            // SettingsLanguageText
            // 
            this.SettingsLanguageText.AutoSize = true;
            this.SettingsLanguageText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsLanguageText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsLanguageText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsLanguageText.Location = new System.Drawing.Point(24, 158);
            this.SettingsLanguageText.Name = "SettingsLanguageText";
            this.SettingsLanguageText.Size = new System.Drawing.Size(125, 14);
            this.SettingsLanguageText.TabIndex = 131;
            this.SettingsLanguageText.Text = "GAME LANGUAGE";
            this.SettingsLanguageText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsLanguage
            // 
            this.SettingsLanguage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(58)))), ((int)(((byte)(76)))));
            this.SettingsLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SettingsLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SettingsLanguage.Font = new System.Drawing.Font("DejaVu Sans Condensed", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsLanguage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(178)))), ((int)(((byte)(210)))), ((int)(((byte)(255)))));
            this.SettingsLanguage.FormattingEnabled = true;
            this.SettingsLanguage.Location = new System.Drawing.Point(26, 180);
            this.SettingsLanguage.Name = "SettingsLanguage";
            this.SettingsLanguage.Size = new System.Drawing.Size(131, 21);
            this.SettingsLanguage.TabIndex = 133;
            // 
            // SettingsWordFilterCheck
            // 
            this.SettingsWordFilterCheck.AutoSize = true;
            this.SettingsWordFilterCheck.BackColor = System.Drawing.Color.Transparent;
            this.SettingsWordFilterCheck.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsWordFilterCheck.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.SettingsWordFilterCheck.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsWordFilterCheck.Location = new System.Drawing.Point(26, 329);
            this.SettingsWordFilterCheck.Name = "SettingsWordFilterCheck";
            this.SettingsWordFilterCheck.Size = new System.Drawing.Size(226, 18);
            this.SettingsWordFilterCheck.TabIndex = 135;
            this.SettingsWordFilterCheck.Text = "Disable Word Filtering on Game Chat";
            this.SettingsWordFilterCheck.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.SettingsWordFilterCheck.UseVisualStyleBackColor = false;
            // 
            // SettingsProxyCheckbox
            // 
            this.SettingsProxyCheckbox.AutoSize = true;
            this.SettingsProxyCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.SettingsProxyCheckbox.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsProxyCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.SettingsProxyCheckbox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsProxyCheckbox.Location = new System.Drawing.Point(26, 349);
            this.SettingsProxyCheckbox.Name = "SettingsProxyCheckbox";
            this.SettingsProxyCheckbox.Size = new System.Drawing.Size(100, 18);
            this.SettingsProxyCheckbox.TabIndex = 136;
            this.SettingsProxyCheckbox.Text = "Disable Proxy";
            this.SettingsProxyCheckbox.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.SettingsProxyCheckbox.UseVisualStyleBackColor = false;
            // 
            // SettingsDiscordRPCCheckbox
            // 
            this.SettingsDiscordRPCCheckbox.AutoSize = true;
            this.SettingsDiscordRPCCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.SettingsDiscordRPCCheckbox.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsDiscordRPCCheckbox.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.SettingsDiscordRPCCheckbox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsDiscordRPCCheckbox.Location = new System.Drawing.Point(26, 369);
            this.SettingsDiscordRPCCheckbox.Name = "SettingsDiscordRPCCheckbox";
            this.SettingsDiscordRPCCheckbox.Size = new System.Drawing.Size(137, 18);
            this.SettingsDiscordRPCCheckbox.TabIndex = 137;
            this.SettingsDiscordRPCCheckbox.Text = "Disable Discord RPC";
            this.SettingsDiscordRPCCheckbox.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.SettingsDiscordRPCCheckbox.UseVisualStyleBackColor = false;
            // 
            // SettingsGameFilesCurrentText
            // 
            this.SettingsGameFilesCurrentText.AutoSize = true;
            this.SettingsGameFilesCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsGameFilesCurrentText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsGameFilesCurrentText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsGameFilesCurrentText.Location = new System.Drawing.Point(313, 82);
            this.SettingsGameFilesCurrentText.Name = "SettingsGameFilesCurrentText";
            this.SettingsGameFilesCurrentText.Size = new System.Drawing.Size(154, 14);
            this.SettingsGameFilesCurrentText.TabIndex = 149;
            this.SettingsGameFilesCurrentText.Text = "CURRENT DIRECTORY:";
            this.SettingsGameFilesCurrentText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsGameFilesCurrent
            // 
            this.SettingsGameFilesCurrent.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsGameFilesCurrent.AutoSize = true;
            this.SettingsGameFilesCurrent.BackColor = System.Drawing.Color.Transparent;
            this.SettingsGameFilesCurrent.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsGameFilesCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.SettingsGameFilesCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.SettingsGameFilesCurrent.Location = new System.Drawing.Point(314, 99);
            this.SettingsGameFilesCurrent.Name = "SettingsGameFilesCurrent";
            this.SettingsGameFilesCurrent.Size = new System.Drawing.Size(200, 14);
            this.SettingsGameFilesCurrent.TabIndex = 138;
            this.SettingsGameFilesCurrent.TabStop = true;
            this.SettingsGameFilesCurrent.Text = "C:\\Soapbox Race World\\Game Files";
            this.SettingsGameFilesCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsGameFilesCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // SettingsCDNCurrentText
            // 
            this.SettingsCDNCurrentText.AutoSize = true;
            this.SettingsCDNCurrentText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsCDNCurrentText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsCDNCurrentText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsCDNCurrentText.Location = new System.Drawing.Point(313, 120);
            this.SettingsCDNCurrentText.Name = "SettingsCDNCurrentText";
            this.SettingsCDNCurrentText.Size = new System.Drawing.Size(110, 14);
            this.SettingsCDNCurrentText.TabIndex = 150;
            this.SettingsCDNCurrentText.Text = "CURRENT CDN:";
            this.SettingsCDNCurrentText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsCDNCurrent
            // 
            this.SettingsCDNCurrent.ActiveLinkColor = System.Drawing.Color.Transparent;
            this.SettingsCDNCurrent.AutoSize = true;
            this.SettingsCDNCurrent.BackColor = System.Drawing.Color.Transparent;
            this.SettingsCDNCurrent.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsCDNCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.SettingsCDNCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.SettingsCDNCurrent.Location = new System.Drawing.Point(314, 137);
            this.SettingsCDNCurrent.Name = "SettingsCDNCurrent";
            this.SettingsCDNCurrent.Size = new System.Drawing.Size(91, 14);
            this.SettingsCDNCurrent.TabIndex = 139;
            this.SettingsCDNCurrent.TabStop = true;
            this.SettingsCDNCurrent.Text = "http://localhost";
            this.SettingsCDNCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsCDNCurrent.VisitedLinkColor = System.Drawing.Color.White;
            this.SettingsCDNCurrent.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.SettingsCDNCurrent_LinkClicked);
            // 
            // SettingsLauncherPathText
            // 
            this.SettingsLauncherPathText.AutoSize = true;
            this.SettingsLauncherPathText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsLauncherPathText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsLauncherPathText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsLauncherPathText.Location = new System.Drawing.Point(313, 158);
            this.SettingsLauncherPathText.Name = "SettingsLauncherPathText";
            this.SettingsLauncherPathText.Size = new System.Drawing.Size(143, 14);
            this.SettingsLauncherPathText.TabIndex = 136;
            this.SettingsLauncherPathText.Text = "LAUNCHER FOLDER:";
            this.SettingsLauncherPathText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsLauncherPathCurrent
            // 
            this.SettingsLauncherPathCurrent.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsLauncherPathCurrent.AutoSize = true;
            this.SettingsLauncherPathCurrent.BackColor = System.Drawing.Color.Transparent;
            this.SettingsLauncherPathCurrent.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsLauncherPathCurrent.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.SettingsLauncherPathCurrent.LinkColor = System.Drawing.Color.LawnGreen;
            this.SettingsLauncherPathCurrent.Location = new System.Drawing.Point(314, 176);
            this.SettingsLauncherPathCurrent.Name = "SettingsLauncherPathCurrent";
            this.SettingsLauncherPathCurrent.Size = new System.Drawing.Size(191, 14);
            this.SettingsLauncherPathCurrent.TabIndex = 140;
            this.SettingsLauncherPathCurrent.TabStop = true;
            this.SettingsLauncherPathCurrent.Text = "C:\\Soapbox Race World\\Launcher";
            this.SettingsLauncherPathCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SettingsLauncherPathCurrent.VisitedLinkColor = System.Drawing.Color.White;
            // 
            // SettingsNetworkText
            // 
            this.SettingsNetworkText.AutoSize = true;
            this.SettingsNetworkText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsNetworkText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsNetworkText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.SettingsNetworkText.Location = new System.Drawing.Point(313, 206);
            this.SettingsNetworkText.Name = "SettingsNetworkText";
            this.SettingsNetworkText.Size = new System.Drawing.Size(157, 14);
            this.SettingsNetworkText.TabIndex = 142;
            this.SettingsNetworkText.Text = "CONNECTION STATUS:";
            this.SettingsNetworkText.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // SettingsMainSrvText
            // 
            this.SettingsMainSrvText.AutoSize = true;
            this.SettingsMainSrvText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsMainSrvText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsMainSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SettingsMainSrvText.Location = new System.Drawing.Point(312, 228);
            this.SettingsMainSrvText.Name = "SettingsMainSrvText";
            this.SettingsMainSrvText.Size = new System.Drawing.Size(165, 14);
            this.SettingsMainSrvText.TabIndex = 143;
            this.SettingsMainSrvText.Text = "Main Server List API: PINGING";
            // 
            // SettingsMainCDNText
            // 
            this.SettingsMainCDNText.AutoSize = true;
            this.SettingsMainCDNText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsMainCDNText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsMainCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SettingsMainCDNText.Location = new System.Drawing.Point(312, 248);
            this.SettingsMainCDNText.Name = "SettingsMainCDNText";
            this.SettingsMainCDNText.Size = new System.Drawing.Size(154, 14);
            this.SettingsMainCDNText.TabIndex = 146;
            this.SettingsMainCDNText.Text = "Main CDN List API: PINGING";
            // 
            // SettingsBkupSrvText
            // 
            this.SettingsBkupSrvText.AutoSize = true;
            this.SettingsBkupSrvText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsBkupSrvText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsBkupSrvText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SettingsBkupSrvText.Location = new System.Drawing.Point(312, 268);
            this.SettingsBkupSrvText.Name = "SettingsBkupSrvText";
            this.SettingsBkupSrvText.Size = new System.Drawing.Size(179, 14);
            this.SettingsBkupSrvText.TabIndex = 144;
            this.SettingsBkupSrvText.Text = "Backup Server List API: PINGING";
            // 
            // SettingsBkupCDNText
            // 
            this.SettingsBkupCDNText.AutoSize = true;
            this.SettingsBkupCDNText.BackColor = System.Drawing.Color.Transparent;
            this.SettingsBkupCDNText.Font = new System.Drawing.Font("DejaVu Sans Condensed", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SettingsBkupCDNText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SettingsBkupCDNText.Location = new System.Drawing.Point(312, 288);
            this.SettingsBkupCDNText.Name = "SettingsBkupCDNText";
            this.SettingsBkupCDNText.Size = new System.Drawing.Size(168, 14);
            this.SettingsBkupCDNText.TabIndex = 145;
            this.SettingsBkupCDNText.Text = "Backup CDN List API: PINGING";
            // 
            // ShowPlayPanel
            // 
            this.ShowPlayPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ShowPlayPanel.BackColor = System.Drawing.Color.Transparent;
            this.ShowPlayPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ShowPlayPanel.Controls.Add(this.LogoutButton);
            this.ShowPlayPanel.Controls.Add(this.PlayButton);
            this.ShowPlayPanel.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.RegisterPanel.Controls.Add(this.RegisterEmail);
            this.RegisterPanel.Controls.Add(this.RegisterEmailBorder);
            this.RegisterPanel.Controls.Add(this.RegisterPassword);
            this.RegisterPanel.Controls.Add(this.RegisterPasswordBorder);
            this.RegisterPanel.Controls.Add(this.RegisterConfirmPassword);
            this.RegisterPanel.Controls.Add(this.RegisterConfirmPasswordBorder);
            this.RegisterPanel.Controls.Add(this.RegisterTicket);
            this.RegisterPanel.Controls.Add(this.RegisterTicketBorder);
            this.RegisterPanel.Controls.Add(this.RegisterAgree);
            this.RegisterPanel.Controls.Add(this.RegisterButton);
            this.RegisterPanel.Controls.Add(this.RegisterCancel);
            this.RegisterPanel.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.DiscordInviteLink.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiscordInviteLink.ForeColor = System.Drawing.Color.Transparent;
            this.DiscordInviteLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DiscordInviteLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.DiscordInviteLink.Location = new System.Drawing.Point(154, 12);
            this.DiscordInviteLink.Name = "DiscordInviteLink";
            this.DiscordInviteLink.Size = new System.Drawing.Size(98, 13);
            this.DiscordInviteLink.TabIndex = 172;
            this.DiscordInviteLink.TabStop = true;
            this.DiscordInviteLink.Text = "Discord Invite";
            this.DiscordInviteLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DiscordInviteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.DiscordInviteLink_LinkClicked);
            // 
            // ServerShutDown
            // 
            this.ServerShutDown.BackColor = System.Drawing.Color.Transparent;
            this.ServerShutDown.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerShutDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerShutDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ServerShutDown.Location = new System.Drawing.Point(310, 42);
            this.ServerShutDown.Name = "ServerShutDown";
            this.ServerShutDown.Size = new System.Drawing.Size(168, 15);
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
            this.ServerInfoPanel.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ServerInfoPanel.ForeColor = System.Drawing.Color.Transparent;
            this.ServerInfoPanel.Location = new System.Drawing.Point(27, 305);
            this.ServerInfoPanel.Name = "ServerInfoPanel";
            this.ServerInfoPanel.Size = new System.Drawing.Size(524, 68);
            this.ServerInfoPanel.TabIndex = 172;
            // 
            // HomePageIcon
            // 
            this.HomePageIcon.BackColor = System.Drawing.Color.Transparent;
            this.HomePageIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_home_page;
            this.HomePageIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.HomePageIcon.Location = new System.Drawing.Point(7, 8);
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
            this.DiscordIcon.Location = new System.Drawing.Point(127, 8);
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
            this.FacebookIcon.Location = new System.Drawing.Point(263, 8);
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
            this.TwitterAccountLink.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TwitterAccountLink.ForeColor = System.Drawing.Color.Transparent;
            this.TwitterAccountLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.TwitterAccountLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.TwitterAccountLink.Location = new System.Drawing.Point(437, 12);
            this.TwitterAccountLink.Name = "TwitterAccountLink";
            this.TwitterAccountLink.Size = new System.Drawing.Size(86, 13);
            this.TwitterAccountLink.TabIndex = 174;
            this.TwitterAccountLink.TabStop = true;
            this.TwitterAccountLink.Text = "Twitter Feed";
            this.TwitterAccountLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TwitterAccountLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TwitterAccountLink_LinkClicked);
            // 
            // TwitterIcon
            // 
            this.TwitterIcon.BackColor = System.Drawing.Color.Transparent;
            this.TwitterIcon.BackgroundImage = global::GameLauncher.Properties.Resources.social_twitter;
            this.TwitterIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.TwitterIcon.Location = new System.Drawing.Point(407, 8);
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
            this.FacebookGroupLink.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FacebookGroupLink.ForeColor = System.Drawing.Color.Transparent;
            this.FacebookGroupLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.FacebookGroupLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.FacebookGroupLink.Location = new System.Drawing.Point(292, 12);
            this.FacebookGroupLink.Name = "FacebookGroupLink";
            this.FacebookGroupLink.Size = new System.Drawing.Size(112, 13);
            this.FacebookGroupLink.TabIndex = 173;
            this.FacebookGroupLink.TabStop = true;
            this.FacebookGroupLink.Text = "Facebook Group";
            this.FacebookGroupLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.FacebookGroupLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.FacebookGroupLink_LinkClicked);
            // 
            // HomePageLink
            // 
            this.HomePageLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HomePageLink.AutoSize = true;
            this.HomePageLink.BackColor = System.Drawing.Color.Transparent;
            this.HomePageLink.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HomePageLink.ForeColor = System.Drawing.Color.Transparent;
            this.HomePageLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.HomePageLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.HomePageLink.Location = new System.Drawing.Point(36, 12);
            this.HomePageLink.Name = "HomePageLink";
            this.HomePageLink.Size = new System.Drawing.Size(81, 13);
            this.HomePageLink.TabIndex = 171;
            this.HomePageLink.TabStop = true;
            this.HomePageLink.Text = "Home Page";
            this.HomePageLink.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.HomePageLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.HomePageLink_LinkClicked);
            // 
            // SceneryGroupText
            // 
            this.SceneryGroupText.BackColor = System.Drawing.Color.Transparent;
            this.SceneryGroupText.Font = new System.Drawing.Font("DejaVu Sans", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SceneryGroupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SceneryGroupText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.SceneryGroupText.Location = new System.Drawing.Point(64, 43);
            this.SceneryGroupText.Name = "SceneryGroupText";
            this.SceneryGroupText.Size = new System.Drawing.Size(140, 14);
            this.SceneryGroupText.TabIndex = 179;
            this.SceneryGroupText.Text = "Scenery: Normal";
            this.SceneryGroupText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ExtractingProgress
            // 
            this.ExtractingProgress.BackColor = System.Drawing.Color.Transparent;
            this.ExtractingProgress.BackgroundColor = System.Drawing.Color.Black;
            this.ExtractingProgress.Border = false;
            this.ExtractingProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ExtractingProgress.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtractingProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ExtractingProgress.Image = global::GameLauncher.Properties.Resources.progress_success;
            this.ExtractingProgress.Location = new System.Drawing.Point(30, 425);
            this.ExtractingProgress.Name = "ExtractingProgress";
            this.ExtractingProgress.ProgressColor = System.Drawing.Color.Green;
            this.ExtractingProgress.RoundedCorners = false;
            this.ExtractingProgress.Size = new System.Drawing.Size(519, 13);
            this.ExtractingProgress.Text = "downloadProgress";
            // 
            // PlayProgress
            // 
            this.PlayProgress.BackColor = System.Drawing.Color.Transparent;
            this.PlayProgress.BackgroundColor = System.Drawing.Color.Black;
            this.PlayProgress.Border = false;
            this.PlayProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.PlayProgress.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.PlayProgress.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PlayProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.PlayProgress.Image = global::GameLauncher.Properties.Resources.progress_preload;
            this.PlayProgress.Location = new System.Drawing.Point(30, 425);
            this.PlayProgress.Name = "PlayProgress";
            this.PlayProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.PlayProgress.RoundedCorners = false;
            this.PlayProgress.Size = new System.Drawing.Size(519, 13);
            this.PlayProgress.Text = "downloadProgress";
            // 
            // MainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackgroundImage = global::GameLauncher.Properties.Resources.mainbackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(891, 529);
            this.Controls.Add(this.CurrentWindowInfo);
            this.Controls.Add(this.ServerPingStatusText);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.RegisterPanel);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.ShowPlayPanel);
            this.Controls.Add(this.CloseBTN);
            this.Controls.Add(this.SettingsPanel);
            this.Controls.Add(this.SelectServerBtn);
            this.Controls.Add(this.translatedBy);
            this.Controls.Add(this.ServerPick);
            this.Controls.Add(this.AddServer);
            this.Controls.Add(this.PlayProgressText);
            this.Controls.Add(this.ExtractingProgress);
            this.Controls.Add(this.LauncherIconStatus);
            this.Controls.Add(this.APIStatusIcon);
            this.Controls.Add(this.ServerStatusIcon);
            this.Controls.Add(this.LauncherStatusText);
            this.Controls.Add(this.LauncherStatusDesc);
            this.Controls.Add(this.ServerStatusText);
            this.Controls.Add(this.ServerStatusDesc);
            this.Controls.Add(this.APIStatusText);
            this.Controls.Add(this.APIStatusDesc);
            this.Controls.Add(this.MainEmail);
            this.Controls.Add(this.MainEmailBorder);
            this.Controls.Add(this.MainPassword);
            this.Controls.Add(this.MainPasswordBorder);
            this.Controls.Add(this.RememberMe);
            this.Controls.Add(this.ForgotPassword);
            this.Controls.Add(this.LoginButton);
            this.Controls.Add(this.RegisterText);
            this.Controls.Add(this.PlayProgress);
            this.Controls.Add(this.PlayProgressTextTimer);
            this.Controls.Add(this.VerticalBanner);
            this.Controls.Add(this.ServerInfoPanel);
            this.Controls.Add(this.ImageServerName);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("DejaVu Sans", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GameLauncher";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBTN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VerticalBanner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LauncherIconStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterEmailBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterPasswordBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RegisterConfirmPasswordBorder)).EndInit();
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
        private System.Windows.Forms.PictureBox CloseBTN;
        private System.Windows.Forms.Button SelectServerBtn;
        private System.Windows.Forms.Label translatedBy;
        private System.Windows.Forms.PictureBox SettingsButton;
        private System.Windows.Forms.ComboBox ServerPick;
        private System.Windows.Forms.Button AddServer;
        private System.Windows.Forms.Label ImageServerName;
        private System.Windows.Forms.PictureBox VerticalBanner;
        internal System.Windows.Forms.Label PlayProgressText;
        private GameLauncherReborn.ProgressBarEx ExtractingProgress;
        private GameLauncherReborn.ProgressBarEx PlayProgress;
        private System.Windows.Forms.PictureBox LauncherIconStatus;
        private System.Windows.Forms.Label LauncherStatusText;
        private System.Windows.Forms.Label LauncherStatusDesc;
        private System.Windows.Forms.PictureBox ServerStatusIcon;
        private System.Windows.Forms.Label ServerStatusText;
        private System.Windows.Forms.Label ServerStatusDesc;
        private System.Windows.Forms.PictureBox APIStatusIcon;
        private System.Windows.Forms.Label APIStatusText;
        private System.Windows.Forms.Label APIStatusDesc;
        private System.Windows.Forms.Label CurrentWindowInfo;
        private System.Windows.Forms.TextBox MainEmail;
        private System.Windows.Forms.PictureBox MainEmailBorder;
        private System.Windows.Forms.TextBox MainPassword;
        private System.Windows.Forms.PictureBox MainPasswordBorder;
        private System.Windows.Forms.CheckBox RememberMe;
        private System.Windows.Forms.LinkLabel ForgotPassword;
        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Button RegisterText;
        private System.Windows.Forms.Label ServerPingStatusText;
        private System.Windows.Forms.Button LogoutButton;
        private System.Windows.Forms.Button PlayButton;
        internal System.Windows.Forms.Label PlayProgressTextTimer;
        private System.Windows.Forms.TextBox RegisterEmail;
        private System.Windows.Forms.PictureBox RegisterEmailBorder;
        private System.Windows.Forms.TextBox RegisterPassword;
        private System.Windows.Forms.PictureBox RegisterPasswordBorder;
        private System.Windows.Forms.TextBox RegisterConfirmPassword;
        private System.Windows.Forms.PictureBox RegisterConfirmPasswordBorder;
        private System.Windows.Forms.TextBox RegisterTicket;
        private System.Windows.Forms.PictureBox RegisterTicketBorder;
        private System.Windows.Forms.CheckBox RegisterAgree;
        private System.Windows.Forms.Button RegisterButton;
        private System.Windows.Forms.Button RegisterCancel;
        private System.Windows.Forms.Panel SettingsPanel;
        private System.Windows.Forms.Button SettingsSave;
        private System.Windows.Forms.Button SettingsCancel;
        private System.Windows.Forms.Label SettingsGamePathText;
        private System.Windows.Forms.Button SettingsGameFiles;
        private System.Windows.Forms.Label SettingsCDNText;
        private System.Windows.Forms.ComboBox SettingsCDNPick;
        private System.Windows.Forms.Label SettingsLanguageText;
        private System.Windows.Forms.ComboBox SettingsLanguage;
        private System.Windows.Forms.CheckBox SettingsWordFilterCheck;
        private System.Windows.Forms.CheckBox SettingsProxyCheckbox;
        private System.Windows.Forms.CheckBox SettingsDiscordRPCCheckbox;
        private System.Windows.Forms.Button SettingsClearCrashLogsButton;
        private System.Windows.Forms.Label SettingsGameFilesCurrentText;
        private System.Windows.Forms.LinkLabel SettingsGameFilesCurrent;
        private System.Windows.Forms.Label SettingsCDNCurrentText;
        private System.Windows.Forms.LinkLabel SettingsCDNCurrent;
        private System.Windows.Forms.Label SettingsLauncherPathText;
        private System.Windows.Forms.LinkLabel SettingsLauncherPathCurrent;
        private System.Windows.Forms.Label SettingsNetworkText;
        private System.Windows.Forms.Label SettingsMainSrvText;
        private System.Windows.Forms.Label SettingsMainCDNText;
        private System.Windows.Forms.Label SettingsBkupSrvText;
        private System.Windows.Forms.Label SettingsBkupCDNText;
        private System.Windows.Forms.Panel ShowPlayPanel;
        private System.Windows.Forms.Panel RegisterPanel;
        private System.Windows.Forms.Button SettingsVFilesButton;
        private System.Windows.Forms.Button SettingsClearCommunicationLogButton;
        private System.Windows.Forms.Button SettingsAboutButton;
        private System.Windows.Forms.Label SettingsLauncherVersion;
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
        private System.Windows.Forms.Button SettingsClearServerModCacheButton;
    }
}
