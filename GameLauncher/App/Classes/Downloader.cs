using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Windows;

namespace CodeProject.Downloader
{
    public class FileDownloader
    {
        private const int downloadBlockSize = 10240*5;
        private bool canceled = false;
        private string downloadingTo;

        public string DownloadingTo
        {
            get { return downloadingTo; }
        }

        public void Cancel()
        {
            this.canceled = true;
        }

        public event DownloadProgressHandler ProgressChanged;

        private IWebProxy proxy = null;
        public IWebProxy Proxy
        {
            get { return proxy; }
            set { proxy = value; }
        }

        public event EventHandler DownloadComplete;

        private void OnDownloadComplete()
        {
            if (this.DownloadComplete != null) this.DownloadComplete(this, new EventArgs());
        }

        public void Download(string url)
        {
            Download(url, "");
        }

        public void Download(string url, string destFolder, string filename = "")
        {
            DownloadData data = null;
            this.canceled = false;

            try
            {
                data = DownloadData.Create(url, destFolder, this.proxy);
                string destFileName = Path.GetFileName(data.Response.ResponseUri.ToString());

                destFolder = destFolder.Replace("file:///", "").Replace("file://", "");

                this.downloadingTo = (filename == "") ? Path.Combine(destFolder, destFileName) : Path.Combine(destFolder, filename);

                if (!File.Exists(downloadingTo))
                {
                    FileStream fs = File.Create(downloadingTo);
                    fs.Close();
                }

                byte[] buffer = new byte[downloadBlockSize];
                int readCount;

                long totalDownloaded = data.StartPoint;
                bool gotCanceled = false;

                while ((int)(readCount = data.DownloadStream.Read(buffer, 0, downloadBlockSize)) > 0)
                {
                    if (canceled)
                    {
                        gotCanceled = true;
                        data.Close();
                        break;
                    }

                    totalDownloaded += readCount;

                    SaveToFile(buffer, readCount, this.downloadingTo);

                    if (data.IsProgressKnown) RaiseProgressChanged(totalDownloaded, data.FileSize);

                    if (canceled)
                    {
                        gotCanceled = true;
                        data.Close();
                        break;
                    }
                }

                if (!gotCanceled)
                    OnDownloadComplete();
            }
            catch (UriFormatException e)
            {
                throw new ArgumentException(
                    String.Format("Could not parse the URL \"{0}\" - it's either malformed or is an unknown protocol.", url), e);
            }
            finally
            {
                if (data != null)
                    data.Close();
            }
        }

        /// <summary>
        /// Download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void Download(List<string> urlList)
        {
            this.Download(urlList, "");
        }
        /// <summary>
        /// Download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void Download(List<string> urlList, string destFolder)
        {
            // validate input
            if (urlList == null)
                throw new ArgumentException("Url list not specified.");

            if (urlList.Count == 0)
                throw new ArgumentException("Url list empty.");

            // try each url in the list.
            // if one succeeds, we are done.
            // if any fail, move to the next.
            Exception ex = null;
            foreach (string s in urlList)
            {
                ex = null;
                try
                {
                    Download(s, destFolder);
                }
                catch (Exception e)
                {
                    ex = e;
                }
                // If we got through that without an exception, we found a good url
                if (ex == null)
                    break;
            }
            if (ex != null)
                throw ex;
        }

        /// <summary>
        /// Asynchronously download a file from the url.
        /// </summary>
        public void AsyncDownload(string url)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new string[] { url, "" });
        }
        /// <summary>
        /// Asynchronously download a file from the url to the destination folder.
        /// </summary>
        public void AsyncDownload(string url, string destFolder)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new string[] { url, destFolder });
        }
        /// <summary>
        /// Asynchronously download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void AsyncDownload(List<string> urlList, string destFolder)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new object[] { urlList, destFolder });
        }
        /// <summary>
        /// Asynchronously download a file from a list or URLs. If downloading from one of the URLs fails,
        /// another URL is tried.
        /// </summary>
        public void AsyncDownload(List<string> urlList)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(this.WaitCallbackMethod), new object[] { urlList, "" });
        }
        /// <summary>
        /// A WaitCallback used by the AsyncDownload methods.
        /// </summary>
        private void WaitCallbackMethod(object data)
        {
            // Can either be a string array of two strings (url and dest folder),
            // or an object array containing a list<string> and a dest folder
            if (data is string[])
            {
                String[] strings = data as String[];
                this.Download(strings[0], strings[1]);
            }
            else
            {
                Object[] list = data as Object[];
                List<String> urlList = list[0] as List<String>;
                String destFolder = list[1] as string;
                this.Download(urlList, destFolder);
            }
        }
        private void SaveToFile(byte[] buffer, int count, string fileName)
        {
            FileStream f = null;

            try {
                f = File.Open(fileName, FileMode.Append, FileAccess.Write);
                f.Write(buffer, 0, count);
            } catch (ArgumentException e) {
                throw new ArgumentException(
                    String.Format("Error trying to save file \"{0}\": {1}", fileName, e.Message), e);
            } finally {
                if (f != null)
                    f.Close();
            }
        }
        private void RaiseProgressChanged(long current, long target)
        {
            if (this.ProgressChanged != null)
                this.ProgressChanged(this, new DownloadEventArgs(target, current));
        }
    }

    /// <summary>
    /// Constains the connection to the file server and other statistics about a file
    /// that's downloading.
    /// </summary>
    class DownloadData
    {
        private WebResponse response;

        private Stream stream;
        private long size;
        private long start;

        private IWebProxy proxy = null;

        public static DownloadData Create(string url, string destFolder)
        {
            return Create(url, destFolder, null);
        }

        public static DownloadData Create(string url, string destFolder, IWebProxy proxy)
        {

            // This is what we will return
            DownloadData downloadData = new DownloadData();
            downloadData.proxy = proxy;

            long urlSize = downloadData.GetFileSize(url);
            downloadData.size = urlSize;

            WebRequest req = downloadData.GetRequest(url);
            try
            {
                downloadData.response = (WebResponse)req.GetResponse();
            }
            catch (Exception e)
            {
                throw new ArgumentException(String.Format(
                    "Error downloading \"{0}\": {1}", url, e.Message), e);
            }

            // Check to make sure the response isn't an error. If it is this method
            // will throw exceptions.
            ValidateResponse(downloadData.response, url);

            // Take the name of the file given to use from the web server.
            String fileName = System.IO.Path.GetFileName(downloadData.response.ResponseUri.ToString());

            String downloadTo = Path.Combine(destFolder, fileName);

            // If we don't know how big the file is supposed to be,
            // we can't resume, so delete what we already have if something is on disk already.
            if (!downloadData.IsProgressKnown && File.Exists(downloadTo))
                File.Delete(downloadTo);

            if (downloadData.IsProgressKnown && File.Exists(downloadTo))
            {
                // We only support resuming on http requests
                if (!(downloadData.Response is HttpWebResponse))
                {
                    File.Delete(downloadTo);
                }
                else
                {
                    // Try and start where the file on disk left off
                    downloadData.start = new FileInfo(downloadTo).Length;

                    // If we have a file that's bigger than what is online, then something 
                    // strange happened. Delete it and start again.
                    if (downloadData.start > urlSize)
                        File.Delete(downloadTo);
                    else if (downloadData.start < urlSize)
                    {
                        // Try and resume by creating a new request with a new start position
                        downloadData.response.Close();
                        req = downloadData.GetRequest(url);
                        ((HttpWebRequest)req).AddRange((int)downloadData.start);
                        ((HttpWebRequest)req).Headers["X-MTNTR-HEADER-VAL"] = (downloadData.start).ToString();
                        downloadData.response = req.GetResponse();

                        if (((HttpWebResponse)downloadData.Response).StatusCode != HttpStatusCode.PartialContent)
                        {
                            // They didn't support our resume request. 
                            File.Delete(downloadTo);
                            downloadData.start = 0;
                        }
                    }
                }
            }
            return downloadData;
        }

        // Used by the factory method
        private DownloadData()
        {
        }

        private DownloadData(WebResponse response, long size, long start)
        {
            this.response = response;
            this.size = size;
            this.start = start;
            this.stream = null;
        }

        /// <summary>
        /// Checks whether a WebResponse is an error.
        /// </summary>
        /// <param name="response"></param>
        private static void ValidateResponse(WebResponse response, string url)
        {
            if (response is HttpWebResponse)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                // If it's an HTML page, it's probably an error page. Comment this
                // out to enable downloading of HTML pages.
                if (httpResponse.ContentType.Contains("text/html") || httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ArgumentException(
                        String.Format("Could not download \"{0}\" - a web page was returned from the web server.",
                        url));
                }
            }
            else if (response is FtpWebResponse)
            {
                FtpWebResponse ftpResponse = (FtpWebResponse)response;
                if (ftpResponse.StatusCode == FtpStatusCode.ConnectionClosed)
                    throw new ArgumentException(
                        String.Format("Could not download \"{0}\" - FTP server closed the connection.", url));
            }
            // FileWebResponse doesn't have a status code to check.
        }

        /// <summary>
        /// Checks the file size of a remote file. If size is -1, then the file size
        /// could not be determined.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="progressKnown"></param>
        /// <returns></returns>
        private long GetFileSize(string url)
        {
            WebResponse response = null;
            long size = -1;
            try
            {
                response = GetRequest(url).GetResponse();
                size = response.ContentLength;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            return size;
        }

        private WebRequest GetRequest(string url)
        {
            //WebProxy proxy = WebProxy.GetDefaultProxy();
            WebRequest request = WebRequest.Create(url);
            if (request is HttpWebRequest)
            {
                request.Credentials = CredentialCache.DefaultCredentials;
                Uri result = request.Proxy.GetProxy(new Uri("http://www.google.com"));
            }

            if (this.proxy != null)
            {
                request.Proxy = this.proxy;
            }

            return request;
        }

        public void Close()
        {
            this.response.Close();
        }

        #region Properties
        public WebResponse Response
        {
            get { return response; }
            set { response = value; }
        }
        public Stream DownloadStream
        {
            get
            {
                if (this.start == this.size)
                    return Stream.Null;
                if (this.stream == null)
                    this.stream = this.response.GetResponseStream();
                return this.stream;
            }
        }
        public long FileSize
        {
            get
            {
                return this.size;
            }
        }
        public long StartPoint
        {
            get
            {
                return this.start;
            }
        }
        public bool IsProgressKnown
        {
            get
            {
                // If the size of the remote url is -1, that means we
                // couldn't determine it, and so we don't know
                // progress information.
                return this.size > -1;
            }
        }
        #endregion
    }

    /// <summary>
    /// Progress of a downloading file.
    /// </summary>
    public class DownloadEventArgs : EventArgs
    {
        private int percentDone;
        private string downloadState;
        private long totalFileSize;

        public long TotalFileSize
        {
            get { return totalFileSize; }
            set { totalFileSize = value; }
        }
        private long currentFileSize;

        public long CurrentFileSize
        {
            get { return currentFileSize; }
            set { currentFileSize = value; }
        }

        public DownloadEventArgs(long totalFileSize, long currentFileSize)
        {
            this.totalFileSize = totalFileSize;
            this.currentFileSize = currentFileSize;

            this.percentDone = (int)((((double)currentFileSize) / totalFileSize) * 100);
        }

        public DownloadEventArgs(string state)
        {
            this.downloadState = state;
        }

        public DownloadEventArgs(int percentDone, string state)
        {
            this.percentDone = percentDone;
            this.downloadState = state;
        }

        public int PercentDone
        {
            get
            {
                return this.percentDone;
            }
        }

        public string DownloadState
        {
            get
            {
                return this.downloadState;
            }
        }
    }
    public delegate void DownloadProgressHandler(object sender, DownloadEventArgs e);
}