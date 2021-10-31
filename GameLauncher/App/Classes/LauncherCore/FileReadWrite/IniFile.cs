using IniParser;
using IniParser.Model;
using SBRW.Launcher.Core.Classes.Extension.Logging_;
using System;
using System.IO;
using System.Text;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class IniFile
    {
        /* Moved 2 Public functions to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/89c2e19c97be7ebc075203f3d998aa9e701892f6/IniFile.cs */

        public string File_Path;
        readonly string Ini_Header = "GameLauncher";
        public FileIniDataParser File_Parser;
        public IniData File_Data;
        public UTF8Encoding UTF8 = new UTF8Encoding(false);

        public IniFile(string Ini_Path = null)
        {
            File_Path = new FileInfo(Ini_Path ?? Ini_Header + ".ini").FullName.ToString();
            File_Parser = new FileIniDataParser();
            if (File.Exists(File_Path))
            {
                File_Data = File_Parser.ReadFile(File_Path, UTF8);
            }
            else
            {
                if (!File.Exists(File_Path))
                {
                    File.Create(File_Path).Dispose();
                }

                File_Data = new IniData();
            }
        }

        public string Read(string Key_Index)
        {
            return File_Data[Ini_Header][Key_Index];
        }

        public void Write(string Key_Index, string Index_Data)
        {
            try
            {
                if (new FileInfo(File_Path).IsReadOnly)
                {
                    Log.Warning("IniFile: ".ToUpper() + "[Key Write] Ini File is Read-Only -> " + Path.GetFileName(File_Path));
                }
                else
                {
                    File_Data[Ini_Header][Key_Index] = Index_Data;
                    File_Parser.WriteFile(File_Path, File_Data, UTF8);
                }
            }
            finally
            {
                GC.Collect();
            }
        }

        public void DeleteKey(string Key_Index)
        {
            try
            {
                if (new FileInfo(File_Path).IsReadOnly)
                {
                    Log.Warning("IniFile: ".ToUpper() + "[Key Remove] Ini File is Read-Only -> " + Path.GetFileName(File_Path));
                }
                else
                {
                    File_Data[Ini_Header].RemoveKey(Key_Index);
                    File_Parser.WriteFile(File_Path, File_Data, UTF8);
                }
            }
            finally
            {
                GC.Collect();
            }
        }

        public bool KeyExists(string Key_Index)
        {
            return File_Data[Ini_Header].ContainsKey(Key_Index);
        }
    }
}
