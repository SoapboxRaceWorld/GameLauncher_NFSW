using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Font_;
using SBRW.Launcher.Core.Theme.Conversion_;
using System.Drawing;
using System.IO;

namespace SBRW.Launcher.App.Classes.LauncherCore.Support
{
    /// <summary>
    /// 
    /// </summary>
    internal class FormsFont
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Primary_Cached() => Launcher_Value.Launcher_Font != null;
        /// <summary>
        /// Custom System Font
        /// </summary>
        /// <returns>DejaVuSans Font</returns>
        public static FontFamily Primary()
        {
            if (!Primary_Cached())
            {
                using (MemoryStream Live_Memory_Cache = new MemoryStream(Embeded_Files.DejaVuSans_Ttf_Bytes()))
                {
                    Launcher_Value.Launcher_Font = Font_Wrapper.Instance.GetFontFamily("DejaVuSans.ttf", Live_Memory_Cache);
                }
            }

            return Launcher_Value.Launcher_Font;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Primary_Bold_Cached() => Launcher_Value.Launcher_Font_Bold != null;
        /// <summary>
        /// Custom System Font
        /// </summary>
        /// <returns>DejaVuSans-Bold Font</returns>
        public static FontFamily Primary_Bold()
        {
            if (!Primary_Bold_Cached())
            {
                using (MemoryStream Live_Memory_Cache = new MemoryStream(Embeded_Files.DejaVuSans_Bold_Ttf_Bytes()))
                {
                    Launcher_Value.Launcher_Font_Bold = Font_Wrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf", Live_Memory_Cache);
                }
            }

            return Launcher_Value.Launcher_Font_Bold;
        }
    }
}
