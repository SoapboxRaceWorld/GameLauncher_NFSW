using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.Proxy
{
    public enum CommunicationLogEntryType
    {
        Request,
        Response,
        Info,
        Error,
        Rejected,
        Warning,
        Timer,
        Unknown
    }

    public interface ICommunicationLogData
    {
        string Path { get; set; }
        string Method { get; set; }
    }

    public class CommunicationLogEntry
    {
        public string RecordedAt { get; set; }
        public string ServerId { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public ICommunicationLogData Data { get; set; }
    }

    public class CommunicationLogRequest : ICommunicationLogData
    {
        public string Body { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }

        public CommunicationLogRequest(string body, string path, string method)
        {
            Body = body;
            Path = path;
            Method = method;
        }
    }

    public class CommunicationLogResponse : ICommunicationLogData
    {
        public string Body { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }

        public CommunicationLogResponse(string body, string path, string method)
        {
            Body = body;
            Path = path;
            Method = method;
        }
    }

    public class CommunicationLogLauncherError : ICommunicationLogData
    {
        public string Message { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }

        public CommunicationLogLauncherError(string message, string path, string method)
        {
            Message = message;
            Path = path;
            Method = method;
        }
    }

    public static class CommunicationLog
    {
        public static void RecordEntry(string ID, string CAT, CommunicationLogEntryType TYPE, ICommunicationLogData DATA)
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
                CommunicationLogEntry Entry = new CommunicationLogEntry
                {
                    ServerId = ID,
                    Category = CAT,
                    Data = DATA,
                    Type = CallMethod(TYPE),
                    RecordedAt = Time.GetTime("Now - UTC Time (Offset)")
                };

                File.AppendAllLines(Locations.LogCommunication, new List<string>
                {
                    JsonConvert.SerializeObject(Entry, Formatting.Indented)
                });
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Communication", null, Error, null, true);
            }
        }

        private static string CallMethod(CommunicationLogEntryType Type)
        {
            switch (Type)
            {
                case CommunicationLogEntryType.Error:
                    return "ERROR";
                case CommunicationLogEntryType.Info:
                    return "INFO";
                case CommunicationLogEntryType.Request:
                    return "REQUEST";
                case CommunicationLogEntryType.Response:
                    return "RESPONSE";
                case CommunicationLogEntryType.Rejected:
                    return "REJECTED";
                case CommunicationLogEntryType.Warning:
                    return "WARNING";
                case CommunicationLogEntryType.Timer:
                    return "TIMER";
                default:
                    return "UNKNOWN";
            }
        }
    }
}