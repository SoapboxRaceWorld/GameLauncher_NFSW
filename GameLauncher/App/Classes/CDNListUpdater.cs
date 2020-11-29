using GameLauncher.App.Classes.Logger;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace GameLauncher.App.Classes
{
    public class CDNListUpdater
    {
        private static List<CDNObject> finalCDNItems = new List<CDNObject>();

        public static void UpdateCDNList()
        {
            List<CDNObject> cdnInfos = new List<CDNObject>();

            foreach (var cdnListURL in Self.cdnlisturl)
            {
                try
                {
                    Log.Debug("LIST CORE: Loading cdnlist from: " + cdnListURL);
                    var wc = new WebClient();
                    var responseList = wc.DownloadString(cdnListURL);
                    Log.Debug("LIST CORE: Loaded cdnlist from: " + cdnListURL);

                    try
                    {
                        cdnInfos.AddRange(
                            JsonConvert.DeserializeObject<List<CDNObject>>(responseList));
                    }
                    catch (Exception error)
                    {
                        Log.Error("LIST CORE: Error occurred while deserializing cdn list from [" + cdnListURL + "]: " + error.Message);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("LIST CORE: Error occurred while loading cdn list from [" + cdnListURL + "]: " + error.Message);
                }

                foreach (var cdnItemGroup in cdnInfos.GroupBy(s => s.Category))
                {
                    if (finalCDNItems.FindIndex(i => string.Equals(i.Name, $"<GROUP>{cdnItemGroup.Key} CDNs")) == -1)
                    {
                        finalCDNItems.Add(new CDNObject
                        {
                            Name = $"<GROUP>{cdnItemGroup.Key} CDNs",
                            IsSpecial = true
                        });
                    }
                    finalCDNItems.AddRange(cdnItemGroup.ToList());
                }
            }
        }

        public static List<CDNObject> GetCDNList()
        {
            List<CDNObject> newFinalCDNItems = new List<CDNObject>();

            foreach (CDNObject xCDN in finalCDNItems)
            {
                if (newFinalCDNItems.FindIndex(i => string.Equals(i.Name, xCDN.Name)) == -1)
                {
                    newFinalCDNItems.Add(xCDN);
                }
            }
            return newFinalCDNItems;
        }
    }
}