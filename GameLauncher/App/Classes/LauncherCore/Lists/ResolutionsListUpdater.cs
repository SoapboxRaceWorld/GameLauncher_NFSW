using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRWCore.Classes.References.Jsons.Newtonsoft;
using SBRWCore.Classes.Required;
using System;
using System.Collections.Generic;
using static SBRWCore.Classes.Extensions.Screens.Resolution_Results;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    class ResolutionsListUpdater
    {
        public static List<Json_List_Resolutions> List = new List<Json_List_Resolutions>();

        public static void Get()
        {
            try
            {
                int AmountOfRes = 0;
                string JSONResolutions = string.Empty;

                List<Json_List_Resolutions> LocalResolutionsList = new List<Json_List_Resolutions>();
                DEVMODE vDevMode = new DEVMODE();

                JSONResolutions += "[";
                while (EnumDisplaySettings(null, AmountOfRes, ref vDevMode))
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

                if (!string.IsNullOrWhiteSpace(FileGameSettingsData.ScreenWidth) && !string.IsNullOrWhiteSpace(FileGameSettingsData.ScreenHeight))
                {
                    JSONResolutions += "{\"resolution\": \"" + FileGameSettingsData.ScreenWidth + "x" + FileGameSettingsData.ScreenHeight +
                            "\", \"dmPelsWidth\": \"" + FileGameSettingsData.ScreenWidth + "\", \"dmPelsHeight\": \"" + FileGameSettingsData.ScreenHeight + "\"}";
                }
                JSONResolutions += "]";

                if (EnableInsiderDeveloper.Allowed())
                {
                    Log.Debug("SCREENRESOLUTIONS: LIST -> " + JSONResolutions);
                }

                try
                {
                    LocalResolutionsList.AddRange(JsonConvert.DeserializeObject<List<Json_List_Resolutions>>(JSONResolutions));
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SCREENRESOLUTIONS", null, Error, null, true);
                }

                try
                {
                    foreach (Json_List_Resolutions CList in LocalResolutionsList)
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