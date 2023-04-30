using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using System.Runtime.CompilerServices;

namespace SBRW.Launcher.RunTime.LauncherCore.FileReadWrite
{
    public static class File_and_Folder_Extention
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_Found(this long Folder_Size)
        {
            return Folder_Size.GameInstall_RU() || Folder_Size.GameInstall_DE() || 
                Folder_Size.GameInstall_Default() || Folder_Size.GameInstall_ES() || 
                Folder_Size.GameInstall_FR() || Folder_Size.GameInstall_TW();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_TW(this long Folder_Size)
        {
            return InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("tw") && (Folder_Size >= 3226671567);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_FR(this long Folder_Size)
        {
            return InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("fr") && (Folder_Size >= 3255567909);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_ES(this long Folder_Size)
        {
            return InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("es") && (Folder_Size >= 3251629477);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_RU(this long Folder_Size)
        {
            return InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("ru") && (Folder_Size >= 3273498661);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_DE(this long Folder_Size)
        {
            return InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("de") && (Folder_Size >= 3257512293);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Size"></param>
        /// <returns></returns>
        public static bool GameInstall_Default(this long Folder_Size)
        {
            return (InformationCache.Lang.TwoLetterISOLanguageName.ToLowerInvariant().Contains("en") && (Folder_Size >= 3296810469)) || (Folder_Size >= 3296810469);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Folder_Name"></param>
        /// <param name="Strings_Or_Folders"></param>
        /// <returns></returns>
        public static bool GetFolderExclusion(string Folder_Name, string[] Strings_Or_Folders)
        {
            if (string.IsNullOrWhiteSpace(Folder_Name))
            {
                return true;
            }
            else
            {
                bool Final_Results = false;

                foreach (string Folder_List_Name in Strings_Or_Folders)
                {
                    if (Folder_Name.StartsWith(Folder_List_Name))
                    {
                        Final_Results = true;
                    }
                }

                return Final_Results;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Directory_Info"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static long GetDirectorySize(System.IO.DirectoryInfo Directory_Info, bool Recursive = true)
        {

            long Start_Directory_Size = default;

            try
            {
                if (Directory_Info == null)
                {
                    /* Return -1 while Directory does not exist. */
                    Start_Directory_Size = -1;
                }
                else if (!Directory_Info.Exists)
                {
                    /* Return 0 while Directory does not exist. */
                    Start_Directory_Size = 0;
                }
                else
                {
                    /* Add size of files in the Current Directory to main size. */
                    foreach (System.IO.FileInfo File_Info in Directory_Info.GetFiles())
                    {
                        System.Threading.Interlocked.Add(ref Start_Directory_Size, File_Info.Length);
                    }

                    /* Loop on Sub Direcotries in the Current Directory and Calculate it's files size. */
                    if (Recursive)
                    {
                        System.Threading.Tasks.Parallel.ForEach(Directory_Info.GetDirectories(), (Sub_Directory) =>
                        System.Threading.Interlocked.Add(ref Start_Directory_Size, GetDirectorySize(Sub_Directory, Recursive)));
                    }
                }
            }
            catch (System.Exception Error)
            {
                LogToFileAddons.OpenLog("Game Folder Checks", string.Empty, Error, string.Empty, true);
                Start_Directory_Size = -1;
            }

            /* Return full Size of this Directory. */
            return Start_Directory_Size;  
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Directory_Info"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        /// <remarks>
        /// Full Install - 3859168701
        /// EN Install - 3859168701
        /// DE Install - 3859168701
        /// ES Install - 3859168701
        /// FR Install - 3859168701
        /// RU Install - 3859168701
        /// TW Install - 3859168701
        /// </remarks>
        public static long GetDirectorySize_GameFiles(System.IO.DirectoryInfo Directory_Info, bool Recursive = true)
        {

            long Start_Directory_Size = default;

            try
            {
                if (Directory_Info == null)
                {
                    /* Return -1 while Directory does not exist. */
                    Start_Directory_Size = -1;
                }
                else if (!Directory_Info.Exists)
                {
                    /* Return 0 while Directory does not exist. */
                    Start_Directory_Size = 0;
                }
                else
                {
                    /* Add size of files in the Current Directory to main size. */
                    foreach (System.IO.FileInfo File_Info in Directory_Info.GetFiles())
                    {
                        string File_Name = File_Info.Extension.ToLowerInvariant();
                        
                        if (!(File_Name.EndsWith(".txt", System.StringComparison.InvariantCultureIgnoreCase) ||
                            File_Name.EndsWith(".dmp", System.StringComparison.InvariantCultureIgnoreCase) ||
                            File_Name.EndsWith(".orig", System.StringComparison.InvariantCultureIgnoreCase) ||
                            File_Name.EndsWith(".bak", System.StringComparison.InvariantCultureIgnoreCase) ||
                            File_Name.EndsWith(".mods", System.StringComparison.InvariantCultureIgnoreCase) ||
                            File_Name.EndsWith(".asi", System.StringComparison.InvariantCultureIgnoreCase)))
                        {
                            System.Threading.Interlocked.Add(ref Start_Directory_Size, File_Info.Length);
                        }
                    }

                    /* Loop on Sub Direcotries in the Current Directory and Calculate it's files size. */
                    if (Recursive)
                    {
                        System.Threading.Tasks.Parallel.ForEach(Directory_Info.GetDirectories(), (Sub_Directory) =>
                        {
                            if (Sub_Directory != null)
                            {
                                if (!GetFolderExclusion(Sub_Directory.Name, new string[]
                                {
                                ".",
                                "scripts",
                                "MODS"
                                }))
                                {
                                    System.Threading.Interlocked.Add(ref Start_Directory_Size, GetDirectorySize_GameFiles(Sub_Directory, Recursive));
                                }
                            }
                        });
                    }
                }
            }
            catch (System.Exception Error)
            {
                LogToFileAddons.OpenLog("Game Folder Checks Game Files", string.Empty, Error, string.Empty, true);
                Start_Directory_Size = -1;
            }

            /* Return full Size of this Directory. */
            return Start_Directory_Size;
        }
    }
}
