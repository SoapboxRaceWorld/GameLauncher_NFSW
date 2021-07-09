using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.Logger;
using System;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static APIStatus CheckStatus(string APIURI)
        {
            if (!string.IsNullOrWhiteSpace(APIURI))
            {
                HttpWebResponse serverResponse = null;

                try
                {
                    FunctionStatus.TLS();
                    Uri ConvertedAPIURI = new Uri(APIURI);
                    /* Releases Connection Socket after 30 seconds */
                    ServicePointManager.FindServicePoint(ConvertedAPIURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    HttpWebRequest requestAPIStatus = (HttpWebRequest)WebRequest.Create(ConvertedAPIURI);
                    requestAPIStatus.AllowAutoRedirect = false; /* Find out if this site is up and don't follow a redirector */
                    requestAPIStatus.Method = "GET";
                    requestAPIStatus.UserAgent = "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    requestAPIStatus.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    requestAPIStatus.KeepAlive = false;
                    serverResponse = (HttpWebResponse)requestAPIStatus.GetResponse();
                    Log.Info("CORE: " + APIURI + " is Online!");
                    return APIStatus.Online;
                    /* Do something with response.Headers to find out information about the request */
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        serverResponse = (HttpWebResponse)e.Response;

                        Console.Write("Errorcode: {0}\n", (int)serverResponse.StatusCode);
                        Log.Error("CORE: " + APIURI + " has an Error! Status Code: " + (int)serverResponse.StatusCode);

                        if ((int)serverResponse.StatusCode == 404)
                        {
                            return APIStatus.NotFound;
                        }
                        else if ((int)serverResponse.StatusCode == 500)
                        {
                            return APIStatus.ServerError;
                        }
                        else if ((int)serverResponse.StatusCode == 502)
                        {
                            return APIStatus.ServerOverloaded;
                        }
                        else if ((int)serverResponse.StatusCode == 503)
                        {
                            return APIStatus.ServerUnavailable;
                        }
                        else if ((int)serverResponse.StatusCode == 504)
                        {
                            return APIStatus.GetWayTimeOut;
                        }
                        else if ((int)serverResponse.StatusCode == 520)
                        {
                            return APIStatus.Unknown;
                        }
                        else if ((int)serverResponse.StatusCode == 521)
                        {
                            return APIStatus.Offline;
                        }
                        else if ((int)serverResponse.StatusCode == 522)
                        {
                            return APIStatus.ConnectionTimeOut;
                        }
                        else if ((int)serverResponse.StatusCode == 523)
                        {
                            return APIStatus.OriginUnreachable;
                        }
                        else if ((int)serverResponse.StatusCode == 524)
                        {
                            return APIStatus.Timeout;
                        }
                        else if ((int)serverResponse.StatusCode == 525)
                        {
                            return APIStatus.SSLFailed;
                        }
                        else if ((int)serverResponse.StatusCode == 526)
                        {
                            return APIStatus.InvaildSSL;
                        }
                        /* Set flag if there was a timeout or some other issues */
                    }
                    else
                    {
                        Console.Write("------------");
                        Console.Write("Error: {0}", e.Status);
                        Console.Write("------------\n");
                        Log.Error("CORE: " + APIURI + " is Offline!");
                        return APIStatus.Offline;
                    }
                }
                finally
                {
                    if (serverResponse != null)
                    {
                        serverResponse.Close();
                        serverResponse.Dispose();
                    }
                }
            }
            return APIStatus.Null;
        }
    }

    class VisualsAPIChecker
    {
        public static bool UnitedAPI = true;

        public static bool CarbonAPI = true;

        public static bool CarbonAPITwo = true;

        public static bool WOPLAPI = true;

        public static bool GitHubAPI = true;

        public static bool LoadedServerList = false;

        public static bool LoadedCDNList = false;

        public static void PingAPIStatus(string Mode, string From)
        {
            if (!LoadedServerList || !LoadedCDNList)
            {
                switch (APIChecker.CheckStatus(URLs.Main + "/" + ((!LoadedServerList)? "serverlist.json" : "cdn_list.json")))
                {
                    case APIStatus.Online:
                        if (!LoadedServerList)
                        {
                            URLs.OnlineServerList = URLs.Main + "/serverlist.json";
                        }
                        else if (!LoadedCDNList)
                        {
                            URLs.OnlineCDNList = URLs.Main + "/cdn_list.json";
                        }
                        break;
                    default:
                        UnitedAPI = false;
                        break;
                }

                if (!UnitedAPI)
                {
                    switch (APIChecker.CheckStatus(URLs.Static + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json")))
                    {
                        case APIStatus.Online:
                            if (!LoadedServerList)
                            {
                                URLs.OnlineServerList = URLs.Static + "/serverlist.json";
                            }
                            else if (!LoadedCDNList)
                            {
                                URLs.OnlineCDNList = URLs.Static + "/cdn_list.json";
                            }
                            break;
                        default:
                            CarbonAPI = false;
                            break;
                    }
                }

                if (!CarbonAPI)
                {
                    switch (APIChecker.CheckStatus(URLs.Static_Alt + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json")))
                    {
                        case APIStatus.Online:
                            if (!LoadedServerList)
                            {
                                URLs.OnlineServerList = URLs.Static_Alt + "/serverlist.json";
                            }
                            else if (!LoadedCDNList)
                            {
                                URLs.OnlineCDNList = URLs.Static_Alt + "/cdn_list.json";
                            }
                            break;
                        default:
                            CarbonAPITwo = false;
                            break;
                    }
                }

                if (!CarbonAPITwo)
                {
                    switch (APIChecker.CheckStatus(URLs.WOPL + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json")))
                    {
                        case APIStatus.Online:
                            if (!LoadedServerList)
                            {
                                URLs.OnlineServerList = URLs.WOPL + "/serverlist.json";
                            }
                            else if (!LoadedCDNList)
                            {
                                URLs.OnlineCDNList = URLs.WOPL + "/cdn_list.json";
                            }
                            break;
                        default:
                            WOPLAPI = false;
                            break;
                    }
                }
            }

            if (Mode == "Startup")
            {
                if (!LoadedServerList)
                {
                    LoadedServerList = true;

                    /* Check If Launcher Failed to Connect to any APIs */
                    if (!WOPLAPI)
                    {
                        DiscordLauncherPresense.Status("Start Up", "Launcher Encountered API Errors");

                        DialogResult restartAppNoApis = MessageBox.Show(null, "Unable to Connect to Any Server List API. Please check your connection." +
                        "\n \nClick Yes to Close Launcher \nor \nClick No Continue", "GameLauncher has Stopped, Failed To Connect To Any Server List API",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (restartAppNoApis == DialogResult.No)
                        {
                            MessageBox.Show("Please keep in Mind Launcher Might Crash Past This Point. Make sure to Resolve the Issue Next Time",
                                "GameLauncher Will Continue, When It Failed To Connect To API");
                            Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and Will Continue");
                        }

                        if (restartAppNoApis == DialogResult.Yes)
                        {
                            FunctionStatus.LauncherForceClose = true;
                        }
                    }

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From API Check Error", false);
                    }
                    else
                    {
                        FunctionStatus.IsVisualAPIsChecked = true;

                        /* (Start Process) Check ServerList Status */
                        ServerListUpdater.GetList();
                    }
                }
            }
            else if (Mode == "CDN List")
            {
                if (!LoadedCDNList)
                {
                    LoadedCDNList = true;

                    /* Check If Launcher Failed to Connect to any APIs */
                    if (!WOPLAPI)
                    {
                        MessageBox.Show(null, "Unable to Connect to Any CDN List API. Please check your connection." +
                        "\n\nCDN Dropdown List will not be Available in on " + From + " Screen",
                        "GameLauncher has Stopped, Failed To Connect To Any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    /*******************************/
                    /* Load CDN List                /
                    /*******************************/

                    if (InformationCache.CDNListStatus != "Loaded")
                    {
                        CDNListUpdater.GetList();
                    }
                }
            }
        }
    }
}
