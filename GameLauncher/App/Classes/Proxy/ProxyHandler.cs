using Flurl;
using Flurl.Http;
using Flurl.Http.Content;
using GameLauncher.App.Classes.RPC;
using GameLauncherReborn;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Extensions;
using Nancy.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Url = Flurl.Url;

namespace GameLauncher.App.Classes.Proxy
{
    public class ProxyHandler : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.BeforeRequest += ProxyRequest;
            pipelines.OnError += OnError;
        }

        private object OnError(NancyContext context, Exception exception)
        {
            if (exception is ProxyException proxyException)
            {
                CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "PROXY",
                    CommunicationLogEntryType.Error,
                    new CommunicationLogLauncherError(proxyException.Message, context.Request.Path,
                        context.Request.Method));

                return new TextResponse(HttpStatusCode.BadRequest, proxyException.Message);
            }

            return null;
        }

        private async Task<Response> ProxyRequest(NancyContext context, CancellationToken cancellationToken)
        {
            string path = context.Request.Path;
            string method = context.Request.Method.ToUpperInvariant();

            if (!path.StartsWith("/nfsw/Engine.svc"))
            {
                throw new ProxyException("Invalid request path: " + path);
            }

            path = path.Substring("/nfsw/Engine.svc".Length);

            Url resolvedUrl = new Url(ServerProxy.Instance.GetServerUrl()).AppendPathSegment(path);

            foreach (var queryParamName in context.Request.Query)
            {
                resolvedUrl = resolvedUrl.SetQueryParam(queryParamName, context.Request.Query[queryParamName],
                    NullValueHandling.Ignore);
            }

            IFlurlRequest request = resolvedUrl.WithTimeout(TimeSpan.FromSeconds(30));

            foreach (var header in context.Request.Headers)
            {
                request = request.WithHeader(header.Key, header.Key == "Host" ? resolvedUrl.ToUri().Host : header.Value.First());
            }

            request = request.AllowAnyHttpStatus();

            var requestBody = context.Request.Method != "GET" ? context.Request.Body.AsString(Encoding.UTF8) : "";

            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "SERVER",
                CommunicationLogEntryType.Request, new CommunicationLogRequest(requestBody, resolvedUrl.ToString(), method));

            HttpResponseMessage responseMessage;

            string POSTContent = String.Empty;
            string GETContent = String.Empty;

            var queryParams = new Dictionary<string, object>();

            foreach (var param in context.Request.Query) {
                var value = context.Request.Query[param];
                queryParams[param] = value;
            }

            GETContent = string.Join(";", queryParams.Select(x => x.Key + "=" + x.Value).ToArray());


            switch (method) {
                case "GET":
                    responseMessage = await request.GetAsync(cancellationToken);
                    break;
                case "POST":
                    responseMessage = await request.PostAsync(new CapturedStringContent(requestBody, Encoding.UTF8), cancellationToken);
                    POSTContent = context.Request.Body.AsString();
                    break;
                case "PUT":
                    responseMessage = await request.PutAsync(new CapturedStringContent(requestBody, Encoding.UTF8), cancellationToken);
                    break;
                case "DELETE":
                    responseMessage = await request.DeleteAsync(cancellationToken);
                    break;
                default:
                    throw new ProxyException("Cannot handle request method: " + method);
            }

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            if (path == "/User/GetPermanentSession") {
                responseBody = Self.CleanFromUnknownChars(responseBody);
            }

            DiscordGamePresence.handleGameState(path, responseBody, POSTContent, GETContent);
            //OfflineSaveFile.SaveContent(path, responseBody);

            TextResponse textResponse = new TextResponse(responseBody,
                responseMessage.Content.Headers.ContentType?.MediaType ?? "application/xml;charset=UTF-8")
            {
                StatusCode = (HttpStatusCode)(int)responseMessage.StatusCode
            };

            queryParams.Clear();

            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "SERVER", CommunicationLogEntryType.Response, new CommunicationLogResponse(
                responseBody, resolvedUrl.ToString(), method));

            return textResponse;
        }
    }
}
