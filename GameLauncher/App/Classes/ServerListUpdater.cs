using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameLauncher.App.Classes
{
    public class ServerListUpdater
    {
        private static List<ServerInfo> finalItems = new List<ServerInfo>();

        public static void UpdateList()
        {
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            foreach (var serverListURL in Self.serverlisturl) {
                try {
                    Log.Debug("Loading serverlist from: " + serverListURL);
                    var wc = new WebClient();
                    var response = wc.DownloadString(serverListURL);
                    Log.Debug("Loaded serverlist from: " + serverListURL);

                    try {
                        serverInfos.AddRange(
                            JsonConvert.DeserializeObject<List<ServerInfo>>(response));
                    } catch (Exception error) {
                        Log.Error("Error occurred while deserializing server list from [" + serverListURL + "]: " + error.Message);
                    }
                } catch (Exception error) {
                    Log.Error("Error occurred while loading server list from [" + serverListURL + "]: " + error.Message);
                }
            }

            if (File.Exists("servers.json"))
            {
                var fileItems = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText("servers.json")) ?? new List<ServerInfo>();

                if (fileItems.Count > 0)
                {
                    fileItems.Select(si =>
                    {
                        si.DistributionUrl = "";
                        si.DiscordPresenceKey = "";
                        si.Id = SHA.HashPassword($"{si.Name}:{si.Id}:{si.IpAddress}");
                        si.IsSpecial = false;
                        si.Category = "CUSTOM";

                        return si;
                    }).ToList().ForEach(si => serverInfos.Add(si));
                }
            }

            if (File.Exists("libOfflineServer.dll"))
            {
                serverInfos.Add(new ServerInfo
                {
                    Name = "Offline Built-In Server",
                    Category = "OFFLINE",
                    DiscordPresenceKey = "",
                    IsSpecial = false,
                    DistributionUrl = "",
                    IpAddress = "http://localhost:4416/sbrw/Engine.svc",
                    Id = "OFFLINE"
                });
            }

            if (Debugger.IsAttached)
            {
                serverInfos.Add(new ServerInfo
                {
                    Name = "Local Debug Server",
                    Category = "DEBUG",
                    DiscordPresenceKey = "",
                    IsSpecial = false,
                    DistributionUrl = "",
                    IpAddress = "http://localhost:8680",
                    Id = "DEV"
                });
            }

            foreach (var serverItemGroup in serverInfos.GroupBy(s => s.Category))
            {
                if (finalItems.FindIndex(i => string.Equals(i.Name, $"<GROUP>{serverItemGroup.Key} Servers")) == -1)
                {
                    finalItems.Add(new ServerInfo
                    {
                        Id = $"__category-{serverItemGroup.Key}__",
                        Name = $"<GROUP>{serverItemGroup.Key} Servers",
                        IsSpecial = true
                    });
                }
                finalItems.AddRange(serverItemGroup.ToList());
            }
        }

        public static List<ServerInfo> GetList()
        {
            List<ServerInfo> newFinalItems = new List<ServerInfo>();

            foreach (ServerInfo xServ in finalItems)
            {
                if (newFinalItems.FindIndex(i => string.Equals(i.Name, xServ.Name)) == -1)
                {
                    newFinalItems.Add(xServ);
                }
            }
            return newFinalItems;
        }
    }
}