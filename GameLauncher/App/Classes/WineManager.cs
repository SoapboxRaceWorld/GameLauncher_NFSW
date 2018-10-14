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
			return Directory.Exists(GetWineDirectory());
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
			if (DetectLinux.UnixDetected()) {
				appdata = GetWinePrefix() + "/drive_c/users/" + Environment.UserName + "/Application Data";
			} else {
				appdata = Environment.GetEnvironmentVariable("AppData");
			}
			return appdata + "/Need for Speed World/Settings/UserSettings.xml";
		}

        public static void SetProcessParams(ref ProcessStartInfo psi) {
            psi.UseShellExecute = false;
            psi.EnvironmentVariables.Add("WINEDLLOVERRIDES", "mscoree,mshtml=");
            psi.EnvironmentVariables.Add("WINEPREFIX", GetWinePrefix());
            var wine = GetWineDirectory();
            if (Directory.Exists(wine))
            {
                psi.EnvironmentVariables.Add("WINEVERPATH", wine);
                psi.EnvironmentVariables.Add("WINESERVER", wine + "/bin/wineserver");
                psi.EnvironmentVariables.Add("WINELOADER", wine + "/bin/wine");
                psi.EnvironmentVariables.Add("WINEDLLPATH", wine + "/lib/wine/fakedlls");
                psi.EnvironmentVariables.Add("LD_LIBRARY_PATH", wine + "/lib");
                psi.FileName = wine + "/bin/wine";
            }
            else
            {
                psi.FileName = "wine";
            }
        }

        public static void InitWinePrefix() {
            var regPath = Path.Combine(Path.GetTempPath(), "gamelauncher_wine_opts.reg");
            File.WriteAllBytes(regPath, ExtractResource.AsByte("GameLauncher.wine_opts.reg"));

            var psi = new ProcessStartInfo();
            SetProcessParams(ref psi);
            psi.Arguments = "regedit \"" + regPath + "\"";
            Process.Start(psi).WaitForExit();
            File.Delete(regPath);
        }
    }
}
