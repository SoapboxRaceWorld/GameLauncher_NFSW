using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GameLauncher.App.Classes.LauncherCore.Visuals
{
    class VerticalBanners
    {
        public static string GetFileExtension(String filename)
        {
            return filename.Split('.').Last();
        }

        public static Image Grayscale(String filename)
        {
            if (!File.Exists(filename)) return null;

            try
            {
                using (var fs = new FileStream(filename, FileMode.Open))
                {
                    var bmp = new Bitmap(fs);
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
                    IntPtr ptr = bmpData.Scan0;
                    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                    byte[] rgbValues = new byte[bytes];
                    Marshal.Copy(ptr, rgbValues, 0, bytes);

                    for (int i = 0; i < rgbValues.Length; i += 3)
                    {
                        byte gray = (byte)(rgbValues[i] * .21 + rgbValues[i + 1] * .71 + rgbValues[i + 2] * .071);
                        rgbValues[i] = rgbValues[i + 1] = rgbValues[i + 2] = gray;
                    }

                    Marshal.Copy(rgbValues, 0, ptr, bytes);
                    bmp.UnlockBits(bmpData);
                    return (Bitmap)bmp.Clone();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
