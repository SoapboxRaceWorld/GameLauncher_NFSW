using GameLauncher.App.Classes.Auth;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Logger;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
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
        /// <param name="Connection">Connection Type: "Non Secure" or "Secure"</param>
        /// <param name="Method">Form Type: "Login" or "Register"</param>
        public static void Client(string Method, string Connection, String Email, String Password, String Token)
        {
            try
            {
                if (Connection == "Non Secure")
                {
                    WebClientWithTimeout wc = new WebClientWithTimeout();

                    if (Method == "Login")
                    {
                        LoginResponse = wc.DownloadString(Tokens.IPAddress + "/User/authenticateUser?email=" + Email + "&password=" + Password);
                    }
                    else
                    {
                        LoginResponse = wc.DownloadString(Tokens.IPAddress + "/User/createUser?email=" + Email + 
                            "&password=" + Password + (!String.IsNullOrWhiteSpace(Token) ? "&inviteTicket=" + Token : ""));
                    }
                }
                else if (Connection == "Secure")
                {
                    FunctionStatus.TLS();
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
                else
                {
                    Log.Error("Authentication: [Login] Can not Determine Function with Specified Connection -> " + Connection);
                }
            }
            catch (WebException Error)
            {
                Log.Error("CLIENT (LOGIN/REGISTER): " + Error.Message);
                if (!string.IsNullOrWhiteSpace(Error.InnerException.Message))
                {
                    Log.ErrorInner("CLIENT (LOGIN/REGISTER): " + Error.InnerException.Message);
                }

                if (Connection == "Non Secure" || Connection == "Secure")
                {
                    ServerResponse = (HttpWebResponse)Error.Response;

                    if (ServerResponse == null)
                    {
                        ServerErrorCode = 500;
                        LoginResponse = (Connection == "Secure") ? "{\"error\":\"Failed to get reply from server. Please retry.\"}" :
                        "<LoginStatusVO><UserId>0</UserId><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                    }
                    else
                    {
                        using (var sr = new StreamReader(ServerResponse.GetResponseStream()))
                        {
                            ServerErrorCode = (int)ServerResponse.StatusCode;
                            ServerErrorResponse = (Connection == "Secure") ? "{\"error\":\"" + ServerResponse.StatusDescription + "\"}" : null;
                            LoginResponse = sr.ReadToEnd();
                        }
                    }
                }
                else
                {
                    Log.Error("Authentication: [WebException] Can not Determine Function with Specified Type -> " + Connection);
                }
            }

            if (Connection == "Non Secure")
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
                        sbrwXml.LoadXml(LoginResponse);

                        if (EnableInsiderDeveloper.Allowed())
                        {
                            Log.Info("Authentication: Loaded XML -> " + sbrwXml.OuterXml);
                        }
                    }
                    catch (Exception Error)
                    {
                        Log.Error("XML LOGIN ERROR: " + Error.Message);
                        if (!string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            Log.ErrorInner("XML LOGIN ERROR: " + Error.InnerException.Message);
                        }

                        XMLIsErrorFree = false;
                        msgBoxInfo = "An error occured: " + Error.Message;
                    }

                    if (XMLIsErrorFree == true)
                    {
                        if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO", "NodeOnly") == "VAILD NODE - LAUNCHER")
                        {
                            if (XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Ban", "InnerText") != "INVALID VALUE - LAUNCHER" &&
                                XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Ban", "InnerText") != "ERROR VALUE - LAUNCHER")
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
                            Log.Error("Authentication: Unable to Read XML File Due to the Node 'LoginStatusVO' had an Error");
                            Tokens.Error = "Unable to Read XML File Due to the Node 'LoginStatusVO' had an Error\nERROR NODE - LAUNCHER";
                        }
                    }
                    else
                    {
                        Log.Error("Authentication: Unable to Read XML File -> " + msgBoxInfo);
                        Tokens.Error = msgBoxInfo;
                    }
                }
            }
            else if (Connection == "Secure")
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
            else
            {
                Log.Error("Authentication: [Tokens] Can not Determine Function with Specified Type -> " + Connection);
            }
        }

        /// <summary>
        /// Hash Method (Used how to Authenticate Logins)
        /// </summary>
        /// <returns>A hash type standard that is used on the server</returns>
        public static AuthHash HashType(string Type)
        {
            if (!string.IsNullOrWhiteSpace(Type))
            {
                if (Type == "1.0" || Type == "true")
                {
                    /* Raw Email and Password */
                    return AuthHash.H10;
                }
                else if (Type == "1.1" || Type == "false")
                {
                    /* Raw Email and Hashed Password (Default Standard) */
                    return AuthHash.H11;
                }
                else if (Type == "1.2")
                {
                    /* Hashed Email and Password in SHA */
                    return AuthHash.H12;
                }
                else if (Type == "1.3")
                {
                    /* Hashed Email and Password in 256 */
                    return AuthHash.H13;
                }
                else
                {
                    return AuthHash.Unknown;
                }
            }
            else
            {
                /* Raw Email and Hashed Password (Default Standard) */
                return AuthHash.H11;
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
                    if (LocationData.SelectSingleNode(FullNodePath).InnerText == null)
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
                    if (LocationData.SelectSingleNode(FullNodePath) == null)
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
                    if (!string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        Log.ErrorInner("XMLSERVERCORE: " + Error.InnerException.Message);
                    }
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