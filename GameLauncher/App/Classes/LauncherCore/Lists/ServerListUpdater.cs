﻿using GameLauncher.App.Classes.Logger;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameLauncher.App.Classes.LauncherCore.Global;
using System.Globalization;
using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using System.Windows.Forms;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.RPC;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    public class ServerListUpdater
    {
        public static List<ServerList> NoCategoryList = new List<ServerList>();

        public static List<ServerList> CleanList = new List<ServerList>();

        public static void GetList()
        {
            DiscordLauncherPresense.Status("Start Up", "Creating Server List");

            List<ServerList> serverInfos = new List<ServerList>();

            try
            {
                Log.UrlCall("LIST CORE: Loading Server List from: " + URLs.OnlineServerList);
                FunctionStatus.TLS();
                Uri URLCall = new Uri(URLs.OnlineServerList);
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                WebClient Client = new WebClient();
                Client.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion + 
                " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                var response = Client.DownloadString(URLCall);
                Log.UrlCall("LIST CORE: Loaded Server List from: " + URLs.OnlineServerList);

                try
                {
                    serverInfos.AddRange(JsonConvert.DeserializeObject<List<ServerList>>(response));
                    InformationCache.ServerListStatus = "Loaded";
                }
                catch (Exception error)
                {
                    Log.Error("LIST CORE: Error occurred while deserializing Server List from [" + URLs.OnlineServerList + "]: " + error.Message);
                    InformationCache.ServerListStatus = "Error";
                }
            }
            catch (Exception error)
            {
                Log.Error("LIST CORE: Error occurred while loading Server List from [" + URLs.OnlineServerList + "]: " + error.Message);
                InformationCache.ServerListStatus = "Error";
            }

            if (File.Exists("servers.json"))
            {
                var fileItems = JsonConvert.DeserializeObject<List<ServerList>>(File.ReadAllText("servers.json")) ?? new List<ServerList>();

                if (fileItems.Count > 0)
                {
                    fileItems.Select(si =>
                    {
                        si.DistributionUrl = "";
                        si.Id = SHA.HashPassword($"{si.Name}:{si.Id}:{si.IpAddress}");
                        si.IsSpecial = false;
                        si.Category = "CUSTOM";

                        return si;
                    }).ToList().ForEach(si => serverInfos.Add(si));
                }
            }

            if (File.Exists("libOfflineServer.dll"))
            {
                serverInfos.Add(new ServerList
                {
                    Name = "Offline Built-In Server",
                    Category = "OFFLINE",
                    IsSpecial = false,
                    DistributionUrl = "",
                    IpAddress = "http://localhost:4416/sbrw/Engine.svc",
                    Id = "OFFLINE"
                });
            }

            if (Debugger.IsAttached)
            {
                serverInfos.Add(new ServerList
                {
                    Name = "Local Debug Server",
                    Category = "DEBUG",
                    IsSpecial = false,
                    DistributionUrl = "",
                    IpAddress = "http://localhost:8680",
                    Id = "DEV"
                });
            }

            /* Create Final Server List without Categories */
            foreach (ServerList NoCatList in serverInfos)
            {
                if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                {
                    NoCategoryList.Add(NoCatList);
                }
            }

            /* Create Rough Draft Server List with Categories */
            List<ServerList> RawList = new List<ServerList>();

            foreach (var serverItemGroup in serverInfos.GroupBy(s => s.Category))
            {
                if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{serverItemGroup.Key} Servers")) == -1)
                {
                    RawList.Add(new ServerList
                    {
                        Id = $"__category-{serverItemGroup.Key}__",
                        Name = $"<GROUP>{serverItemGroup.Key} Servers",
                        IsSpecial = true
                    });
                }
                RawList.AddRange(serverItemGroup.ToList());
            }

            /* Create Final Server List with Categories */
            foreach (ServerList CList in RawList)
            {
                if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                {
                    CleanList.Add(CList);
                }
            }

            /* (Start Process) Check Up to Date Certificate Status */
            CertificateStore.Latest();
        }

        /* Converts 2 Letter Country Code and Returns Full Country Name (In English) */
        public static string CountryName(string twoLetterCountryCode)
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo culture in cultures)
            {
                RegionInfo region = new RegionInfo(culture.LCID);
                if (region.TwoLetterISORegionName.ToUpper() == twoLetterCountryCode.ToUpper())
                {
                    return region.EnglishName;
                }
            }

            return "Unknown";
        }

        /* Server Name */
        /** Returns a Server Name by first checking if the Server has provided one
         *  if so use that, otherwise it will see if the ServerList has provide one
         *  if not then it will see if its a custom server, which will provide "Custom"
         *  otherwise it will be "uknown" **/
        public static string ServerName(string State)
        {
            if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.serverName))
            {
                return InformationCache.SelectedServerJSON.serverName;
            }
            else if (!string.IsNullOrWhiteSpace(InformationCache.SelectedServerData.Name))
            {
                return InformationCache.SelectedServerData.Name;
            }
            else if (InformationCache.SelectedServerCategory == "CUSTOM")
            {
                if (State == "Register")
                {
                    return "Custom Server";
                }
                else if (State == "Settings")
                {
                    if (FileAccountSave.ChoosenGameServer.StartsWith("https"))
                    {
                        return "The Saved Server";
                    }
                    else
                    {
                        return "The Selected Server";
                    }
                }
                else
                {
                    return "Custom";
                }
            }
            else
            {
                return "Unknown";
            }
        }
    }
}