using SBRW.Launcher.RunTime.LauncherCore.APICheckers;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.Collections.Generic;
using System.Linq;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;

namespace SBRW.Launcher.RunTime.LauncherCore.Lists
{
    public class LanguageListUpdater
    {
        public static List<Json_List_Language> NoCategoryList { get; set; } = new List<Json_List_Language>();
        public static List<Json_List_Language> CleanList { get; set; } = new List<Json_List_Language>();

        public static void GetList()
        {
            LogToFileAddons.Parent_Log_Screen(2, "LIST CORE", "Creating Language List");
            Presence_Launcher.Status(0, "Creating Language List");

            List<Json_List_Language> langInfos = new List<Json_List_Language>();

            string json_language = string.Empty;

            try
            {
                json_language += "[";
                json_language += "    { \"category\": \"Official\", \"name\": \"English\",             \"xml_value\": \"EN\", \"ini_value\": \"EN\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Deutsch\",             \"xml_value\": \"DE\", \"ini_value\": \"DE\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Español\",             \"xml_value\": \"ES\", \"ini_value\": \"ES\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Français\",            \"xml_value\": \"FR\", \"ini_value\": \"FR\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Polski\",              \"xml_value\": \"PL\", \"ini_value\": \"PL\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Русский\",             \"xml_value\": \"RU\", \"ini_value\": \"RU\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Português (Brasil)\",  \"xml_value\": \"PT\", \"ini_value\": \"PT\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"繁體中文\",             \"xml_value\": \"TC\", \"ini_value\": \"TC\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"简体中文\",             \"xml_value\": \"SC\", \"ini_value\": \"SC\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"ภาษาไทย\",              \"xml_value\": \"TH\", \"ini_value\": \"TH\"},";
                json_language += "    { \"category\": \"Official\", \"name\": \"Türkçe\",               \"xml_value\": \"TR\", \"ini_value\": \"TR\"},";
                json_language += "    { \"category\": \"Custom\",   \"name\": \"Italiano\",             \"xml_value\": \"EN\", \"ini_value\": \"IT\"}";
                json_language += "]";
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("LIST CORE", string.Empty, Error, string.Empty, true);
                if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                {
                    LogToFileAddons.Parent_Log_Screen(5, "LIST CORE", Error.InnerException.Message, false, true);
                }
            }
            finally
            {
                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            }

            if (json_language.Valid_Json())
            {
                try
                {
#pragma warning disable CS8604 // Null Safe Check Done Above
                    langInfos.AddRange(JsonConvert.DeserializeObject<List<Json_List_Language>>(json_language));
#pragma warning restore CS8604 // Possible null reference argument.
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LIST CORE Deserialize", string.Empty, Error, string.Empty, true);
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LIST CORE Deserialize", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }

                try
                {
                    foreach (Json_List_Language NoCatList in langInfos)
                    {
                        if (NoCategoryList.FindIndex(i => string.Equals(i.Name, NoCatList.Name)) == -1)
                        {
                            NoCategoryList.Add(NoCatList);
                        }
                    }

                    List<Json_List_Language> RawList = new List<Json_List_Language>();

                    foreach (var langItemGroup in langInfos.GroupBy(s => s.Category))
                    {
                        if (RawList.FindIndex(i => string.Equals(i.Name, $"<GROUP>{langItemGroup.Key} Mirrors")) == -1)
                        {
                            RawList.Add(new Json_List_Language
                            {
                                Name = $"<GROUP>{langItemGroup.Key} Languages",
                                IsSpecial = true
                            });
                        }
                        RawList.AddRange(langItemGroup.ToList());
                    }

                    foreach (Json_List_Language CList in RawList)
                    {
                        if (CleanList.FindIndex(i => string.Equals(i.Name, CList.Name)) == -1)
                        {
                            CleanList.Add(CList);
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("LIST CORE Compile", string.Empty, Error, string.Empty, true);
                    if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "LIST CORE Compile", Error.InnerException.Message, false, true);
                    }
                }
                finally
                {
                    #if !(RELEASE_UNIX || DEBUG_UNIX) 
                    GC.Collect(); 
                    #endif
                }

                LogToFileAddons.Parent_Log_Screen(3, "LIST CORE", "Done");
            }
            else
            {
                LogToFileAddons.Parent_Log_Screen(5, "LIST CORE", "Invalid JSON String");
            }

            LogToFileAddons.Parent_Log_Screen(1, "API", "Moved to Function");
            /* Run the API Checks to Make Sure it Visually Displayed Correctly */
            VisualsAPIChecker.PingAPIStatus();
        }
    }
}