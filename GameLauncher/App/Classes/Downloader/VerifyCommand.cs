namespace GameLauncher
{
    public class VerifyCommand : DownloaderCommand
	{
		public VerifyCommand(GameLauncher.Downloader downloader) : base(downloader)
		{
		}

		public override void Execute(object[] parameters)
		{
			this._downloader.StartVerification((string)parameters[0], (string)parameters[1], (string)parameters[2], (bool)parameters[3], (bool)parameters[4], (bool)parameters[5]);
		}
	}
}