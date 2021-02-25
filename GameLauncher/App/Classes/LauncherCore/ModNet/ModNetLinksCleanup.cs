using System;
using System.IO;
using System.Windows.Forms;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes.LauncherCore.ModNet
{
    class ModNetLinksCleanup
    {
        public static void CleanLinks(string linksPath)
        {
            try
            {
                if (File.Exists(linksPath))
                {
                    Log.Info("CLEANLINKS: Found Server Mod Files to remove {Process}");
                    string dir = FileSettingsSave.GameInstallation;
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
                            string origPath = realLoc + ".orig";

                            if (!File.Exists(realLoc))
                            {
                                if (!File.Exists(origPath))
                                {
                                    Log.Warning("CLEANLINKS: .links file includes nonexistent file, but there is no .orig file: [" + realLoc + "] skipping file");
                                    continue;
                                }
                                else if (File.Exists(origPath))
                                {
                                    Log.Error("CLEANLINKS: Found .orig file that should not be present: " + origPath);
                                    DialogResult skipFolder = MessageBox.Show(null, "Found .orig file that should not be present:\n" +
                                        origPath, "GameLauncher", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    Log.Error("CLEANLINKS: .links file includes nonexistent file: " + realLoc);
                                    DialogResult skipFolder = MessageBox.Show(null, ".links file includes nonexistent file:\n" +
                                        realLoc + "\n\nChoose \"Yes\" to Skip File \nChoose \"No\" to Close Launcher", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                                    if (skipFolder == DialogResult.Yes)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                            }

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
                            catch (Exception ex)
                            {
                                Log.Error("CLEANLINKS: Error while deleting a file: {realLoc}");
                                Log.Error("CLEANLINKS: " + ex.Message);
                            }
                        }
                        else
                        {
                            if (!Directory.Exists(realLoc))
                            {
                                Log.Error("CLEANLINKS: .links file includes nonexistent directory: " + realLoc);

                                DialogResult skipFolder = MessageBox.Show(null, ".links file includes nonexistent file:\n" +
                                    realLoc + "\n\nChoose \"Yes\" to Skip File \nChoose \"No\" to Close Launcher", "GameLauncher", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);

                                if (skipFolder == DialogResult.Yes)
                                {
                                    continue;
                                }
                                else
                                {
                                    Environment.Exit(0);
                                }
                            }
                            Directory.Delete(realLoc, true);
                        }
                    }

                    File.Delete(linksPath);
                }
            }
            catch (Exception ex)
            {
                Log.Error("CLEANLINKS: " + ex.Message);
            }
        }
    }
}
