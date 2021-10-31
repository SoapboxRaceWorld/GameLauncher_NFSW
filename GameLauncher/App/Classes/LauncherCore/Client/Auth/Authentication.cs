using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.SystemPlatform.Components;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher.App.Classes.LauncherCore.Client.Auth
{
    class Authentication
    {
        private static int ServerErrorCode;
        private static String LoginResponse;

        private static string ServerErrorResponse;
        private static HttpWebResponse ServerResponse;

        /// <summary>
        /// Form Url or Post Request to the Server for Login and Registration
        /// </summary>
        /// <remarks>Non Secure: Uses regualar URL Request. Secure: Uses Post Request</remarks>
        /// <returns>Receives UserId and Auth Key for Login. Sends Email and Password to Server</returns>
        /// <param name="ConnectionProtocol">Connection Protocol: Check AuthProtocol</param>
        /// <param name="Method">Form Type: "Login" or "Register"</param>
        public static void Client(string Method, bool Modern_Auth, String Email, String Password, String Token)
        {
            try
            {
                if (!Modern_Auth)
                {
                    Uri URLCall =
                        new Uri((Method == "Login") ? Tokens.IPAddress + "/User/authenticateUser?email=" + Email + "&password=" + Password :
                        Tokens.IPAddress + "/User/createUser?email=" + Email + "&password=" + Password +
                        (!String.IsNullOrWhiteSpace(Token) ? "&inviteTicket=" + Token : ""));
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
                        Client.Headers["X-HWID"] = HardwareID.FingerPrint.Value();
                        Client.Headers["X-HiddenHWID"] = HardwareID.FingerPrint.ValueAlt();
                        Client.Headers["X-UserAgent"] = "GameLauncherReborn " +
                            Application.ProductVersion + " WinForms (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                        Client.Headers["X-GameLauncherHash"] = WebHelpers.Value();
                        Client.Headers["X-GameLauncherCertificate"] = CertificateStore.LauncherSerial;
                        Client.Headers["X-DiscordID"] = DiscordLauncherPresence.UserID;
                    }

                    try
                    {
                        if (Method == "Login")
                        {
                            LoginResponse = Client.DownloadString(URLCall);
                        }
                        else
                        {
                            LoginResponse = Client.DownloadString(URLCall);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (Client != null)
                        {
                            Client.Dispose();
                        }
                    }
                }
                else
                {
                    string ServerUrl = Tokens.IPAddress + "/User/modernAuth";
                    if (Method == "Register")
                    {
                        ServerUrl = Tokens.IPAddress + "/User/modernRegister";
                    }

                    Uri SendRequest = new Uri(ServerUrl);
                    ServicePointManager.FindServicePoint(SendRequest).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(SendRequest);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                    using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        String JSON;

                        if (Method == "Login")
                        {
                            JSON = new JavaScriptSerializer().Serialize(new { Email, Password, upgrade = true });
                        }
                        else
                        {
                            JSON = new JavaScriptSerializer().Serialize(new { Email, Password, Ticket = Token });
                        }

                        streamWriter.Write(JSON);
                    }

                    ServerResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var sr = new StreamReader(ServerResponse.GetResponseStream()))
                    {
                        ServerErrorCode = (int)ServerResponse.StatusCode;
                        LoginResponse = sr.ReadToEnd();
                    }
                }
            }
            catch (WebException Error)
            {
                LogToFileAddons.OpenLog("CLIENT [LOGIN/REGISTER]", null, Error, null, true);

                ServerResponse = (HttpWebResponse)Error.Response;

                if (ServerResponse == null)
                {
                    ServerErrorCode = 500;
                    LoginResponse = Modern_Auth ? "{\"error\":\"Failed to get reply from server. Please retry.\"}" :
                    "<LoginStatusVO><UserId>0</UserId><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                }
                else
                {
                    using (var sr = new StreamReader(ServerResponse.GetResponseStream()))
                    {
                        ServerErrorCode = (int)ServerResponse.StatusCode;
                        ServerErrorResponse = Modern_Auth ? "{\"error\":\"" + ServerResponse.StatusDescription + "\"}" : null;
                        LoginResponse = sr.ReadToEnd();
                    }
                }
            }

            if (!Modern_Auth)
            {
                if (string.IsNullOrWhiteSpace(LoginResponse))
                {
                    Tokens.Error = "Server Seems to be Offline.";
                }
                else
                {
                    XmlDocument sbrwXml = new XmlDocument();
                    var msgBoxInfo = string.Empty;
                    bool XMLIsErrorFree = true;

                    try
                    {
                        if (EnableInsiderDeveloper.Allowed())
                        {
                            Log.Info("Authentication: Received XML -> " + LoginResponse);
                        }

                        sbrwXml.LoadXml(LoginResponse);

                        if (EnableInsiderDeveloper.Allowed())
                        {
                            Log.Info("Authentication: Loaded XML -> " + sbrwXml.OuterXml);
                        }
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("XML LOGIN", null, Error, null, true);

                        XMLIsErrorFree = false;
                        msgBoxInfo = "An error occured: " + Error.Message;
                    }

                    if (XMLIsErrorFree == true)
                    {
                        if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO", "NodeOnly") == "VAILD NODE - LAUNCHER")
                        {
                            if (XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Ban", "InnerText") != "EMPTY VALUE - LAUNCHER")
                            {
                                if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Description", "NodeOnly") == "VAILD NODE - LAUNCHER")
                                {
                                    msgBoxInfo += XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Description", "InnerText");
                                }
                                else
                                {
                                    msgBoxInfo = string.Format("You got banned on {0}.", Tokens.ServerName) + "\n";

                                    if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Ban/Reason", "NodeOnly") != "INVALID NODE - LAUNCHER")
                                    {
                                        msgBoxInfo += "Reason: " + XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Ban/Reason", "InnerText") + "\n";
                                    }
                                    else
                                    {
                                        msgBoxInfo += "Reason: Unknown \n";
                                    }

                                    if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Ban/Expires", "NodeOnly") != "INVALID NODE - LAUNCHER")
                                    {
                                        msgBoxInfo += "Ban Expires: " + XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Ban/Expires", "InnerText");
                                    }
                                    else
                                    {
                                        msgBoxInfo += "Banned Forever";
                                    }
                                }
                            }
                            else if (XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/UserId", "InnerText") == "0")
                            {
                                if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Description", "NodeOnly") != "ERROR NODE - LAUNCHER" &&
                                    XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Description", "InnerText") != "EMPTY VALUE - LAUNCHER")
                                {
                                    if (XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Description", "InnerText") == "LOGIN ERROR")
                                    {
                                        msgBoxInfo += "Invalid E-mail or Password";
                                    }
                                    else
                                    {
                                        msgBoxInfo += XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Description", "InnerText");
                                    }
                                }
                                else
                                {
                                    msgBoxInfo += "ERROR " + ServerErrorCode + ": " + XMLServerCore.NodeReader(sbrwXml, "InnerText", "html/body", "InnerText");
                                }
                            }

                            if (string.IsNullOrWhiteSpace(msgBoxInfo) || msgBoxInfo == "SERVER FULL")
                            {
                                if (Method == "Login" && string.IsNullOrWhiteSpace(msgBoxInfo))
                                {
                                    Tokens.UserId = XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/UserId", "InnerText");
                                    Tokens.LoginToken = XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/LoginToken", "InnerText");

                                    if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Warning", null) == "VAILD NODE - LAUNCHER")
                                    {
                                        Tokens.Warning = XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Warning", "InnerText");
                                    }
                                }
                                else if (Method == "Register")
                                {
                                    string MessageSuccess;
                                    string MessageServerWelcome = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.messageSrv))
                                    {
                                        if (InformationCache.SelectedServerJSON.messageSrv.ToLower().Contains("welcome"))
                                        {
                                            MessageServerWelcome = InformationCache.SelectedServerJSON.messageSrv + "\n";
                                        }
                                        else
                                        {
                                            MessageServerWelcome = "Welcome: " + InformationCache.SelectedServerJSON.messageSrv + "\n";
                                        }
                                    }

                                    if (msgBoxInfo == "SERVER FULL")
                                    {
                                        MessageSuccess = string.Format(MessageServerWelcome + "Successfully registered on {0}. However, server is actually full, " +
                                            "therefore you cannot play it right now.", Tokens.ServerName);
                                    }
                                    else
                                    {
                                        MessageSuccess = string.Format(MessageServerWelcome + "Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                                    }

                                    Tokens.Success = MessageSuccess;
                                }
                                else
                                {
                                    Tokens.Error = msgBoxInfo;
                                }
                            }
                            else
                            {
                                Tokens.Error = msgBoxInfo;
                            }
                        }
                        else
                        {
                            Log.Error("Authentication: Unable to Read XML File Due to the Node 'LoginStatusVO' having an Error");
                            Tokens.Error = "Unable to Read XML File Due to the Node 'LoginStatusVO' having an Error\nERROR NODE - LAUNCHER";
                        }
                    }
                    else
                    {
                        Log.Error("Authentication: Unable to Read XML File -> " + msgBoxInfo);
                        Tokens.Error = msgBoxInfo;
                    }
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(LoginResponse))
                {
                    Tokens.Error = "Server seems to be offline.";
                }
                else
                {
                    ModernAuthObject ServerObjectResponse;

                    try
                    {
                        ServerObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(LoginResponse);
                    }
                    catch
                    {
                        ServerObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(ServerErrorResponse);
                    }

                    if (String.IsNullOrWhiteSpace(ServerObjectResponse.Error) || ServerObjectResponse.Error == "SERVER FULL")
                    {
                        if (Method == "Login")
                        {
                            Tokens.UserId = ServerObjectResponse.UserId;
                            Tokens.LoginToken = ServerObjectResponse.Token;

                            if (!String.IsNullOrWhiteSpace(ServerObjectResponse.Warning))
                            {
                                Tokens.Warning = ServerObjectResponse.Warning;
                            }
                        }
                        else if (Method == "Register")
                        {
                            string MessageSuccess;
                            string MessageServerWelcome = string.Empty;

                            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.messageSrv))
                            {
                                if (InformationCache.SelectedServerJSON.messageSrv.ToLower().Contains("welcome"))
                                {
                                    MessageServerWelcome = InformationCache.SelectedServerJSON.messageSrv + "\n";
                                }
                                else
                                {
                                    MessageServerWelcome = "Welcome: " + InformationCache.SelectedServerJSON.messageSrv + "\n";
                                }
                            }

                            if (ServerObjectResponse.Error == "SERVER FULL")
                            {
                                MessageSuccess = string.Format(MessageServerWelcome + "Successfully registered on {0}. However, server is actually full, " +
                                    "therefore you cannot play it right now.", Tokens.ServerName);
                            }
                            else
                            {
                                MessageSuccess = string.Format(MessageServerWelcome + "Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                            }

                            Tokens.Success = MessageSuccess;
                        }
                    }
                    else
                    {
                        Tokens.Error = ServerObjectResponse.Error;
                    }
                }
            }
        }

        /// <summary>
        /// Hash Method (Used how to Authenticate Logins)
        /// </summary>
        /// <returns>A hash type standard that is used on the server</returns>
        public static AuthHash HashType(string HType)
        {
            if (!string.IsNullOrWhiteSpace(HType))
            {
                switch (HType)
                {
                    case "1.0":
                    case "true":
                        return AuthHash.H10;
                    case "1.1":
                        return AuthHash.H11;
                    case "1.2":
                    case "false":
                        return AuthHash.H12;
                    case "1.3":
                        return AuthHash.H13;
                    case "2.0":
                        return AuthHash.H20;
                    case "2.1":
                        return AuthHash.H21;
                    case "2.2":
                        return AuthHash.H22;
                    default:
                        return AuthHash.Unknown;
                }
            }
            else
            {
                return InformationCache.SelectedServerJSON.modernAuthSupport ? AuthHash.H10 : AuthHash.H12;
            }
        }
    }

    class XMLServerCore
    {
        public static string NodeReader(XmlDocument LocationData, string Type, string FullNodePath, string AttributeName)
        {
            try
            {
                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Log.Info("XMLSERVERCORE: Attmempting to Read XML [NodePath: '" + FullNodePath + "' Attribute: '" + AttributeName + "']");
                }
                if (Type == "InnerText")
                {
                    if (string.IsNullOrWhiteSpace(LocationData.SelectSingleNode(FullNodePath) != null ?
                        LocationData.SelectSingleNode(FullNodePath).InnerText : string.Empty))
                    {
                        if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                        {
                            Log.Info("XMLSERVERCORE: EMPTY VALUE - LAUNCHER");
                        }
                        return "EMPTY VALUE - LAUNCHER";
                    }

                    return LocationData.SelectSingleNode(FullNodePath).InnerText;
                }
                else if (Type == "NodeOnly")
                {
                    if ((LocationData.SelectSingleNode(FullNodePath) ?? null) == null)
                    {
                        if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                        {
                            Log.Info("XMLSERVERCORE: INVALID NODE - LAUNCHER");
                        }
                        return "INVALID NODE - LAUNCHER";
                    }

                    return "VAILD NODE - LAUNCHER";
                }
                else
                {
                    return "UNKNOWN TYPE - LAUNCHER";
                }
            }
            catch (Exception Error)
            {
                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                {
                    Log.Error("XMLSERVERCORE: Unable to Read XML [NodePath: '" + FullNodePath + "' Attribute: '" + AttributeName + "']" + Error.Message);
                    Log.ErrorIC("XMLSERVERCORE: " + Error.HResult);
                    Log.ErrorFR("XMLSERVERCORE: " + Error.ToString());
                }
                if (Type == "InnerText")
                {
                    if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                    {
                        Log.Error("XMLSERVERCORE: ERROR VALUE - LAUNCHER");
                    }
                    return "ERROR VALUE - LAUNCHER";
                }
                else if (Type == "NodeOnly")
                {
                    if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                    {
                        Log.Error("XMLSERVERCORE: ERROR NODE - LAUNCHER");
                    }
                    return "ERROR NODE - LAUNCHER";
                }
                else
                {
                    return "ERROR UNKNOWN TYPE - LAUNCHER";
                }
            }
        }
    }

    /// <summary>
    /// Form JSON for Requests
    /// </summary>
    /// <returns>Used for ensuring certain Login or Register function checks</returns>
    public class ModernAuthObject
    {
        ///<value>Gets a Auth Token. Used to Login into the Server</value>
        public string Token { get; set; }
        ///<value>Gets a UserID. Used to tell the Server which Account to use</value>
        public string UserId { get; set; }
        ///<value>Gets a Warning Code. Used to inform the user about an issue, but can still login in</value>
        public string Warning { get; set; }
        ///<value>Gets a Error Code. Used to inform the user about an issue and can not proceed to login into the server</value>
        public string Error { get; set; }
    }
}