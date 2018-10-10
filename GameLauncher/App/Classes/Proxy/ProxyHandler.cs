using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Content;
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

            if (string.IsNullOrEmpty(serverUrl))
            {
                return new TextResponse(HttpStatusCode.BadGateway, "Not open for business");
            }

            var fixedPath = context.Request.Path.Replace("/nfsw/Engine.svc", "");
            var fullUrl = new Uri(serverUrl).Append(fixedPath);

            Console.WriteLine($@"{context.Request.Method} {fixedPath}");

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
                headers[header.Key] = header.Value.First();
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

            DiscordRPC.handleGameState(fixedPath, response.Content.ReadAsStringAsync().Result, POSTContent, GETContent);

            return new TextResponse(
                response.Content.ReadAsStringAsync().Result,
                response.Content.Headers.ContentType.ToString()
            )
            {
                StatusCode = (HttpStatusCode)(int)response.StatusCode
            };
        }
    }
}
