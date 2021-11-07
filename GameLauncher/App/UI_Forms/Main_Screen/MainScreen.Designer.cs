namespace GameLauncher.App.UI_Forms.Main_Screen
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
            this.SelectServerBtn = new System.Windows.Forms.Button();
            this.translatedBy = new System.Windows.Forms.Label();
            this.ServerPick = new System.Windows.Forms.ComboBox();
            this.AddServer = new System.Windows.Forms.Button();
            this.PlayProgressText = new System.Windows.Forms.Label();
            this.LauncherStatusText = new System.Windows.Forms.Label();
            this.LauncherStatusDesc = new System.Windows.Forms.Label();
            this.ServerStatusText = new System.Windows.Forms.Label();
            this.ServerStatusDesc = new System.Windows.Forms.Label();
            this.APIStatusText = new System.Windows.Forms.Label();
            this.APIStatusDesc = new System.Windows.Forms.Label();
            this.CurrentWindowInfo = new System.Windows.Forms.Label();
            this.MainEmail = new System.Windows.Forms.TextBox();
            this.MainPassword = new System.Windows.Forms.TextBox();
            this.RememberMe = new System.Windows.Forms.CheckBox();
            this.ForgotPassword = new System.Windows.Forms.LinkLabel();
            this.LoginButton = new System.Windows.Forms.Button();
            this.RegisterText = new System.Windows.Forms.Button();
            this.ServerPingStatusText = new System.Windows.Forms.Label();
            this.LogoutButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            this.PlayProgressTextTimer = new System.Windows.Forms.Label();
            this.ShowPlayPanel = new System.Windows.Forms.Panel();
            this.InsiderBuildNumberText = new System.Windows.Forms.Label();
            this.logo = new System.Windows.Forms.PictureBox();
            this.SettingsButton = new System.Windows.Forms.PictureBox();
            this.CloseBTN = new System.Windows.Forms.PictureBox();
            this.ServerInfoPanel = new System.Windows.Forms.Panel();
            this.HomePageIcon = new System.Windows.Forms.PictureBox();
            this.DiscordIcon = new System.Windows.Forms.PictureBox();
            this.FacebookIcon = new System.Windows.Forms.PictureBox();
            this.TwitterAccountLink = new System.Windows.Forms.LinkLabel();
            this.TwitterIcon = new System.Windows.Forms.PictureBox();
            this.FacebookGroupLink = new System.Windows.Forms.LinkLabel();
            this.HomePageLink = new System.Windows.Forms.LinkLabel();
            this.DiscordInviteLink = new System.Windows.Forms.LinkLabel();
            this.ServerShutDown = new System.Windows.Forms.Label();
            this.SceneryGroupText = new System.Windows.Forms.Label();
            this.LauncherIconStatus = new System.Windows.Forms.PictureBox();
            this.APIStatusIcon = new System.Windows.Forms.PictureBox();
            this.ServerStatusIcon = new System.Windows.Forms.PictureBox();
            this.MainEmailBorder = new System.Windows.Forms.PictureBox();
            this.MainPasswordBorder = new System.Windows.Forms.PictureBox();
            this.ProgressBarOutline = new System.Windows.Forms.PictureBox();
            this.Banner = new System.Windows.Forms.PictureBox();
            this.Notification = new System.Windows.Forms.NotifyIcon(this.components);
            this.ButtonSecurityCenter = new System.Windows.Forms.PictureBox();
            this.ExtractingProgress = new GameLauncher.App.Classes.LauncherCore.Visuals.ProgressBarEx();
            this.PlayProgress = new GameLauncher.App.Classes.LauncherCore.Visuals.ProgressBarEx();
            this.ShowPlayPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBTN)).BeginInit();
            this.ServerInfoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HomePageIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscordIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacebookIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TwitterIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LauncherIconStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressBarOutline)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Banner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonSecurityCenter)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectServerBtn
            // 
            resources.ApplyResources(this.SelectServerBtn, "SelectServerBtn");
            this.SelectServerBtn.Name = "SelectServerBtn";
            this.SelectServerBtn.UseVisualStyleBackColor = true;
            // 
            // translatedBy
            // 
            this.translatedBy.BackColor = System.Drawing.Color.Transparent;
            this.translatedBy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.translatedBy, "translatedBy");
            this.translatedBy.ForeColor = System.Drawing.Color.DarkGray;
            this.translatedBy.Name = "translatedBy";
            // 
            // ServerPick
            // 
            this.ServerPick.BackColor = System.Drawing.Color.White;
            this.ServerPick.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.ServerPick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.ServerPick, "ServerPick");
            this.ServerPick.ForeColor = System.Drawing.Color.Black;
            this.ServerPick.FormattingEnabled = true;
            this.ServerPick.Name = "ServerPick";
            // 
            // AddServer
            // 
            this.AddServer.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.AddServer, "AddServer");
            this.AddServer.ForeColor = System.Drawing.Color.Black;
            this.AddServer.Name = "AddServer";
            this.AddServer.UseVisualStyleBackColor = false;
            // 
            // PlayProgressText
            // 
            this.PlayProgressText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.PlayProgressText, "PlayProgressText");
            this.PlayProgressText.ForeColor = System.Drawing.Color.White;
            this.PlayProgressText.Name = "PlayProgressText";
            // 
            // LauncherStatusText
            // 
            this.LauncherStatusText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LauncherStatusText, "LauncherStatusText");
            this.LauncherStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(132)))), ((int)(((byte)(132)))));
            this.LauncherStatusText.Name = "LauncherStatusText";
            this.LauncherStatusText.UseCompatibleTextRendering = true;
            // 
            // LauncherStatusDesc
            // 
            this.LauncherStatusDesc.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LauncherStatusDesc, "LauncherStatusDesc");
            this.LauncherStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.LauncherStatusDesc.Name = "LauncherStatusDesc";
            // 
            // ServerStatusText
            // 
            this.ServerStatusText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ServerStatusText, "ServerStatusText");
            this.ServerStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerStatusText.Name = "ServerStatusText";
            // 
            // ServerStatusDesc
            // 
            this.ServerStatusDesc.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ServerStatusDesc, "ServerStatusDesc");
            this.ServerStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ServerStatusDesc.Name = "ServerStatusDesc";
            // 
            // APIStatusText
            // 
            this.APIStatusText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.APIStatusText, "APIStatusText");
            this.APIStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.APIStatusText.Name = "APIStatusText";
            // 
            // APIStatusDesc
            // 
            this.APIStatusDesc.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.APIStatusDesc, "APIStatusDesc");
            this.APIStatusDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.APIStatusDesc.Name = "APIStatusDesc";
            // 
            // CurrentWindowInfo
            // 
            this.CurrentWindowInfo.BackColor = System.Drawing.Color.Transparent;
            this.CurrentWindowInfo.Cursor = System.Windows.Forms.Cursors.Default;
            this.CurrentWindowInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.CurrentWindowInfo, "CurrentWindowInfo");
            this.CurrentWindowInfo.ForeColor = System.Drawing.Color.White;
            this.CurrentWindowInfo.Name = "CurrentWindowInfo";
            this.CurrentWindowInfo.UseCompatibleTextRendering = true;
            this.CurrentWindowInfo.UseMnemonic = false;
            // 
            // MainEmail
            // 
            this.MainEmail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.MainEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.MainEmail, "MainEmail");
            this.MainEmail.ForeColor = System.Drawing.Color.White;
            this.MainEmail.Name = "MainEmail";
            this.MainEmail.TextChanged += new System.EventHandler(this.Email_TextChanged);
            // 
            // MainPassword
            // 
            this.MainPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.MainPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.MainPassword, "MainPassword");
            this.MainPassword.ForeColor = System.Drawing.Color.White;
            this.MainPassword.Name = "MainPassword";
            this.MainPassword.UseSystemPasswordChar = true;
            this.MainPassword.TextChanged += new System.EventHandler(this.Password_TextChanged);
            // 
            // RememberMe
            // 
            this.RememberMe.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.RememberMe, "RememberMe");
            this.RememberMe.ForeColor = System.Drawing.Color.White;
            this.RememberMe.Name = "RememberMe";
            this.RememberMe.UseVisualStyleBackColor = false;
            // 
            // ForgotPassword
            // 
            this.ForgotPassword.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            this.ForgotPassword.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ForgotPassword, "ForgotPassword");
            this.ForgotPassword.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(200)))), ((int)(((byte)(0)))));
            this.ForgotPassword.Name = "ForgotPassword";
            this.ForgotPassword.TabStop = true;
            this.ForgotPassword.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(228)))), ((int)(((byte)(0)))));
            // 
            // LoginButton
            // 
            this.LoginButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LoginButton, "LoginButton");
            this.LoginButton.FlatAppearance.BorderSize = 0;
            this.LoginButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LoginButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LoginButton.ForeColor = System.Drawing.Color.White;
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.UseVisualStyleBackColor = false;
            // 
            // RegisterText
            // 
            this.RegisterText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.RegisterText, "RegisterText");
            this.RegisterText.FlatAppearance.BorderSize = 0;
            this.RegisterText.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.RegisterText.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.RegisterText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(193)))), ((int)(((byte)(32)))));
            this.RegisterText.Name = "RegisterText";
            this.RegisterText.UseVisualStyleBackColor = false;
            // 
            // ServerPingStatusText
            // 
            this.ServerPingStatusText.BackColor = System.Drawing.Color.Transparent;
            this.ServerPingStatusText.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerPingStatusText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.ServerPingStatusText, "ServerPingStatusText");
            this.ServerPingStatusText.ForeColor = System.Drawing.Color.White;
            this.ServerPingStatusText.Name = "ServerPingStatusText";
            // 
            // LogoutButton
            // 
            this.LogoutButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LogoutButton, "LogoutButton");
            this.LogoutButton.FlatAppearance.BorderSize = 0;
            this.LogoutButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LogoutButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LogoutButton.ForeColor = System.Drawing.Color.White;
            this.LogoutButton.Name = "LogoutButton";
            this.LogoutButton.UseVisualStyleBackColor = false;
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.PlayButton, "PlayButton");
            this.PlayButton.FlatAppearance.BorderSize = 0;
            this.PlayButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PlayButton.ForeColor = System.Drawing.Color.Transparent;
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.UseVisualStyleBackColor = false;
            // 
            // PlayProgressTextTimer
            // 
            this.PlayProgressTextTimer.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.PlayProgressTextTimer, "PlayProgressTextTimer");
            this.PlayProgressTextTimer.ForeColor = System.Drawing.Color.White;
            this.PlayProgressTextTimer.Name = "PlayProgressTextTimer";
            // 
            // ShowPlayPanel
            // 
            resources.ApplyResources(this.ShowPlayPanel, "ShowPlayPanel");
            this.ShowPlayPanel.BackColor = System.Drawing.Color.Transparent;
            this.ShowPlayPanel.Controls.Add(this.ServerPingStatusText);
            this.ShowPlayPanel.Controls.Add(this.LogoutButton);
            this.ShowPlayPanel.Controls.Add(this.PlayButton);
            this.ShowPlayPanel.ForeColor = System.Drawing.Color.Transparent;
            this.ShowPlayPanel.Name = "ShowPlayPanel";
            // 
            // InsiderBuildNumberText
            // 
            this.InsiderBuildNumberText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.InsiderBuildNumberText, "InsiderBuildNumberText");
            this.InsiderBuildNumberText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.InsiderBuildNumberText.Name = "InsiderBuildNumberText";
            // 
            // logo
            // 
            this.logo.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.logo, "logo");
            this.logo.Name = "logo";
            this.logo.TabStop = false;
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.SettingsButton, "SettingsButton");
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.TabStop = false;
            // 
            // CloseBTN
            // 
            this.CloseBTN.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.CloseBTN, "CloseBTN");
            this.CloseBTN.Name = "CloseBTN";
            this.CloseBTN.TabStop = false;
            // 
            // ServerInfoPanel
            // 
            this.ServerInfoPanel.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ServerInfoPanel, "ServerInfoPanel");
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
            this.ServerInfoPanel.ForeColor = System.Drawing.Color.Transparent;
            this.ServerInfoPanel.Name = "ServerInfoPanel";
            // 
            // HomePageIcon
            // 
            this.HomePageIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.HomePageIcon, "HomePageIcon");
            this.HomePageIcon.Name = "HomePageIcon";
            this.HomePageIcon.TabStop = false;
            // 
            // DiscordIcon
            // 
            this.DiscordIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.DiscordIcon, "DiscordIcon");
            this.DiscordIcon.Name = "DiscordIcon";
            this.DiscordIcon.TabStop = false;
            // 
            // FacebookIcon
            // 
            this.FacebookIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.FacebookIcon, "FacebookIcon");
            this.FacebookIcon.Name = "FacebookIcon";
            this.FacebookIcon.TabStop = false;
            // 
            // TwitterAccountLink
            // 
            this.TwitterAccountLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TwitterAccountLink.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.TwitterAccountLink, "TwitterAccountLink");
            this.TwitterAccountLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.TwitterAccountLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.TwitterAccountLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.TwitterAccountLink.Name = "TwitterAccountLink";
            this.TwitterAccountLink.TabStop = true;
            // 
            // TwitterIcon
            // 
            this.TwitterIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.TwitterIcon, "TwitterIcon");
            this.TwitterIcon.Name = "TwitterIcon";
            this.TwitterIcon.TabStop = false;
            // 
            // FacebookGroupLink
            // 
            this.FacebookGroupLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FacebookGroupLink.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.FacebookGroupLink, "FacebookGroupLink");
            this.FacebookGroupLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.FacebookGroupLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.FacebookGroupLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.FacebookGroupLink.Name = "FacebookGroupLink";
            this.FacebookGroupLink.TabStop = true;
            // 
            // HomePageLink
            // 
            this.HomePageLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HomePageLink.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.HomePageLink, "HomePageLink");
            this.HomePageLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.HomePageLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.HomePageLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.HomePageLink.Name = "HomePageLink";
            this.HomePageLink.TabStop = true;
            // 
            // DiscordInviteLink
            // 
            this.DiscordInviteLink.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DiscordInviteLink.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.DiscordInviteLink, "DiscordInviteLink");
            this.DiscordInviteLink.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.DiscordInviteLink.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.DiscordInviteLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.DiscordInviteLink.Name = "DiscordInviteLink";
            this.DiscordInviteLink.TabStop = true;
            // 
            // ServerShutDown
            // 
            this.ServerShutDown.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ServerShutDown, "ServerShutDown");
            this.ServerShutDown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ServerShutDown.Name = "ServerShutDown";
            // 
            // SceneryGroupText
            // 
            this.SceneryGroupText.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.SceneryGroupText, "SceneryGroupText");
            this.SceneryGroupText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.SceneryGroupText.Name = "SceneryGroupText";
            // 
            // LauncherIconStatus
            // 
            this.LauncherIconStatus.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.LauncherIconStatus, "LauncherIconStatus");
            this.LauncherIconStatus.Name = "LauncherIconStatus";
            this.LauncherIconStatus.TabStop = false;
            // 
            // APIStatusIcon
            // 
            this.APIStatusIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.APIStatusIcon, "APIStatusIcon");
            this.APIStatusIcon.Name = "APIStatusIcon";
            this.APIStatusIcon.TabStop = false;
            // 
            // ServerStatusIcon
            // 
            this.ServerStatusIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ServerStatusIcon, "ServerStatusIcon");
            this.ServerStatusIcon.Name = "ServerStatusIcon";
            this.ServerStatusIcon.TabStop = false;
            // 
            // MainEmailBorder
            // 
            this.MainEmailBorder.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.MainEmailBorder, "MainEmailBorder");
            this.MainEmailBorder.Name = "MainEmailBorder";
            this.MainEmailBorder.TabStop = false;
            // 
            // MainPasswordBorder
            // 
            this.MainPasswordBorder.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.MainPasswordBorder, "MainPasswordBorder");
            this.MainPasswordBorder.Name = "MainPasswordBorder";
            this.MainPasswordBorder.TabStop = false;
            // 
            // ProgressBarOutline
            // 
            this.ProgressBarOutline.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ProgressBarOutline, "ProgressBarOutline");
            this.ProgressBarOutline.Name = "ProgressBarOutline";
            this.ProgressBarOutline.TabStop = false;
            // 
            // Banner
            // 
            this.Banner.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.Banner, "Banner");
            this.Banner.Name = "Banner";
            this.Banner.TabStop = false;
            // 
            // Notification
            // 
            resources.ApplyResources(this.Notification, "Notification");
            // 
            // ButtonSecurityCenter
            // 
            this.ButtonSecurityCenter.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.ButtonSecurityCenter, "ButtonSecurityCenter");
            this.ButtonSecurityCenter.Name = "ButtonSecurityCenter";
            this.ButtonSecurityCenter.TabStop = false;
            // 
            // ExtractingProgress
            // 
            this.ExtractingProgress.BackColor = System.Drawing.Color.Transparent;
            this.ExtractingProgress.BackgroundColor = System.Drawing.Color.Black;
            this.ExtractingProgress.Border = false;
            this.ExtractingProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            resources.ApplyResources(this.ExtractingProgress, "ExtractingProgress");
            this.ExtractingProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.ExtractingProgress.GradiantColor = System.Drawing.Color.Transparent;
            this.ExtractingProgress.Image = global::GameLauncher.Properties.Resources.progress_success;
            this.ExtractingProgress.Name = "ExtractingProgress";
            this.ExtractingProgress.ProgressColor = System.Drawing.Color.Green;
            this.ExtractingProgress.RoundedCorners = false;
            // 
            // PlayProgress
            // 
            this.PlayProgress.BackColor = System.Drawing.Color.Transparent;
            this.PlayProgress.BackgroundColor = System.Drawing.Color.Black;
            this.PlayProgress.Border = false;
            this.PlayProgress.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.PlayProgress.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.PlayProgress, "PlayProgress");
            this.PlayProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(179)))), ((int)(((byte)(189)))));
            this.PlayProgress.GradiantColor = System.Drawing.Color.Transparent;
            this.PlayProgress.Image = global::GameLauncher.Properties.Resources.progress_preload;
            this.PlayProgress.Name = "PlayProgress";
            this.PlayProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(84)))), ((int)(((byte)(92)))));
            this.PlayProgress.RoundedCorners = false;
            // 
            // MainScreen
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.ButtonSecurityCenter);
            this.Controls.Add(this.InsiderBuildNumberText);
            this.Controls.Add(this.CurrentWindowInfo);
            this.Controls.Add(this.logo);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.ShowPlayPanel);
            this.Controls.Add(this.CloseBTN);
            this.Controls.Add(this.SelectServerBtn);
            this.Controls.Add(this.ServerInfoPanel);
            this.Controls.Add(this.translatedBy);
            this.Controls.Add(this.ServerPick);
            this.Controls.Add(this.AddServer);
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
            this.Controls.Add(this.PlayProgressTextTimer);
            this.Controls.Add(this.PlayProgressText);
            this.Controls.Add(this.ExtractingProgress);
            this.Controls.Add(this.PlayProgress);
            this.Controls.Add(this.ProgressBarOutline);
            this.Controls.Add(this.Banner);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "MainScreen";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.ShowPlayPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SettingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CloseBTN)).EndInit();
            this.ServerInfoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.HomePageIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscordIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FacebookIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TwitterIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LauncherIconStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.APIStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ServerStatusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainEmailBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MainPasswordBorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ProgressBarOutline)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Banner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ButtonSecurityCenter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox logo;
        private System.Windows.Forms.PictureBox CloseBTN;
        private System.Windows.Forms.Button SelectServerBtn;
        private System.Windows.Forms.Label translatedBy;
        private System.Windows.Forms.PictureBox SettingsButton;
        private System.Windows.Forms.ComboBox ServerPick;
        private System.Windows.Forms.Button AddServer;
        private System.Windows.Forms.PictureBox Banner;
        internal System.Windows.Forms.Label PlayProgressText;
        private GameLauncher.App.Classes.LauncherCore.Visuals.ProgressBarEx ExtractingProgress;
        private GameLauncher.App.Classes.LauncherCore.Visuals.ProgressBarEx PlayProgress;
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
        private System.Windows.Forms.Panel ShowPlayPanel;
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
        private System.Windows.Forms.Label InsiderBuildNumberText;
        private System.Windows.Forms.PictureBox ProgressBarOutline;
        private System.Windows.Forms.NotifyIcon Notification;
        private System.Windows.Forms.PictureBox ButtonSecurityCenter;
    }
}
