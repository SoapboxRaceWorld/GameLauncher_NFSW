using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Font_;
using System.Drawing;
using System.IO;

namespace SBRW.Launcher.App.Classes.LauncherCore.Support
{
    internal class FormsFont
    {
        public static bool Primary_Cached() => Launcher_Value.Launcher_Font != null;
        public static FontFamily Primary()
        {
            if (Launcher_Value.Launcher_Font == null)
            {
                MemoryStream Live_Memory_Cache = new MemoryStream(Core.Theme.Properties.Resources.DejaVuSans)
                {
                    Position = 0
                };

                Launcher_Value.Launcher_Font = Font_Wrapper.Instance.GetFontFamily("DejaVuSans.ttf", Live_Memory_Cache);
            }

            return Launcher_Value.Launcher_Font;
        }
        public static bool Primary_Bold_Cached() => Launcher_Value.Launcher_Font_Bold != null;
        public static FontFamily Primary_Bold()
        {
            if (Launcher_Value.Launcher_Font_Bold == null)
            {
                MemoryStream Live_Memory_Cache = new MemoryStream(Core.Theme.Properties.Resources.DejaVuSans_Bold)
                {
                    Position = 0
                };

                Launcher_Value.Launcher_Font_Bold = Font_Wrapper.Instance.GetFontFamily("DejaVuSans-Bold.ttf", Live_Memory_Cache);
            }

            return Launcher_Value.Launcher_Font_Bold;
        }
    }
}
