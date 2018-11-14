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
    public class LoginObject {
        public string token { get; set; }
        public string userId { get; set; }
        public string warning { get; set; }
        public string error { get; set; }
    }

    class ModernAuth {
        private static int _errorcode;
        private static string serverLoginResponse;

        public static void Login(String email, String password) {
            String serverLoginResponse;
            HttpWebResponse httpResponse;

            try { 
                var buildUrl = Tokens.IPAddress + "/User/modernAuth";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(buildUrl);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using(StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) { 
                    String json = new JavaScriptSerializer().Serialize(new {
                        email = email,
                        password = password,
                        upgrade = false
                    });

                    streamWriter.Write(json);
                }

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var sr = new StreamReader(httpResponse.GetResponseStream()))  {
                    _errorcode = (int)httpResponse.StatusCode;
                    serverLoginResponse = sr.ReadToEnd();
                }
            } catch (WebException ex) {
                httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse == null) {
                    _errorcode = 500;
                    serverLoginResponse = "{\"error\":\"Failed to get reply from server. Please retry.\"}";
                } else {
                    using (var sr = new StreamReader(httpResponse.GetResponseStream())) {
                        _errorcode = (int)httpResponse.StatusCode;

                        serverLoginResponse = sr.ReadToEnd();
                        if (_errorcode == 500) {
                            serverLoginResponse = "{\"error\":\"Internal Server Error.\"}";
                        }
                    }
                }
            }

            if(String.IsNullOrEmpty(serverLoginResponse)) {
                Tokens.Error = "Server seems to be offline.";
            } else {
                var LoginObjectResponse = JsonConvert.DeserializeObject<LoginObject>(serverLoginResponse);

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
    }
}
