using System.IO;
using System.Reflection;
using IniParser;
using IniParser.Model;

namespace GameLauncher.App.Classes {
	class IniFile {
        string Path;
		string EXE = Assembly.GetExecutingAssembly().GetName().Name;
        FileIniDataParser Parser;
        IniData Data;

        public IniFile(string IniPath = null) {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
			Parser = new FileIniDataParser();
			if (File.Exists(Path))
			{
				Data = Parser.ReadFile(Path);
			} else {
				Data = new IniData();
			}
        }

        public string Read(string Key, string Section = null) {
			if (Section == null)
			{
				return Data.Global[Key];
			}
			else
			{
				return Data[Section][Key];
			}
        }

        public void Write(string Key, string Value, string Section = null) {
			if (Section == null)
            {
                Data.Global[Key] = Value;
            }
            else
            {
				Data[Section][Key] = Value;
            }
            Parser.WriteFile(Path, Data);
        }

        public void DeleteKey(string Key, string Section = null) {
			if (Section == null)
            {
				Data.Global.RemoveKey(Key);
            }
            else
            {
				Data[Section].RemoveKey(Key);
            }
            Parser.WriteFile(Path, Data);
        }

        public void DeleteSection(string Section) {
			Data.Sections.RemoveSection(Section);
            Parser.WriteFile(Path, Data);
        }

        public bool KeyExists(string Key, string Section = null) {
			if (Section == null)
			{
				return Data.Global.ContainsKey(Key);
			}
			return Data[Section].ContainsKey(Key);
		}
    }
}
