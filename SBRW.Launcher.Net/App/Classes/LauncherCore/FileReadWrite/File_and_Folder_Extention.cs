using SBRW.Launcher.App.Classes.LauncherCore.Logger;

namespace SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite
{
    public class File_and_Folder_Extention
    {
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
    }
}
