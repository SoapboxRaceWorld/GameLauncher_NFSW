using GameLauncher.App.Classes.Logger;
using System;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.ModNet
{
    class ModNetLinksCleanup
    {
        private static readonly IniFile _settingFile = new IniFile("Settings.ini");

        public static void CleanLinks(string linksPath)
        {
            if (File.Exists(linksPath))
            {
                Log.Debug("CLEANLINKS: Found Server Mod Files to remove {Process}");
                string dir = _settingFile.Read("InstallationDirectory");
                foreach (var readLine in File.ReadLines(linksPath))
                {
                    var parts = readLine.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    string loc = parts[0];
                    int type = int.Parse(parts[1]);
                    string realLoc = Path.Combine(dir, loc);
                    if (type == 0)
                    {
                        if (!File.Exists(realLoc))
                        {
                            throw new Exception(".links file includes nonexistent file: " + realLoc);
                        }

                        string origPath = realLoc + ".orig";

                        if (!File.Exists(origPath))
                        {
                            File.Delete(realLoc);
                            continue;
                        }

                        try
                        {
                            File.Delete(realLoc);
                            File.Move(origPath, realLoc);
                        }
                        catch
                        {
                            Log.Error("CLEANLINKS: Error while deleting a file: {realLoc}");
                        }
                    }
                    else
                    {
                        if (!Directory.Exists(realLoc))
                        {
                            Log.Error("CLEANLINKS: .links file includes nonexistent directory: " + realLoc);
                            throw new Exception(".links file includes nonexistent directory: " + realLoc);
                        }
                        Directory.Delete(realLoc, true);
                    }
                }

                File.Delete(linksPath);
            }
        }
    }
}
