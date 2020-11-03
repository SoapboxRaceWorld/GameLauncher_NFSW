namespace GameLauncher
{
    public abstract class DownloaderCommand
	{
		protected GameLauncher.Downloader _downloader;

		public GameLauncher.Downloader Downloader
		{
			get
			{
				return this._downloader;
			}
		}

		protected DownloaderCommand(GameLauncher.Downloader downloader)
		{
			this._downloader = downloader;
		}

		public abstract void Execute(object[] parameters);
	}
}