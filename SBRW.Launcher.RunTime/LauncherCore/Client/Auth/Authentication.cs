using SBRW.Launcher.RunTime.Auth;
using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Nancy.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Net.Cache;

namespace SBRW.Launcher.RunTime.LauncherCore.Client.Auth
{
    class Authentication
    {
        private static int ServerErrorCode { get; set; }
        private static string LoginResponse { get; set; } = string.Empty;

        private static string ServerErrorResponse { get; set; } = string.Empty;
        private static HttpWebResponse? ServerResponse { get; set; }

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
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        Headers = Custom_Header.Headers_WHC(),
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                    { 
                        Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                    }
                    else
                    {
                        Client.Headers.Add(HttpRequestHeader.UserAgent, Custom_Header.Primary);
                    }

                    try
                    {
                        LoginResponse = Client.DownloadString(URLCall);
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
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(SendRequest).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;

                    HttpWebRequest? httpWebRequest = WebRequest.Create(SendRequest) as HttpWebRequest;
#pragma warning restore SYSLIB0014 // Type or member is obsolete
                    if (httpWebRequest != null)
                    {
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                        httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                        using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string JSON;

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
                        using var sr = new StreamReader(ServerResponse.GetResponseStream(), Encoding.UTF8, false);
                        ServerErrorCode = (int)ServerResponse.StatusCode;
                        LoginResponse = sr.ReadToEnd();
                    }
                }
            }
            catch (WebException Error)
            {
                LogToFileAddons.OpenLog("CLIENT [LOGIN/REGISTER]", string.Empty, Error, string.Empty, true);

                ServerResponse = Error.Response as HttpWebResponse;

                if (ServerResponse == null)
                {
                    ServerErrorCode = 500;
                    LoginResponse = Modern_Auth ? "{\"error\":\"Failed to get reply from server. Please retry.\"}" :
                    "<LoginStatusVO><UserId>0</UserId><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                }
                else
                {
                    using var sr = new StreamReader(ServerResponse.GetResponseStream(), Encoding.UTF8, false);
                    ServerErrorCode = (int)ServerResponse.StatusCode;
                    ServerErrorResponse = Modern_Auth ? "{\"error\":\"" + ServerResponse.StatusDescription + "\"}" : string.Empty;
                    LoginResponse = sr.ReadToEnd();
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
                    string msgBoxInfo = string.Empty;
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
                        LogToFileAddons.OpenLog("XML LOGIN", String.Empty, Error, String.Empty, true);

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

                                    if (XMLServerCore.NodeReader(sbrwXml, "NodeOnly", "LoginStatusVO/Warning", string.Empty) == "VAILD NODE - LAUNCHER")
                                    {
                                        Tokens.Warning = XMLServerCore.NodeReader(sbrwXml, "InnerText", "LoginStatusVO/Warning", "InnerText");
                                    }
                                }
                                else if (Method == "Register")
                                {
                                    string MessageSuccess;
                                    string MessageServerWelcome = string.Empty;

                                    if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Message))
                                    {
                                        if (Launcher_Value.Launcher_Select_Server_JSON.Server_Message.ToLower().Contains("welcome"))
                                        {
                                            MessageServerWelcome = Launcher_Value.Launcher_Select_Server_JSON.Server_Message + "\n";
                                        }
                                        else
                                        {
                                            MessageServerWelcome = "Welcome: " + Launcher_Value.Launcher_Select_Server_JSON.Server_Message + "\n";
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
                if (string.IsNullOrWhiteSpace(LoginResponse))
                {
                    Tokens.Error = "Did not Receive a Response from the Server";
                }
                else
                {
                    ModernAuthObject? ServerObjectResponse = null;
                    string ObjectResponse_Error = string.Empty;

                    try
                    {
                        try
                        {
                            ServerObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(LoginResponse);
                        }
                        catch
                        {
                            ServerObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(ServerErrorResponse);
                        }
                    }
                    catch (Exception Error)
                    {
                        if (Error != null)
                        {
                            ObjectResponse_Error = Error.Message;
                        }
                        else
                        {
                            ObjectResponse_Error = "Unknown Error Ecnountered";
                        }
                    }

                    if (ServerObjectResponse != null)
                    {
                        if (string.IsNullOrWhiteSpace(ServerObjectResponse.Error) || ServerObjectResponse.Error == "SERVER FULL")
                        {
                            if (Method == "Login")
                            {
                                Tokens.UserId = ServerObjectResponse.UserId;
                                Tokens.LoginToken = ServerObjectResponse.Token;

                                if (!string.IsNullOrWhiteSpace(ServerObjectResponse.Warning))
                                {
                                    Tokens.Warning = ServerObjectResponse.Warning;
                                }
                            }
                            else if (Method == "Register")
                            {
                                string MessageSuccess;
                                string MessageServerWelcome = string.Empty;

                                if (!string.IsNullOrWhiteSpace(Launcher_Value.Launcher_Select_Server_JSON.Server_Message))
                                {
                                    if (Launcher_Value.Launcher_Select_Server_JSON.Server_Message.ToLower().Contains("welcome"))
                                    {
                                        MessageServerWelcome = Launcher_Value.Launcher_Select_Server_JSON.Server_Message + "\n";
                                    }
                                    else
                                    {
                                        MessageServerWelcome = "Welcome: " + Launcher_Value.Launcher_Select_Server_JSON.Server_Message + "\n";
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
                    else
                    {
                        Tokens.Error = ObjectResponse_Error;
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
                return Launcher_Value.Launcher_Select_Server_JSON.Server_Authentication_Post ? AuthHash.H10 : AuthHash.H12;
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    if (string.IsNullOrWhiteSpace(LocationData.SelectSingleNode(FullNodePath) != null ?
                        LocationData.SelectSingleNode(FullNodePath).InnerText : string.Empty))
                    {
                        if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                        {
                            Log.Info("XMLSERVERCORE: EMPTY VALUE - LAUNCHER");
                        }
                        return "EMPTY VALUE - LAUNCHER";
                    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    return LocationData.SelectSingleNode(FullNodePath).InnerText;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
        public string Token { get; set; } = string.Empty;
        ///<value>Gets a UserID. Used to tell the Server which Account to use</value>
        public string UserId { get; set; } = string.Empty;
        ///<value>Gets a Warning Code. Used to inform the user about an issue, but can still login in</value>
        public string Warning { get; set; } = string.Empty;
        ///<value>Gets a Error Code. Used to inform the user about an issue and can not proceed to login into the server</value>
        public string Error { get; set; } = string.Empty;
    }
}