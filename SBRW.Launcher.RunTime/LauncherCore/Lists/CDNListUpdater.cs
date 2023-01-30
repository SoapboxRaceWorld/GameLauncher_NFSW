using SBRW.Launcher.RunTime.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using System.Linq;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;

namespace SBRW.Launcher.RunTime.LauncherCore.Lists
{
    public class CDNListUpdater
    {
        /// <summary>
        /// If the CDN List Is Ready to be Consumed
        /// </summary>
        public static bool LoadedList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static List<Json_List_CDN> CleanList { get; set; } = new List<Json_List_CDN>();
        /// <summary>
        /// 
        /// </summary>
        public static string CachedJSONList { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public static void GetList()
        {
            List<Json_List_CDN> cdnInfos = new List<Json_List_CDN>();

            try
            {
                if (CachedJSONList.Valid_Json())
                {
#pragma warning disable CS8604 //Is Null Safe with the check above
                    cdnInfos.AddRange(JsonConvert.DeserializeObject<List<Json_List_CDN>>(CachedJSONList));
#pragma warning restore CS8604
                    LoadedList = true;
                }
                else
                {
                    CachedJSONList = string.Empty;
                    LoadedList = false;
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("CDN LIST CORE", string.Empty, Error, string.Empty, true);
                LoadedList = false;
            }
            finally
            {
                if (CachedJSONList != null)
                {
                    CachedJSONList = string.Empty;
                }
            }

            if (cdnInfos != null)
            {
                if (cdnInfos.Any())
                {
                    /* Create Final CDN List without Categories */
                    foreach (Json_List_CDN NoCatList in cdnInfos)
                    {
                        if (CleanList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                        {
                            CleanList.Add(NoCatList);
                        }
                    }
                }
            }
        }
    }
}