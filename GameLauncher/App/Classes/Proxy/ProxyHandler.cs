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
using GameLauncher.App.Classes.Logger;
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

        private async Task<TextResponse> OnError(NancyContext context, Exception exception)
        {
            Log.Error($"PROXY ERROR [handling {context.Request.Path}]");
            Log.Error($"\tMESSAGE: {exception.Message}");
            Log.Error($"\t{exception.StackTrace}");
            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "PROXY",
                CommunicationLogEntryType.Error,
                new CommunicationLogLauncherError(exception.Message, context.Request.Path,
                    context.Request.Method));
            await Self.SubmitError(exception);

            return new TextResponse(HttpStatusCode.BadRequest, exception.Message);
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

            IFlurlRequest request = resolvedUrl.AllowAnyHttpStatus();

            foreach (var header in context.Request.Headers)
            {
                request = request.WithHeader(header.Key,
                    header.Key == "Host" ? resolvedUrl.ToUri().Host : header.Value.First());
            }

            var requestBody = context.Request.Method != "GET" ? context.Request.Body.AsString(Encoding.UTF8) : "";

            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "SERVER",
                CommunicationLogEntryType.Request,
                new CommunicationLogRequest(requestBody, resolvedUrl.ToString(), method));

            HttpResponseMessage responseMessage;

            var POSTContent = String.Empty;

            var queryParams = new Dictionary<string, object>();

            foreach (var param in context.Request.Query)
            {
                var value = context.Request.Query[param];
                queryParams[param] = value;
            }

            var GETContent = string.Join(";", queryParams.Select(x => x.Key + "=" + x.Value).ToArray());

            // ReSharper disable once LocalizableElement
            //Console.WriteLine($"[LOG] [{method}] ProxyHandler: {path}");

            switch (method)
            {
                case "GET":
                    responseMessage = await request.GetAsync(cancellationToken);
                    break;
                case "POST":
                    responseMessage = await request.PostAsync(new CapturedStringContent(requestBody, Encoding.UTF8),
                        cancellationToken);
                    POSTContent = context.Request.Body.AsString();
                    break;
                case "PUT":
                    responseMessage = await request.PutAsync(new CapturedStringContent(requestBody, Encoding.UTF8),
                        cancellationToken);
                    break;
                case "DELETE":
                    responseMessage = await request.DeleteAsync(cancellationToken);
                    break;
                default:
                    throw new ProxyException("Cannot handle request method: " + method);
            }

            var responseBody = await responseMessage.Content.ReadAsStringAsync();

            if (path == "/User/GetPermanentSession")
            {
                responseBody = Self.CleanFromUnknownChars(responseBody);
            }

            int statusCode = (int)responseMessage.StatusCode;

            try
            {
                DiscordGamePresence.HandleGameState(path, responseBody, POSTContent, GETContent);
            }
            catch (Exception e)
            {
                Log.Error($"DISCORD RPC ERROR [handling {context.Request.Path}]");
                Log.Error($"\tMESSAGE: {e.Message}");
                Log.Error($"\t{e.StackTrace}");
                await Self.SubmitError(e);
            }

            TextResponse textResponse = new TextResponse(responseBody,
                responseMessage.Content.Headers.ContentType?.MediaType ?? "application/xml;charset=UTF-8")
            {
                StatusCode = (HttpStatusCode)statusCode
            };

            queryParams.Clear();

            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "SERVER",
                CommunicationLogEntryType.Response, new CommunicationLogResponse(
                    responseBody, resolvedUrl.ToString(), method));

            return textResponse;
        }
    }

    public class Helper
    {
        public static int session = 0;
        public static String personaid = String.Empty;
    }
}
