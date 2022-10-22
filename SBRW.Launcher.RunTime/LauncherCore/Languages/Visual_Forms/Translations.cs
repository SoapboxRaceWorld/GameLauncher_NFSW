using SBRW.Launcher.RunTime.InsiderKit;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Extension.Logging_;
using System;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms
{
    class Translations
    {
        private static ResourceManager? Lang_Launcher = null;

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
                    Lang_Launcher = UI(Application_Language) switch
                    {
                        _ => new ResourceManager("SBRW.Launcher.App.Languages.English_Texts", Assembly.GetExecutingAssembly()),
                    };
                    ResetCache = false;
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(Text_Request) && Lang_Launcher != null)
                    {
                        return Regex.Unescape(Lang_Launcher.GetString(Text_Request)??"Languages Not Found");
                    }
                    else
                    {
                        return "Languages Programer ERROR";
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Translations Database Selection", string.Empty, Error, string.Empty, true);
                    return "Languages Program ERROR";
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("Translations Database", string.Empty, Error, string.Empty, true);
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
