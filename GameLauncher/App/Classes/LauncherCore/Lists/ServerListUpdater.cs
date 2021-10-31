using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.LauncherUpdater;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.UI_Forms.SelectServer_Screen;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.Hash_;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Classes.Reference.Json_.Newtonsoft_;
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

        public static List<Json_List_Server> NoCategoryList = new List<Json_List_Server>();

        public static List<Json_List_Server> NoCategoryList_CSO = new List<Json_List_Server>();

        public static List<Json_List_Server> CleanList = new List<Json_List_Server>();

        public static string CachedJSONList;

        public static void GetList()
        {
            Log.Checking("SERVER LIST CORE: Creating Server List");
            DiscordLauncherPresence.Status("Start Up", "Creating Server List");

            List<Json_List_Server> serverInfos = new List<Json_List_Server>();

            List<Json_List_Server> CustomServerInfos = new List<Json_List_Server>();

            try
            {
                serverInfos.AddRange(JsonConvert.DeserializeObject<List<Json_List_Server>>(CachedJSONList));
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
                    var fileItems = JsonConvert.DeserializeObject<List<Json_List_Server>>
                    (File.ReadAllText(Locations.LauncherCustomServers)) ?? new List<Json_List_Server>();

                    if (fileItems.Count > 0)
                    {
                        fileItems.Select(si =>
                        {
                            si.ID = string.IsNullOrWhiteSpace(si.ID) ? Hashes.Hash_String(1, $"{si.Name}:{si.ID}:{si.IPAddress}") : si.ID;
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
                serverInfos.Add(new Json_List_Server
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
                serverInfos.Add(new Json_List_Server
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
                        foreach (Json_List_Server NoCatList in serverInfos)
                        {
                            if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                            {
                                NoCategoryList.Add(NoCatList);
                            }
                        }

                        if (CustomServerInfos.Count >= 1)
                        {
                            /* Create Final Server List without Categories (Custom Servers Only) */
                            foreach (Json_List_Server NoCatList in CustomServerInfos)
                            {
                                if (NoCategoryList_CSO.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                                {
                                    NoCategoryList_CSO.Add(NoCatList);
                                }
                            }
                        }

                        /* Create Rough Draft Server List with Categories */
                        List<Json_List_Server> RawList = new List<Json_List_Server>();

                        foreach (var serverItemGroup in serverInfos.GroupBy(s => s.Category))
                        {
                            if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{serverItemGroup.Key} Servers")) == -1)
                            {
                                RawList.Add(new Json_List_Server
                                {
                                    ID = $"__category-{serverItemGroup.Key}__",
                                    Name = $"<GROUP>{serverItemGroup.Key} Servers",
                                    IsSpecial = true
                                });
                            }
                            RawList.AddRange(serverItemGroup.ToList());
                        }

                        /* Create Final Server List with Categories */
                        foreach (Json_List_Server CList in RawList)
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

            Log.Info("LAUNCHER UPDATER: Moved to Function");
            /* (Start Process) Check If Updater Exists or Requires an Update */
            UpdaterExecutable.Check();
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
                    !string.IsNullOrWhiteSpace(InformationCache.SelectedServerJSON.Server_Name))
                {
                    return InformationCache.SelectedServerJSON.Server_Name;
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
                                !string.IsNullOrWhiteSpace(SelectServer.ServerJsonData.Server_Name))
                            {
                                return SelectServer.ServerJsonData.Server_Name;
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