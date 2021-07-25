using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Support.Language.JSON;
using Newtonsoft.Json;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.Support.Language
{
    class Texts
    {
        public static string LangFileExtention;
        public static string GetLang(string SystemLang, string From, string ControlID)
        {
            if (string.IsNullOrWhiteSpace(LangFileExtention))
            {
                switch (SystemLang)
                {
                    case "en-UK":
                        LangFileExtention = "UK";
                        break;
                    default:
                        LangFileExtention = "EN";
                        break;
                }
            }
            string JsonFile = File.ReadAllText(Path.Combine(Locations.LauncherFolder, "Languages", LangFileExtention + ".json"));

            JsonLanguage Json = JsonConvert.DeserializeObject<JsonLanguage>(JsonFile);

            switch (From)
            {
                case "MainScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.MainScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "RegisterScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.RegisterScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "SelectScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.SelectScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "SettingsScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.SettingsScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "UpdatePopupScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.UpdatePopupScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "USXEScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.USXEScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "VerifyHashScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.VerifyHashScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                case "WelcomeScreen":
                    foreach (JsonLanguage.TextCore TextEntrie in Json.WelcomeScreen)
                    {
                        if (TextEntrie.Element == ControlID)
                        {
                            return TextEntrie.Text;
                        }
                    }

                    return ControlID;
                default:
                    return ControlID;
            }
        }
    }
}
