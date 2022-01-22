using SBRW.Launcher.App.Classes.Auth;
using SBRW.Launcher.App.Classes.LauncherCore.Client.Auth;
using SBRW.Launcher.App.Classes.LauncherCore.Global;
using SBRW.Launcher.App.Classes.LauncherCore.Lists;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using SBRW.Launcher.App.Classes.SystemPlatform.Unix;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SBRW.Launcher.App.UI_Forms.Register_Screen
{
    public partial class Screen_Register : Form
    {
        private static bool IsRegisterScreenOpen { get; set; }
        private bool Ticket_Required { get; set; }

        private void Button_Register_Click(object sender, EventArgs e)
        {
            Refresh();

            List<string> registerErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(RegisterEmail.Text))
            {
                registerErrors.Add("Please enter your e-mail.");
                RegisterEmailBorder.Image = Theming.BorderEmailError;

            }
            else if (!Is_Email.Valid(RegisterEmail.Text))
            {
                registerErrors.Add("Please enter a valid e-mail address.");
                RegisterEmailBorder.Image = Theming.BorderEmailError;
            }

            if (string.IsNullOrWhiteSpace(RegisterTicket.Text) && _ticketRequired)
            {
                registerErrors.Add("Please enter your ticket.");
                Picture_Input_Ticket.Image = Image_Other.Text_Border_Ticket_Error;
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

            if (!CheckBox_Rules_Agreement.Checked)
            {
                registerErrors.Add("You have not agreed to the Terms of Service.");
                CheckBox_Rules_Agreement.ForeColor = Theming.Error;
            }

            if (registerErrors.Count == 0)
            {
                bool allowReg = false;

                String Email;
                String Password;

                switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
                {
                    case AuthHash.H10:
                        Email = RegisterEmail.Text.ToString();
                        Password = RegisterPassword.Text.ToString();
                        break;
                    case AuthHash.H11:
                        Email = RegisterEmail.Text.ToString();
                        Password = Hashes.Hash_String(0, RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H12:
                        Email = RegisterEmail.Text.ToString();
                        Password = Hashes.Hash_String(1, RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H13:
                        Email = RegisterEmail.Text.ToString();
                        Password = Hashes.Hash_String(2, RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H20:
                        Email = Hashes.Hash_String(0, RegisterEmail.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(0, RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H21:
                        Email = Hashes.Hash_String(1, RegisterEmail.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(1, RegisterPassword.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H22:
                        Email = Hashes.Hash_String(2, RegisterEmail.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(2, RegisterPassword.Text.ToString()).ToLower();
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
                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
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
                        API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
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

                    Tokens.IPAddress = Launcher_Value.Launcher_Select_Server_Data.IPAddress;
                    Tokens.ServerName = ServerListUpdater.ServerName("Register");

                    Authentication.Client("Register", Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Post, Email, Password, _ticketRequired ? RegisterTicket.Text : null);

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
            Button_Register.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Theming.GreenButton;
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Theming.GreenButtonHover;
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Register.BackgroundImage = Theming.GreenButtonClick;
        }

        private void CheckBox_Rules_Agreement_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox_Rules_Agreement.ForeColor = Theming.FivithTextForeColor;
        }

        private void Input_Email_TextChanged(object sender, EventArgs e)
        {
            RegisterEmailBorder.Image = Theming.BorderEmail;
        }

        private void Input_Ticket_TextChanged(object sender, EventArgs e)
        {
            Picture_Input_Ticket.Image = Theming.BorderTicket;
        }

        private void Input_Password_Confirm_TextChanged(object sender, EventArgs e)
        {
            RegisterConfirmPasswordBorder.Image = Theming.BorderPassword;
        }

        private void Input_Password_TextChanged(object sender, EventArgs e)
        {
            RegisterPasswordBorder.Image = Theming.BorderPassword;
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            Button_Cancel.BackgroundImage = Theming.GrayButtonClick;
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            Button_Cancel.BackgroundImage = Theming.GrayButtonHover;
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            Button_Cancel.BackgroundImage = Theming.GrayButton;
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            Button_Cancel.BackgroundImage = Theming.GrayButtonHover;
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
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
            CheckBox_Rules_Agreement.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            Button_Register.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            Button_Cancel.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);
            CurrentWindowInfo.Font = new Font(DejaVuSansBold, MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Set Background with Transparent Key */
            BackgroundImage = Image_Background.Registration;
            TransparencyKey = Color_Screen.BG_Registration;

            CurrentWindowInfo.ForeColor = Theming.FivithTextForeColor;

            RegisterEmail.BackColor = Theming.Input;
            RegisterEmail.ForeColor = Theming.FivithTextForeColor;
            RegisterEmailBorder.Image = Image_Other.Text_Border_Email;

            RegisterPasswordBorder.Image = Image_Other.Text_Border_Password;
            RegisterPassword.BackColor = Theming.Input;
            RegisterPassword.ForeColor = Theming.FivithTextForeColor;

            RegisterConfirmPasswordBorder.Image = Image_Other.Text_Border_Password;
            RegisterConfirmPassword.BackColor = Theming.Input;
            RegisterConfirmPassword.ForeColor = Theming.FivithTextForeColor;

            Picture_Input_Ticket.Image = Image_Other.Text_Border_Ticket;
            RegisterTicket.BackColor = Theming.Input;
            RegisterTicket.ForeColor = Theming.FivithTextForeColor;

            CheckBox_Rules_Agreement.ForeColor = Theming.WinFormWarningTextForeColor;

            Button_Register.BackgroundImage = Theming.GreenButton;
            Button_Register.ForeColor = Theming.SeventhTextForeColor;

            Button_Cancel.BackgroundImage = Theming.GrayButton;
            Button_Cancel.ForeColor = Theming.FivithTextForeColor;

            /********************************/
            /* Events                        /
            /********************************/

            Input_Email.TextChanged += new EventHandler(Input_Email_TextChanged);
            Input_Password.TextChanged += new EventHandler(Input_Password_TextChanged);
            Input_Password_Confirm.TextChanged += new EventHandler(Input_Password_Confirm_TextChanged);
            Input_Ticket.TextChanged += new EventHandler(Input_Ticket_TextChanged);
            CheckBox_Rules_Agreement.CheckedChanged += new EventHandler(CheckBox_Rules_Agreement_CheckedChanged);

            Button_Register.MouseEnter += Greenbutton_hover_MouseEnter;
            Button_Register.MouseLeave += Greenbutton_MouseLeave;
            Button_Register.MouseUp += Greenbutton_hover_MouseUp;
            Button_Register.MouseDown += Greenbutton_click_MouseDown;
            Button_Register.Click += Button_Register_Click;

            Button_Cancel.MouseEnter += new EventHandler(Graybutton_hover_MouseEnter);
            Button_Cancel.MouseLeave += new EventHandler(Graybutton_MouseLeave);
            Button_Cancel.MouseUp += new MouseEventHandler(Graybutton_hover_MouseUp);
            Button_Cancel.MouseDown += new MouseEventHandler(Graybutton_click_MouseDown);
            Button_Cancel.Click += new EventHandler(Button_Cancel_Click);

            /********************************/
            /* Functions                     /
            /********************************/

            Label_Information_Window.Text = "REGISTER ON \n" + ServerListUpdater.ServerName("Register").ToUpper();
            Ticket_Required = Launcher_Value.Launcher_Select_Server_JSON.Server_Registration_Token;
            /* Show Ticket Box if its Required  */
            Input_Ticket.Visible = Ticket_Required;
            Picture_Input_Ticket.Visible = Ticket_Required;
        }

        public static void OpenScreen()
        {
            if (IsRegisterScreenOpen || Application.OpenForms["Screen_Register"] != null)
            {
                if (Application.OpenForms["Screen_Register"] != null) { Application.OpenForms["Screen_Register"].Activate(); }
            }
            else
            {
                try { new Screen_Register().ShowDialog(); }
                catch (Exception Error)
                {
                    string ErrorMessage = "Register Screen Encountered an Error";
                    LogToFileAddons.OpenLog("Register Screen", ErrorMessage, Error, "Exclamation", false);
                }
            }
        }

        public Screen_Register()
        {
            IsRegisterScreenOpen = true;
            InitializeComponent();
            SetVisuals();
            Presence_Launcher.Status("Register", ServerListUpdater.ServerName("Register"));
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status("Idle Ready", null);
                IsRegisterScreenOpen = false;
                GC.Collect();
            };
        }
    }
}
