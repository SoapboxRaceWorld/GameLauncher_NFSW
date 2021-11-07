using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.Client.Auth;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.Email;
using GameLauncher.App.Classes.LauncherCore.Visuals;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace GameLauncher.App.UI_Forms.Register_Screen
{
    public partial class RegisterScreen : Form
    {
        private static bool IsRegisterScreenOpen = false;
        private bool _ticketRequired;

        public static void OpenScreen()
        {
            if (IsRegisterScreenOpen || Application.OpenForms["RegisterScreen"] != null)
            {
                if (Application.OpenForms["RegisterScreen"] != null) { Application.OpenForms["RegisterScreen"].Activate(); }
            }
            else
            {
                try { new RegisterScreen().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Register Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Register Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public RegisterScreen()
        {
            IsRegisterScreenOpen = true;
            InitializeComponent();
            SetVisuals();
            DiscordLauncherPresence.Status("Register", ServerListUpdater.ServerName("Register"));
            this.Closing += (x, y) =>
            {
                DiscordLauncherPresence.Status("Idle Ready", null);
                IsRegisterScreenOpen = false;
                GC.Collect();
            };
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

                String Email;
                String Password;

                switch (Authentication.HashType(InformationCache.SelectedServerJSON.authHash ?? string.Empty))
                {
                    case AuthHash.H10:
                        Email = RegisterEmail.Text.ToString();
                        Password = RegisterPassword.Text.ToString();
                        break;
                    case AuthHash.H11:
                        Email = RegisterEmail.Text.ToString();
                        Password = MDFive.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H12:
                        Email = RegisterEmail.Text.ToString();
                        Password = SHA.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H13:
                        Email = RegisterEmail.Text.ToString();
                        Password = SHATwoFiveSix.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H20:
                        Email = MDFive.Hashes(RegisterEmail.Text.ToString()).ToLower();
                        Password = MDFive.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H21:
                        Email = SHA.Hashes(RegisterEmail.Text.ToString()).ToLower();
                        Password = SHA.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H22:
                        Email = SHATwoFiveSix.Hashes(RegisterEmail.Text.ToString()).ToLower();
                        Password = SHATwoFiveSix.Hashes(RegisterPassword.Text.ToString()).ToLower();
                        break;
                    default:
                        Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                        return;
                }

                try
                {
                    string[] regex = new Regex(@"([0-9A-Z]{5})([0-9A-Z]{35})").Split(Password.ToUpper());

                    Uri URLCall = new Uri("https://api.pwnedpasswords.com/range/" + regex[1]);
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8
                    };
                    if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                    else
                    {
                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    String serverReply = null;
                    try
                    {
                        serverReply = Client.DownloadString(URLCall);
                    }
                    catch (WebException Error)
                    {
                        APIChecker.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                            Error, (HttpWebResponse)Error.Response);
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("Register", null, Error, null, true);
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(serverReply))
                    {
                        String verify = regex[2];

                        string[] hashes = serverReply.Split('\n');
                        foreach (string hash in hashes)
                        {
                            var splitChecks = hash.Split(':');
                            if (splitChecks[0] == verify)
                            {
                                var passwordCheckReply = MessageBox.Show(null, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1]) +
                                    " times, you should consider using a different one.\n\nAlternatively you can use the unsafe password anyway." +
                                    "\nWould you like to continue to use it?", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
                    else
                    {
                        allowReg = true;
                    }
                }
                catch
                {
                    allowReg = true;
                }

                if (allowReg)
                {
                    Tokens.Clear();

                    Tokens.IPAddress = InformationCache.SelectedServerData.IPAddress;
                    Tokens.ServerName = ServerListUpdater.ServerName("Register");

                    Authentication.Client("Register", InformationCache.SelectedServerJSON.modernAuthSupport, Email, Password, _ticketRequired ? RegisterTicket.Text : null);

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

            float MainFontSize = UnixOS.Detected() ? 9f : 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = UnixOS.Detected() ? 8f : 8f * 96f / CreateGraphics().DpiY;
            Font = new Font(DejaVuSans, SecondaryFontSize, FontStyle.Regular);

            /* Registering Panel */
            RegisterEmail.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterConfirmPassword.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterTicket.Font = new Font(DejaVuSans, MainFontSize, FontStyle.Regular);
            RegisterAgree.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            RegisterButton.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            RegisterCancel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            CurrentWindowInfo.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Set Background with Transparent Key */
            BackgroundImage = Theming.RegisterScreen;
            TransparencyKey = Theming.RegisterScreenTransparencyKey;

            CurrentWindowInfo.ForeColor = Theming.FivithTextForeColor;

            RegisterEmail.BackColor = Theming.Input;
            RegisterEmail.ForeColor = Theming.FivithTextForeColor;
            RegisterEmailBorder.Image = Theming.BorderEmail;

            RegisterPasswordBorder.Image = Theming.BorderPassword;
            RegisterPassword.BackColor = Theming.Input;
            RegisterPassword.ForeColor = Theming.FivithTextForeColor;

            RegisterConfirmPasswordBorder.Image = Theming.BorderPassword;
            RegisterConfirmPassword.BackColor = Theming.Input;
            RegisterConfirmPassword.ForeColor = Theming.FivithTextForeColor;

            RegisterTicketBorder.Image = Theming.BorderTicket;
            RegisterTicket.BackColor = Theming.Input;
            RegisterTicket.ForeColor = Theming.FivithTextForeColor;

            RegisterAgree.ForeColor = Theming.WinFormWarningTextForeColor;

            RegisterButton.BackgroundImage = Theming.GreenButton;
            RegisterButton.ForeColor = Theming.SeventhTextForeColor;

            RegisterCancel.BackgroundImage = Theming.GrayButton;
            RegisterCancel.ForeColor = Theming.FivithTextForeColor;

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
                if (InformationCache.SelectedServerJSON.requireTicket != null && !string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.requireTicket))
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
