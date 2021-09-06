using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.IO;
using System.Reflection;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
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

        public static String AsString(String File_Name)
        {
            if (string.IsNullOrWhiteSpace(File_Name))
            {
                return String.Empty;
            }
            else
            {
                try
                {
                    Assembly TheRun = Assembly.GetExecutingAssembly();
                    using (Stream LiveStream = TheRun.GetManifestResourceStream(File_Name))
                    {
                        if (LiveStream == null) { return String.Empty; }
                        else
                        {
                            using (StreamReader StreamViewer = new StreamReader(LiveStream))
                            {
                                return StreamViewer.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("Extract Resource AsString", null, Error, null, true);
                    return String.Empty;
                }
            }
        }
    }
}
