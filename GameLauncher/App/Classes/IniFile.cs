using System;
using System.IO;
using System.Reflection;
using System.Windows;
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

        public string Read(string Key) {
            return Data[EXE][Key];
        }

	    public int ReadInt(string Key)
	    {
	        return Convert.ToInt32(Data[EXE][Key]);
	    }

        public void Write(string Key, string Value) {
			try {
				Data[EXE][Key] = Value;
				Parser.WriteFile(Path, Data);
			} catch(Exception ex) {
                MessageBox.Show( String.Format("Failed to write {0} = {1} to {2}\n{3}", Key, Value, Path, ex.Message));
            }
        }

        public void DeleteKey(string Key) {
            try {
			    Data[EXE].RemoveKey(Key);
                Parser.WriteFile(Path, Data);
            } catch { }
        }

        public void DeleteSection(string Section) {
			Data.Sections.RemoveSection(Section);
            Parser.WriteFile(Path, Data);
        }

        public bool KeyExists(string Key) {
			return Data[EXE].ContainsKey(Key);
		}
    }
}
