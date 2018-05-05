using System;
using System.Diagnostics;
using System.IO;

namespace GameLauncher.App.Classes
{
    public class WineManager
    {
		public static bool NeedEmbeddedWine() {
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
				return true;
			}

			var versionString = process.StandardOutput.ReadToEnd().Split(' ')[0].Split('-')[1];
			var version = versionString.Split('.');
			return Int32.Parse(version[1]) > 1;
		}

		public static string GetWinePrefix() {
			return Directory.GetCurrentDirectory() + "/wineprefix";
		}

		public static string GetWineDirectory()
        {
            return Directory.GetCurrentDirectory() + "/wine";
        }
    }
}
