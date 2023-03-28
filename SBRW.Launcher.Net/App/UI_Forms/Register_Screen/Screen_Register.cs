using SBRW.Launcher.RunTime.Auth;
using SBRW.Launcher.RunTime.LauncherCore.Client.Auth;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.RunTime.LauncherCore.Support;
using SBRW.Launcher.RunTime.SystemPlatform.Unix;
using SBRW.Launcher.App.UI_Forms.Main_Screen;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Discord.RPC_;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Hash_;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Validation_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Theme;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SBRW.Launcher.App.UI_Forms.Register_Screen
{
    public partial class Screen_Register : Form
    {
        private bool Ticket_Required { get; set; }

        private async void Button_Register_Click(object sender, EventArgs e)
        {
            Refresh();

            List<string> registerErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(Input_Email.Text))
            {
                registerErrors.Add("Please enter your e-mail.");
                if (Picture_Input_Email.Image != Image_Other.Text_Border_Email_Error)
                {
                    Picture_Input_Email.Image = Image_Other.Text_Border_Email_Error;
                }
            }
            else if (!Input_Email.Text.Valid_Email())
            {
                registerErrors.Add("Please enter a valid e-mail address.");
                if (Picture_Input_Email.Image != Image_Other.Text_Border_Email_Error)
                {
                    Picture_Input_Email.Image = Image_Other.Text_Border_Email_Error;
                }
            }

            if (string.IsNullOrWhiteSpace(Input_Ticket.Text) && Ticket_Required)
            {
                registerErrors.Add("Please enter your ticket.");
                if (Picture_Input_Ticket.Image != Image_Other.Text_Border_Ticket_Error)
                {
                    Picture_Input_Ticket.Image = Image_Other.Text_Border_Ticket_Error;
                }
            }

            if (string.IsNullOrWhiteSpace(Input_Password.Text))
            {
                registerErrors.Add("Please enter your password.");
                if (Picture_Input_Password.Image != Image_Other.Text_Border_Password_Error)
                {
                    Picture_Input_Password.Image = Image_Other.Text_Border_Password_Error;
                }
            }

            if (string.IsNullOrWhiteSpace(Input_Password_Confirm.Text))
            {
                registerErrors.Add("Please confirm your password.");
                if (Picture_Input_Password_Confirm.Image != Image_Other.Text_Border_Password_Error)
                {
                    Picture_Input_Password_Confirm.Image = Image_Other.Text_Border_Password_Error;
                }
            }

            if (Input_Password_Confirm.Text != Input_Password.Text)
            {
                registerErrors.Add("Passwords don't match.");
                if (Picture_Input_Password_Confirm.Image != Image_Other.Text_Border_Password_Error)
                {
                    Picture_Input_Password_Confirm.Image = Image_Other.Text_Border_Password_Error;
                }
            }

            if (!CheckBox_Rules_Agreement.Checked)
            {
                registerErrors.Add("You have not agreed to the Terms of Service.");
                if (CheckBox_Rules_Agreement.ForeColor != Color_Text.S_Error)
                {
                    CheckBox_Rules_Agreement.ForeColor = Color_Text.S_Error;
                }
            }

            if (registerErrors.Count == 0)
            {
                bool allowReg = false;

                string Email;
                string Password;

                switch (Authentication.HashType(Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Version ?? string.Empty))
                {
                    case AuthHash.H10:
                        Email = Input_Email.Text.ToString();
                        Password = Input_Password.Text.ToString();
                        break;
                    case AuthHash.H11:
                        Email = Input_Email.Text.ToString();
                        Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H12:
                        Email = Input_Email.Text.ToString();
                        Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H13:
                        Email = Input_Email.Text.ToString();
                        Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H20:
                        Email = Hashes.Hash_String(0, Input_Email.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(0, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H21:
                        Email = Hashes.Hash_String(1, Input_Email.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(1, Input_Password.Text.ToString()).ToLower();
                        break;
                    case AuthHash.H22:
                        Email = Hashes.Hash_String(2, Input_Email.Text.ToString()).ToLower();
                        Password = Hashes.Hash_String(2, Input_Password.Text.ToString()).ToLower();
                        break;
                    default:
                        Log.Error("HASH TYPE: Unknown Hash Standard was Provided");
                        return;
                }

                try
                {
                    string[] regex = new Regex(@"([0-9A-Z]{5})([0-9A-Z]{35})").Split(Password.ToUpper());

                    Uri URLCall = new Uri("https://api.pwnedpasswords.com/range/" + regex[1]);
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Launcher_Value.Launcher_WebCall_Timeout_Enable ?
                                    Launcher_Value.Launcher_WebCall_Timeout() : 60).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                    { 
                        Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                    }
                    else
                    {
                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    string serverReply = string.Empty;

                    await Task.Run(() =>
                    {
                        
                        try
                        {
                            serverReply = Client.DownloadString(URLCall);
                        }
                        catch (WebException Error)
                        {
                            API_Core.StatusCodes(URLCall.GetComponents(UriComponents.HttpRequestUrl, UriFormat.SafeUnescaped),
                                Error, Error.Response as HttpWebResponse);
                        }
                        catch (Exception Error)
                        {
                            LogToFileAddons.OpenLog("Register", string.Empty, Error, string.Empty, true);
                        }
                        finally
                        {
                            if (Client != null)
                            {
                                Client.Dispose();
                            }
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(serverReply))
                    {
                        string verify = regex[2];
                        string[] hashes = serverReply.Split('\n');
                        foreach (string hash in hashes)
                        {
                            var splitChecks = hash.Split(':');
                            if (splitChecks[0] == verify)
                            {
                                if (Picture_Information_Window.Image != Image_Other.Information_Window_Warning)
                                {
                                    Picture_Information_Window.Image = Image_Other.Information_Window_Warning;
                                }

                                DialogResult passwordCheckReply = MessageBox.Show(this, "Password used for registration has been breached " + Convert.ToInt32(splitChecks[1]) +
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

                    await Task.Run(() =>
                    {
                        Authentication.Client("Register", Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Post, Email, Password, Ticket_Required ? Input_Ticket.Text : string.Empty);

                        if (!string.IsNullOrWhiteSpace(Tokens.Success))
                        {
                            if (Picture_Information_Window.Image != (!string.IsNullOrWhiteSpace(Tokens.Warning) ? Image_Other.Information_Window_Warning : Image_Other.Information_Window_Success))
                            {
                                Picture_Information_Window.Image = !string.IsNullOrWhiteSpace(Tokens.Warning) ? Image_Other.Information_Window_Warning : Image_Other.Information_Window_Success;
                            }

                            DialogResult Success = MessageBox.Show(this, Tokens.Success, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            if (Success == DialogResult.OK)
                            {
                                Close();
                            }
                        }
                        else
                        {
                            if (Picture_Information_Window.Image != Image_Other.Information_Window_Error)
                            {
                                Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                            }

                            MessageBox.Show(this, Tokens.Error, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    });
                }
                else
                {
                    string message = "There were some errors while registering. Please fix them:\n\n";

                    foreach (string error in registerErrors)
                    {
                        message += "• " + error + "\n";
                    }

                    if (Picture_Information_Window.Image != Image_Other.Information_Window_Error)
                    {
                        Picture_Information_Window.Image = Image_Other.Information_Window_Error;
                    }

                    MessageBox.Show(this, message, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (Picture_Information_Window.Image != Image_Other.Information_Window_Error)
            {
                Picture_Information_Window.Image = Image_Other.Information_Window_Error;
            }
        }

        private void Default_Picture_Information_Window()
        {
            if (Picture_Information_Window.Image != Image_Other.Information_Window)
            {
                Picture_Information_Window.Image = Image_Other.Information_Window;
            }
        }

        private void Greenbutton_hover_MouseEnter(object sender, EventArgs e)
        {
            if (Button_Register.BackgroundImage != Image_Button.Green_Hover)
            {
                Button_Register.BackgroundImage = Image_Button.Green_Hover;
            }
        }

        private void Greenbutton_MouseLeave(object sender, EventArgs e)
        {
            if (Button_Register.BackgroundImage != Image_Button.Green)
            {
                Button_Register.BackgroundImage = Image_Button.Green;
            }
        }

        private void Greenbutton_hover_MouseUp(object sender, EventArgs e)
        {
            if (Button_Register.BackgroundImage != Image_Button.Green_Hover)
            {
                Button_Register.BackgroundImage = Image_Button.Green_Hover;
            }
        }

        private void Greenbutton_click_MouseDown(object sender, EventArgs e)
        {
            if (Button_Register.BackgroundImage != Image_Button.Green_Click)
            {
                Button_Register.BackgroundImage = Image_Button.Green_Click;
            }
        }

        private void CheckBox_Rules_Agreement_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_Rules_Agreement.ForeColor != Color_Text.L_Five)
            {
                CheckBox_Rules_Agreement.ForeColor = Color_Text.L_Five;
                Default_Picture_Information_Window();
            }
        }

        private void Input_Email_TextChanged(object sender, EventArgs e)
        {
            if (Picture_Input_Email.Image != Image_Other.Text_Border_Email)
            {
                Picture_Input_Email.Image = Image_Other.Text_Border_Email;
                Default_Picture_Information_Window();
            }
        }

        private void Input_Ticket_TextChanged(object sender, EventArgs e)
        {
            if (Picture_Input_Ticket.Image != Image_Other.Text_Border_Ticket)
            {
                Picture_Input_Ticket.Image = Image_Other.Text_Border_Ticket;
                Default_Picture_Information_Window();
            }
        }

        private void Input_Password_Confirm_TextChanged(object sender, EventArgs e)
        {
            if (Picture_Input_Password_Confirm.Image != Image_Other.Text_Border_Password)
            {
                Picture_Input_Password_Confirm.Image = Image_Other.Text_Border_Password;
                Default_Picture_Information_Window();
            }
        }

        private void Input_Password_TextChanged(object sender, EventArgs e)
        {
            if (Picture_Input_Password.Image != Image_Other.Text_Border_Password)
            {
                Picture_Input_Password.Image = Image_Other.Text_Border_Password;
                Default_Picture_Information_Window();
            }
        }

        private void Graybutton_click_MouseDown(object sender, EventArgs e)
        {
            if (Button_Cancel.BackgroundImage != Image_Button.Grey_Click)
            {
                Button_Cancel.BackgroundImage = Image_Button.Grey_Click;
            }
        }

        private void Graybutton_hover_MouseEnter(object sender, EventArgs e)
        {
            if (Button_Cancel.BackgroundImage != Image_Button.Grey_Hover)
            {
                Button_Cancel.BackgroundImage = Image_Button.Grey_Hover;
            }
        }

        private void Graybutton_MouseLeave(object sender, EventArgs e)
        {
            if (Button_Cancel.BackgroundImage != Image_Button.Grey)
            {
                Button_Cancel.BackgroundImage = Image_Button.Grey;
            }
        }

        private void Graybutton_hover_MouseUp(object sender, EventArgs e)
        {
            if (Button_Cancel.BackgroundImage != Image_Button.Grey_Hover)
            {
                Button_Cancel.BackgroundImage = Image_Button.Grey_Hover;
            }
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

            Icon = FormsIcon.Retrive_Icon();
            if (Parent_Screen.Screen_Instance != null)
            {
                Parent_Screen.Screen_Instance.Text = "Register - SBRW Launcher: " + Application.ProductVersion;
            }

            /*******************************/
            /* Set Initial position & Icon  /
            /*******************************/

            FunctionStatus.CenterParent(this);

            /*******************************/
            /* Set Font                     /
            /*******************************/
#if !(RELEASE_UNIX || DEBUG_UNIX)
            float MainFontSize = 9f * 96f / CreateGraphics().DpiY;
            float SecondaryFontSize = 8f * 96f / CreateGraphics().DpiY;
#else
            float MainFontSize = 9f;
            float SecondaryFontSize = 8f;
#endif
            Font = new Font(FormsFont.Primary(), SecondaryFontSize, FontStyle.Regular);

            /* Registering Panel */
            Input_Email.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Input_Password.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Input_Password_Confirm.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            Input_Ticket.Font = new Font(FormsFont.Primary(), MainFontSize, FontStyle.Regular);
            CheckBox_Rules_Agreement.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Register.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Button_Cancel.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);
            Label_Information_Window.Font = new Font(FormsFont.Primary_Bold(), MainFontSize, FontStyle.Bold);

            /********************************/
            /* Set Theme Colors & Images     /
            /********************************/

            /* Set Background with Transparent Key */
            BackgroundImage = Image_Background.Registration;
            TransparencyKey = Color_Screen.BG_Registration;

            Label_Information_Window.ForeColor = Color_Text.L_Five;
            Picture_Information_Window.Image = Image_Other.Information_Window;

            Input_Email.BackColor = Color_Winform_Other.Input;
            Input_Email.ForeColor = Color_Text.L_Five;
            Picture_Input_Email.Image = Image_Other.Text_Border_Email;

            Picture_Input_Password.Image = Image_Other.Text_Border_Password;
            Input_Password.BackColor = Color_Winform_Other.Input;
            Input_Password.ForeColor = Color_Text.L_Five;

            Picture_Input_Password_Confirm.Image = Image_Other.Text_Border_Password;
            Input_Password_Confirm.BackColor = Color_Winform_Other.Input;
            Input_Password_Confirm.ForeColor = Color_Text.L_Five;

            Picture_Input_Ticket.Image = Image_Other.Text_Border_Ticket;
            Input_Ticket.BackColor = Color_Winform_Other.Input;
            Input_Ticket.ForeColor = Color_Text.L_Five;

            CheckBox_Rules_Agreement.ForeColor = Color_Winform.Warning_Text_Fore_Color;

            Button_Register.BackgroundImage = Image_Button.Green;
            Button_Register.ForeColor = Color_Text.L_Seven;

            Button_Cancel.BackgroundImage = Image_Button.Grey;
            Button_Cancel.ForeColor = Color_Text.L_Five;

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

            if (Screen_Main.Screen_Instance != null)
            {
                Screen_Main.Screen_Instance.Button_Custom_Server.Enabled = Screen_Main.Screen_Instance.ComboBox_Server_List.Enabled = false;
            }
        }

        public Screen_Register()
        {
            InitializeComponent();
            SetVisuals();
            Presence_Launcher.Status(21, ServerListUpdater.ServerName("Register"));
            this.Closing += (x, y) =>
            {
                Presence_Launcher.Status(4);
                if (Screen_Main.Screen_Instance != null)
                {
                    Screen_Main.Clear_Hide_Screen_Form_Panel(true);
                    Screen_Main.Screen_Instance.Button_Custom_Server.Enabled = Screen_Main.Screen_Instance.ComboBox_Server_List.Enabled = true;
                }
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            };
        }
    }
}
