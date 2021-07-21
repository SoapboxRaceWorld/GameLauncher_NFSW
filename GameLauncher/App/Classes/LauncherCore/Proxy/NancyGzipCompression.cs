using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using GameLauncher.App.Classes.LauncherCore.Logger;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Responses;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    public class NancyGzipCompression : IApplicationStartup
    {
        public static MemoryStream content = null;

        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += CheckForCompression;
            pipelines.OnError += OnError;
        }

        private TextResponse OnError(NancyContext context, Exception Error)
        {
            Log.Error("PROXY HANDLER: " + context.Request.Path);
            LogToFileAddons.OpenLog("PROXY HANDLER", null, Error, null, true);

            CommunicationLog.RecordEntry(ServerProxy.Instance.GetServerName(), "PROXY",
                CommunicationLogEntryType.Error,
                new CommunicationLogLauncherError(Error.Message, context.Request.Path,
                    context.Request.Method));

            context.Request.Dispose();

            return new TextResponse(HttpStatusCode.BadRequest, Error.Message);
        }

        private static void CheckForCompression(NancyContext context)
        {
            if (!RequestIsGzipCompatible(context.Request))
            {
                return;
            }

            CompressResponse(context.Response);
        }

        private static void CompressResponse(Response response)
        {
            if (content != null)
            {
                /* Dispose Current Memory */
                content.Dispose();
            }

            response.Headers["Content-Encoding"] = "gzip";
            response.Headers["Connection"] = "close";

            /* Ask System to Allocate Memory */
            content = new MemoryStream();
            /* Response Contents is now feed into Allocated Memory */
            response.Contents(content);
            /* Set Position for data in Allocated Memory */
            content.Position = 0;
            /* Read the Contents from Allocated Memory */
            response.Contents = responseStream =>
            {
                using (var gzip = new GZipStream(responseStream, CompressionMode.Compress, true))
                {
                    /* Instead of Feeding content Raw (Which can potentially cause OoM) Lets read it from Allocated Memory */
                    gzip.Write(content.ToArray(), 0, (int)content.Length);
                }
            };

            /* Different Solutions With Different OoM Errors */
            /* https://gist.github.com/DavidCarbon/e0b37e7bc58b5e1a46f6dfedc87c966d */
        }

        private static bool RequestIsGzipCompatible(Request request)
        {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}