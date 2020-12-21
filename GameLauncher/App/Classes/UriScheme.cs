using Microsoft.Win32;
using System;
using System.Text;

namespace GameLauncher.App.Classes
{
    class UriScheme {
        //Login Param
        public static String Email = String.Empty;
        public static String Password = String.Empty;
        public static bool ForceLogin = false;

        //LaunchGame
        public static String UserID = String.Empty;
        public static String LoginToken = String.Empty;
        public static String ServerIP = String.Empty;
        public static bool ForceGame = false;

        //Global
        public static bool StartMinimized = false;

        public UriScheme(string Parse) {
            if (Parse.StartsWith("nfswlaunch://")) {
                Uri parseLauncherUri = new Uri(Parse);

                switch(parseLauncherUri.Host) {
                    case "auth":
                        try {
                            String base64dec = parseLauncherUri.Segments[1];
                            var base64EncodedBytes = Convert.FromBase64String(base64dec);
                            String decoded = Encoding.UTF8.GetString(base64EncodedBytes);

                            String[] split = decoded.Split(new string[] { "__" }, StringSplitOptions.None);

                            UserID = split[0];
                            LoginToken = split[1];
                            ServerIP = split[2];
                            ForceGame = true;
                            StartMinimized = true;
                        } catch { }
                        break;
                    default:
                        break;
                }
            }
        }

        public static void InstallCommandLineArguments(String CurrentLauncherPath) {
            Registry.ClassesRoot.CreateSubKey(@"nfswlaunch").SetValue(null, "URL:NFSWLaunch Protocol");
            Registry.ClassesRoot.CreateSubKey(@"nfswlaunch").SetValue("URL Protocol", String.Empty);
            Registry.ClassesRoot.CreateSubKey(@"nfswlaunch").SetValue("LauncherVersion", System.Windows.Forms.Application.ProductVersion);
            Registry.ClassesRoot.CreateSubKey(@"nfswlaunch\DefaultIcon").SetValue(null, CurrentLauncherPath + ",1");
            Registry.ClassesRoot.CreateSubKey(@"nfswlaunch\shell\open\command").SetValue(null, "\"" + CurrentLauncherPath + "\" --parse \"%1\"");
        }

        public static bool IsCommandLineArgumentsInstalled() {
            Console.WriteLine("IsCommandLineArgumentsInstalled: " + ((String)Registry.ClassesRoot.CreateSubKey(@"nfswlaunch\shell\open\command").GetValue("LauncherVersion") != String.Empty));
            return (String)Registry.ClassesRoot.CreateSubKey(@"nfswlaunch\shell\open\command").GetValue("LauncherVersion") != String.Empty;
        }
    }
}
