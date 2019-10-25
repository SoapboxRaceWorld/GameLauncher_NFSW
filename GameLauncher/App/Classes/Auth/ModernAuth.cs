using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Classes.Auth {
    public class modernAuthObject {
        public string token { get; set; }
        public string userId { get; set; }
        public string warning { get; set; }
        public string error { get; set; }
    }

    class ModernAuth {
        private static int _serverErrorcode;
        private static string _serverErrormsg;
        private static string serverLoginResponse;
        private static HttpWebResponse httpResponse;

        public static void Login(String email, String password) {
            try { 
                var buildUrl = Tokens.IPAddress + "/User/modernAuth";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(buildUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { 
                    String json = new JavaScriptSerializer().Serialize(new {
                        email = email,
                        password = password,
                        upgrade = true
                    });

                    streamWriter.Write(json);
                }

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpResponse.GetResponseStream()))  {
                    _serverErrorcode = (int)httpResponse.StatusCode;
                    serverLoginResponse = sr.ReadToEnd();
                }
            } catch (WebException ex) {
                httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse == null) {
                    _serverErrorcode = 500;
                    serverLoginResponse = "{\"error\":\"Failed to get reply from server. Please retry.\"}";
                } else {
                    using (var sr = new StreamReader(httpResponse.GetResponseStream())) {
                        _serverErrorcode = (int)httpResponse.StatusCode;
                        _serverErrormsg = "{\"error\":\""+httpResponse.StatusDescription+"\"}";

                        serverLoginResponse = sr.ReadToEnd();
                    }
                }
            }

            if(String.IsNullOrEmpty(serverLoginResponse)) {
                Tokens.Error = "Server seems to be offline.";
            } else {
                modernAuthObject LoginObjectResponse;

                try {
                    LoginObjectResponse = JsonConvert.DeserializeObject<modernAuthObject>(serverLoginResponse);
                } catch {
                    LoginObjectResponse = JsonConvert.DeserializeObject<modernAuthObject>(_serverErrormsg);
                }

                if (String.IsNullOrEmpty(LoginObjectResponse.error)) { 
                    Tokens.UserId = LoginObjectResponse.userId;
                    Tokens.LoginToken = LoginObjectResponse.token;

                    if (!String.IsNullOrEmpty(LoginObjectResponse.warning)) {
                        Tokens.Warning = LoginObjectResponse.warning;
                    }
                } else {
                    Tokens.Error = LoginObjectResponse.error;
                }
            }
        }

        public static void Register(String email, String password, String token = null) {
            try {
                var buildUrl = Tokens.IPAddress + "/User/modernRegister";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(buildUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                    String json = new JavaScriptSerializer().Serialize(new { email = email, password = password, ticket = token });
                    streamWriter.Write(json);
                }

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpResponse.GetResponseStream())) {
                    _serverErrorcode = (int)httpResponse.StatusCode;
                    serverLoginResponse = sr.ReadToEnd();
                }
            } catch (WebException ex) {
                httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse == null) {
                    _serverErrorcode = 500;
                    serverLoginResponse = "{\"error\":\"Failed to get reply from server. Please retry.\"}";
                } else {
                    using (var sr = new StreamReader(httpResponse.GetResponseStream())) {
                        _serverErrorcode = (int)httpResponse.StatusCode;
                        _serverErrormsg = "{\"error\":\"" + httpResponse.StatusDescription + "\"}";

                        serverLoginResponse = sr.ReadToEnd();
                    }
                }
            }

            if (String.IsNullOrEmpty(serverLoginResponse)) {
                Tokens.Error = "Server seems to be offline.";
            } else {
                modernAuthObject RegisterObjectResponse;

                try {
                    RegisterObjectResponse = JsonConvert.DeserializeObject<modernAuthObject>(serverLoginResponse);
                } catch {
                    RegisterObjectResponse = JsonConvert.DeserializeObject<modernAuthObject>(_serverErrormsg);
                }

                if (String.IsNullOrEmpty(RegisterObjectResponse.error) || RegisterObjectResponse.error == "SERVER FULL") {
                    Tokens.UserId = RegisterObjectResponse.userId;
                    Tokens.LoginToken = RegisterObjectResponse.token;

                    if (RegisterObjectResponse.error == "SERVER FULL") {
                        Tokens.Success = string.Format("Successfully registered on {0}. However, server is actually full, therefore you cannot play it right now.", Tokens.ServerName);
                    } else {
                        Tokens.Success = string.Format("Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                    }
                } else {
                    Tokens.Error = RegisterObjectResponse.error;
                }
            }
        }
    }
}
