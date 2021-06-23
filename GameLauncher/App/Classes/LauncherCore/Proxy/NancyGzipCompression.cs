using System.IO;
using System.IO.Compression;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    public class NancyGzipCompression : IApplicationStartup
    {
        public void Initialize(IPipelines pipelines)
        {
            pipelines.AfterRequest += CheckForCompression;
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
            response.Headers["Content-Encoding"] = "gzip";
            response.Headers["Connection"] = "close";

            /* Ask System to Allocate Memory */
            var content = new MemoryStream();
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