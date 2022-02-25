using SBRW.Launcher.App.Classes.InsiderKit;
using SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite;
using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Extension.Logging_;
using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using SBRW.Launcher.Core.Required.DLL.User32_;
using SBRW.Launcher.Core.Extra.XML_;

namespace SBRW.Launcher.App.Classes.LauncherCore.Lists
{
    class ResolutionsListUpdater
    {
        public static List<Json_List_Resolution> List { get; set; } = new List<Json_List_Resolution>();

        public static void Get()
        {
            try
            {
                int AmountOfRes = 0;
                string JSONResolutions = string.Empty;

                List<Json_List_Resolution> LocalResolutionsList = new List<Json_List_Resolution>();
                Resolution_Results.DEVMODE vDevMode = new Resolution_Results.DEVMODE();

                JSONResolutions += "[";
                while (Resolution_Results.EnumDisplaySettings(null, AmountOfRes, ref vDevMode))
                {
                    JSONResolutions += "{\"resolution\": \"" + vDevMode.dmPelsWidth + "x" + vDevMode.dmPelsHeight + "\", \"dmPelsWidth\": \"" +
                        vDevMode.dmPelsWidth + "\", \"dmPelsHeight\": \"" + vDevMode.dmPelsHeight + "\"},";
                    if (EnableInsiderDeveloper.Allowed())
                    {
                        Log.Debug("SCREENRESOLUTIONS: " + AmountOfRes + " Width: " + vDevMode.dmPelsWidth + " Height: " + vDevMode.dmPelsHeight +
                            " Color: " + (1 << vDevMode.dmBitsPerPel) + " Frequency: " + vDevMode.dmDisplayFrequency);
                    }
                    AmountOfRes++;
                }

                if (!string.IsNullOrWhiteSpace(XML_File.XML_Settings_Data.ScreenWidth) && !string.IsNullOrWhiteSpace(XML_File.XML_Settings_Data.ScreenHeight))
                {
                    JSONResolutions += "{\"resolution\": \"" + XML_File.XML_Settings_Data.ScreenWidth + "x" + XML_File.XML_Settings_Data.ScreenHeight +
                            "\", \"dmPelsWidth\": \"" + XML_File.XML_Settings_Data.ScreenWidth + "\", \"dmPelsHeight\": \"" + XML_File.XML_Settings_Data.ScreenHeight + "\"}";
                }
                JSONResolutions += "]";

                if (EnableInsiderDeveloper.Allowed())
                {
                    Log.Debug("SCREENRESOLUTIONS: LIST -> " + JSONResolutions);
                }

                try
                {
                    LocalResolutionsList.AddRange(JsonConvert.DeserializeObject<List<Json_List_Resolution>>(JSONResolutions));
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SCREENRESOLUTIONS", null, Error, null, true);
                }

                try
                {
                    foreach (Json_List_Resolution CList in LocalResolutionsList)
                    {
                        if (List.FindIndex(i => string.Equals(i.Resolution, CList.Resolution)) == -1)
                        {
                            List.Add(CList);
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SCREENRESOLUTIONS", null, Error, null, true);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SCREENRESOLUTIONS", null, Error, null, true);
            }
        }
    }
}