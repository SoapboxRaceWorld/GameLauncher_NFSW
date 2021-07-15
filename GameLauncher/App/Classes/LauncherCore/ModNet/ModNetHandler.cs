using System;
using System.IO;
using System.Net;
using System.Text;
using GameLauncher.App.Classes.InsiderKit;
using GameLauncher.App.Classes.LauncherCore.APICheckers;
using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes.LauncherCore.ModNet
{
    class ModNetHandler
    {
        public static void FileANDFolder(string Paths)
        {
            String[] FoldersToRemove = new string[]
            {
                /* Folders */
                "modules"
            };

            String[] FoldersToCreate = new string[]
            {
                /* Folders */
                "scripts"
            };

            String[] FilesToRemove = new string[] 
            {
                /* Legacy Files */
                "modules/udpcrc.soapbox.module",
                "modules/udpcrypt1.soapbox.module",
                "modules/udpcrypt2.soapbox.module",
                "modules/xmppsubject.soapbox.module",
                "scripts/global.ini",
                "lightfx.dll",
                "ModManager.dat",
                "PocoFoundation.dll",
                "PocoNet.dll"
            };

            foreach (string Folder in FoldersToRemove)
            {
                if (Directory.Exists(Path.Combine(Paths, Folder)))
                {
                    try
                    {
                        Directory.Delete(Path.Combine(Paths, Folder), true);
                    }
                    catch (Exception Error)
                    {
                        Log.Error("LAUNCHER: Deleting Folder " + Folder + " -> " + Error.Message);
                        Log.ErrorIC("LAUNCHER: " + Error.HResult);
                        Log.ErrorFR("LAUNCHER: " + Error.ToString());
                    }
                }
            }

            foreach (string Folder in FoldersToCreate)
            {
                if (!Directory.Exists(Path.Combine(Paths, Folder)))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.Combine(Paths, Folder));
                    }
                    catch (Exception Error)
                    {
                        Log.Error("LAUNCHER: Creating Folder " + Folder + " -> " + Error.Message);
                        Log.ErrorIC("LAUNCHER: " + Error.HResult);
                        Log.ErrorFR("LAUNCHER: " + Error.ToString());
                    }
                }
            }

            foreach (string Files in FilesToRemove)
            {
                if (File.Exists(Path.Combine(Paths, Files)))
                {
                    try
                    {
                        File.Delete(Path.Combine(Paths, Files));
                    }
                    catch (Exception Error)
                    {
                        Log.Error("LAUNCHER: Deleting File " + Files + " -> " + Error.Message);
                        Log.ErrorIC("LAUNCHER: " + Error.HResult);
                        Log.ErrorFR("LAUNCHER: " + Error.ToString());
                    }
                }
            }
        }

        public static bool Supported()
        {
            switch (APIChecker.CheckStatus(InformationCache.SelectedServerData.IpAddress + "/Modding/GetModInfo"))
            {
                case APIStatus.Online:
                    return true;
                default:
                    return false;
            }
        }

        public static int FileErrors = 0;

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

                            if (IsSymbolic(realLoc) && File.Exists(realLoc))
                            {
                                File.Delete(realLoc);
                                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                                {
                                    Log.Info("CLEANLINKS: Removed Symbolic File " + realLoc);
                                }
                                continue;
                            }
                            else if (!File.Exists(realLoc))
                            {
                                FileErrors++;

                                if (!File.Exists(origPath))
                                {
                                    Log.Warning("CLEANLINKS: .links file includes nonexistent file, but there is no .orig file: [" + realLoc + "] Skipping File");
                                    continue;
                                }
                                else if (File.Exists(origPath))
                                {
                                    Log.Error("CLEANLINKS: Found .orig file that should not be present. Deleting File: " + origPath);
                                    File.Delete(origPath);
                                    continue;
                                }
                                else
                                {
                                    Log.Error("CLEANLINKS: .links file includes nonexistent file. Skipping File: " + realLoc);
                                    continue;
                                }
                            }
                            else if (!File.Exists(origPath))
                            {
                                if (File.Exists(realLoc))
                                {
                                    File.Delete(realLoc);
                                }
                                continue;
                            }

                            try
                            {
                                if (File.Exists(realLoc))
                                {
                                    File.Delete(realLoc);
                                }
                                
                                File.Move(origPath, realLoc);
                            }
                            catch (Exception Error)
                            {
                                FileErrors++;

                                Log.Error("CLEANLINKS: Error while deleting a file: {realLoc}");
                                Log.Error("CLEANLINKS: " + Error.Message);
                                Log.ErrorIC("CLEANLINKS: " + Error.HResult);
                                Log.ErrorFR("CLEANLINKS: " + Error.ToString());
                            }
                        }
                        else
                        {
                            if (IsSymbolic(realLoc) && Directory.Exists(realLoc))
                            {
                                Directory.Delete(realLoc, true);
                                if (EnableInsiderDeveloper.Allowed() || EnableInsiderBetaTester.Allowed())
                                {
                                    Log.Info("CLEANLINKS: Removed Symbolic Folder " + realLoc);
                                }
                                continue;
                            }
                            else if (!Directory.Exists(realLoc))
                            {
                                FileErrors++;

                                Log.Error("CLEANLINKS: .links file includes nonexistent directory. Skipping Directory: " + realLoc);
                                continue;
                            }

                            if (Directory.Exists(realLoc))
                            {
                                Directory.Delete(realLoc, true);
                            }
                        }
                    }

                    if (FileErrors > 0)
                    {
                        FileSettingsSave.GameIntegrity = "Bad";
                        FileSettingsSave.SaveSettings();
                        FileErrors = 0;
                    }

                    File.Delete(linksPath);
                }
            }
            catch (Exception Error)
            {
                Log.Error("CLEANLINKS: " + Error.Message);
                Log.ErrorIC("CLEANLINKS: " + Error.HResult);
                Log.ErrorFR("CLEANLINKS: " + Error.ToString());
            }
        }

        public static bool IsSymbolic(string path)
        {
            try
            {
                FileInfo pathInfo = new FileInfo(path);
                return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch
            {
                return false;
            }
        }
    }
}