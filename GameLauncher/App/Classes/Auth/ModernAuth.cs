using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;

namespace GameLauncher.App.Classes.Auth
{
    public class ModernAuthObject {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Warning { get; set; }
        public string Error { get; set; }
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
                ModernAuthObject LoginObjectResponse;

                try {
                    LoginObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(serverLoginResponse);
                } catch {
                    LoginObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(_serverErrormsg);
                }

                if (String.IsNullOrEmpty(LoginObjectResponse.Error)) { 
                    Tokens.UserId = LoginObjectResponse.UserId;
                    Tokens.LoginToken = LoginObjectResponse.Token;

                    if (!String.IsNullOrEmpty(LoginObjectResponse.Warning)) {
                        Tokens.Warning = LoginObjectResponse.Warning;
                    }
                } else {
                    Tokens.Error = LoginObjectResponse.Error;
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
                ModernAuthObject RegisterObjectResponse;

                try {
                    RegisterObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(serverLoginResponse);
                } catch {
                    RegisterObjectResponse = JsonConvert.DeserializeObject<ModernAuthObject>(_serverErrormsg);
                }

                if (String.IsNullOrEmpty(RegisterObjectResponse.Error) || RegisterObjectResponse.Error == "SERVER FULL") {
                    Tokens.UserId = RegisterObjectResponse.UserId;
                    Tokens.LoginToken = RegisterObjectResponse.Token;

                    if (RegisterObjectResponse.Error == "SERVER FULL") {
                        Tokens.Success = string.Format("Successfully registered on {0}. However, server is actually full, therefore you cannot play it right now.", Tokens.ServerName);
                    } else {
                        Tokens.Success = string.Format("Successfully registered on {0}. You can log in now.", Tokens.ServerName);
                    }
                } else {
                    Tokens.Error = RegisterObjectResponse.Error;
                }
            }
        }
    }
}
