using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Support;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.LauncherCore.Logger
{
    /// <summary>
    /// Used for Logging Launcher entires such as Information, Checks, Error Messages, Etc.
    /// </summary>
    class Log
    {
        readonly static ConcurrentQueue<string> buffer = new ConcurrentQueue<string>();
        /// <summary>
        /// Starts Logging Task
        /// </summary>
        public static void StartLogging() => Task.Run(() => TaskKernel());
        /// <summary>
        /// Main Logging to File
        /// </summary>
        private static void ToFile(string text, string errorname = "DEBUG") => buffer.Enqueue(Time.GetTime("Time") + " [" + errorname + "] " + text);
        /// <summary>
        /// Used for Debugging Purposes 
        /// </summary>
        /// <remarks>Should <c>Not</c> be Used outside of Development Builds</remarks>
        /// <param name="text">Log Message</param>
        public static void Debug(string text) => ToFile(text, "      DEBUG");
        /// <summary>
        /// Used for Informational Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Info(string text) => ToFile(text, "       INFO");
        /// <summary>
        /// Used for Checking Information Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Checking(string text) => ToFile(text, "   CHECKING");
        /// <summary>
        /// Used for Completed Event Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Completed(string text) => ToFile(text, "   COMPLETE");
        /// <summary>
        /// Used for Warning Event Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Warning(string text) => ToFile(text, "       WARN");
        /// <summary>
        /// Used for Error Event Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Error(string text) => ToFile(text, "      ERROR");
        /// <summary>
        /// Used for Full Error Event Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void ErrorFR(string text) => ToFile(text, " FULL ERROR");
        /// <summary>
        /// Used for Numerical Error Code Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void ErrorIC(string text) => ToFile(text, " ERROR CODE");
        /// <summary>
        /// Used for Webcalls URL Details
        /// </summary>
        /// <remarks><c>Used for WebClientTimeout</c></remarks>
        /// <param name="text">Log Message</param>
        public static void UrlCall(string text) => ToFile(text, "        URL");
        /// <summary>
        /// Used for Operating System Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void System(string text) => ToFile(text, "      SYSTM");
        /// <summary>
        /// Used for Launcher Build Details
        /// </summary>
        /// <remarks><c>Used for Identifing Launcher Builds</c></remarks>
        /// <param name="text">Log Message</param>
        public static void Build(string text) => ToFile(text, "      BUILD");
        /// <summary>
        /// Used for Launcher Theming Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Visuals(string text) => ToFile(text, "      VISUL");
        /// <summary>
        /// Used for API Details
        /// </summary>
        /// <param name="text">Log Message</param>
        public static void Api(string text) => ToFile(text, "        API");
        /// <summary>
        /// Used for Logging Launcher Core Details
        /// </summary>
        /// <remarks><c>Can be used as a General Log</c></remarks>
        /// <param name="text">Log Message</param>
        public static void Core(string text) => ToFile(text, "       CORE");
        /// <summary>
        /// Used for Logging Launcher Function Details
        /// </summary>
        /// <remarks><c>Can be used as a Function General Log</c></remarks>
        /// <param name="text">Log Message</param>
        public static void Function(string text) => ToFile(text, "   FUNCTION");
        //"      "
        private static async void TaskKernel()
        {
            while (true)
            {
                if (buffer.Count > 0 && buffer.TryDequeue(out string merged))
                {
                    try
                    {
                        if (!Directory.Exists(Strings.Encode(Locations.LogCurrentFolder)))
                        {
                            Directory.CreateDirectory(Strings.Encode(Locations.LogCurrentFolder));
                        }
                    }
                    catch { }

                    try
                    {
                        File.AppendAllText(Locations.LogLauncher, merged + Environment.NewLine);
                    }
                    catch { }
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
        private static void ToFile(string text, string errorname = "DEBUG") => buffer.Enqueue(Time.GetTime("Time") + " [" + errorname + "] " + text);
        public static void Error(string text) => ToFile(text, "         ERROR");
        public static void ErrorIC(string text) => ToFile(text, "ERROR CODE");
        public static void ErrorFR(string text) => ToFile(text, " FULL ERROR");
        public static void Info(string text) => ToFile(text, "          INFO");
        public static void Valid(string text) => ToFile(text, "         VAILD");
        public static void Invalid(string text) => ToFile(text, "       INVALID");
        public static void Deleted(string text) => ToFile(text, "       DELETED");
        public static void Missing(string text) => ToFile(text, "       MISSING");
        public static void Downloaded(string text) => ToFile(text, "      REPLACED");

        private static async void VerifyTaskKernel()
        {
            while (true)
            {
                if (buffer.Count > 0 && buffer.TryDequeue(out string merged))
                {
                    try
                    {
                        if (!Directory.Exists(Strings.Encode(Locations.LogCurrentFolder)))
                        {
                            Directory.CreateDirectory(Strings.Encode(Locations.LogCurrentFolder));
                        }
                    }
                    catch { }

                    try
                    {
                        File.AppendAllText(Locations.LogVerify, merged + Environment.NewLine);
                    }
                    catch { }
                }
                else
                {
                    await Task.Delay(30);
                }
            }
        }
    }
}
