using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.Logger
{
    class Log
    {
        readonly static ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
        private static String filename = String.Empty;
        public Log(String file = "launcher.log") => filename = file;
        public static void StartLogging() => Task.Run(() => TaskKernel());
        private static void ToFile(string text, string errorname = "DEBUG") => buffer.Enqueue($"[{errorname}] {text}");
        public static void Debug(string text) => ToFile(text, "DEBUG");
        public static void Info(string text) => ToFile(text, " INFO");
        public static void Warning(string text) => ToFile(text, " WARN");
        public static void Error(string text) => ToFile(text, "ERROR");
        public static void UrlCall(string text) => ToFile(text, "  URL");
        public static void System(string text) => ToFile(text, "SYSTM");
        public static void Build(string text) => ToFile(text, "BUILD");
        public static void Visuals(string text) => ToFile(text, "VISUL");
        public static void Api(string text) => ToFile(text, "  API");
        public static void Core(string text) => ToFile(text, " CORE");

        private static async void TaskKernel()
        {
            while (true)
            {
                if (buffer.Count > 0 && buffer.TryDequeue(out string merged))
                {
                    try
                    {
                        File.AppendAllText("launcher.log", merged + Environment.NewLine);
                    }
                    catch(Exception ex)
                    {
                        new Log(filename);
                        Log.Error(ex.Message);
                    }
                    Console.WriteLine(merged);
                }
                else
                {
                    await Task.Delay(30);
                }
            }
        }
    }
    class LogVerify
    {
        readonly static ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
        public static void StartVerifyLogging() => Task.Run(() => VerifyTaskKernel());
        private static void ToFile(string text, string errorname = "DEBUG") => buffer.Enqueue($"[{errorname}] {text}");
        public static void Error(string text) => ToFile(text, "   ERROR");
        public static void Info(string text) => ToFile(text, "    INFO");
        public static void Valid(string text) => ToFile(text, "   VAILD");
        public static void Invalid(string text) => ToFile(text, " INVALID");
        public static void Deleted(string text) => ToFile(text, " DELETED");
        public static void Missing(string text) => ToFile(text, " MISSING");
        public static void Downloaded(string text) => ToFile(text, "REPLACED");
 
        private static async void VerifyTaskKernel()
        {
            while (true)
            {
                if (buffer.Count > 0 && buffer.TryDequeue(out string merged))
                {
                    File.AppendAllText("Verify.log", merged + Environment.NewLine);
                    Console.WriteLine(merged);
                }
                else
                {
                    await Task.Delay(30);
                }
            }
        }
    }
}
