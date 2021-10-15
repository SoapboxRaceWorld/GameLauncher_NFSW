using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms
{
    class Translations
    {
        private static ResourceManager Lang_Launcher = null;

        public static bool ResetCache = false;

        public static string Application_Language = "en-US";

        public static string Database(string Text_Request)
        {
            if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
            {
                Log.Function("DATABASE: Requested: " + Text_Request + " Lang: " + Application_Language);
            }

            try
            {
                if (Lang_Launcher == null || ResetCache)
                {
                    switch (UI(Application_Language))
                    {
                        default:
                            Lang_Launcher = new ResourceManager("GameLauncher.App.Languages.English_Texts", Assembly.GetExecutingAssembly());
                            break;
                    }

                    ResetCache = false;
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(Text_Request) && Lang_Launcher != null)
                    {
                        return Regex.Unescape(Lang_Launcher.GetString(Text_Request));
                    }
                    else
                    {
                        return "Languages Programer ERROR";
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Translations Database Selection", null, Error, null, true);
                    return "Languages Program ERROR";
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Translations Database", null, Error, null, true);
                return "Languages ERROR";
            }
        }

        public static string UI(string Chosen_Lang)
        {
            if (!string.IsNullOrWhiteSpace(Chosen_Lang))
            {
                if (Chosen_Lang.Contains("fr"))
                {
                    return "fr";
                }
                else if (Chosen_Lang.Contains("en"))
                {
                    return "en";
                }
                else
                {
                    return Chosen_Lang;
                }
            }
            else
            {
                return "en-US";
            }
        }

        public static string UI(string Chosen_Lang, bool Force_Reset_Cache)
        {
            ResetCache = Force_Reset_Cache;
            return UI(Chosen_Lang);
        }
    }
}
