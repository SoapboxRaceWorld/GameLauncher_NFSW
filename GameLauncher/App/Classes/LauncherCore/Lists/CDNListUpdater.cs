using GameLauncher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    class SelectedCDN
    {
        public static string CDNUrl = String.Empty;
        public static string TrackHigh = String.Empty;
    }

    public class CDNListUpdater
    {
        public static bool LoadedList = false;

        public static List<Json_List_CDN> NoCategoryList = new List<Json_List_CDN>();

        public static List<Json_List_CDN> CleanList = new List<Json_List_CDN>();

        public static string CachedJSONList;

        public static void GetList()
        {
            List<Json_List_CDN> cdnInfos = new List<Json_List_CDN>();

            try
            {
                cdnInfos.AddRange(JsonConvert.DeserializeObject<List<Json_List_CDN>>(CachedJSONList));
                LoadedList = true;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN LIST CORE", null, Error, null, true);
                LoadedList = false;
            }
            finally
            {
                if (CachedJSONList != null)
                {
                    CachedJSONList = null;
                }
            }

            if (cdnInfos != null)
            {
                if (cdnInfos.Any())
                {
                    /* Create Final CDN List without Categories */
                    foreach (Json_List_CDN NoCatList in cdnInfos)
                    {
                        if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                        {
                            NoCategoryList.Add(NoCatList);
                        }
                    }

                    /* Create Rough Draft CDN List with Categories */
                    List<Json_List_CDN> RawList = new List<Json_List_CDN>();

                    foreach (var cdnItemGroup in cdnInfos.GroupBy(s => s.Category))
                    {
                        if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{cdnItemGroup.Key} Mirrors")) == -1)
                        {
                            RawList.Add(new Json_List_CDN
                            {
                                Name = $"<GROUP>{cdnItemGroup.Key} Mirrors",
                                IsSpecial = true
                            });
                        }
                        RawList.AddRange(cdnItemGroup.ToList());
                    }

                    /* Create Final CDN List with Categories */
                    foreach (Json_List_CDN CList in RawList)
                    {
                        if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                        {
                            CleanList.Add(CList);
                        }
                    }
                }
            }
        }
    }
}