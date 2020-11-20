using GameLauncher.App.Classes.Logger;
using GameLauncherReborn;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Collections.Generic;

namespace GameLauncher.App.Classes
{
    public class CDNListUpdater
    {
        private static List<CDNObject> finalCDNItems = new List<CDNObject>();

        public static void UpdateCDNList()
        {
            foreach (var cdnListURL in Self.cdnlisturl)
            {
                try
                {
                    Log.Debug("Loading cdnlist from: " + cdnListURL);
                    var wc = new WebClient();
                    var responseList = wc.DownloadString(cdnListURL);
                    Log.Debug("Loaded cdnlist from: " + cdnListURL);

                    try
                    {
                        finalCDNItems.AddRange(JsonConvert.DeserializeObject<List<CDNObject>>(responseList));
                    }
                    catch (Exception error)
                    {
                        Log.Error("Error occurred while deserializing cdn list from [" + cdnListURL + "]: " + error.Message);
                    }
                }
                catch (Exception error)
                {
                    Log.Error("Error occurred while loading cdn list from [" + cdnListURL + "]: " + error.Message);
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