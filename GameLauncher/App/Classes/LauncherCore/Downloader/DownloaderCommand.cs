namespace GameLauncher.App.Classes.LauncherCore.Downloader
{
    public abstract class DownloaderCommand
    {
        protected Downloader _downloader;

        public Downloader Downloader
        {
            get { return this._downloader; }
        }

        protected DownloaderCommand(Downloader downloader)
        {
            this._downloader = downloader;
        }

        public abstract void Execute(object[] parameters);
    }
}