using SBRW.Launcher.RunTime.LauncherCore.Logger;

namespace SBRW.Launcher.RunTime.LauncherCore.FileReadWrite
{
    public class File_and_Folder_Extention
    {
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

        public static long GetDirectorySize(System.IO.DirectoryInfo Directory_Info, bool Recursive = true)
        {

            long Start_Directory_Size = default;

            try
            {
                if (Directory_Info == null || !Directory_Info.Exists)
                {
                    /* Return 0 while Directory does not exist. */
                    return Start_Directory_Size;
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

        public static long GetDirectorySize_GameFiles(System.IO.DirectoryInfo Directory_Info, bool Recursive = true)
        {

            long Start_Directory_Size = default;

            try
            {
                if (Directory_Info == null || !Directory_Info.Exists)
                {
                    /* Return 0 while Directory does not exist. */
                    return Start_Directory_Size;
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
