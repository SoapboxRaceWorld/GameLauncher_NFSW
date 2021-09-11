using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class Banners
    {
        public static string GetFileExtension(String filename)
        {
            return filename.Split('.').Last().ToLower();
        }

        public static Image Grayscale(String filename)
        {
            if (!File.Exists(filename)) return null;

            try
            {
                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    Bitmap bmp = new Bitmap(fs);
                    return BitmapHandler.GrayScale(bmp);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("BANNER", null, Error, null, true);
                return null;
            }
        }
    }
}
