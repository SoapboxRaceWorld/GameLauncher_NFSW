using GameLauncher.App.Classes;
using GameLauncherReborn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace GameLauncher
{
    public class Downloader
    {
        private const int LZMAOutPropsSize = 5;

        private const int LZMALengthSize = 8;

        private const int LZMAHeaderSize = 13;

        private const int HashThreads = 3;

        private const int DownloadThreads = 3;

        private const int DownloadChunks = 16;

        private ISynchronizeInvoke mFE;

        private Thread mThread;

        private ProgressUpdated mProgressUpdated;

        private DownloadFinished mDownloadFinished;

        private DownloadFailed mDownloadFailed;

		private ShowMessage mShowMessage;

		private ShowExtract mShowExtract;

		private static string mCurrentLocalVersion = string.Empty;

        private static string mCurrentServerVersion = string.Empty;

        private bool mDownloading;

        private int mHashThreads;

        private DownloadManager mDownloadManager;

        private static XmlDocument mIndexCached = null;

		private static bool mStopFlag = false;

		public static Label label2;

		public bool Downloading {
            get {
                return this.mDownloading;
            }
        }

        public ProgressUpdated ProgressUpdated {
            get {
                return this.mProgressUpdated;
            }
            set {
                this.mProgressUpdated = value;
            }
        }

        public DownloadFinished DownloadFinished {
            get {
                return this.mDownloadFinished;
            }
            set {
                this.mDownloadFinished = value;
            }
        }

        public DownloadFailed DownloadFailed {
            get {
                return this.mDownloadFailed;
            }
            set {
                this.mDownloadFailed = value;
            }
        }

		public ShowMessage ShowMessage {
			get {
				return this.mShowMessage;
			}
			set {
				this.mShowMessage = value;
			}
		}

		public ShowExtract ShowExtract {
			get {
				return this.mShowExtract;
			}
			set {
				this.mShowExtract = value;
			}
		}

		public static string ServerVersion {
            get {
                return Downloader.mCurrentServerVersion;
            }
        }

        public Downloader(ISynchronizeInvoke fe) : this(fe, 3, 3, 16)
        {
        }

        public Downloader(ISynchronizeInvoke fe, int hashThreads, int downloadThreads, int downloadChunks)
        {
            this.mHashThreads = hashThreads;
            this.mFE = fe;
            this.mDownloadManager = new DownloadManager(downloadThreads, downloadChunks);
		}

        public void StartDownload(string indexUrl, string package, string patchPath, bool calculateHashes, bool useIndexCache, ulong downloadSize)
        {
            Downloader.mStopFlag = false;
            this.mThread = new Thread(new ParameterizedThreadStart(this.Download));
            string[] parameter = new string[]
            {
                indexUrl,
                package,
                patchPath,
                calculateHashes.ToString(),
                useIndexCache.ToString(),
                downloadSize.ToString()
            };
            this.mThread.Start(parameter);
        }

        public void StartVerification(string indexUrl, string package, string patchPath, bool stopOnFail, bool clearHashes, bool writeHashes)
        {
            Downloader.mStopFlag = false;
            this.mThread = new Thread(new ParameterizedThreadStart(this.Verify));
            string[] parameter = new string[]
            {
                indexUrl,
                package,
                patchPath,
                stopOnFail.ToString(),
                clearHashes.ToString(),
                writeHashes.ToString()
            };
            this.mThread.Start(parameter);
        }

        public void Stop()
        {
            Downloader.mStopFlag = true;
            if (this.mDownloadManager != null && this.mDownloadManager.ManagerRunning)
            {
                this.mDownloadManager.CancelAllDownloads();
            }
        }

        private void Downloader_DownloadFileCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            string arg = e.UserState.ToString();
            if (!e.Cancelled && e.Error == null)
            {
                //Downloader.mLogger.DebugFormat("File '{0}' downloaded", arg);
                return;
            }
            //MessageBox.Show("Error downloading file '" + arg + "'");
            if (e.Error != null)
            {
                //MessageBox.Show("Downloader_DownloadFileCompleted Exception: " + e.Error.ToString());
                
            }
        }

        private XmlDocument GetIndexFile(string url, bool useCache)
        {
            XmlDocument result;
            try
            {
                if (useCache && Downloader.mIndexCached != null)
                {
                    result = Downloader.mIndexCached;
                }
                else
                {

                    try {
                        WebClient webClient = new WebClient();
                        webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(this.Downloader_DownloadFileCompleted);
                        string tempFileName = Path.GetTempFileName();
                        webClient.DownloadFileAsync(new Uri(url), tempFileName);
                        while (webClient.IsBusy)
                        {
                            if (Downloader.mStopFlag)
                            {
                                webClient.CancelAsync();
                                result = null;
                                return result;
                            }
                            Thread.Sleep(100);
                        }
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.Load(tempFileName);
                        Downloader.mIndexCached = xmlDocument;
                        result = xmlDocument;
                    } catch {
                        result = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetIndexFile Exception: " + ex.ToString());
                result = null;
            }
            return result;
        }

        private void Download(object parameters)
        {
            this.mDownloading = true;
            string[] array = (string[])parameters;
            byte[] array2 = null;
            XmlNodeList xmlNodeList = null;
            string text = array[0];
            string text2 = array[1];
            if (!string.IsNullOrEmpty(text2))
            {
                text = text + "/" + text2;
            }
            string text3 = array[2];
            bool flag = bool.Parse(array[3]);
            bool useCache = bool.Parse(array[4]);
            ulong num = ulong.Parse(array[5]);
            try
            {
                XmlDocument indexFile = this.GetIndexFile(text + "/index.xml", useCache);
                if (indexFile == null)
                {
                    ISynchronizeInvoke arg_AE_0 = this.mFE;
                    Delegate arg_AE_1 = this.mDownloadFailed;
                    object[] args = new object[1];
                    arg_AE_0.BeginInvoke(arg_AE_1, args);
                }
                else
                {
                    long num2 = long.Parse(indexFile.SelectSingleNode("/index/header/length").InnerText);
                    long num3 = 0L;
                    long num4;
                    if (num == 0uL)
                    {
                        num4 = long.Parse(indexFile.SelectSingleNode("/index/header/compressed").InnerText);
                    }
                    else
                    {
                        num4 = (long)num;
                    }
                    long num5 = 0L;
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
                    webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                    webClient.Headers.Add("Accept-Encoding", "gzip,deflate");
                    webClient.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                    int num6 = 1;
                    array2 = null;
                    xmlNodeList = indexFile.SelectNodes("/index/fileinfo");
                    this.mDownloadManager.Initialize(indexFile, text);
                    if (flag)
                    {
                        HashManager.Instance.Clear();
                        HashManager.Instance.Start(indexFile, text3, text2 + ".hsh", this.mHashThreads);
                    }
                    int num7 = 0;
                    List<string> list = new List<string>();
                    int i = 0;
                    bool flag2 = false;
                    int num11;
					long fileschecked = 0;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        XmlNodeList xmlNodeList2 = xmlNode.SelectNodes("compressed");
                        int num8;
                        if (xmlNodeList2.Count == 0)
                        {
                            num8 = int.Parse(xmlNode.SelectNodes("length")[0].InnerText);
                        }
                        else
                        {
                            num8 = int.Parse(xmlNodeList2[0].InnerText);
                        }
                        num7 = ((num8 > num7) ? num8 : num7);
                        string text4 = xmlNode.SelectSingleNode("path").InnerText;
                        if (!string.IsNullOrEmpty(text3))
                        {
                            int num9 = text4.IndexOf("/");
                            if (num9 >= 0)
                            {
                                text4 = text4.Replace(text4.Substring(0, num9), text3);
                            }
                            else
                            {
                                text4 = text3;
                            }
                        }
                        string innerText = xmlNode.SelectSingleNode("file").InnerText;
                        string fileName = text4 + "/" + innerText;
                        int num10 = int.Parse(xmlNode.SelectSingleNode("section").InnerText);
                        num11 = int.Parse(xmlNode.SelectSingleNode("offset").InnerText);
                        if (flag)
                        {
                            if (list.Count == 0)
                            {
                                i = num10;
                            }
                            while (i <= num10)
                            {
                                list.Insert(0, string.Format("{0}/section{1}.dat", text, i));
                                i++;
                            }
                        }
                        else if (!HashManager.Instance.HashesMatch(fileName))
                        {
                            if (i <= num10)
                            {
                                if (list.Count == 0)
                                {
                                    i = num10;
                                }
                                while (i <= num10)
                                {
                                    list.Insert(0, string.Format("{0}/section{1}.dat", text, i));
                                    i++;
                                }
                            }
                            flag2 = true;
                        }
                        else
                        {
                            if (flag2)
                            {
                                int num12 = num10;
                                if (num11 == 0)
                                {
                                    num12--;
                                }
                                while (i <= num12)
                                {
                                    list.Insert(0, string.Format("{0}/section{1}.dat", text, i));
                                    i++;
                                }
                            }
                            if (i < num10)
                            {
                                i = num10;
                            }
                            flag2 = false;
                        }
                    }
                    foreach (string current in list)
                    {
                        this.mDownloadManager.ScheduleFile(current);
                    }
                    list.Clear();
                    list = null;
                    num11 = 0;
                    this.mDownloadManager.Start();
                    byte[] array3 = new byte[num7];
                    byte[] array4 = new byte[13];
                    int num13 = 0;
                    foreach (XmlNode xmlNode2 in xmlNodeList)
                    {
						//fileschecked++;

						if (Downloader.mStopFlag)
                        {
                            break;
                        }
                        string text5 = xmlNode2.SelectSingleNode("path").InnerText;
                        string innerText2 = xmlNode2.SelectSingleNode("file").InnerText;
                        if (!string.IsNullOrEmpty(text3))
                        {
                            int num14 = text5.IndexOf("/");
                            if (num14 >= 0)
                            {
                                text5 = text5.Replace(text5.Substring(0, num14), text3);
                            }
                            else
                            {
                                text5 = text3;
                            }
                        }
                        string text6 = text5 + "/" + innerText2;
                        int num15 = int.Parse(xmlNode2.SelectSingleNode("length").InnerText);
                        int num16 = 0;
                        XmlNode xmlNode3 = xmlNode2.SelectSingleNode("compressed");
                        if (xmlNode2.SelectSingleNode("section") != null && num6 < int.Parse(xmlNode2.SelectSingleNode("section").InnerText))
                        {
                            num6 = int.Parse(xmlNode2.SelectSingleNode("section").InnerText);
                        }
                        string text7 = null;
                        if (xmlNode2.SelectSingleNode("hash") != null && HashManager.Instance.HashesMatch(text6))
                        {
                            num16 += num15;
                            if (xmlNode3 != null)
                            {
                                if (num == 0uL)
                                {
                                    num3 += (long)int.Parse(xmlNode3.InnerText);
                                }
                                num5 += (long)int.Parse(xmlNode3.InnerText);
                                num11 += int.Parse(xmlNode3.InnerText);
                            }
                            else
                            {
                                if (num == 0uL)
                                {
                                    num3 += (long)num15;
                                }
                                num5 += (long)num15;
                                num11 += num15;
                            }
                            if (this.mProgressUpdated != null)
                            {
                                object[] args2 = new object[]
                                {
                                    num2,
                                    num3,
                                    num4,
                                    text6,
                                    0
                                };

                                this.mFE.Invoke(this.mProgressUpdated, args2);
                            }
                            int num17 = int.Parse(xmlNode2.SelectSingleNode("section").InnerText);
                            if (num13 != num17)
                            {
                                for (int j = num13 + 1; j < num17; j++)
                                {
                                    this.mDownloadManager.CancelDownload(string.Format("{0}/section{1}.dat", text, j));
                                }
                                num13 = num17 - 1;
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(text5);
                            FileStream fileStream = File.Create(text6);
                            int num18 = num15;
                            if (xmlNode3 != null)
                            {
                                num18 = int.Parse(xmlNode3.InnerText);
                            }
                            int k = 0;
                            bool flag3 = false;
                            int num19 = 13;
                            while (k < num18)
                            {
                                if (array2 == null || num11 >= array2.Length)
                                {
                                    if (xmlNode2.SelectSingleNode("offset") != null && !flag3)
                                    {
                                        num11 = int.Parse(xmlNode2.SelectSingleNode("offset").InnerText);
                                    }
                                    else
                                    {
                                        num11 = 0;
                                    }
                                    text7 = string.Format("{0}/section{1}.dat", text, num6);
                                    for (int l = num13 + 1; l < num6; l++)
                                    {
                                        this.mDownloadManager.CancelDownload(string.Format("{0}/section{1}.dat", text, l));
                                    }
                                    array2 = null;
                                    GC.Collect();
                                    array2 = this.mDownloadManager.GetFile(text7);
                                    if (array2 == null)
                                    {
                                        //MessageBox.Show("DownloadManager returned a null buffer for file '" + text7 + "', aborting");
                                        if (this.mDownloadFailed != null)
                                        {
                                            if (!Downloader.mStopFlag)
                                            {
                                                this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                                                {
                                                    new Exception("DownloadManager returned a null buffer")
                                                });
                                            }
                                            else
                                            {
                                                ISynchronizeInvoke arg_887_0 = this.mFE;
                                                Delegate arg_887_1 = this.mDownloadFailed;
                                                object[] args = new object[1];
                                                arg_887_0.BeginInvoke(arg_887_1, args);
                                            }
                                        }
                                        return;
                                    }
                                    num13 = num6;
                                    num5 += (long)array2.Length;
                                    num6++;
                                    if (!this.mDownloadManager.GetStatus(string.Format("{0}/section{1}.dat", text, num6)).HasValue && num5 < num4)
                                    {
                                        this.mDownloadManager.ScheduleFile(string.Format("{0}/section{1}.dat", text, num6));
                                    }
                                }
                                else
                                {
                                    if (num18 - k > array2.Length - num11)
                                    {
                                        text7 = string.Format("{0}/section{1}.dat", text, num6);
                                        this.mDownloadManager.ScheduleFile(text7);
                                        flag3 = true;
                                    }
                                    int num20 = Math.Min(array2.Length - num11, num18 - k);
                                    if (num19 != 0)
                                    {
                                        if (xmlNode3 != null)
                                        {
                                            int num21 = Math.Min(num19, num20);
                                            Buffer.BlockCopy(array2, num11, array4, 13 - num19, num21);
                                            Buffer.BlockCopy(array2, num11 + num21, array3, 0, num20 - num21);
                                            num19 -= num21;
                                        }
                                        else
                                        {
                                            Buffer.BlockCopy(array2, num11, array3, 0, num20);
                                            num19 = 0;
                                        }
                                    }
                                    else
                                    {
                                        Buffer.BlockCopy(array2, num11, array3, k - ((xmlNode3 != null) ? 13 : 0), num20);
                                    }
                                    num11 += num20;
                                    k += num20;
                                    num3 += (long)num20;
                                }
                                if (this.mProgressUpdated != null)
                                {
                                    object[] args3 = new object[]
                                    {
                                        num2,
                                        num3,
                                        num4,
                                        text6,
                                        0
                                    };

                                    this.mFE.BeginInvoke(this.mProgressUpdated, args3);
                                }
                            }
                            if (xmlNode3 != null)
                            {
                                if (!Downloader.IsLzma(array4))
                                {
                                    MessageBox.Show("Compression algorithm not recognized" + text7);
                                    throw new DownloaderException("Compression algorithm not recognized: " + text7);
                                }
                                fileStream.Close();
                                fileStream.Dispose();
                                //(IntPtr)num15;
                                IntPtr outPropsSize = new IntPtr(5);
                                byte[] array5 = new byte[5];
                                for (int m = 0; m < 5; m++)
                                {
                                    array5[m] = array4[m];
                                }
                                long num22 = 0L;
                                for (int n = 0; n < 8; n++)
                                {
                                    num22 += (long)((long)array4[n + 5] << 8 * n);
                                }
                                if (num22 != (long)num15)
                                {
                                    MessageBox.Show("Compression data length in header '" + num22 + "' != than in metadata '" + num15 + "'");
                                    throw new DownloaderException("Compression data length in header '" + num22 + "' != than in metadata '" + num15 + "'");
                                }
                                int num23 = num18;
                                num18 -= 13;
                                IntPtr intPtr = new IntPtr(num18);
                                IntPtr value = new IntPtr(num22);
                                int num24 = LZMA.LzmaUncompressBuf2File(text6, ref value, array3, ref intPtr, array5, outPropsSize);


                                //TODO: use total file lenght and extracted file length instead of files checked and total array size.
                                fileschecked =+ num3;

                                object[] xxxxxx = new object[] { text6, fileschecked, num4};
								this.mFE.BeginInvoke(this.mShowExtract, xxxxxx);

								if (num24 != 0)
                                {
                                    MessageBox.Show("Decompression returned " + num24);
                                    throw new UncompressionException(num24, "Decompression returned " + num24);
                                }
                                if (value.ToInt32() != num15)
                                {
                                    MessageBox.Show("Decompression returned different size '" + value.ToInt32() + "' than metadata '" + num15 + "'");
                                    throw new DownloaderException("Decompression returned different size '" + value.ToInt32() + "' than metadata '" + num15 + "'");
                                }
                                num16 += (int)value;
                            }
                            else
                            {
                                fileStream.Write(array3, 0, num15);
                                num16 += num15;
                            }
                            if (fileStream != null)
                            {
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                        }
                    }
                    if (!Downloader.mStopFlag)
                    {
                        HashManager.Instance.WriteHashCache(text2 + ".hsh", false);
                    }
                    if (Downloader.mStopFlag)
                    {
                        if (this.mDownloadFailed != null)
                        {
                            ISynchronizeInvoke arg_D16_0 = this.mFE;
                            Delegate arg_D16_1 = this.mDownloadFailed;
                            object[] args = new object[1];
                            arg_D16_0.BeginInvoke(arg_D16_1, args);
                        }
                    }
                    else if (this.mDownloadFinished != null)
                    {
                        this.mFE.BeginInvoke(this.mDownloadFinished, null);
                    }
                }
            }
            catch (DownloaderException ex)
            {
                MessageBox.Show("Download DownloaderException: " + ex.ToString());
                if (this.mDownloadFailed != null)
                {
                    try
                    {
                        this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                        {
                            ex
                        });
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show("Download Exception: " + ex2.ToString());
                if (this.mDownloadFailed != null)
                {
                    try
                    {
                        this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                        {
                            ex2
                        });
                    }
                    catch
                    {
                    }
                }
            }
            finally
            {
                if (flag)
                {
                    HashManager.Instance.Clear();
                }
                this.mDownloadManager.Clear();
                array2 = null;
                xmlNodeList = null;
                GC.Collect();
                this.mDownloading = false;
            }
        }

        private void Verify(object parameters)
        {
            string[] array = (string[])parameters;
            string str = array[0].Trim();
            string text = array[1].Trim();
            if (!string.IsNullOrEmpty(text))
            {
                str = str + "/" + text;
            }
            string text2 = array[2].Trim();
            bool flag = bool.Parse(array[3]);
            bool flag2 = bool.Parse(array[4]);
            bool flag3 = bool.Parse(array[5]);
            bool flag4 = false;
            try
            {
                XmlDocument indexFile = this.GetIndexFile(str + "/index.xml", false);
                if (indexFile == null)
                {
                    ISynchronizeInvoke arg_B9_0 = this.mFE;
                    Delegate arg_B9_1 = this.mDownloadFailed;
                    object[] args = new object[1];
                    arg_B9_0.BeginInvoke(arg_B9_1, args);
                }
                else
                {
                    long num = long.Parse(indexFile.SelectSingleNode("/index/header/length").InnerText);
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
                    webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
                    webClient.Headers.Add("Accept-Encoding", "gzip,deflate");
                    webClient.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
                    XmlNodeList xmlNodeList = indexFile.SelectNodes("/index/fileinfo");
                    HashManager.Instance.Clear();
                    HashManager.Instance.Start(indexFile, text2, text + ".hsh", this.mHashThreads);
                    long num2 = 0L;
                    ulong num3 = 0uL;
                    ulong num4 = 0uL;
                    foreach (XmlNode xmlNode in xmlNodeList)
                    {
                        string text3 = xmlNode.SelectSingleNode("path").InnerText;
                        string innerText = xmlNode.SelectSingleNode("file").InnerText;
                        if (!string.IsNullOrEmpty(text2))
                        {
                            int num5 = text3.IndexOf("/");
                            if (num5 >= 0)
                            {
                                text3 = text3.Replace(text3.Substring(0, num5), text2);
                            }
                            else
                            {
                                text3 = text2;
                            }
                        }
                        string text4 = text3 + "/" + innerText;
                        int num6 = int.Parse(xmlNode.SelectSingleNode("length").InnerText);
                        if (xmlNode.SelectSingleNode("hash") != null)
                        {
                            if (!HashManager.Instance.HashesMatch(text4))
                            {
                                num3 += ulong.Parse(xmlNode.SelectSingleNode("length").InnerText);
                                ulong num7;
                                if (xmlNode.SelectSingleNode("compressed") != null)
                                {
                                    num7 = ulong.Parse(xmlNode.SelectSingleNode("compressed").InnerText);
                                }
                                else
                                {
                                    num7 = ulong.Parse(xmlNode.SelectSingleNode("length").InnerText);
                                }
                                num4 += num7;
                                if (flag)
                                {
                                    this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                                    {

                                    });
                                    return;
                                }
                                flag4 = true;
                            }
                        }
                        else
                        {
                            if (flag)
                            {
                                throw new DownloaderException("Without hash in the metadata I cannot verify the download");
                            }
                            flag4 = true;
                        }
                        if (Downloader.mStopFlag)
                        {
                            ISynchronizeInvoke arg_367_0 = this.mFE;
                            Delegate arg_367_1 = this.mDownloadFailed;
                            object[] args2 = new object[1];
                            arg_367_0.BeginInvoke(arg_367_1, args2);
                            return;
                        }
                        num2 += (long)num6;
                        object[] args3 = new object[]
                        {
                            num,
                            num2,
                            0,
                            innerText,
                            0
                        };

                        this.mFE.BeginInvoke(this.mProgressUpdated, args3);
                    }
                    if (flag3)
                    {
                        //Downloader.mLogger.Info("Writing hash cache");
                        HashManager.Instance.WriteHashCache(text + ".hsh", true);
                    }
                    if (flag4)
                    {
                        this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                        {

                        });
                    }
                    else
                    {
                        this.mFE.BeginInvoke(this.mDownloadFinished, null);
                    }
                }
            }
            catch (DownloaderException ex)
            {
                this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                {
                    ex
                });
            }
            catch (Exception ex2)
            {
                this.mFE.BeginInvoke(this.mDownloadFailed, new object[]
                {
                    ex2
                });
            }
            finally
            {
                if (flag2)
                {
                    HashManager.Instance.Clear();
                }
                GC.Collect();
            }
        }

        public static string GetXml(string url)
        {
            byte[] data = Downloader.GetData(url);
            if (Downloader.IsLzma(data))
            {
                return Downloader.DecompressLZMA(data);
            }
            return Encoding.UTF8.GetString(data).Trim();
        }

        public static byte[] GetData(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
            webClient.Headers.Add("Accept-Encoding", "gzip");
            webClient.Headers.Add("Accept-Charset", "utf-8;q=0.7,*;q=0.7");
            webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
            byte[] result = webClient.DownloadData(url);
            webClient.Dispose();
            return result;
        }

        public static bool IsLzma(byte[] arr)
        {
            return arr.Length >= 2 && arr[0] == 93 && arr[1] == 0;
        }

        public static string DecompressLZMA(byte[] compressedFile)
        {
            IntPtr intPtr = new IntPtr(compressedFile.Length - 13);
            byte[] array = new byte[intPtr.ToInt64()];
            IntPtr outPropsSize = new IntPtr(5);
            byte[] array2 = new byte[5];
            compressedFile.CopyTo(array, 13);
            for (int i = 0; i < 5; i++)
            {
                array2[i] = compressedFile[i];
            }
            int num = 0;
            for (int j = 0; j < 8; j++)
            {
                num += (int)compressedFile[j + 5] << 8 * j;
            }
            IntPtr intPtr2 = new IntPtr(num);
            byte[] array3 = new byte[num];
            int num2 = LZMA.LzmaUncompress(array3, ref intPtr2, array, ref intPtr, array2, outPropsSize);
            return new string(Encoding.UTF8.GetString(array3).ToCharArray());
        }
    }
}
