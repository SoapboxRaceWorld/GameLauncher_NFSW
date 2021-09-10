using GameLauncher.App.Classes.LauncherCore.Logger;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    /// <summary>
    /// Image loading toolset class which corrects the bug that prevents paletted PNG images with transparency from being loaded as paletted.
    /// </summary>
    /// <remarks>Includes Gray Scale for Bitmap Images</remarks>
    class BitmapHandler
    {
        /// <summary>
        /// Coverts an Image into a Gray Scale
        /// </summary>
        /// <param name="original">Bitmap Image File</param>
        /// <returns>The Bitmap Image in Gray Scale</returns>
        /// Source: https://web.archive.org/web/20130208001434/http://tech.pro:80/tutorial/660/csharp-tutorial-convert-a-color-image-to-grayscale
        public static Bitmap GrayScale(Bitmap Image)
        {
            try
            {
                //Create a blank bitmap the same size as original
                Bitmap newBitmap = new Bitmap(Image.Width, Image.Height);

                //Get a graphics object from the new image
                Graphics g = Graphics.FromImage(newBitmap);

                //Create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(colorMatrix);

                //Draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(Image, new Rectangle(0, 0, Image.Width, Image.Height),
                    0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel, attributes);

                //Dispose the Graphics object
                g.Dispose();
                return newBitmap;
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("SUPPORT CLASS [GrayScale]", null, Error, null, true);
                return null;
            }
        }
    }
}
