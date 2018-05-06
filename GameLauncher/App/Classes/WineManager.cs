using System;
using System.Diagnostics;
using System.IO;

namespace GameLauncher.App.Classes
{
    public class WineManager
    {
		public static bool NeedEmbeddedWine() {
			var version = GetWineVersion();
			if (version == "") {
				return true;
			}
			return Int32.Parse(version.Split('.')[1]) > 1;
		}

		public static bool HasEmbeddedWine() {
			return Directory.Exists("wine");
		}

		public static string GetWineVersion() {
			Process process = new Process();
            process.StartInfo.FileName = "wine";
            process.StartInfo.Arguments = "--version";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            try
            {
                process.Start();
            }
            catch
            {
                Console.WriteLine("Wine not found");
                return "";
            }

            return process.StandardOutput.ReadToEnd().Split(' ')[0].Split('-')[1];
		}

		public static string GetWinePrefix() {
			return Directory.GetCurrentDirectory() + "/wineprefix";
		}

		public static string GetWineDirectory()
        {
            return Directory.GetCurrentDirectory() + "/wine";
        }

		public static string GetUserSettingsPath() {
			string appdata = "";
			if (DetectLinux.NativeLinuxDetected()) {
				appdata = GetWinePrefix() + "/drive_c/users/" + Environment.UserName + "/Application Data";
			} else {
				appdata = Environment.GetEnvironmentVariable("AppData");
			}
			return appdata + "/Need for Speed World/Settings/UserSettings.xml";
		}
    }
}
