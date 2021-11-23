using SBRW.Launcher.Core.Reference.Json_.Newtonsoft_;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    class SelectedServer
    {
        public static Json_List_Server List;
        public static Json_List_Server Data
        {
            get { return List; }
            set { List = value; }
        }
    }
}
