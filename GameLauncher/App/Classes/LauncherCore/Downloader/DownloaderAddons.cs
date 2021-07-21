using GameLauncher.App.Classes.LauncherCore.Global;
using System;
using System.Runtime.Serialization;

namespace GameLauncher.App.Classes.LauncherCore.Downloader
{
    class DownloaderAddons
    {
        /* Check System Language and Return Current Lang for Speech Files */
        public static string SpeechFiles(string Language)
        {
            string CurrentLang = string.IsNullOrWhiteSpace(Language) ? InformationCache.Lang.ThreeLetterISOLanguageName : Language.ToLower();

            if (CurrentLang == "eng") return "en";
            else if (CurrentLang == "ger" || CurrentLang == "deu") return "de";
            else if (CurrentLang == "rus") return "ru";
            else if (CurrentLang == "spa") return "es";
            else return "en";
        }

        public static int SpeechFilesSize()
        {
            string CurrentLang = InformationCache.Lang.ThreeLetterISOLanguageName;

            if (CurrentLang == "eng") return 141805935;
            else if (CurrentLang == "ger" || CurrentLang == "deu") return 105948386;
            else if (CurrentLang == "rus") return 121367723;
            else if (CurrentLang == "spa") return 101540466;
            else return 141805935;
        }
    }

    public delegate void DownloadFinished();

    public delegate void DownloadFailed(Exception ex);

    public delegate void ProgressUpdated(long dowloadLength, long downloadCurrent, long compressedLength, string fileName, int skipdownload = 0);

    public delegate void ShowExtract(string filename, long currentCount, long allFilesCount);

    public delegate void ShowMessage(string message, string header);

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

    public class VerifyCommand : DownloaderCommand
    {
        public VerifyCommand(Downloader downloader) : base(downloader)
        {
        }

        public override void Execute(object[] parameters)
        {
            this._downloader.StartVerification((string)parameters[0], (string)parameters[1], (string)parameters[2], (bool)parameters[3], (bool)parameters[4], (bool)parameters[5]);
        }
    }

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

    [Serializable]
    public class UncompressionException : Exception
    {
        public int mErrorCode;

        public int ErrorCode
        {
            get { return this.mErrorCode; }
        }

        public UncompressionException(int errorCode)
        {
            this.mErrorCode = errorCode;
        }

        public UncompressionException(int errorCode, string message) : base(message)
        {
            this.mErrorCode = errorCode;
        }

        public UncompressionException(int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            this.mErrorCode = errorCode;
        }

        protected UncompressionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
