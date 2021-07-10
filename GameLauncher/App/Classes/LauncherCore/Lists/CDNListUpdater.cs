using GameLauncher.App.Classes.Logger;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using System.Text;
using System.Windows.Forms;

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
                Uri URLCall = new Uri(URLs.OnlineCDNList);
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                WebClient wc = new WebClient();
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("user-agent", "GameLauncher " + Application.ProductVersion +
                " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                string responseList = wc.DownloadString(URLCall);
                Log.UrlCall("LIST CORE: Loaded CDN List from: " + URLs.OnlineCDNList);

                try
                {
                    cdnInfos.AddRange(JsonConvert.DeserializeObject<List<CDNList>>(responseList));
                    InformationCache.CDNListStatus = "Loaded";
                }
                catch (Exception Error)
                {
                    Log.Error("LIST CORE: Error occurred while deserializing CDN List from [" + URLs.OnlineCDNList + "]: " + Error.Message);
                    Log.Error("LIST CORE [HResult]: " + Error.HResult);
                    Log.ErrorInner("LIST CORE [Full Report]: " + Error.ToString());
                    InformationCache.CDNListStatus = "Error";
                }
            }
            catch (Exception Error)
            {
                Log.Error("LIST CORE: Error occurred while loading CDN List from [" + URLs.OnlineCDNList + "]: " + Error.Message);
                Log.Error("LIST CORE [HResult]: " + Error.HResult);
                Log.ErrorInner("LIST CORE [Full Report]: " + Error.ToString());
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