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

        public static string Database(string Text_Request, string Which_Language)
        {
            Log.Debug("DATABASE: Requested: " + Text_Request + " Lang: " + Which_Language);
            try
            {
                if (Lang_Launcher == null || ResetCache)
                {
                    switch (Which_Language.ToLower())
                    {
                        default:
                            Lang_Launcher = new ResourceManager("GameLauncher.App.Languages.English_Texts", Assembly.GetExecutingAssembly());
                            break;
                    }

                    ResetCache = false;
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(Text_Request))
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
