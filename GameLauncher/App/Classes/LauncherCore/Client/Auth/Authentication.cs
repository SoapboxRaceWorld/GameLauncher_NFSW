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
                        LoginResponse = wc.DownloadString(Tokens.IPAddress + "/User/createUser?email=" + Email + "&password=" + Password + (!String.IsNullOrEmpty(Token) ? "&inviteTicket=" + Token : ""));
                    }
                }
                else if (Connection == "Secure")
                {
                    FunctionStatus.TLS();
                    var ServerUrl = Tokens.IPAddress + "/User/modernAuth";
                    if (Method == "Register")
                    {
                        ServerUrl = Tokens.IPAddress + "/User/modernRegister";
                    }
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(ServerUrl);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

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
                if (string.IsNullOrEmpty(LoginResponse))
                {
                    Tokens.Error = "Server Seems to be Offline.";
                }
                else
                {
                    XmlDocument sbrwXml = new XmlDocument();
                    var msgBoxInfo = "";
                    bool XMLIsErrorFree = true;

                    try
                    {
                        sbrwXml.LoadXml(LoginResponse);

                        if (EnableInsiderDeveloper.Allowed() == true)
                        {
                            Log.Info("Authentication: Loaded XML -> " + sbrwXml.OuterXml);
                        }
                    }
                    catch (Exception Error)
                    {
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

                            if (string.IsNullOrEmpty(msgBoxInfo) || msgBoxInfo == "SERVER FULL")
                            {
                                if (Method == "Login" && string.IsNullOrEmpty(msgBoxInfo))
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
                                    if (msgBoxInfo == "SERVER FULL")
                                    {
                                        Tokens.Success = string.Format("Successfully registered on {0}. However, the Server is currently full. Therefore you cannot play on it right now.", Tokens.ServerName);
                                    }
                                    else
                                    {
                                        Tokens.Success = string.Format("Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                                    }
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
                if (String.IsNullOrEmpty(LoginResponse))
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

                    if (String.IsNullOrEmpty(ServerObjectResponse.Error) || ServerObjectResponse.Error == "SERVER FULL")
                    {
                        Tokens.UserId = ServerObjectResponse.UserId;
                        Tokens.LoginToken = ServerObjectResponse.Token;

                        if (Method == "Login")
                        {
                            if (!String.IsNullOrEmpty(ServerObjectResponse.Warning))
                            {
                                Tokens.Warning = ServerObjectResponse.Warning;
                            }
                        }
                        else if (Method == "Register")
                        {
                            if (ServerObjectResponse.Error == "SERVER FULL")
                            {
                                Tokens.Success = string.Format("Successfully registered on {0}. However, server is actually full, therefore you cannot play it right now.", Tokens.ServerName);
                            }
                            else
                            {
                                Tokens.Success = string.Format("Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                            }
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
    }

    class XMLServerCore
    {
        public static string NodeReader(XmlDocument LocationData, string Type, string FullNodePath, string AttributeName)
        {
            try
            {
                if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
                {
                    Log.Info("XMLSERVERCORE: Attmempting to Read XML [NodePath: '" + FullNodePath + "' Attribute: '" + AttributeName + "']");
                }
                if (Type == "InnerText")
                {
                    if (LocationData.SelectSingleNode(FullNodePath).InnerText == null)
                    {
                        if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
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
                        if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
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
                if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
                {
                    Log.Error("XMLSERVERCORE: Unable to Read XML [NodePath: '" + FullNodePath + "' Attribute: '" + AttributeName + "']" + Error.Message);
                }
                if (Type == "InnerText")
                {
                    if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
                    {
                        Log.Error("XMLSERVERCORE: ERROR VALUE - LAUNCHER");
                    }
                    return "ERROR VALUE - LAUNCHER";
                }
                else if (Type == "NodeOnly")
                {
                    if (EnableInsiderDeveloper.Allowed() == true || EnableInsiderBetaTester.Allowed() == true)
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

    public class ModernAuthObject
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Warning { get; set; }
        public string Error { get; set; }
    }
}