using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Diagnostics.Tracing.Session;
using GameLauncher.App.Classes;

namespace GameLauncher.App.Classes {
    public class FPSCount {

        public static int StoreFPS = 0;
        public static int getFPS() {
            return StoreFPS;
        }
    }
}

namespace FPSCounterClass {
    //helper class to store frame timestamps
    public class TimestampCollection
    {
        const int MAXNUM = 1000;

        public string Name { get; set; }

        List<long> timestamps = new List<long>(MAXNUM + 1);
        object sync = new object();

        //add value to the collection
        public void Add(long timestamp)
        {
            lock (sync)
            {
                timestamps.Add(timestamp);
                if (timestamps.Count > MAXNUM) timestamps.RemoveAt(0);
            }
        }

        //get the number of timestamps withing interval
        public int QueryCount(long from, long to)
        {
            int c = 0;

            lock (sync)
            {
                foreach (var ts in timestamps)
                {
                    if (ts >= from && ts <= to) c++;
                }
            }
            return c;
        }
    }

    class CountHelp {
        //event codes (https://github.com/GameTechDev/PresentMon/blob/40ee99f437bc1061a27a2fc16a8993ee8ce4ebb5/PresentData/PresentMonTraceConsumer.cpp)
        public const int EventID_D3D9PresentStart = 1;
        public const int EventID_DxgiPresentStart = 42;

        //ETW provider codes
        public static readonly Guid D3D9_provider = Guid.Parse("{783ACA0A-790E-4D7F-8451-AA850511C6B9}");

        static TraceEventSession m_EtwSession;
        static Dictionary<int, TimestampCollection> frames = new Dictionary<int, TimestampCollection>();
        static Stopwatch watch = null;
        static object sync = new object();

        static void EtwThreadProc() {
            try { 
                m_EtwSession.Source.Process();
            } catch {
                Console.WriteLine("FPS Failure");
            }
        }

        static void OutputThreadProc() {
            //console output loop
            while (true) {
                long t1, t2;
                long dt = 2000;

                lock (sync) {
                    t2 = watch.ElapsedMilliseconds;
                    t1 = t2 - dt;

                    foreach (var x in frames.Values) {
                        if(x.Name == "nfsw") {
                            int count = x.QueryCount(t1, t2);
                            FPSCount.StoreFPS = Convert.ToInt32((double)count / dt * 1000.0);
                        }
                    }
                }
            }
        }

        public static void StartMe() {
            m_EtwSession = new TraceEventSession("mysess");
            m_EtwSession.StopOnDispose = true;
            m_EtwSession.EnableProvider("Microsoft-Windows-D3D9");

            //handle event
            m_EtwSession.Source.AllEvents += data => {
                //filter out frame presentation events
                if (((int)data.ID == EventID_D3D9PresentStart && data.ProviderGuid == D3D9_provider)) {
                    int pid = data.ProcessID;
                    long t;

                    lock (sync) {
                        t = watch.ElapsedMilliseconds;

                        //if process is not yet in Dictionary, add it
                        if (!frames.ContainsKey(pid)) {
                            frames[pid] = new TimestampCollection();

                            string name = "";
                            var proc = Process.GetProcessById(pid);
                            if (proc != null) {
                                using (proc)
                                {
                                    name = proc.ProcessName;
                                }
                            }
                            else name = pid.ToString();

                            frames[pid].Name = name;
                        }

                        frames[pid].Add(t);
                    }
                }
            };

            watch = new Stopwatch();
            watch.Start();

            Thread thETW = new Thread(EtwThreadProc);
            thETW.IsBackground = true;
            thETW.Start();

            Thread thOutput = new Thread(OutputThreadProc);
            thOutput.IsBackground = true;
            thOutput.Start();
        }
    }
}