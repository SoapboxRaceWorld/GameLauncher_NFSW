﻿using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

//Source of this class: https://stackoverflow.com/questions/217902/reading-writing-an-ini-file
namespace GameLauncher.App.Classes {
    class IniFile {
        string Path;
        string EXE = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public IniFile(string IniPath = null) {
            Path = new FileInfo(IniPath ?? EXE + ".ini").FullName.ToString();
        }

        public string Read(string Key, string Section = null) {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? EXE, Key, "", RetVal, 255, Path);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null) {
            WritePrivateProfileString(Section ?? EXE, Key, Value, Path);
        }

        public void DeleteKey(string Key, string Section = null) {
            Write(Key, null, Section ?? EXE);
        }

        public void DeleteSection(string Section = null) {
            Write(null, null, Section ?? EXE);
        }

        public bool KeyExists(string Key, string Section = null) {
            return Read(Key, Section).Length > 0;
        }
    }
}
