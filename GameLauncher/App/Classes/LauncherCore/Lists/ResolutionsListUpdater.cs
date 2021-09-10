using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Lists.JSON;
using GameLauncher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static GameLauncher.App.Classes.SystemPlatform.Windows.ScreenResolutions;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    class ResolutionsListUpdater
    {
        public static List<JsonResolutions> List = new List<JsonResolutions>();

        public static void Get()
        {
            try
            {
                int AmountOfRes = 0;
                string JSONResolutions = string.Empty;

                List<JsonResolutions> LocalResolutionsList = new List<JsonResolutions>();
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
                    LocalResolutionsList.AddRange(JsonConvert.DeserializeObject<List<JsonResolutions>>(JSONResolutions));
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("SCREENRESOLUTIONS", null, Error, null, true);
                }

                try
                {
                    foreach (JsonResolutions CList in LocalResolutionsList)
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