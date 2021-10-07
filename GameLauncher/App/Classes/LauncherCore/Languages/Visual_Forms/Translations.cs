using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Reflection;
using System.Resources;

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
                    switch (Application_Language)
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
                        return Lang_Launcher.GetString(Text_Request);
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
    }
}
