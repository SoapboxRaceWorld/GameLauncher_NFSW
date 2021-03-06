﻿using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.LauncherCore.Client.Auth;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.Email;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Linux;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GameLauncher.App
{
    public partial class RegisterScreen : Form
    {
        private bool _ticketRequired;

        public RegisterScreen()
        {
            InitializeComponent();
            DiscordLauncherPresense.Status("Register", ServerListUpdater.ServerName("Register"));
            this.Closing += (x, y) =>
            {
                DiscordLauncherPresense.Status("Idle Ready", null);
            };
            SetVisuals();
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            Refresh();

            List<string> registerErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(RegisterEmail.Text))
            {
                registerErrors.Add("Please enter your e-mail.");
                RegisterEmailBorder.Image = Theming.BorderEmailError;

            }
            else if (!IsEmailValid.Validate(RegisterEmail.Text))
            {
                registerErrors.Add("Please enter a valid e-mail address.");
                RegisterEmailBorder.Image = Theming.BorderEmailError;
            }

            if (string.IsNullOrWhiteSpace(RegisterTicket.Text) && _ticketRequired)
            {
                registerErrors.Add("Please enter your ticket.");
                RegisterTicketBorder.Image = Theming.BorderTicketError;
            }

            if (string.IsNullOrWhiteSpace(RegisterPassword.Text))
            {
                registerErrors.Add("Please enter your password.");
                RegisterPasswordBorder.Image = Theming.BorderPasswordError;
            }

            if (string.IsNullOrWhiteSpace(RegisterConfirmPassword.Text))
            {
                registerErrors.Add("Please confirm your password.");
                RegisterConfirmPasswordBorder.Image = Theming.BorderPasswordError;
            }

            if (RegisterConfirmPassword.Text != RegisterPassword.Text)
            {
                registerErrors.Add("Passwords don't match.");
                RegisterConfirmPasswordBorder.Image = Theming.BorderPasswordError;
            }

            if (!RegisterAgree.Checked)
            {
                registerErrors.Add("You have not agreed to the Terms of Service.");
                RegisterAgree.ForeColor = Theming.Error;
            }

            if (registerErrors.Count == 0)
            {
                bool allowReg = false;

                try
                {
                    String checkPassword = SHA.HashPassword(RegisterPassword.Text.ToString()).ToUpper();
                    var regex = new Regex(@"([0-9A-Z]{5})([0-9A-Z]{35})").Split(checkPassword);
                    String range = regex[1];

                    FunctionStatus.TLS();
                    Uri URLCall = new Uri("https://api.pwnedpasswords.com/range/" + range);
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    WebClient breachCheck = new WebClient();

                    String verify = regex[2];
                    String serverReply = breachCheck.DownloadString(URLCall);

                    string[] hashes = serverReply.Split('\n');
                    foreach (string hash in hashes)
                    {
                        var splitChecks = hash.Split(':');
                        if (splitChecks[0] == verify)
                        {
                            var passwordCheckReply = MessageBox.Show(null, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1]) + 
                                " times, you should consider using different one.\n\nAlternatively you can use the unsafe password anyway." +
                                "\nWould you like to Use it?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (passwordCheckReply == DialogResult.Yes)
                            {
                                allowReg = true;
                            }
                            else
                            {
                                allowReg = false;
                            }
                        }
                        else
                        {
                            allowReg = true;
                        }
                    }
                }
                catch
                {
                    allowReg = true;
                }

                if (allowReg)
                {
                    Tokens.Clear();

                    String username = RegisterEmail.Text.ToString();
                    String realpass;
                    String token = (_ticketRequired) ? RegisterTicket.Text : null;

                    Tokens.IPAddress = InformationCache.SelectedServerData.IpAddress;
                    Tokens.ServerName = ServerListUpdater.ServerName("Register");

                    if (InformationCache.ModernAuthSupport == false)
                    {
                        realpass = SHA.HashPassword(RegisterPassword.Text.ToString()).ToLower();
                        Authentication.Client("Register", "Non Secure", username, realpass, token);
                    }
                    else
                    {
                        realpass = RegisterPassword.Text.ToString();
                        Authentication.Client("Register", "Secure", username, realpass, token);
                    }

                    if (!String.IsNullOrWhiteSpace(Tokens.Success))
                    {
                        DialogResult Success = MessageBox.Show(null, Tokens.Success, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        if (Success == DialogResult.OK)
                        {
                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show(null, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var message = "There were some errors while registering. Please fix them:\n\n";

                    foreach (var error in registerErrors)
                    {
                        message += "• " + error + "\n";
                    }

                    MessageBox.Show(null, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            RegisterButton.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            RegisterButton.BackgroundImage = Theming.GreenButton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            RegisterButton.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            RegisterButton.BackgroundImage = Theming.GreenButtonClick;
        }

        private void RegisterAgree_CheckedChanged(object sender, EventArgs e)
        {
            RegisterAgree.ForeColor = Theming.FivithTextForeColor;
        }

        private void RegisterEmail_TextChanged(object sender, EventArgs e)
        {
            RegisterEmailBorder.Image = Theming.BorderEmail;
        }

        private void RegisterTicket_TextChanged(object sender, EventArgs e)
        {
            RegisterTicketBorder.Image = Theming.BorderTicket;
        }

        private void RegisterConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            RegisterConfirmPasswordBorder.Image = Theming.BorderPassword;
        }

        private void RegisterPassword_TextChanged(object sender, EventArgs e)
        {
            RegisterPasswordBorder.Image = Theming.BorderPassword;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            RegisterCancel.BackgroundImage = Theming.GrayButtonClick;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            RegisterCancel.BackgroundImage = Theming.GrayButtonHover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            RegisterCancel.BackgroundImage = Theming.GrayButton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            RegisterCancel.BackgroundImage = Theming.GrayButtonHover;
        }

        private void RegisterCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SetVisuals()
        {
            /*******************************/
            /* Set Window Name              /
            /*******************************/

            Text = "Register - SBRW Launcher: v" + Application.ProductVersion;

            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/

            FunctionStatus.CenterParent(this);

            /*******************************/
            /* Set Font                     /
            /*******************************/

            FontFamily DejaVuSans = FontWrapper.Instance.GetFontFamily("DejaVuSans.ttf");
            FontFamily DejaVuSansBold = FontWrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf");

            var MainFontSize = 9f * 100f / CreateGraphics().DpiY;
            var SecondaryFontSize = 8f * 100f / CreateGraphics().DpiY;

            if (DetectLinux.LinuxDetected())
            {
                MainFontSize = 9f;
                SecondaryFontSize = 8f;
            }
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);

            /* Registering Panel */
            RegisterEmail.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterConfirmPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterTicket.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterAgree.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            RegisterButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            RegisterCancel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Set Background with Transparent Key */
            BackgroundImage = Theming.RegisterScreen;
            TransparencyKey = Theming.RegisterScreenTransparencyKey;

            RegisterEmailBorder.Image = Theming.BorderEmail;
            RegisterPasswordBorder.Image = Theming.BorderPassword;
            RegisterConfirmPasswordBorder.Image = Theming.BorderPassword;
            RegisterTicketBorder.Image = Theming.BorderTicket;
            RegisterAgree.ForeColor = Theming.WinFormWarningTextForeColor;

            RegisterButton.BackgroundImage = Theming.GreenButton;
            RegisterCancel.BackgroundImage = Theming.GrayButton;

            /********************************/
            /* Events                        /
            /********************************/

            RegisterEmail.TextChanged += new EventHandler(RegisterEmail_TextChanged);
            RegisterPassword.TextChanged += new EventHandler(RegisterPassword_TextChanged);
            RegisterConfirmPassword.TextChanged += new EventHandler(RegisterConfirmPassword_TextChanged);
            RegisterTicket.TextChanged += new EventHandler(RegisterTicket_TextChanged);
            RegisterAgree.CheckedChanged += new EventHandler(RegisterAgree_CheckedChanged);

            RegisterButton.MouseEnter += Greenbutton_hover_MouseEnter;
            RegisterButton.MouseLeave += Greenbutton_MouseLeave;
            RegisterButton.MouseUp += Greenbutton_hover_MouseUp;
            RegisterButton.MouseDown += Greenbutton_click_MouseDown;
            RegisterButton.Click += RegisterButton_Click;

            RegisterCancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            RegisterCancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            RegisterCancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            RegisterCancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            RegisterCancel.Click += new EventHandler(RegisterCancel_Click);

            /********************************/
            /* Functions                     /
            /********************************/

            CurrentWindowInfo.Text = "REGISTER ON \n" + ServerListUpdater.ServerName("Register").ToUpper();

            try
            {
                if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.requireTicket))
                {
                    _ticketRequired = InformationCache.SelectedServerJSON.requireTicket.ToLower() == "true";
                }
                else
                {
                    _ticketRequired = false;
                }
            }
            catch { }

            /* Show Ticket Box if its Required  */
            RegisterTicket.Visible = _ticketRequired;
            RegisterTicketBorder.Visible = _ticketRequired;
        }
    }
}
