using GameLauncher.App.Classes.Hash;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Support;
using GameLauncher.App.Classes.SystemPlatform.Windows;
using GameLauncher.App.UI_Forms.SelectServer_Screen;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    public class ServerListUpdater
    {
        public static bool LoadedList = false;

        public static List<ServerList> NoCategoryList = new List<ServerList>();

        public static List<ServerList> NoCategoryList_CSO = new List<ServerList>();

        public static List<ServerList> CleanList = new List<ServerList>();

        public static string CachedJSONList;

        public static void GetList()
        {
            Log.Checking("SERVER LIST CORE: Creating Server List");
            DiscordLauncherPresence.Status("Start Up", "Creating Server List");

            List<ServerList> serverInfos = new List<ServerList>();

            List<ServerList> CustomServerInfos = new List<ServerList>();

            try
            {
                serverInfos.AddRange(JsonConvert.DeserializeObject<List<ServerList>>(CachedJSONList));
                LoadedList = true;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SERVER LIST CORE", null, Error, null, true);
                LoadedList = false;
            }
            finally
            {
                if (CachedJSONList != null)
                {
                    CachedJSONList = null;
                }
            }

            if (File.Exists(Locations.LauncherCustomServers))
            {
                try
                {
                    var fileItems = JsonConvert.DeserializeObject<List<ServerList>>
                    (Strings.Encode(File.ReadAllText(Locations.LauncherCustomServers))) ?? new List<ServerList>();

                    if (fileItems.Count > 0)
                    {
                        fileItems.Select(si =>
                        {
                            si.ID = string.IsNullOrWhiteSpace(si.ID) ? SHA.Hashes($"{si.Name}:{si.ID}:{si.IPAddress}") : si.ID;
                            si.IsSpecial = false;
                            si.Category = string.IsNullOrWhiteSpace(si.Category) ? "CUSTOM" : si.Category;

                            return si;
                        }).ToList().ForEach(si => CustomServerInfos.Add(si));
                        serverInfos.AddRange(CustomServerInfos);
                        LoadedList = true;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SERVER LIST CORE", null, Error, null, true);
                }
            }

            if (File.Exists("libOfflineServer.dll"))
            {
                serverInfos.Add(new ServerList
                {
                    Name = "Offline Built-In Server",
                    Category = "OFFLINE",
                    IsSpecial = false,
                    IPAddress = "http://localhost:4416/sbrw/Engine.svc",
                    ID = "OFFLINE"
                });
            }

            if (Debugger.IsAttached)
            {
                serverInfos.Add(new ServerList
                {
                    Name = "Local Debug Server",
                    Category = "DEBUG",
                    IsSpecial = false,
                    IPAddress = "http://localhost:8680",
                    ID = "DEV"
                });
            }

            try
            {
                if (serverInfos != null)
                {
                    if (serverInfos.Any())
                    {
                        /* Create Final Server List without Categories (All Servers) */
                        foreach (ServerList NoCatList in serverInfos)
                        {
                            if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                            {
                                NoCategoryList.Add(NoCatList);
                            }
                        }

                        if (CustomServerInfos.Count >= 1)
                        {
                            /* Create Final Server List without Categories (Custom Servers Only) */
                            foreach (ServerList NoCatList in CustomServerInfos)
                            {
                                if (NoCategoryList_CSO.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                                {
                                    NoCategoryList_CSO.Add(NoCatList);
                                }
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
                                    ID = $"__category-{serverItemGroup.Key}__",
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
                        Log.Completed("SERVER LIST CORE: Server List Done");
                    }
                    else
                    {
                        Log.Completed("SERVER LIST CORE: Server List has no Elements");
                    }
                }
                else
                {
                    Log.Completed("SERVER LIST CORE: Server List is NULL");
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SERVER LIST CORE", null, Error, null, true);
            }

            Log.Info("CERTIFICATE STORE: Moved to Function");
            /* (Start Process) Check Up to Date Certificate Status */
            CertificateStore.Latest();
        }

        /* Converts 2 Letter Country Code and Returns Full Country Name (In English) */
        public static string CountryName(string twoLetterCountryCode)
        {
            try
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
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("COUNTRYNAME", null, Error, null, true);
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
            try
            {
                if (InformationCache.SelectedServerJSON != null &&
                    !string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.serverName))
                {
                    return InformationCache.SelectedServerJSON.serverName;
                }
                else if (InformationCache.SelectedServerData != null &&
                    !string.IsNullOrWhiteSpace(InformationCache.SelectedServerData.Name))
                {
                    return InformationCache.SelectedServerData.Name;
                }
                else
                {
                    switch (State)
                    {
                        case "Register":
                        case "RPC":
                            return "Custom Server";
                        case "Settings":
                            return "The Selected Server";
                        case "Select Server":
                            if (SelectServer.ServerJsonData != null &&
                                !string.IsNullOrWhiteSpace(SelectServer.ServerJsonData.serverName))
                            {
                                return SelectServer.ServerJsonData.serverName;
                            }
                            else
                            {
                                return SelectServer.ServerName;
                            }
                        default:
                            return "Custom";
                    }
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Server Name Provider", null, Error, null, true);
                return "Unknown";
            }
        }
    }
}