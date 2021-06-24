using GameLauncher.App.Classes.Logger;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    class SelectedCDN
    {
        public static string CDNUrl = String.Empty;
        public static string TrackHigh = String.Empty;
    }

    public class CDNListUpdater
    {
        public static List<CDNList> NoCategoryList = new List<CDNList>();

        public static List<CDNList> CleanList = new List<CDNList>();

        public static void GetList()
        {
            List<CDNList> cdnInfos = new List<CDNList>();

            try
            {
                Log.UrlCall("LIST CORE: Loading CDN List from: " + URLs.OnlineCDNList);
                FunctionStatus.TLS();
                var wc = new WebClient();
                var responseList = wc.DownloadString(URLs.OnlineCDNList);
                Log.UrlCall("LIST CORE: Loaded CDN List from: " + URLs.OnlineCDNList);

                try
                {
                    cdnInfos.AddRange(JsonConvert.DeserializeObject<List<CDNList>>(responseList));
                    InformationCache.CDNListStatus = "Loaded";
                }
                catch (Exception error)
                {
                    Log.Error("LIST CORE: Error occurred while deserializing CDN List from [" + URLs.OnlineCDNList + "]: " + error.Message);
                    InformationCache.CDNListStatus = "Error";
                }
            }
            catch (Exception error)
            {
                Log.Error("LIST CORE: Error occurred while loading CDN List from [" + URLs.OnlineCDNList + "]: " + error.Message);
                InformationCache.CDNListStatus = "Error";
            }

            /* Create Final CDN List without Categories */
            foreach (CDNList NoCatList in cdnInfos)
            {
                if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                {
                    NoCategoryList.Add(NoCatList);
                }
            }

            /* Create Rough Draft CDN List with Categories */
            List<CDNList> RawList = new List<CDNList>();

            foreach (var cdnItemGroup in cdnInfos.GroupBy(s => s.Category))
            {
                if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{cdnItemGroup.Key} Mirrors")) == -1)
                {
                    RawList.Add(new CDNList
                    {
                        Name = $"<GROUP>{cdnItemGroup.Key} Mirrors",
                        IsSpecial = true
                    });
                }
                RawList.AddRange(cdnItemGroup.ToList());
            }

            /* Create Final CDN List with Categories */
            foreach (CDNList CList in RawList)
            {
                if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                {
                    CleanList.Add(CList);
                }
            }
        }
    }
}