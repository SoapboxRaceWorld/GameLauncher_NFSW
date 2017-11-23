using GameLauncher.App.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GameLauncherReborn {
    class Language {
        public static List<Object> getLanguages() {
            List<Object> languages = new List<Object>();

            foreach (var substring in Directory.GetFiles("Languages", "*.lng")) {
                if (!String.IsNullOrEmpty(substring)) {
                    languages.Add(new { Text = substring.Replace("Languages\\", "").Replace(".lng", ""), Value = substring.Replace("Languages\\", "").Replace(".lng", "") });
                }
            }

            return languages;
        }

        public static string getLangString(string ID, string LangIdentification) {
            string languageText = "";
            String[] languageContent;

            try {
                languageContent = File.ReadAllLines("Languages\\" + LangIdentification + ".lng");
            } catch {
                //In case language will not be found
                if(File.Exists("Languages\\English.lng")) {
                    languageContent = File.ReadAllLines("Languages\\English.lng");
                } else {
                    languageContent = null;
                }
            }

            foreach (var substring in languageContent) {
                if (!String.IsNullOrEmpty(substring)) {
                    String[] substrings2 = substring.Split(new string[] { "=" }, StringSplitOptions.None);
                    if (substrings2[0] == ID) {
                        languageText = substrings2[1];
                    }
                }
            }

            if (!String.IsNullOrEmpty(languageText)) {
                return languageText;
            } else {
                return ID;
            }
        }
    }
}