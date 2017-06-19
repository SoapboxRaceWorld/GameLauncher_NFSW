using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GameLauncher.Server {
    class WebServer {
        private static WebServer _instance = new WebServer();
        public static WebServer Instance {
            get {
                return WebServer._instance;
            }
        }

        public void CreateServer() {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:7331/");
            listener.Start();

            new Thread(() => {
                while (true) {
                    HttpListenerContext ctx = listener.GetContext();
                    ThreadPool.QueueUserWorkItem((_) => {
                        //%TODO% Implement custom files here
                        string responseText = 
                        "<LoginStatusVO>" +
                            "<UserId>11111111</UserId>" +
                            "<LoginToken>aaaaaaaa-aaaa-aaaa-aaaaaaaa</LoginToken>" +
                            "<Description>Comming Soon</Description>" +
                        "</LoginStatusVO>";

                        ctx.Response.ContentType = "text/xml";
                        byte[] buf = Encoding.UTF8.GetBytes(responseText);
                        ctx.Response.ContentEncoding = Encoding.UTF8;
                        ctx.Response.ContentLength64 = buf.Length;
                        ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                        ctx.Response.Close();
                    });
                }
            }).Start();
            return;
        }
    }
}
