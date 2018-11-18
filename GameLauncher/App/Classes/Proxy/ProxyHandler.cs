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
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest += ProxyRequest;
        }

        private static Response ProxyRequest(NancyContext context)
        {
            string POSTContent = String.Empty;
            string GETContent = String.Empty;
            var serverUrl = ServerProxy.Instance.GetServerUrl();

            if (string.IsNullOrEmpty(serverUrl)) {
                return new TextResponse(HttpStatusCode.BadGateway, "Not open for business");
            }

            var fixedPath = context.Request.Path.Replace("/nfsw/Engine.svc", "");
            var fullUrl = new Uri(serverUrl).Append(fixedPath);

            Log.Debug($@"{context.Request.Method} {fixedPath} -> {fullUrl.Host}");

            var queryParams = new Dictionary<string, object>();
            var headers = new Dictionary<string, object>();

            foreach (var param in context.Request.Query)
            {
                var value = context.Request.Query[param];

                queryParams[param] = value;
            }

            GETContent = string.Join(";", queryParams.Select(x => x.Key + "=" + x.Value).ToArray());

            foreach (var header in context.Request.Headers)
            {
                headers[header.Key] = (header.Key == "Host") ? fullUrl.Host : header.Value.First();
            }

            var url = new Flurl.Url(fullUrl.ToString())
                .SetQueryParams(queryParams)
                .WithHeaders(headers);

            HttpResponseMessage response;

            switch (context.Request.Method)
            {
                case "GET":
                    {
                        response = url.GetAsync().Result;
                        //response = await url.GetAsync();
                        break;
                    }
                case "POST":
                    {
                        POSTContent = context.Request.Body.AsString();
                        response = url.PostAsync(
                            new CapturedStringContent(
                                POSTContent
                            )
                        ).Result;
                        break;
                    }
                case "PUT":
                    {
                        response = url.PutAsync(
                            new CapturedStringContent(
                                context.Request.Body.AsString()
                            )
                        ).Result;
                        break;
                    }
                case "DELETE":
                    {
                        response = url.DeleteAsync().Result;
                        break;
                    }
                default:
                    {
                        throw new Exception($"unsupported method: {context.Request.Method}");
                    }
            }

            String replyToServer = response.Content.ReadAsStringAsync().Result;

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

            //Let's create faketimer
            Dictionary<string, bool> executedPowerups = new Dictionary<string, bool>();
            executedPowerups.Add("-1681514783", false);
            executedPowerups.Add("-537557654", false);
            executedPowerups.Add("-1692359144", false);
            executedPowerups.Add("-364944936", false);
            executedPowerups.Add("2236629", false);
            executedPowerups.Add("957701799", false);
            executedPowerups.Add("1805681994", false);
            executedPowerups.Add("-611661916", false);
            executedPowerups.Add("-1564932069", false);
            executedPowerups.Add("1627606782", false);
            executedPowerups.Add("1113720384", false);
            executedPowerups.Add("125509666", false);

            Dictionary<string, int> executedPowerupsRemainingSecs = new Dictionary<string, int>();

            if (Regex.Match(fixedPath, "/powerups/activated/", RegexOptions.IgnoreCase).Success) {
                String activatedHash = fixedPath.Split('/').Last();

                Console.WriteLine("--- CHECK ACTIVATED POWERUPS ---");
                foreach(KeyValuePair<string, bool> entry in executedPowerups) {
                    Console.WriteLine(powerups[entry.Key] + ": " + entry.Value);
                }

                if (executedPowerups[activatedHash]) {
                    var notification = new NotifyIcon() {
                        Visible = true,
                        Icon = System.Drawing.SystemIcons.Information,
                        BalloonTipIcon = ToolTipIcon.Info,
                        BalloonTipTitle = "GameLauncherReborn",
                        BalloonTipText = "Hey! You can't use " + powerups[activatedHash] + " right now, wait " + executedPowerupsRemainingSecs[activatedHash] + "s for regeneration.",
                    };

                    notification.ShowBalloonTip(5000);
                    notification.Dispose();

                    replyToServer = null;
                } else {
                    Console.WriteLine("User activated " + powerups[activatedHash]);

                    executedPowerups[activatedHash] = true;
                    executedPowerupsRemainingSecs[activatedHash] = 15;

                    System.Timers.Timer poweruptimer = new System.Timers.Timer();
                    poweruptimer.Elapsed += (x, y) => { 
                        if(executedPowerupsRemainingSecs[activatedHash] == 0) {
                            executedPowerups[activatedHash] = false;
                            executedPowerupsRemainingSecs.Remove(activatedHash);
                            Console.WriteLine("Removed " + powerups[activatedHash]);

                            poweruptimer.Close();
                        } else {
                            executedPowerupsRemainingSecs[activatedHash] -= 1;
                            Console.WriteLine("Counting: " + executedPowerupsRemainingSecs[activatedHash]);
                        }
                    };

                    poweruptimer.Interval = 1000;
                    poweruptimer.Enabled = true;
                }
            }

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

            DiscordGamePresence.handleGameState(fixedPath, replyToServer, POSTContent, GETContent);

            return new TextResponse(replyToServer, response.Content.Headers.ContentType.ToString()) {
                StatusCode = (HttpStatusCode)(int)response.StatusCode
            };
        }
    }
}
