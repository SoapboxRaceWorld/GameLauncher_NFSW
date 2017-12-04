using GameLauncher.App.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace GameLauncherReborn {
    class Language {
        public static List<Object> getLanguages() {
            List<Object> languages = new List<Object>();
            languages.Add(new { Text = "Default", Value = "Default" });

            foreach (var substring in Directory.GetFiles("Languages", "*.lng")) {
                if (!String.IsNullOrEmpty(substring) && substring != "Languages\\Default.lng") {
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
                if(File.Exists("Languages\\Default.lng")) {
                    languageContent = File.ReadAllLines("Languages\\Default.lng");
                } else {
                    string newlang;
                    try { newlang = Directory.GetFiles("Languages", "*.lng")[0]; } catch { newlang = null; }

                    if(String.IsNullOrEmpty(newlang)) {
                        WebClientWithTimeout client = new WebClientWithTimeout();
                        client.DownloadFile("https://raw.githubusercontent.com/metonator/GameLauncher_NFSW-translations/master/Languages/English.lng", "Languages\\Default.lng");
                    }
                    
                    languageContent = File.ReadAllLines(Directory.GetFiles("Languages", "*.lng")[0]);
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