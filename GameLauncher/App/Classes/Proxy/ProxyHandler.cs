using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Flurl.Http;
using Flurl.Http.Content;
using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.RPC;
using GameLauncherReborn;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.Responses;

namespace GameLauncher.App.Classes.Proxy
{
    public class ProxyHandler : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines) {
            Console.WriteLine(pipelines);
            pipelines.BeforeRequest += ProxyRequest;
        }

        public static Dictionary<string, int> executedPowerupsRemainingSecs = new Dictionary<string, int>();
        public static Dictionary<string, bool> executedPowerups = new Dictionary<string, bool>();
        public static bool activated;

        private static Response ProxyRequest(NancyContext context) {
            string POSTContent = String.Empty;
            string GETContent = String.Empty;

            Self.sendRequest = true;
            
            Dictionary<string, string> powerups = new Dictionary<string, string>();
            powerups.Add("-1681514783", "NITROUS");
            powerups.Add("-537557654", "RUN FLATS");
            powerups.Add("-1692359144", "INSTANT COOLDOWN");
            powerups.Add("-364944936", "SHIELD");
            powerups.Add("2236629", "SLINGSHOT");
            powerups.Add("957701799", "READY");
            powerups.Add("1805681994", "JUGGERNAUT");
            powerups.Add("-611661916", "EMERGENCY EVADE");
            powerups.Add("-1564932069", "TEAM EMERGENCY EVADE");
            powerups.Add("1627606782", "ONE MORE LAP");
            powerups.Add("1113720384", "TEAM SLINGSHOT");
            powerups.Add("125509666", "TRAFFIC MAGNET");

            /*if (Regex.Match(context.Request.Path, "/powerups/activated/", RegexOptions.IgnoreCase).Success) {
                String activatedHash = context.Request.Path.Split('/').Last();

                try { 
                    Console.WriteLine("--- CHECK ACTIVATED POWERUPS ---");
                    foreach (KeyValuePair<string, bool> entry in executedPowerups) {
                        Console.WriteLine(powerups[entry.Key] + ": " + entry.Value);
                    }
                } catch {
                    Console.WriteLine("No activated powerups were found!");
                }

                executedPowerups.TryGetValue(activatedHash, out activated);

                if (activated) {
                    var notification = new NotifyIcon() {
                        Visible = true,
                        Icon = System.Drawing.SystemIcons.Information,
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipTitle = "Powerup Spam Detected",
                        BalloonTipText = "Hey! You can't use " + powerups[activatedHash] + " right now. Therefore, this powerup has been disabled till next event/freeroam session.",
                    };

                    notification.ShowBalloonTip(5000);
                    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainScreen));
                    notification.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                    notification.Dispose();

                    Self.sendRequest = false;
                } else {
                    executedPowerupsRemainingSecs[activatedHash] = 14;

                    System.Timers.Timer poweruptimer = new System.Timers.Timer();
                    poweruptimer.Elapsed += (x, y) => {
                        if(activatedHash == "957701799") {
                            foreach(KeyValuePair<string, bool> allpowerups in executedPowerups) {
                                executedPowerups[allpowerups.Key] = false;
                                executedPowerupsRemainingSecs[allpowerups.Key] = 0;

                                executedPowerups.Remove(allpowerups.Key);
                                executedPowerupsRemainingSecs.Remove(allpowerups.Key);
                            }

                            executedPowerups[activatedHash] = false;
                            executedPowerupsRemainingSecs[activatedHash] = 0;

                            poweruptimer.Close();
                        } else { 
                            if (executedPowerupsRemainingSecs[activatedHash] == 0) {
                                executedPowerups[activatedHash] = false;
                                executedPowerupsRemainingSecs[activatedHash] = 0;
                                poweruptimer.Close();
                            } else {
                                executedPowerups[activatedHash] = true;
                                executedPowerupsRemainingSecs[activatedHash] -= 1;
                            }
                        }
                    };

                    poweruptimer.Interval = 1000;
                    poweruptimer.Enabled = true;
                }
            }*/

            var serverUrl = ServerProxy.Instance.GetServerUrl();

            if (string.IsNullOrEmpty(serverUrl)) {
                return new TextResponse(HttpStatusCode.BadGateway, "Not open for business");
            }

            var queryParams = new Dictionary<string, object>();
            var headers = new Dictionary<string, object>();

            if (Self.sendRequest == true) {
                var fixedPath = context.Request.Path.Replace("/nfsw/Engine.svc", "");
                var fullUrl = new Uri(serverUrl).Append(fixedPath);

                foreach (var param in context.Request.Query) {
                    var value = context.Request.Query[param];
                    queryParams[param] = value;
                }

                GETContent = string.Join(";", queryParams.Select(x => x.Key + "=" + x.Value).ToArray());

                foreach (var header in context.Request.Headers) {
                    headers[header.Key] = (header.Key == "Host") ? fullUrl.Host : header.Value.First();
                }

                var url = new Flurl.Url(fullUrl.ToString()).SetQueryParams(queryParams).WithHeaders(headers);
                HttpResponseMessage response;

                switch (context.Request.Method){
                    case "GET": {
                            response = url.GetAsync().Result;
                            break;
                        }
                    case "POST":  {
                            POSTContent = context.Request.Body.AsString();
                            response = url.PostAsync(
                                new CapturedStringContent(
                                    POSTContent
                                )
                            ).Result;
                            break;
                        }
                    case "PUT":  {
                            response = url.PutAsync(
                                new CapturedStringContent(
                                    context.Request.Body.AsString()
                                )
                            ).Result;
                            break;
                        }
                    case "DELETE": {
                            response = url.DeleteAsync().Result;
                            break;
                        }
                    default: {
                            throw new Exception($"unsupported method: {context.Request.Method}");
                        }
                }

                String replyToServer = response.Content.ReadAsStringAsync().Result;

                /*if (fixedPath == "/getrebroadcasters"){
                    replyToServer = Regex.Replace(replyToServer, @"<Host>(.*?)<\/Host>", "<Host>37.233.101.12</Host>");
                    replyToServer = Regex.Replace(replyToServer, @"<Port>(.*?)<\/Port>", "<Port>9999</Port>");
                }*/

                if (fixedPath == "/User/GetPermanentSession") {
                    replyToServer = Self.CleanFromUnknownChars(replyToServer);

                    var SBRW_XML = new XmlDocument();
                    SBRW_XML.LoadXml(replyToServer);
                    XmlNode UserInfo = SBRW_XML.SelectSingleNode("UserInfo");
                    XmlNodeList personas = UserInfo.SelectNodes("personas/ProfileData");

                    if(personas.Count == 0) {
                        replyToServer = replyToServer.Replace("false", "true");
                    }
                }

                if(fixedPath == "") {

                }

                Log.Debug($@"{context.Request.Method} {fixedPath}"); // -> {POSTContent} -> {GETContent} -> {replyToServer}");

                DiscordGamePresence.handleGameState(fixedPath, replyToServer, POSTContent, GETContent);

                return new TextResponse(replyToServer, response.Content.Headers.ContentType.ToString()) { StatusCode = (HttpStatusCode)(int)response.StatusCode };
            } else {
                var fullUrl = new Uri(Self.internetcheckurl);
                Log.Debug($@"{context.Request.Method} -> {fullUrl}");

                foreach (var param in context.Request.Query) {
                    var value = context.Request.Query[param];
                    queryParams[param] = value;
                }

                foreach (var header in context.Request.Headers) {
                    headers[header.Key] = (header.Key == "Host") ? fullUrl.Host : header.Value.First();
                }

                var url = new Flurl.Url(fullUrl.ToString()).SetQueryParams().WithHeaders(headers);

                HttpResponseMessage response;
                switch (context.Request.Method) {
                    case "GET": { response = url.GetAsync().Result; break; } 
                    case "POST": {  POSTContent = context.Request.Body.AsString(); response = url.PostAsync( new CapturedStringContent( POSTContent ) ).Result; break; } 
                    default: { throw new Exception($"unsupported method: {context.Request.Method}"); }
                }

                Self.sendRequest = true;
                return new TextResponse(String.Empty, response.Content.Headers.ContentType.ToString()) { StatusCode = (HttpStatusCode)(int)response.StatusCode };
            }
        }
    }
}
