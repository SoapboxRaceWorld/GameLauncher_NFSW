using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Xml;

namespace GameLauncher
{
    internal class DownloadManager
	{
		private const int MaxWorkers = 3;

		private const int MaxActiveChunks = 16;

		private static int _workerCount;

		private int _maxWorkers;

		private Dictionary<string, DownloadManager.DownloadItem> _downloadList;

		private LinkedList<string> _downloadQueue;

		private List<BackgroundWorker> _workers;

		private int _freeChunks;

		private object _freeChunksLock;

		private bool _managerRunning;


		public bool ManagerRunning
		{
			get
			{
				return this._managerRunning;
			}
		}

		static DownloadManager()
		{
			DownloadManager._workerCount = 0;
		}

		public DownloadManager() : this(3, 16)
		{
		}

		public DownloadManager(int maxWorkers, int maxActiveChunks)
		{
			this._maxWorkers = maxWorkers;
			this._freeChunks = maxActiveChunks;
			this._downloadList = new Dictionary<string, DownloadManager.DownloadItem>();
			this._downloadQueue = new LinkedList<string>();
			this._workers = new List<BackgroundWorker>();
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs args)
		{
			try
			{
				using (WebClient webClient = new WebClient())
				{
					webClient.DownloadDataCompleted += new DownloadDataCompletedEventHandler(this.DownloadManager_DownloadDataCompleted);
					webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
					while (true)
					{
						if (this._freeChunks <= 0)
						{
							Thread.Sleep(100);
						}
						else
						{
							lock (this._downloadQueue)
							{
								if (this._downloadQueue.Count == 0)
								{
									lock (this._workers)
									{
										this._workers.Remove((BackgroundWorker)sender);
									}
									DownloadManager._workerCount--;
									break;
								}
							}
							string value = null;
							lock (this._downloadQueue)
							{
								value = this._downloadQueue.Last.Value;
								this._downloadQueue.RemoveLast();
								lock (this._freeChunksLock)
								{
									this._freeChunks--;
								}
							}
							lock (this._downloadList[value])
							{
								if (this._downloadList[value].Status != DownloadManager.DownloadStatus.Canceled)
								{
									this._downloadList[value].Status = DownloadManager.DownloadStatus.Downloading;
								}
							}
							while (webClient.IsBusy)
							{
								Thread.Sleep(100);
							}
							webClient.DownloadDataAsync(new Uri(value), value);
							DownloadManager.DownloadStatus status = DownloadManager.DownloadStatus.Downloading;
							while (status == DownloadManager.DownloadStatus.Downloading)
							{
								status = this._downloadList[value].Status;
								if (status == DownloadManager.DownloadStatus.Canceled)
								{
									break;
								}
								Thread.Sleep(100);
							}
							if (status == DownloadManager.DownloadStatus.Canceled)
							{
								webClient.CancelAsync();
							}
							lock (this._workers)
							{
								if (DownloadManager._workerCount > this._maxWorkers || !this._managerRunning)
								{
									this._workers.Remove((BackgroundWorker)sender);
									DownloadManager._workerCount--;
									break;
								}
							}
						}
					}
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				lock (this._workers)
				{
					this._workers.Remove((BackgroundWorker)sender);
					DownloadManager._workerCount--;
				}
			}
		}

		private void BackgroundWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{

			}
		}

		public void CancelAllDownloads()
		{
			this.Stop();
			lock (this._downloadQueue)
			{
				this._downloadQueue.Clear();
			}
			foreach (string key in this._downloadList.Keys)
			{
				lock (this._downloadList[key])
				{
					if (this._downloadList[key].Data != null)
					{
						lock (this._freeChunksLock)
						{
							this._freeChunks++;
						}
					}
					this._downloadList[key].Status = DownloadManager.DownloadStatus.Canceled;
					this._downloadList[key].Data = null;
				}
			}
		}

		public void CancelDownload(string fileName)
		{
			lock (this._downloadQueue)
			{
				if (this._downloadQueue.Contains(fileName))
				{
					this._downloadQueue.Remove(fileName);
				}
			}
			if (this._downloadList.ContainsKey(fileName))
			{
				lock (this._downloadList[fileName])
				{
					if (this._downloadList[fileName].Data != null)
					{
						lock (this._freeChunksLock)
						{
							this._freeChunks++;
						}
					}
					this._downloadList[fileName].Status = DownloadManager.DownloadStatus.Canceled;
					this._downloadList[fileName].Data = null;
				}
			}
		}

		public void Clear()
		{
			this.CancelAllDownloads();
			while (DownloadManager._workerCount > 0)
			{
				Thread.Sleep(100);
			}
			lock (this._downloadList)
			{
				this._downloadList.Clear();
			}
		}

		private void DownloadManager_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
		{
			string str = e.UserState.ToString();
			if (e.Cancelled || e.Error != null)
			{
				if (e.Error != null)
				{
					if (this._downloadList.ContainsKey(str))
					{
						lock (this._downloadList[str])
						{
							if (this._downloadList[str].Status == DownloadManager.DownloadStatus.Canceled || this._maxWorkers <= 1)
							{
								this._downloadList[str].Data = null;
								this._downloadList[str].Status = DownloadManager.DownloadStatus.Canceled;
							}
							else
							{
								this._downloadList[str].Data = null;
								this._downloadList[str].Status = DownloadManager.DownloadStatus.Queued;
								lock (this._downloadQueue)
								{
									this._downloadQueue.AddLast(str);
								}
								lock (this._workers)
								{
									this._maxWorkers--;
								}
							}
						}
					}
				}
				lock (this._freeChunksLock)
				{
					this._freeChunks++;
				}
			}
			else
			{
				lock (this._downloadList[str])
				{
					if (this._downloadList[str].Status != DownloadManager.DownloadStatus.Downloaded)
					{
						this._downloadList[str].Data = new byte[(int)e.Result.Length];
						Buffer.BlockCopy(e.Result, 0, this._downloadList[str].Data, 0, (int)e.Result.Length);
						this._downloadList[str].Status = DownloadManager.DownloadStatus.Downloaded;
					}
				}
			}
		}

		public byte[] GetFile(string fileName)
		{
			DownloadManager.DownloadStatus status;
			byte[] data = null;
			this.ScheduleFile(fileName);
			lock (this._downloadList[fileName])
			{
				status = this._downloadList[fileName].Status;
			}
			while (status != DownloadManager.DownloadStatus.Downloaded && status != DownloadManager.DownloadStatus.Canceled)
			{
				Thread.Sleep(100);
				lock (this._downloadList[fileName])
				{
					status = this._downloadList[fileName].Status;
				}
			}
			if (this._downloadList[fileName].Status == DownloadManager.DownloadStatus.Downloaded)
			{
				lock (this._downloadList[fileName])
				{
					data = this._downloadList[fileName].Data;
					this._downloadList[fileName].Data = null;
					lock (this._freeChunksLock)
					{
						this._freeChunks++;
					}
				}
			}
			return data;
		}

		public DownloadManager.DownloadStatus? GetStatus(string fileName)
		{
			if (!this._downloadList.ContainsKey(fileName))
			{
				return null;
			}
			return new DownloadManager.DownloadStatus?(this._downloadList[fileName].Status);
		}

		public void Initialize(XmlDocument doc, string serverPath)
		{
			this._freeChunksLock = new object();
			int num = 0;
			foreach (XmlNode xmlNodes in doc.SelectNodes("/index/fileinfo"))
			{
				string innerText = xmlNodes.SelectSingleNode("path").InnerText;
				string str = xmlNodes.SelectSingleNode("file").InnerText;
				if (xmlNodes.SelectSingleNode("section") == null)
				{
					continue;
				}
				num = int.Parse(xmlNodes.SelectSingleNode("section").InnerText);
			}
			for (int i = 1; i <= num; i++)
			{
				string str1 = string.Format("{0}/section{1}.dat", serverPath, i);
				if (!this._downloadList.ContainsKey(str1))
				{
					this._downloadList.Add(str1, new DownloadManager.DownloadItem());
				}
			}
		}

		public void ScheduleFile(string fileName)
		{
			if (this._downloadList.ContainsKey(fileName))
			{
				DownloadManager.DownloadStatus status = DownloadManager.DownloadStatus.Queued;
				lock (this._downloadList[fileName])
				{
					status = this._downloadList[fileName].Status;
				}
				if (status != DownloadManager.DownloadStatus.Queued && status != DownloadManager.DownloadStatus.Canceled)
				{
					return;
				}
				lock (this._downloadQueue)
				{
					if (this._downloadQueue.Contains(fileName) && this._downloadQueue.Last.Value != fileName)
					{
						this._downloadQueue.Remove(fileName);
						this._downloadQueue.AddLast(fileName);
					}
					else if (!this._downloadQueue.Contains(fileName))
					{
						this._downloadQueue.AddLast(fileName);
					}
				}
				lock (this._downloadList[fileName])
				{
					this._downloadList[fileName].Status = DownloadManager.DownloadStatus.Queued;
				}
			}
			else
			{
				this._downloadList.Add(fileName, new DownloadManager.DownloadItem());
				lock (this._downloadQueue)
				{
					this._downloadQueue.AddLast(fileName);
				}
			}
			if (this._managerRunning && DownloadManager._workerCount < this._maxWorkers)
			{
				lock (this._workers)
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorker_DoWork);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerComplete);
					backgroundWorker.RunWorkerAsync();
					this._workers.Add(backgroundWorker);
					DownloadManager._workerCount++;
				}
			}
		}

		public void Start()
		{
			this._managerRunning = true;
			lock (this._workers)
			{
				while (DownloadManager._workerCount < this._maxWorkers)
				{
					BackgroundWorker backgroundWorker = new BackgroundWorker();
					backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWorker_DoWork);
					backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerComplete);
					backgroundWorker.RunWorkerAsync();
					this._workers.Add(backgroundWorker);
					DownloadManager._workerCount++;
				}
			}
		}

		public void Stop()
		{
			this._managerRunning = false;
		}

		private class DownloadItem
		{
			public DownloadManager.DownloadStatus Status;

			private byte[] _data;

			public byte[] Data
			{
				get
				{
					return this._data;
				}
				set
				{
					this._data = value;
				}
			}

			public DownloadItem()
			{
				this.Status = DownloadManager.DownloadStatus.Queued;
			}
		}

		public enum DownloadStatus
		{
			Queued,
			Downloading,
			Downloaded,
			Canceled
		}
	}
}