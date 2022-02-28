using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using System.Linq;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;

namespace SBRW.Launcher.App.Classes.LauncherCore.Lists
{
    class SelectedCDN
    {
        /// <summary>
        /// 
        /// </summary>
        public static string CDNUrl { get; set; } = string.Empty;
    }

    public class CDNListUpdater
    {
        /// <summary>
        /// If the CDN List Is Ready to be Consumed
        /// </summary>
        public static bool LoadedList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static List<Json_List_CDN> NoCategoryList { get; set; } = new List<Json_List_CDN>();
        /// <summary>
        /// 
        /// </summary>
        public static List<Json_List_CDN> NoCategoryList_LZMA { get; set; } = new List<Json_List_CDN>();
        /// <summary>
        /// 
        /// </summary>
        public static List<Json_List_CDN> CleanList { get; set; } = new List<Json_List_CDN>();
        /// <summary>
        /// 
        /// </summary>
        public static List<Json_List_CDN> CleanList_LZMA { get; set; } = new List<Json_List_CDN>();
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
                if (Is_Json.Valid(CachedJSONList))
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
                        if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                        {
                            if (long.TryParse(NoCatList.Legacy_Support, out long CDN_Result))
                            {
                                /* SBRW Pack (1) Only */
                                if (CDN_Result >= 1)
                                {
                                    NoCategoryList.Add(NoCatList);
                                }

                                /* LZMA (0) & SBRW Pack (1) = Both (2) */
                                if (CDN_Result == 0 || CDN_Result == 2)
                                {
                                    NoCategoryList_LZMA.Add(NoCatList);
                                }
                            }
                            else
                            {
                                NoCategoryList.Add(new Json_List_CDN() 
                                { 
                                    Name = "JSON FORMAT ERROR", 
                                    Url = "http://localhost",
                                    Offline = true
                                });

                                NoCategoryList_LZMA.Add(new Json_List_CDN()
                                {
                                    Name = "JSON FORMAT ERROR",
                                    Url = "http://localhost",
                                    Offline = true
                                });

                                break;
                            }
                        }
                    }

                    /* Create Rough Draft CDN List with Categories */
                    List<Json_List_CDN> RawList = new List<Json_List_CDN>();
                    
                    foreach (var CDNCategoryGroup in cdnInfos.GroupBy(Aoba => new { Aoba.Category, Aoba.Legacy_Support }))
                    {
                        if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{CDNCategoryGroup.Key.Category} Mirrors")) == -1)
                        {
                            RawList.Add(new Json_List_CDN
                            {
                                Name = $"<GROUP>{CDNCategoryGroup.Key.Category} Mirrors",
                                Legacy_Support = CDNCategoryGroup.Key.Legacy_Support,
                                IsSpecial = true
                            });
                        }

                        RawList.AddRange(CDNCategoryGroup.ToList());
                    }

                    /* Create Final CDN List with Categories */
                    foreach (Json_List_CDN CList in RawList)
                    {
                        if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                        {
                            if (long.TryParse(CList.Legacy_Support, out long CDN_Result))
                            {
                                /* SBRW Pack (1) Only */
                                if (CDN_Result >= 1)
                                {
                                    CleanList.Add(CList);
                                }

                                /* LZMA (0) & SBRW Pack (1) = Both (2) */
                                if (CDN_Result == 0 || CDN_Result == 2)
                                {
                                    CleanList_LZMA.Add(CList);
                                }
                            }
                            else
                            {
                                CleanList.Add(new Json_List_CDN()
                                {
                                    Category = "BLANK 'Legacy_Support' ENTRY",
                                    Name = "JSON FORMAT ERROR",
                                    Url = "http://localhost",
                                    Offline = true
                                });

                                CleanList_LZMA.Add(new Json_List_CDN()
                                {
                                    Category = "BLANK 'Legacy_Support' ENTRY",
                                    Name = "JSON FORMAT ERROR",
                                    Url = "http://localhost",
                                    Offline = true
                                });

                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}