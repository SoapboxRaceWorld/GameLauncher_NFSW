using GameLauncher.App.Classes.Logger;
using GameLauncher.HashPassword;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes
{
    public class ServerListUpdater
    {
        private static List<ServerInfo> finalItems = new List<ServerInfo>();
        private static Thread thread;

        public static void UpdateList()
        {
            thread = new Thread(new ThreadStart(() =>
            {
                List<Task> tasks = new List<Task>();
                ConcurrentBag<ServerInfo> serverInfos = new ConcurrentBag<ServerInfo>();

                foreach (string serverurl in Self.serverlisturl)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        string response = null;
                        try
                        {
                            Log.Debug("Loading serverlist from: " + serverurl);
                            WebClientWithTimeout wc = new WebClientWithTimeout();

                            response = wc.DownloadString(serverurl);
                        }
                        catch (Exception error)
                        {
                            Log.Error(error.Message);
                        }
                        if (response != null)
                        {
                            Log.Debug("Loaded serverlist from: " + serverurl);

                            foreach (ServerInfo si in JsonConvert.DeserializeObject<List<ServerInfo>>(response))
                            {
                                serverInfos.Add(si);
                            }
                        }
                    }));
                }

                if (File.Exists("servers.json"))
                {
                    var fileItems = JsonConvert.DeserializeObject<List<ServerInfo>>(File.ReadAllText("servers.json")) ?? new List<ServerInfo>();

                    if (fileItems.Count > 0)
                    {
                        serverInfos.Add(new ServerInfo
                        {
                            Id = "__category-CUSTOMCUSTOM__",
                            Name = "<GROUP>Custom Servers",
                            IsSpecial = true
                        });

                        fileItems.Select(si =>
                        {
                            si.DistributionUrl = "";
                            si.DiscordPresenceKey = "";
                            si.Id = SHA.HashPassword($"{si.Name}:{si.Id}:{si.IpAddress}");
                            si.IsSpecial = false;
                            si.Category = "CUSTOMCUSTOM";

                            return si;
                        }).ToList().ForEach(si => serverInfos.Add(si));
                    }
                }

                if (File.Exists("libOfflineServer.dll"))
                {
                    serverInfos.Add(new ServerInfo
                    {
                        Id = "__category-OFFLINEOFFLINE__",
                        Name = "<GROUP>Offline Server",
                        IsSpecial = true
                    });

                    serverInfos.Add(new ServerInfo
                    {
                        Name = "Offline Built-In Server",
                        Category = "OFFLINEOFFLINE",
                        DiscordPresenceKey = "",
                        IsSpecial = false,
                        DistributionUrl = "",
                        IpAddress = "http://localhost:4416/sbrw/Engine.svc",
                        Id = "OFFLINE"
                    });
                }

                //Somewhere here i have to remove duplicates...

                foreach (Task task in tasks)
                {
                    task.Wait();
                }

                foreach (var serverItemGroup in serverInfos.Reverse().GroupBy(s => s.Category))
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
            }));

            thread.IsBackground = true;
            thread.Start();
        }

        public static List<ServerInfo> GetList()
        {
            if (thread.IsAlive == true)
            {
                thread.Join();
            }

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