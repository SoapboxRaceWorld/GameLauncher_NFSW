using SBRW.Launcher.RunTime.LauncherCore.Logger;
using System;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.RunTime.LauncherCore.FileReadWrite
{
    class ExtractResource
    {
        public static byte[]? AsByte(string File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return default;
            }
            else
            {
                try
                {
                    Assembly? TheRun = Assembly.GetExecutingAssembly();
                    if (TheRun != null)
                    {
                        if (TheRun.GetManifestResourceStream(File_Name) != null)
                        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                            using Stream LiveStream = TheRun.GetManifestResourceStream(File_Name);
                            if (LiveStream == null)
                            {
                                return default;
                            }
                            else
                            {
                                byte[] ba = new byte[LiveStream.Length];
                                LiveStream.Read(ba, 0, ba.Length);
                                return ba;
                            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                        }
                        else
                        {
                            return default;
                        }
                    }
                    else
                    {
                        return default;
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Extract Resource AsByte", string.Empty, Error, string.Empty, true);
                    return default;
                }
            }
        }
    }
}
