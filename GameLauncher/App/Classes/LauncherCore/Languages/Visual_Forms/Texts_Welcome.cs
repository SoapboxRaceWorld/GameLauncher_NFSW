using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.LauncherCore.Languages.Visual_Forms
{
    class Texts_Welcome
    {
        public static string Frontend(string Selection)
        {
            return string.Empty;
        }

        public static string Backend(string Selection)
        {
            switch (Selection)
            {
                case "":
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }

        private static string Database(string Selection, string UI_Element)
        {
            if (Selection == "Frontend")
            {
                switch (FileSettingsSave.Lang ?? "EN".ToUpper())
                {
                    case "EN":
                        return string.Empty;
                    case "ES":
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
            else if (Selection == "Backend")
            {
                switch (FileSettingsSave.Lang ?? "EN".ToUpper())
                {
                    case "EN":
                        switch (UI_Element)
                        {
                            case "WelcomeText_Startup":
                                return string.Empty;
                            case "WelcomeText_Startup1":
                                return "Looks like the Game Launcher failed to Reach our APIs. Clicking 'Manual Bypass' will allow you to continue with the Error";
                            default:
                                return string.Empty;
                        }
                    case "ES":
                        return string.Empty;
                    default:
                        return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
