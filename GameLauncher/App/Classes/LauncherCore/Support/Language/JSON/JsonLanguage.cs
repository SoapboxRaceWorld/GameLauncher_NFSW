using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameLauncher.App.Classes.LauncherCore.Support.Language.JSON
{
    class JsonLanguage
    {
        public class TextCore
        {
            public string Element { get; set; }

            public string Text { get; set; }
        }

        public string Version { get; set; }
        public string Name { get; set; }

        public List<TextCore> MainScreen { get; set; }
        public List<TextCore> SettingsScreen { get; set; }
        public List<TextCore> RegisterScreen { get; set; }
        public List<TextCore> SelectScreen { get; set; }
        public List<TextCore> UpdatePopupScreen { get; set; }
        public List<TextCore> USXEScreen { get; set; }
        public List<TextCore> VerifyHashScreen { get; set; }
        public List<TextCore> WelcomeScreen { get; set; }
    }
}
