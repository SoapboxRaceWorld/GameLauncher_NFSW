using SBRW.Launcher.Core.Theme;
using SBRW.Launcher.Core.Theme.Conversion_;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace SBRW.Launcher.RunTime.LauncherCore.Support
{
    /// <summary>
    /// 
    /// </summary>
    internal class FormsFont
    {
        private static Font_Wrapper? Live_Instance { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Primary_Cached() 
        {
            if (Live_Instance == default)
            {
                Live_Instance = new Font_Wrapper();
            }

            return Live_Instance.Font_Regular != null; 
        }
        /// <summary>
        /// Custom System Font
        /// </summary>
        /// <returns>DejaVuSans Font</returns>
        public static FontFamily Primary()
        {
            if (!Primary_Cached())
            {
                using MemoryStream Live_Memory_Cache = new MemoryStream(Embeded_Files.DejaVuSans_Ttf_Bytes());
                Live_Instance.Font_Regular = Live_Instance.GetFontFamily("DejaVuSans.ttf", Live_Memory_Cache);
            }

            return Live_Instance.Font_Regular;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Primary_Bold_Cached()
        {
            if (Live_Instance == default)
            {
                Live_Instance = new Font_Wrapper();
            }

            return Live_Instance.Font_Bold != null;
        }
        /// <summary>
        /// Custom System Font
        /// </summary>
        /// <returns>DejaVuSans-Bold Font</returns>
        public static FontFamily Primary_Bold()
        {
            if (!Primary_Bold_Cached())
            {
                using MemoryStream Live_Memory_Cache = new MemoryStream(Embeded_Files.DejaVuSans_Bold_Ttf_Bytes());
                Live_Instance.Font_Bold = Live_Instance.GetFontFamily("DejaVuSans-Bold.ttf", Live_Memory_Cache);
            }

            return Live_Instance.Font_Bold;
        }
    }
}
