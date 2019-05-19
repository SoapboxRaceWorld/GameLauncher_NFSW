using GameLauncherReborn;
using System;
using System.IO;
using System.Net;
using System.Xml;

namespace GameLauncher.App.Classes.Auth
{
    class ClassicAuth {
        private static int _errorcode;
        private static String serverLoginResponse;

        public static void Login(String email, String password) {
            try {
                WebClientWithTimeout wc = new WebClientWithTimeout();
                var buildUrl = Tokens.IPAddress + "/User/authenticateUser?email=" + email + "&password=" + password;
                serverLoginResponse = wc.DownloadString(buildUrl);
            } catch (WebException ex) {
                var serverReply = (HttpWebResponse)ex.Response;

                if (serverReply == null) {
                    _errorcode = 500;
                    serverLoginResponse = "<LoginStatusVO><UserId/><LoginToken/><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                } else {
                    using (var sr = new StreamReader(serverReply.GetResponseStream())) {
                        _errorcode = (int)serverReply.StatusCode;
                        serverLoginResponse = sr.ReadToEnd();
                    }
                }
            }

            if (string.IsNullOrEmpty(serverLoginResponse)) {
                Tokens.Error = "Server seems to be offline.";
            } else {
                try {
                    var sbrwXml = new XmlDocument();
                    sbrwXml.LoadXml(serverLoginResponse);

                    XmlNode extraNode;
                    XmlNode loginTokenNode;
                    XmlNode userIdNode;
                    var msgBoxInfo = "";

                    loginTokenNode = sbrwXml.SelectSingleNode("LoginStatusVO/LoginToken");
                    userIdNode = sbrwXml.SelectSingleNode("LoginStatusVO/UserId");

                    if (sbrwXml.SelectSingleNode("LoginStatusVO/Ban") == null) {
                        if (sbrwXml.SelectSingleNode("LoginStatusVO/Description") == null) {
                            extraNode = sbrwXml.SelectSingleNode("html/body");
                        } else {
                            extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Description");
                        }
                    } else {
                        extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Ban");
                    }

                    if (!string.IsNullOrEmpty(extraNode.InnerText)) {
                        if (extraNode.SelectSingleNode("Expires") != null || extraNode.SelectSingleNode("Reason") != null) {
                            msgBoxInfo = string.Format("You got banned on {0}.", Tokens.ServerName) + "\n";

                            if(extraNode.SelectSingleNode("Reason") != null){
                                msgBoxInfo += string.Format("Reason: {0}", extraNode.SelectSingleNode("Reason").InnerText);
                            } else {
                                msgBoxInfo += "Reason: unknown";
                            }

                            if (extraNode.SelectSingleNode("Expires") != null) {
                                msgBoxInfo += "\n" + string.Format("Ban expires: {0}", extraNode.SelectSingleNode("Expires").InnerText);
                            } else {
                                msgBoxInfo += "\n" + "Banned forever.";
                            }
                        } else {
                            if (extraNode.InnerText == "Please use MeTonaTOR's launcher. Or, are you tampering?") {
                                msgBoxInfo = "Launcher tampering detected. Please use original build.";
                            } else {
                                if (sbrwXml.SelectSingleNode("html/body") == null) {
                                    if (extraNode.InnerText == "LOGIN ERROR") {
                                        msgBoxInfo = "Invalid e-mail or password.";
                                    } else if (string.IsNullOrEmpty(msgBoxInfo)) {
                                        msgBoxInfo = extraNode.InnerText;
                                    }
                                } else {
                                    msgBoxInfo = "ERROR " + _errorcode + ": " + extraNode.InnerText;
                                }
                            }
                        }
                        
                        Tokens.Error = msgBoxInfo;
                    } else {
                        Tokens.UserId = userIdNode.InnerText;
                        Tokens.LoginToken = loginTokenNode.InnerText;

                        if (sbrwXml.SelectSingleNode("LoginStatusVO/Warning") != null) {
                            Tokens.Warning = sbrwXml.SelectSingleNode("LoginStatusVO/Warning").InnerText;
                        }
                    }
                } catch (Exception ex) {
                    Tokens.Error = "An error occured: " + ex.Message;
                }
            }
        }

        public static void Register(String email, String password, String token = null) {
            try {
                WebClientWithTimeout wc = new WebClientWithTimeout();
                String buildUrl = Tokens.IPAddress + "/User/createUser?email=" + email + "&password=" + password + (!String.IsNullOrEmpty(token) ? "&inviteTicket=" + token : "");
                serverLoginResponse = wc.DownloadString(buildUrl);
            } catch (WebException ex) {
                var serverReply = (HttpWebResponse)ex.Response;
                if (serverReply == null) {
                    _errorcode = 500;
                    serverLoginResponse = "<LoginStatusVO><UserId/><LoginToken/><Description>Failed to get reply from server. Please retry.</Description></LoginStatusVO>";
                } else {
                    using (var sr = new StreamReader(serverReply.GetResponseStream())) {
                        _errorcode = (int)serverReply.StatusCode;
                        serverLoginResponse = sr.ReadToEnd();
                    }
                }
            }

            try {
                var sbrwXml = new XmlDocument();
                sbrwXml.LoadXml(serverLoginResponse);

                XmlNode extraNode;
                XmlNode loginTokenNode;
                XmlNode userIdNode;
                var msgBoxInfo = "";

                loginTokenNode = sbrwXml.SelectSingleNode("LoginStatusVO/LoginToken");
                userIdNode = sbrwXml.SelectSingleNode("LoginStatusVO/UserId");

                if (sbrwXml.SelectSingleNode("LoginStatusVO/Ban") == null) {
                    if (sbrwXml.SelectSingleNode("LoginStatusVO/Description") == null) {
                        extraNode = sbrwXml.SelectSingleNode("html/body");
                    } else {
                        extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Description");
                    }
                } else {
                    extraNode = sbrwXml.SelectSingleNode("LoginStatusVO/Ban");
                }

                if (string.IsNullOrEmpty(extraNode.InnerText) || extraNode.InnerText == "SERVER FULL") {
                    Tokens.UserId = userIdNode.InnerText;
                    Tokens.LoginToken = loginTokenNode.InnerText;

                    if (extraNode.InnerText == "SERVER FULL") {
                        Tokens.Success = string.Format("Successfully registered on {0}. However, server is actually full, therefore you cannot play it right now.", Tokens.ServerName);
                    } else {
                        Tokens.Success = string.Format("Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                    }
                } else {
                    if (extraNode.SelectSingleNode("Reason") != null) {
                        msgBoxInfo = string.Format("You got banned on {0}.", Tokens.ServerName) + "\n";
                        msgBoxInfo += string.Format("Reason: {0}", extraNode.SelectSingleNode("Reason").InnerText);

                        if (extraNode.SelectSingleNode("Expires") != null) {
                            msgBoxInfo += "\n" + string.Format("Ban expires {0}", extraNode.SelectSingleNode("Expires").InnerText);
                        } else {
                            msgBoxInfo += "\n" + "Banned forever.";
                        }
                    } else {
                        if (extraNode.InnerText == "Please use MeTonaTOR's launcher. Or, are you tampering?") {
                            msgBoxInfo = "Launcher tampering detected. Please use original build.";
                        } else {
                            if (sbrwXml.SelectSingleNode("html/body") == null) {
                                if (extraNode.InnerText == "LOGIN ERROR") {
                                    msgBoxInfo = "Invalid e-mail or password.";
                                } else {
                                    msgBoxInfo = extraNode.InnerText;
                                }
                            } else {
                                msgBoxInfo = "ERROR " + _errorcode + ": " + extraNode.InnerText;
                            }
                        }
                    }

                    Tokens.Error = msgBoxInfo;
                }
            } catch (Exception ex) {
                Tokens.Error = "An error occured: " + ex.Message;
            }
        }
    }
}
