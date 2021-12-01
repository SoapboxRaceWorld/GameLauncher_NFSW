using SBRW.Launcher.App.UI_Forms.Main_Screen;
using System;
using System.Windows.Forms;

namespace SBRW.Launcher.Net
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if !NETFRAMEWORK
#if NET6_0
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
#endif
            ApplicationConfiguration.Initialize();
#else
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#endif
            Application.Run(new Screen_Main());
        }
    }
}