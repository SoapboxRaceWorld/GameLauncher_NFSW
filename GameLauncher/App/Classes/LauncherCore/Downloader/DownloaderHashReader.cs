using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;

namespace GameLauncher.App.Classes.LauncherCore.Downloader
{
    internal class DownloaderHashReader
    {
        public const int MaxWorkers = 3;
        public const string HashFileName = "HashFile";
        readonly Dictionary<string, DownloaderHashReader.HashTuple> _fileList;
        readonly Queue<string> _queueHash;
        private readonly static object _queueHashLock;
        private static int _workerCount;
        readonly bool _useCache = true;
        readonly static DownloaderHashReader _instance;

        internal static DownloaderHashReader Instance
        {
            get { return DownloaderHashReader._instance; }
        }

        static DownloaderHashReader()
        {
            DownloaderHashReader._queueHashLock = new object();
            DownloaderHashReader._workerCount = 0;
            DownloaderHashReader._instance = new DownloaderHashReader();
        }

        private DownloaderHashReader()
        {
            this._useCache = true;
            this._fileList = new Dictionary<string, DownloaderHashReader.HashTuple>();
            this._queueHash = new Queue<string>();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs args)
        {
            while (true)
            {
                lock (DownloaderHashReader._queueHashLock)
                {
                    if (this._queueHash.Count == 0)
                    {
                        DownloaderHashReader._workerCount--;
                        break;
                    }
                }
                string str = null;
                lock (DownloaderHashReader._queueHashLock)
                {
                    str = this._queueHash.Dequeue();
                }
                string base64String = null;
                if (File.Exists(str))
                {
                    if (string.IsNullOrEmpty(this._fileList[str].Old))
                    {
                        try
                        {
                            using (FileStream fileStream = File.OpenRead(str))
                            {
                                using (MD5 mD5 = MD5.Create())
                                {
                                    base64String = Convert.ToBase64String(mD5.ComputeHash(fileStream));
                                }
                            }
                        }
                        catch (Exception exception1)
                        {
                            Exception exception = exception1;
                        }
                    }
                    else
                    {
                        base64String = this._fileList[str].Old;
                    }
                }
                lock (this._fileList[str])
                {
                    this._fileList[str].Old = base64String;
                }
            }
        }

        private void BackgroundWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
            }
        }

        public void Clear()
        {
            lock (this._queueHash)
            {
                this._queueHash.Clear();
            }
            while (DownloaderHashReader._workerCount > 0)
            {
                Thread.Sleep(100);
            }
            this._fileList.Clear();
        }

        public string GetHashOld(string fileName)
        {
            string empty = string.Empty;
            while (true)
            {
                lock (this._fileList[fileName])
                {
                    empty = this._fileList[fileName].Old;
                }
                if (empty != string.Empty)
                {
                    break;
                }
                Thread.Sleep(100);
            }
            return empty;
        }

        public bool HashesMatch(string fileName)
        {
            bool @new;
            try
            {
                while (true)
                {
                    lock (this._fileList[fileName])
                    {
                        if (this._fileList[fileName].Old != string.Empty)
                        {
                            @new = this._fileList[fileName].New == this._fileList[fileName].Old;
                            break;
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                return false;
            }
            return @new;
        }

        public void Start(XmlDocument doc, string patchPath, string hashFileNameSuffix, int maxWorkers)
        {
            foreach (XmlNode xmlNodes in doc.SelectNodes("/index/fileinfo"))
            {
                string innerText = xmlNodes.SelectSingleNode("path").InnerText;
                string str = xmlNodes.SelectSingleNode("file").InnerText;
                if (!string.IsNullOrEmpty(patchPath))
                {
                    int num = innerText.IndexOf("/");
                    innerText = (num < 0 ? patchPath : innerText.Replace(innerText.Substring(0, num), patchPath));
                }
                string str1 = string.Concat(innerText, "/", str);
                if (xmlNodes.SelectSingleNode("hash") != null)
                {
                    this._fileList.Add(str1, new DownloaderHashReader.HashTuple(string.Empty, xmlNodes.SelectSingleNode("hash").InnerText));
                    this._queueHash.Enqueue(str1);
                }
                else
                {
                    this._fileList.Add(str1, new DownloaderHashReader.HashTuple(null, null));
                }
            }
            if (this._useCache && File.Exists(string.Concat("HashFile", hashFileNameSuffix)))
            {
                FileStream fileStream = null;
                CryptoStream cryptoStream = null;
                StreamReader streamReader = null;
                try
                {
                    try
                    {
                        DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider()
                        {
                            Key = Encoding.ASCII.GetBytes("12345678"),
                            IV = Encoding.ASCII.GetBytes("12345678")
                        };
                        ICryptoTransform cryptoTransform = dESCryptoServiceProvider.CreateDecryptor();
                        fileStream = new FileStream(string.Concat("HashFile", hashFileNameSuffix), FileMode.Open);
                        cryptoStream = new CryptoStream(fileStream, cryptoTransform, CryptoStreamMode.Read);
                        streamReader = new StreamReader(cryptoStream);
                        string str2 = null;
                        while (true)
                        {
                            string str3 = streamReader.ReadLine();
                            str2 = str3;
                            if (str3 == null)
                            {
                                break;
                            }
                            string[] strArrays = str2.Split(new char[] { '\t' });
                            string str4 = strArrays[0];
                            if (this._fileList.ContainsKey(str4) && File.Exists(str4) && long.Parse(strArrays[2]) == (new FileInfo(str4)).LastWriteTime.Ticks)
                            {
                                this._fileList[str4].Old = strArrays[1];
                                this._fileList[str4].Ticks = long.Parse(strArrays[2]);
                            }
                        }
                    }
                    catch (CryptographicException cryptographicException1)
                    {
                        CryptographicException cryptographicException = cryptographicException1;
                        streamReader = null;
                        cryptoStream = null;
                        this._fileList.Clear();
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                        this._fileList.Clear();
                    }
                }
                finally
                {
                    if (streamReader != null)
                    {
                        streamReader.Close();
                    }
                    if (cryptoStream != null)
                    {
                        cryptoStream.Close();
                    }
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                    File.Delete(string.Concat("HashFile", hashFileNameSuffix));
                }
            }
            DownloaderHashReader._workerCount = 0;
            while (DownloaderHashReader._workerCount < maxWorkers && this._queueHash.Count > 0)
            {
                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorker_DoWork);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerComplete);
                backgroundWorker.RunWorkerAsync();
                DownloaderHashReader._workerCount++;
            }
        }

        public void WriteHashCache(string hashFileNameSuffix, bool writeOldHashes)
        {
            lock (this._fileList)
            {
                FileStream fileStream = null;
                CryptoStream cryptoStream = null;
                StreamWriter streamWriter = null;
                try
                {
                    try
                    {
                        fileStream = new FileStream(string.Concat("HashFile", hashFileNameSuffix), FileMode.Create);
                        DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
                        byte[] bytes = Encoding.ASCII.GetBytes("12345678");
                        byte[] numArray = bytes;
                        dESCryptoServiceProvider.IV = bytes;
                        dESCryptoServiceProvider.Key = numArray;
                        cryptoStream = new CryptoStream(fileStream, dESCryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
                        streamWriter = new StreamWriter(cryptoStream);
                        foreach (string key in this._fileList.Keys)
                        {
                            string empty = string.Empty;
                            empty = (!writeOldHashes ? this._fileList[key].New : this._fileList[key].Old);
                            if (!File.Exists(key) || string.IsNullOrEmpty(empty))
                            {
                                continue;
                            }
                            DateTime lastWriteTime = (new FileInfo(key)).LastWriteTime;
                            streamWriter.WriteLine(string.Format("{0}\t{1}\t{2}", key, empty, lastWriteTime.Ticks));
                        }
                    }
                    catch (Exception exception1)
                    {
                        Exception exception = exception1;
                    }
                }
                finally
                {
                    if (streamWriter != null)
                    {
                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                    if (cryptoStream != null)
                    {
                        cryptoStream.Close();
                    }
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
        }

        private class HashTuple
        {
            public string Old;
            public string New;
            public long Ticks;

            public HashTuple(string oldHash, string newHash, long ticks)
            {
                this.Old = oldHash;
                this.New = newHash;
                this.Ticks = ticks;
            }

            public HashTuple(string oldHash, string newHash) : this(oldHash, newHash, DateTime.Now.AddYears(1).Ticks)
            {
            }
        }
    }
}