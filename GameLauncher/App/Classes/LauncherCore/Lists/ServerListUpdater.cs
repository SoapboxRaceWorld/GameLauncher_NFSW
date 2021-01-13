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
        public static List<ServerInfo> NoCategoryList = new List<ServerInfo>();

        public static List<ServerInfo> CleanList = new List<ServerInfo>();

        public static void GetList()
        {
            List<ServerInfo> serverInfos = new List<ServerInfo>();

            foreach (var serverListURL in Self.serverlisturl)
            {
                try
                {
                    Log.UrlCall("LIST CORE: Loading serverlist from: " + serverListURL);
                    var wc = new WebClient();
                    var response = wc.DownloadString(serverListURL);
                    Log.UrlCall("LIST CORE: Loaded serverlist from: " + serverListURL);

                    try
                    {
                        serverInfos.AddRange(
                            JsonConvert.DeserializeObject<List<ServerInfo>>(response));
                        break;
                    }
                    catch (Exception error)
                    {
                        Log.Error("LIST CORE: Error occurred while deserializing server list from [" + serverListURL + "]: " + error.Message);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LIST CORE: Error occurred while loading server list from [" + serverListURL + "]: " + error.Message);
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

            /* Create Final Server List without Categories */
            foreach (ServerInfo NoCatList in serverInfos)
            {
                if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                {
                    NoCategoryList.Add(NoCatList);
                }
            }

            /* Create Rough Draft Server List with Categories */
            List<ServerInfo> RawList = new List<ServerInfo>();

            foreach (var serverItemGroup in serverInfos.GroupBy(s => s.Category))
            {
                if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{serverItemGroup.Key} Servers")) == -1)
                {
                    RawList.Add(new ServerInfo
                    {
                        Id = $"__category-{serverItemGroup.Key}__",
                        Name = $"<GROUP>{serverItemGroup.Key} Servers",
                        IsSpecial = true
                    });
                }
                RawList.AddRange(serverItemGroup.ToList());
            }

            /* Create Final Server List with Categories */
            foreach (ServerInfo CList in RawList)
            {
                if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                {
                    CleanList.Add(CList);
                }
            }

            /* Increase ProgressStatus for Splash Screen */
            SplashScreen.SucessfulProgress();
        }
    }
}