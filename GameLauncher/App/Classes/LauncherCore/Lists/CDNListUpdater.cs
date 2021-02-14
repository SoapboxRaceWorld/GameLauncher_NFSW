using GameLauncher.App.Classes.Logger;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;

namespace GameLauncher.App.Classes
{
    public class CDNListUpdater
    {
        public static List<CDNObject> NoCategoryList = new List<CDNObject>();

        public static List<CDNObject> CleanList = new List<CDNObject>();

        public static void GetList()
        {
            List<CDNObject> cdnInfos = new List<CDNObject>();

            foreach (var cdnListURL in URLs.cdnlisturl)
            {
                try
                {
                    Log.UrlCall("LIST CORE: Loading CDN List from: " + cdnListURL);
                    var wc = new WebClient();
                    var responseList = wc.DownloadString(cdnListURL);
                    Log.UrlCall("LIST CORE: Loaded CDN List from: " + cdnListURL);

                    try
                    {
                        cdnInfos.AddRange(
                            JsonConvert.DeserializeObject<List<CDNObject>>(responseList));
                        FunctionStatus.CDNListStatus = "Loaded";
                        break;
                    }
                    catch (Exception error)
                    {
                        Log.Error("LIST CORE: Error occurred while deserializing CDN List from [" + cdnListURL + "]: " + error.Message);
                        FunctionStatus.CDNListStatus = "Error";
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LIST CORE: Error occurred while loading CDN List from [" + cdnListURL + "]: " + error.Message);
                    FunctionStatus.CDNListStatus = "Error";
                }
            }

            /* Create Final CDN List without Categories */
            foreach (CDNObject NoCatList in cdnInfos)
            {
                if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                {
                    NoCategoryList.Add(NoCatList);
                }
            }

            /* Create Rough Draft CDN List with Categories */
            List<CDNObject> RawList = new List<CDNObject>();

            foreach (var cdnItemGroup in cdnInfos.GroupBy(s => s.Category))
            {
                if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{cdnItemGroup.Key} Mirrors")) == -1)
                {
                    RawList.Add(new CDNObject
                    {
                        Name = $"<GROUP>{cdnItemGroup.Key} Mirrors",
                        IsSpecial = true
                    });
                }
                RawList.AddRange(cdnItemGroup.ToList());
            }

            /* Create Final CDN List with Categories */
            foreach (CDNObject CList in RawList)
            {
                if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                {
                    CleanList.Add(CList);
                }
            }
        }
    }
}