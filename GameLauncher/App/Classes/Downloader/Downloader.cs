using GameLauncher.App.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
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

		private GameLauncher.ProgressUpdated mProgressUpdated;

		private GameLauncher.DownloadFinished mDownloadFinished;

		private GameLauncher.DownloadFailed mDownloadFailed;

		private GameLauncher.ShowMessage mShowMessage;

		private static string mCurrentLocalVersion;

		private static string mCurrentServerVersion;

		private bool mDownloading;

		private int mHashThreads;

		private DownloadManager mDownloadManager;

		private static XmlDocument mIndexCached;

		private static bool mStopFlag;

		public GameLauncher.DownloadFailed DownloadFailed
		{
			get
			{
				return this.mDownloadFailed;
			}
			set
			{
				this.mDownloadFailed = value;
			}
		}

		public GameLauncher.DownloadFinished DownloadFinished
		{
			get
			{
				return this.mDownloadFinished;
			}
			set
			{
				this.mDownloadFinished = value;
			}
		}

		public bool Downloading
		{
			get
			{
				return this.mDownloading;
			}
		}

		public GameLauncher.ProgressUpdated ProgressUpdated
		{
			get
			{
				return this.mProgressUpdated;
			}
			set
			{
				this.mProgressUpdated = value;
			}
		}

		public static string ServerVersion
		{
			get
			{
				return Downloader.mCurrentServerVersion;
			}
		}

		public GameLauncher.ShowMessage ShowMessage
		{
			get
			{
				return this.mShowMessage;
			}
			set
			{
				this.mShowMessage = value;
			}
		}

		static Downloader()
		{
			Downloader.mCurrentLocalVersion = string.Empty;
			Downloader.mCurrentServerVersion = string.Empty;
			Downloader.mIndexCached = null;
			Downloader.mStopFlag = false;
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

		public static string DecompressLZMA(byte[] compressedFile)
		{
			IntPtr intPtr = new IntPtr((int)compressedFile.Length - 13);
			byte[] numArray = new byte[intPtr.ToInt64()];
			IntPtr intPtr1 = new IntPtr(5);
			byte[] numArray1 = new byte[5];
			compressedFile.CopyTo(numArray, 13);
			for (int i = 0; i < 5; i++)
			{
				numArray1[i] = compressedFile[i];
			}
			int num = 0;
			for (int j = 0; j < 8; j++)
			{
				num = num + (compressedFile[j + 5] << (8 * j & 31));
			}
			IntPtr intPtr2 = new IntPtr(num);
			byte[] numArray2 = new byte[num];
			int num1 = LZMA.LzmaUncompress(numArray2, ref intPtr2, numArray, ref intPtr, numArray1, intPtr1);
			if (num1 != 0)
			{
				throw new UncompressionException(num1, string.Format("Error uncompressing data, return: {0}", num1));
			}
			numArray = null;
			return new string(Encoding.UTF8.GetString(numArray2).ToCharArray());
		}

		private void Download(object parameters)
		{
			object[] exception;
			long num;
			this.mDownloading = true;
			string[] strArrays = (string[])parameters;
			byte[] numArray = null;
			byte[] numArray1 = null;
			byte[] file = null;
			XmlDocument indexFile = null;
			XmlNodeList xmlNodeLists = null;
			string str = strArrays[0];
			string str1 = strArrays[1];
			if (!string.IsNullOrEmpty(str1))
			{
				str = string.Concat(str, "/", str1);
			}
			string str2 = strArrays[2];
			bool flag = bool.Parse(strArrays[3]);
			bool flag1 = bool.Parse(strArrays[4]);
			ulong num1 = ulong.Parse(strArrays[5]);
			try
			{
				try
				{
					indexFile = this.GetIndexFile(string.Concat(str, "/index.xml"), flag1);
					if (indexFile != null)
					{
						long num2 = long.Parse(indexFile.SelectSingleNode("/index/header/length").InnerText);
						long num3 = (long)0;
						if (num1 != (long)0)
						{
							num = (long)num1;
						}
						else
						{
							num = long.Parse(indexFile.SelectSingleNode("/index/header/compressed").InnerText);
						}
						long length = (long)0;
						WebClient webClient = new WebClient();
						webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
						webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
						webClient.Headers.Add("Accept-Encoding", "gzip,deflate");
						webClient.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
						int num4 = 1;
						int num5 = 0;
						file = null;
						xmlNodeLists = indexFile.SelectNodes("/index/fileinfo");
						this.mDownloadManager.Initialize(indexFile, str);
						if (flag)
						{
							HashManager.Instance.Clear();
							HashManager.Instance.Start(indexFile, str2, string.Concat(str1, ".hsh"), this.mHashThreads);
						}
						int num6 = 0;
						List<string> strs = new List<string>();
						int num7 = 0;
						bool flag2 = false;
						foreach (XmlNode xmlNodes in xmlNodeLists)
						{
							XmlNodeList xmlNodeLists1 = xmlNodes.SelectNodes("compressed");
							int num8 = 0;
							num8 = (xmlNodeLists1.Count != 0 ? int.Parse(xmlNodeLists1[0].InnerText) : int.Parse(xmlNodes.SelectNodes("length")[0].InnerText));
							num6 = (num8 > num6 ? num8 : num6);
							string innerText = xmlNodes.SelectSingleNode("path").InnerText;
							if (!string.IsNullOrEmpty(str2))
							{
								int num9 = innerText.IndexOf("/");
								innerText = (num9 < 0 ? str2 : innerText.Replace(innerText.Substring(0, num9), str2));
							}
							string innerText1 = xmlNodes.SelectSingleNode("file").InnerText;
							string str3 = string.Concat(innerText, "/", innerText1);
							int num10 = int.Parse(xmlNodes.SelectSingleNode("section").InnerText);
							num5 = int.Parse(xmlNodes.SelectSingleNode("offset").InnerText);
							if (flag)
							{
								if (strs.Count == 0)
								{
									num7 = num10;
								}
								while (num7 <= num10)
								{
									strs.Insert(0, string.Format("{0}/section{1}.dat", str, num7));
									num7++;
								}
							}
							else if (HashManager.Instance.HashesMatch(str3))
							{
								if (flag2)
								{
									int num11 = num10;
									if (num5 == 0)
									{
										num11--;
									}
									while (num7 <= num11)
									{
										strs.Insert(0, string.Format("{0}/section{1}.dat", str, num7));
										num7++;
									}
								}
								if (num7 < num10)
								{
									num7 = num10;
								}
								flag2 = false;
							}
							else
							{
								if (num7 <= num10)
								{
									if (strs.Count == 0)
									{
										num7 = num10;
									}
									while (num7 <= num10)
									{
										strs.Insert(0, string.Format("{0}/section{1}.dat", str, num7));
										num7++;
									}
								}
								flag2 = true;
							}
						}
						foreach (string str4 in strs)
						{
							this.mDownloadManager.ScheduleFile(str4);
						}
						strs.Clear();
						strs = null;
						num5 = 0;
						this.mDownloadManager.Start();
						numArray = new byte[num6];
						numArray1 = new byte[13];
						int num12 = 0;
						foreach (XmlNode xmlNodes1 in xmlNodeLists)
						{
							if (Downloader.mStopFlag)
							{
								break;
							}
							string innerText2 = xmlNodes1.SelectSingleNode("path").InnerText;
							string innerText3 = xmlNodes1.SelectSingleNode("file").InnerText;
							if (!string.IsNullOrEmpty(str2))
							{
								int num13 = innerText2.IndexOf("/");
								innerText2 = (num13 < 0 ? str2 : innerText2.Replace(innerText2.Substring(0, num13), str2));
							}
							string str5 = string.Concat(innerText2, "/", innerText3);
							int num14 = int.Parse(xmlNodes1.SelectSingleNode("length").InnerText);
							int num15 = 0;
							XmlNode xmlNodes2 = xmlNodes1.SelectSingleNode("compressed");
							if (xmlNodes1.SelectSingleNode("section") != null && num4 < int.Parse(xmlNodes1.SelectSingleNode("section").InnerText))
							{
								num4 = int.Parse(xmlNodes1.SelectSingleNode("section").InnerText);
							}
							string str6 = null;
							if (xmlNodes1.SelectSingleNode("hash") == null || !HashManager.Instance.HashesMatch(str5))
							{
								Directory.CreateDirectory(innerText2);
								FileStream fileStream = File.Create(str5);
								int num16 = num14;
								if (xmlNodes2 != null)
								{
									num16 = int.Parse(xmlNodes2.InnerText);
								}
								int num17 = 0;
								bool flag3 = false;
								int num18 = 13;
								while (num17 < num16)
								{
									if (file == null || num5 >= (int)file.Length)
									{
										num5 = (xmlNodes1.SelectSingleNode("offset") == null || flag3 ? 0 : int.Parse(xmlNodes1.SelectSingleNode("offset").InnerText));
										str6 = string.Format("{0}/section{1}.dat", str, num4);
										for (int i = num12 + 1; i < num4; i++)
										{
											this.mDownloadManager.CancelDownload(string.Format("{0}/section{1}.dat", str, i));
										}
										file = null;
										GC.Collect();
										file = this.mDownloadManager.GetFile(str6);
										if (file != null)
										{
											num12 = num4;
											length += (long)((int)file.Length);
											num4++;
											if (!this.mDownloadManager.GetStatus(string.Format("{0}/section{1}.dat", str, num4)).HasValue && length < num)
											{
												this.mDownloadManager.ScheduleFile(string.Format("{0}/section{1}.dat", str, num4));
											}
										}
										else
										{
											if (this.mDownloadFailed != null)
											{
												if (Downloader.mStopFlag)
												{
													ISynchronizeInvoke synchronizeInvoke = this.mFE;
													GameLauncher.DownloadFailed downloadFailed = this.mDownloadFailed;
													exception = new object[1];
													synchronizeInvoke.BeginInvoke(downloadFailed, exception);
												}
												else
												{
													ISynchronizeInvoke synchronizeInvoke1 = this.mFE;
													GameLauncher.DownloadFailed downloadFailed1 = this.mDownloadFailed;
													exception = new object[] { new Exception("DownloadManager returned a null buffer") };
													synchronizeInvoke1.BeginInvoke(downloadFailed1, exception);
												}
											}
											return;
										}
									}
									else
									{
										if (num16 - num17 > (int)file.Length - num5)
										{
											str6 = string.Format("{0}/section{1}.dat", str, num4);
											this.mDownloadManager.ScheduleFile(str6);
											flag3 = true;
										}
										int num19 = Math.Min((int)file.Length - num5, num16 - num17);
										if (num18 == 0)
										{
											Buffer.BlockCopy((Array)file, num5, (Array)numArray, num17 - (xmlNodes2 != null ? 13 : 0), num19);
										}
										else if (xmlNodes2 == null)
										{
											Buffer.BlockCopy(file, num5, numArray, 0, num19);
											num18 = 0;
										}
										else
										{
											int num20 = Math.Min(num18, num19);
											Buffer.BlockCopy(file, num5, numArray1, 13 - num18, num20);
											Buffer.BlockCopy(file, num5 + num20, numArray, 0, num19 - num20);
											num18 -= num20;
										}
										num5 += num19;
										num17 += num19;
										num3 += (long)num19;
									}
									if (this.mProgressUpdated == null)
									{
										continue;
									}
									exception = new object[] { num2, num3, num, str5 };
									this.mFE.BeginInvoke(this.mProgressUpdated, exception);
								}
								if (xmlNodes2 == null)
								{
									fileStream.Write(numArray, 0, num14);
									num15 += num14;
								}
								else
								{
									if (!Downloader.IsLzma(numArray1))
									{
										throw new DownloaderException(string.Format("Compression algorithm used in '{0}' not recognized, it is possible that the data is corrupted", str6));
									}
									fileStream.Close();
									fileStream.Dispose();
									IntPtr intPtr = (IntPtr)num14;
									IntPtr intPtr1 = new IntPtr(5);
									byte[] numArray2 = new byte[5];
									for (int j = 0; j < 5; j++)
									{
										numArray2[j] = numArray1[j];
									}
									long num21 = (long)0;
									for (int k = 0; k < 8; k++)
									{
										num21 += (long)(numArray1[k + 5] << (8 * k & 31));
									}
									if (num21 != (long)num14)
									{
										throw new DownloaderException(string.Format("The length of the file in the metadata ({0}) does not match with the length in the header ({1})", num14, num21));
									}
									int num22 = num16;
									num16 -= 13;
									IntPtr intPtr2 = new IntPtr(num16);
									IntPtr intPtr3 = new IntPtr(num21);
									int num23 = LZMA.LzmaUncompressBuf2File(str5, ref intPtr3, numArray, ref intPtr2, numArray2, intPtr1);
									if (num23 != 0)
									{
										throw new UncompressionException(num23, string.Format("Error uncompressing data, return: {0}", num23));
									}
									if (intPtr3.ToInt32() != num14)
									{
										throw new DownloaderException("Error uncompressing data, not all the data was written.");
									}
									num15 += (int)intPtr3;
								}
								if (fileStream == null)
								{
									continue;
								}
								fileStream.Close();
								fileStream.Dispose();
							}
							else
							{
								num15 += num14;
								if (xmlNodes2 == null)
								{
									if (num1 == (long)0)
									{
										num3 += (long)num14;
									}
									length += (long)num14;
									num5 += num14;
								}
								else
								{
									if (num1 == (long)0)
									{
										num3 += (long)int.Parse(xmlNodes2.InnerText);
									}
									length += (long)int.Parse(xmlNodes2.InnerText);
									num5 += int.Parse(xmlNodes2.InnerText);
								}
								if (this.mProgressUpdated != null)
								{
									exception = new object[] { num2, num3, num, str5 };
									this.mFE.Invoke(this.mProgressUpdated, exception);
								}
								int num24 = int.Parse(xmlNodes1.SelectSingleNode("section").InnerText);
								if (num12 == num24)
								{
									continue;
								}
								for (int l = num12 + 1; l < num24; l++)
								{
									this.mDownloadManager.CancelDownload(string.Format("{0}/section{1}.dat", str, l));
								}
								num12 = num24 - 1;
							}
						}
						if (!Downloader.mStopFlag)
						{
							HashManager.Instance.WriteHashCache(string.Concat(str1, ".hsh"), false);
						}
						if (Downloader.mStopFlag)
						{
							if (this.mDownloadFailed != null)
							{
								ISynchronizeInvoke synchronizeInvoke2 = this.mFE;
								GameLauncher.DownloadFailed downloadFailed2 = this.mDownloadFailed;
								exception = new object[1];
								synchronizeInvoke2.BeginInvoke(downloadFailed2, exception);
							}
						}
						else if (this.mDownloadFinished != null)
						{
							this.mFE.BeginInvoke(this.mDownloadFinished, null);
						}
					}
					else
					{
						ISynchronizeInvoke synchronizeInvoke3 = this.mFE;
						GameLauncher.DownloadFailed downloadFailed3 = this.mDownloadFailed;
						exception = new object[1];
						synchronizeInvoke3.BeginInvoke(downloadFailed3, exception);
						return;
					}
				}
				catch (DownloaderException downloaderException1)
				{
					DownloaderException downloaderException = downloaderException1;
					if (this.mDownloadFailed != null)
					{
						try
						{
							ISynchronizeInvoke synchronizeInvoke4 = this.mFE;
							GameLauncher.DownloadFailed downloadFailed4 = this.mDownloadFailed;
							exception = new object[] { downloaderException };
							synchronizeInvoke4.BeginInvoke(downloadFailed4, exception);
						}
						catch
						{
						}
					}
				}
				catch (Exception exception2)
				{
					Exception exception1 = exception2;
					if (this.mDownloadFailed != null)
					{
						try
						{
							ISynchronizeInvoke synchronizeInvoke5 = this.mFE;
							GameLauncher.DownloadFailed downloadFailed5 = this.mDownloadFailed;
							exception = new object[] { exception1 };
							synchronizeInvoke5.BeginInvoke(downloadFailed5, exception);
						}
						catch
						{
						}
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
				numArray = null;
				numArray1 = null;
				file = null;
				indexFile = null;
				xmlNodeLists = null;
				GC.Collect();
				this.mDownloading = false;
			}
		}

		private void Downloader_DownloadFileCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			string str = e.UserState.ToString();
			if (!e.Cancelled && e.Error == null)
			{
				return;
			}
			if (e.Error != null)
			{

			}
		}

		public static byte[] GetData(string url)
		{
			WebClient webClient = new WebClient();
			webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
			webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
			webClient.Headers.Add("Accept-Encoding", "gzip");
			webClient.Headers.Add("Accept-Charset", "utf-8;q=0.7,*;q=0.7");
			webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
			byte[] numArray = webClient.DownloadData(url);
			webClient.Dispose();
			return numArray;
		}

		private XmlDocument GetIndexFile(string url, bool useCache)
		{
			XmlDocument xmlDocument;
			try
			{
				if (!useCache || Downloader.mIndexCached == null)
				{
					WebClient webClient = new WebClient();
					webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(this.Downloader_DownloadFileCompleted);
					string tempFileName = Path.GetTempFileName();
					webClient.DownloadFileAsync(new Uri(url), tempFileName);
					while (webClient.IsBusy)
					{
						if (!Downloader.mStopFlag)
						{
							Thread.Sleep(100);
						}
						else
						{
							webClient.CancelAsync();
							xmlDocument = null;
							return xmlDocument;
						}
					}
					XmlDocument xmlDocument1 = new XmlDocument();
					xmlDocument1.Load(tempFileName);
					Downloader.mIndexCached = xmlDocument1;
					xmlDocument = xmlDocument1;
				}
				else
				{
					xmlDocument = Downloader.mIndexCached;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				xmlDocument = null;
			}
			return xmlDocument;
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

		public static bool IsLzma(byte[] arr)
		{
			if ((int)arr.Length < 2 || arr[0] != 93)
			{
				return false;
			}
			return arr[1] == 0;
		}

		public void StartDownload(string indexUrl, string package, string patchPath, bool calculateHashes, bool useIndexCache, ulong downloadSize)
		{
			Downloader.mStopFlag = false;
			this.mThread = new Thread(new ParameterizedThreadStart(this.Download));
			string[] strArrays = new string[] { indexUrl, package, patchPath, calculateHashes.ToString(), useIndexCache.ToString(), downloadSize.ToString() };
			this.mThread.Start(strArrays);
		}

		public void StartVerification(string indexUrl, string package, string patchPath, bool stopOnFail, bool clearHashes, bool writeHashes)
		{
			Downloader.mStopFlag = false;
			this.mThread = new Thread(new ParameterizedThreadStart(this.Verify));
			string[] strArrays = new string[] { indexUrl, package, patchPath, stopOnFail.ToString(), clearHashes.ToString(), writeHashes.ToString() };
			this.mThread.Start(strArrays);
		}

		public void Stop()
		{
			Downloader.mStopFlag = true;
			if (this.mDownloadManager != null && this.mDownloadManager.ManagerRunning)
			{
				this.mDownloadManager.CancelAllDownloads();
			}
		}

		private void Verify(object parameters)
		{
			string[] strArrays = (string[])parameters;
			string str = strArrays[0].Trim();
			string str1 = strArrays[1].Trim();
			if (!string.IsNullOrEmpty(str1))
			{
				str = string.Concat(str, "/", str1);
			}
			string str2 = strArrays[2].Trim();
			XmlDocument indexFile = null;
			XmlNodeList xmlNodeLists = null;
			bool flag = bool.Parse(strArrays[3]);
			bool flag1 = bool.Parse(strArrays[4]);
			bool flag2 = bool.Parse(strArrays[5]);
			bool flag3 = false;
			try
			{
				try
				{
					indexFile = this.GetIndexFile(string.Concat(str, "/index.xml"), false);
					if (indexFile != null)
					{
						long num = long.Parse(indexFile.SelectSingleNode("/index/header/length").InnerText);
						WebClient webClient = new WebClient();
						webClient.Headers.Add("Accept", "text/html,text/xml,application/xhtml+xml,application/xml,application/*,*/*;q=0.9,*/*;q=0.8");
						webClient.Headers.Add("Accept-Language", "en-us,en;q=0.5");
						webClient.Headers.Add("Accept-Encoding", "gzip,deflate");
						webClient.Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
						xmlNodeLists = indexFile.SelectNodes("/index/fileinfo");
						HashManager.Instance.Clear();
						HashManager.Instance.Start(indexFile, str2, string.Concat(str1, ".hsh"), this.mHashThreads);
						long num1 = (long)0;
						ulong num2 = (ulong)0;
						ulong num3 = (ulong)0;
						foreach (XmlNode xmlNodes in xmlNodeLists)
						{
							string innerText = xmlNodes.SelectSingleNode("path").InnerText;
							string innerText1 = xmlNodes.SelectSingleNode("file").InnerText;
							if (!string.IsNullOrEmpty(str2))
							{
								int num4 = innerText.IndexOf("/");
								innerText = (num4 < 0 ? str2 : innerText.Replace(innerText.Substring(0, num4), str2));
							}
							string str3 = string.Concat(innerText, "/", innerText1);
							int num5 = int.Parse(xmlNodes.SelectSingleNode("length").InnerText);
							if (xmlNodes.SelectSingleNode("hash") == null)
							{
								if (flag)
								{
									throw new DownloaderException("Without hash in the metadata I cannot verify the download");
								}
								flag3 = true;
							}
							else if (!HashManager.Instance.HashesMatch(str3))
							{
								num2 += ulong.Parse(xmlNodes.SelectSingleNode("length").InnerText);
								ulong num6 = (ulong)0;
								num6 = (xmlNodes.SelectSingleNode("compressed") == null ? ulong.Parse(xmlNodes.SelectSingleNode("length").InnerText) : ulong.Parse(xmlNodes.SelectSingleNode("compressed").InnerText));
								num3 += num6;
								if (!flag)
								{
									flag3 = true;
								}
								else
								{
									ISynchronizeInvoke synchronizeInvoke = this.mFE;
									GameLauncher.DownloadFailed downloadFailed = this.mDownloadFailed;
									object[] verificationException = new object[] { new Exception(num2 + num3 + str1) };
									synchronizeInvoke.BeginInvoke(downloadFailed, verificationException);
									return;
								}
							}
							if (!Downloader.mStopFlag)
							{
								num1 += (long)num5;
								object[] objArray = new object[] { num, num1, 0, innerText1 };
								this.mFE.BeginInvoke(this.mProgressUpdated, objArray);
							}
							else
							{
								this.mFE.BeginInvoke(this.mDownloadFailed, new object[1]);
								return;
							}
						}
						if (flag2)
						{
							HashManager.Instance.WriteHashCache(string.Concat(str1, ".hsh"), true);
						}
						if (!flag3)
						{
							this.mFE.BeginInvoke(this.mDownloadFinished, null);
						}
						else
						{
							ISynchronizeInvoke synchronizeInvoke1 = this.mFE;
							GameLauncher.DownloadFailed downloadFailed1 = this.mDownloadFailed;
							object[] verificationException1 = new object[] { new Exception(num + "" + num3 + str1) };
							synchronizeInvoke1.BeginInvoke(downloadFailed1, verificationException1);
						}
					}
					else
					{
						this.mFE.BeginInvoke(this.mDownloadFailed, new object[1]);
						return;
					}
				}
				catch (DownloaderException downloaderException1)
				{
					DownloaderException downloaderException = downloaderException1;
					ISynchronizeInvoke synchronizeInvoke2 = this.mFE;
					GameLauncher.DownloadFailed downloadFailed2 = this.mDownloadFailed;
					object[] objArray1 = new object[] { downloaderException };
					synchronizeInvoke2.BeginInvoke(downloadFailed2, objArray1);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					ISynchronizeInvoke synchronizeInvoke3 = this.mFE;
					GameLauncher.DownloadFailed downloadFailed3 = this.mDownloadFailed;
					object[] objArray2 = new object[] { exception };
					synchronizeInvoke3.BeginInvoke(downloadFailed3, objArray2);
				}
			}
			finally
			{
				if (flag1)
				{
					HashManager.Instance.Clear();
				}
				indexFile = null;
				xmlNodeLists = null;
				GC.Collect();
			}
		}
	}
}