using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses;
using HttpStatusCode = System.Net.HttpStatusCode;

namespace GameLauncher.App.Classes.Proxy
{
    /// <summary>
    /// Server request proxy
    /// </summary>
    public class ProxyHandler : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            Console.WriteLine(pipelines);
            pipelines.BeforeRequest += this.ProxyRequest;
        }

        private async Task<Response> ProxyRequest(NancyContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var serverURL = ServerProxy.Instance.GetServerUrl();

            FlurlClient client = new FlurlClient(serverURL);
            IFlurlRequest newRequest = client.Request(request.Path.Replace("/nfsw/Engine.svc", ""));
            Dictionary<string, object> query = new Dictionary<string, object>();
            HttpResponseMessage responseMessage = null;

            // Add headers
            foreach (var requestHeader in request.Headers)
            {
                newRequest.Headers.Add(requestHeader.Key, ((string[])requestHeader.Value)[0]);
            }

            // Add query params
            foreach (var q in request.Query)
            {
                query.Add(q, request.Query[q]);
            }

            newRequest = newRequest.SetQueryParams(query);

            switch (request.Method.ToUpperInvariant())
            {
                case "GET":
                    responseMessage = await newRequest.GetAsync(cancellationToken);
                    break;
                case "POST":
                    responseMessage = await newRequest.PostAsync(new StreamContent(request.Body), cancellationToken);
                    break;
                case "PUT":
                    responseMessage = await newRequest.PutAsync(new StreamContent(request.Body), cancellationToken);
                    break;
                case "DELETE":
                    responseMessage = await newRequest.DeleteAsync(cancellationToken);
                    break;
                default:
                    responseMessage = new HttpResponseMessage(HttpStatusCode.Accepted);
                    break;
            }

            return new TextResponse(
                (Nancy.HttpStatusCode)responseMessage.StatusCode, 
                await responseMessage.Content.ReadAsStringAsync(), 
                Encoding.UTF8, 
                responseMessage.Headers.ToDictionary(h => h.Key, h => h.Value.First()));
        }
    }
}
