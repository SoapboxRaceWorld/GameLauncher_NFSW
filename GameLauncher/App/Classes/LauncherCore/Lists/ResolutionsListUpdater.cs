using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Logger;
using Newtonsoft.Json;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using SBRW.Launcher.Core.Classes.Reference.Json_.Newtonsoft_;
using System;
using System.Collections.Generic;
using SBRW.Launcher.Core.Classes.Required.DLL.User32_;

namespace GameLauncher.App.Classes.LauncherCore.Lists
{
    class ResolutionsListUpdater
    {
        public static List<Json_List_Resolution> List = new List<Json_List_Resolution>();

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