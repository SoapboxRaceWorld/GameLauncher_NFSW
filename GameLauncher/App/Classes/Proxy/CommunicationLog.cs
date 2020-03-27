using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GameLauncher.App.Classes.Proxy
{
    public enum CommunicationLogEntryType
    {
        Request,
        Response,
        Info,
        Error
    }

    public interface ICommunicationLogData
    {
        string Path { get; set; }
        string Method { get; set; }
    }

    public class CommunicationLogEntry
    {
        public DateTimeOffset RecordedAt { get; set; }
        public string ServerId { get; set; }
        public string Category { get; set; }
        public CommunicationLogEntryType Type { get; set; }
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
        private static readonly string LogFileName;

        static CommunicationLog()
        {
            LogFileName = "communication.log";
            if (File.Exists(LogFileName))
                File.Delete(LogFileName);
        }

        public static void RecordEntry(string serverId, string category, CommunicationLogEntryType type,
            ICommunicationLogData data)
        {
            CommunicationLogEntry entry = new CommunicationLogEntry { ServerId = serverId, Category = category, Data = data, Type = type, RecordedAt = DateTimeOffset.Now };

            File.AppendAllLines(LogFileName, new List<string>
            {
                JsonConvert.SerializeObject(entry, Formatting.Indented)
            });
        }
    }
}