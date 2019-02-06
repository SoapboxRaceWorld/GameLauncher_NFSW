using System;
using System.Runtime.Serialization;

namespace GameLauncher
{
	[Serializable]
	public class DownloaderException : Exception
	{
		public DownloaderException()
		{
		}

		public DownloaderException(string message) : base(message)
		{
		}

		public DownloaderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DownloaderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}