using SBRW.Launcher.App.Classes.LauncherCore.Logger;
using System;
using System.IO;
using System.Reflection;

namespace SBRW.Launcher.App.Classes.LauncherCore.FileReadWrite
{
    class ExtractResource
    {
        public static byte[] AsByte(String File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return null;
            }
            else
            {
                try
                {
                    Assembly TheRun = Assembly.GetExecutingAssembly();
                    using (Stream LiveStream = TheRun.GetManifestResourceStream(File_Name))
                    {
                        if (LiveStream == null) { return null; }
                        else
                        {
                            byte[] ba = new byte[LiveStream.Length];
                            LiveStream.Read(ba, 0, ba.Length);
                            return ba;
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Extract Resource AsByte", null, Error, null, true);
                    return null;
                }
            }
        }
    }
}
