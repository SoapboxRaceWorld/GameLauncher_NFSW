using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.Logger
{
    class Log
    {
        private static ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();

        public static void StartLogging()
        {
            Task.Run(() => TaskKernel());
        }

        private static void _toFile(string text, string errorname = "DEBUG")
        {
            buffer.Enqueue($"[{errorname}] {text}");
        }

        public static void Debug(string text)
        {
            _toFile(text, "DEBUG");
        }

        public static void Info(string text)
        {
            _toFile(text, " INFO");
        }

        public static void Warning(string text)
        {
            _toFile(text, " WARN");
        }

        public static void Error(string text)
        {
            _toFile(text, "ERROR");
        }

        public static void UrlCall(string text)
        {
            _toFile(text, "  URL");
        }

        public static void System(string text)
        {
            _toFile(text, "SYSTM");
        }

        public static void Build(string text)
        {
            _toFile(text, "BUILD");
        }

        public static void Visuals(string text)
        {
            _toFile(text, "VISUL");
        }

        public static void Api(string text)
        {
            _toFile(text, "  API");
        }

        public static void Core(string text)
        {
            _toFile(text, " CORE");
        }

        private static async void TaskKernel()
        {
            while (true)
            {
                if(buffer.Count > 0 && buffer.TryDequeue(out string merged))
                {
                    File.AppendAllText("launcher.log", merged + Environment.NewLine);
                    Console.WriteLine(merged);
                } else
                {
                    await Task.Delay(30);
                }
            }
        }
    }
}
