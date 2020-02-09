using GameLauncher.App.Classes.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Classes {
    class Statuses {
        public static String   Token  = String.Empty;

        public static async Task getToken() {
            String json = "{\"LoginToken\": \"" + Tokens.LoginToken + "\", \"HWID\": \"" + Security.FingerPrint.Value() + "\"}";
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            var response = await client.PostAsync("http://launcher.worldunited.gg/api/1.0/getToken", data);
            string result = response.Content.ReadAsStringAsync().Result;
            Token = result;
        }
    }
}
