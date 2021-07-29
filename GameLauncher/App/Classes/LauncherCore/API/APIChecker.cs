using GameLauncher.App.Classes.LauncherCore.Client.Web;
using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.LauncherCore.Lists;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.LauncherCore.RPC;
using GameLauncher.App.Classes.LauncherCore.Validator.JSON;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GameLauncher.App.Classes.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static APIStatus CheckStatus(string URI)
        {
            if (!string.IsNullOrWhiteSpace(URI))
            {
                HttpWebResponse ServerResponse = null;
                HttpWebRequest URLConnection = null;

                try
                {
                    FunctionStatus.TLS();
                    Uri ConvertedAPIURI = new Uri(URI);
                    /* Releases Connection Socket after 30 seconds */
                    ServicePointManager.FindServicePoint(ConvertedAPIURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    URLConnection = (HttpWebRequest)WebRequest.Create(ConvertedAPIURI);
                    URLConnection.AllowAutoRedirect = false; /* Find out if this site is up and don't follow a redirector */
                    URLConnection.Method = "GET";
                    URLConnection.UserAgent = "SBRW Launcher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    URLConnection.Timeout = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                    URLConnection.KeepAlive = false;

                    try
                    {
                        ServerResponse = (HttpWebResponse)URLConnection.GetResponse();
                        Log.Info("CORE: " + URI + " is Online!");
                        return APIStatus.Online;
                        /* Do something with response.Headers to find out information about the request */
                    }
                    catch (WebException Error)
                    {
                        return StatusCodes(URI, Error, (HttpWebResponse)Error.Response);
                    }
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("API Checker", null, Error, null, true);
                    return APIStatus.UnknownError;
                }
                finally
                {
                    if (ServerResponse != null)
                    {
                        ServerResponse.Close();
                        ServerResponse.Dispose();
                    }

                    if (URLConnection != null)
                    {
                        URLConnection.Abort();
                    }
                }
            }
            else
            {
                Log.Error("CORE: " + URI + " URI can not be Null!");
                return APIStatus.Null;
            }
        }

        public static APIStatus StatusCodes(string URI, WebException Error, HttpWebResponse Response)
        {
            if (!string.IsNullOrWhiteSpace(URI))
            {
                switch (Error.Status)
                {
                    case WebExceptionStatus.CacheEntryNotFound:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Cache Entry Not Found");
                        return APIStatus.CacheEntryNotFound;
                    case WebExceptionStatus.ConnectFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Connect Failure");
                        return APIStatus.ConnectFailure;
                    case WebExceptionStatus.ConnectionClosed:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Connection Closed");
                        return APIStatus.ConnectionClosed;
                    case WebExceptionStatus.KeepAliveFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Keep Alive Failure");
                        return APIStatus.KeepAliveFailure;
                    case WebExceptionStatus.MessageLengthLimitExceeded:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Message Length Limit Exceeded");
                        return APIStatus.MessageLengthLimitExceeded;
                    case WebExceptionStatus.NameResolutionFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Name Resolution Failure");
                        return APIStatus.NameResolutionFailure;
                    case WebExceptionStatus.Pending:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Pending");
                        return APIStatus.Pending;
                    case WebExceptionStatus.PipelineFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Pipeline Failure");
                        return APIStatus.PipelineFailure;
                    case WebExceptionStatus.ProtocolError:
                        Log.Error("CORE: " + URI + " has an Error! Status Code: " + (int)Response.StatusCode);

                        /* Set flag if there was a timeout or some other issues */
                        switch ((int)Response.StatusCode)
                        {
                            case 404:
                                return APIStatus.NotFound;
                            case 500:
                                return APIStatus.ServerError;
                            case 502:
                                return APIStatus.ServerOverloaded;
                            case 503:
                                return APIStatus.ServerUnavailable;
                            case 504:
                                return APIStatus.GetWayTimeOut;
                            case 520:
                                return APIStatus.Unknown;
                            case 521:
                                return APIStatus.Offline;
                            case 522:
                                return APIStatus.ConnectionTimeOut;
                            case 523:
                                return APIStatus.OriginUnreachable;
                            case 524:
                                return APIStatus.Timeout;
                            case 525:
                                return APIStatus.SSLFailed;
                            case 526:
                                return APIStatus.InvaildSSL;
                            default:
                                return APIStatus.UnknownStatusCode;
                        }
                    case WebExceptionStatus.ProxyNameResolutionFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Proxy Name Resolution Failure");
                        return APIStatus.ProxyNameResolutionFailure;
                    case WebExceptionStatus.ReceiveFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Receive Failure");
                        return APIStatus.ReceiveFailure;
                    case WebExceptionStatus.RequestCanceled:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Request Canceled");
                        return APIStatus.RequestCanceled;
                    case WebExceptionStatus.RequestProhibitedByCachePolicy:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Request Prohibited By Cache Policy");
                        return APIStatus.RequestProhibitedByCachePolicy;
                    case WebExceptionStatus.SecureChannelFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Secure Channel Failure");
                        return APIStatus.SecureChannelFailure;
                    case WebExceptionStatus.SendFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Send Failure");
                        return APIStatus.SendFailure;
                    case WebExceptionStatus.ServerProtocolViolation:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Server Protocol Violation");
                        return APIStatus.ServerProtocolViolation;
                    case WebExceptionStatus.Success:
                        Log.Warning("CORE: " + URI + " had Encounterd Error -> Success");
                        return APIStatus.Success;
                    case WebExceptionStatus.Timeout:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Timeout");
                        return APIStatus.Timeout;
                    case WebExceptionStatus.TrustFailure:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Trust Failure");
                        return APIStatus.TrustFailure;
                    case WebExceptionStatus.UnknownError:
                        Log.Error("CORE: " + URI + " had Encounterd Error -> Unknown Error");
                        return APIStatus.UnknownError;
                    default:
                        Log.Error("CORE: " + URI + " is Offline!");
                        return APIStatus.Offline;
                }
            }
            else
            {
                Log.Error("CORE: " + URI + " URI can not be Null!");
                return APIStatus.Null;
            }
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
                Log.Checking("API: Checking Status");

                switch (APIChecker.CheckStatus(URLs.Main + "/" + ((!LoadedServerList)? "serverlist.json" : "cdn_list.json")))
                {
                    case APIStatus.Online:
                        UnitedAPI = RetriveJSON(URLs.Main + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json"),
                                                 (!LoadedServerList) ? "SL" : "CDNL");
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
                            CarbonAPI = RetriveJSON(URLs.Static + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json"),
                                                 (!LoadedServerList) ? "SL" : "CDNL");
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
                            CarbonAPITwo = RetriveJSON(URLs.Static_Alt + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json"),
                                                 (!LoadedServerList) ? "SL" : "CDNL");
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
                            WOPLAPI = RetriveJSON(URLs.WOPL + "/" + ((!LoadedServerList) ? "serverlist.json" : "cdn_list.json"),
                                                 (!LoadedServerList) ? "SL" : "CDNL");
                            break;
                        default:
                            WOPLAPI = false;
                            break;
                    }
                }

                Log.Completed("API: Done");
            }

            if (Mode == "Startup")
            {
                if (!LoadedServerList)
                {
                    Log.Checking("API: Test #2");
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
                    Log.Completed("API: Test #2 Done");

                    if (FunctionStatus.LauncherForceClose)
                    {
                        FunctionStatus.ErrorCloseLauncher("Closing From API Check Error", false);
                    }
                    else
                    {
                        FunctionStatus.IsVisualAPIsChecked = true;

                        Log.Info("LIST CORE: Moved to Function");
                        /* (Start Process) Check ServerList Status */
                        ServerListUpdater.GetList();
                    }
                }
            }
            else if (Mode == "CDN List")
            {
                if (!LoadedCDNList)
                {
                    Log.Checking("API: Test #3");
                    LoadedCDNList = true;

                    /* Check If Launcher Failed to Connect to any APIs */
                    if (!WOPLAPI)
                    {
                        MessageBox.Show(null, "Unable to Connect to Any CDN List API. Please check your connection." +
                        "\n\nCDN Dropdown List will not be Available in on " + From + " Screen",
                        "GameLauncher has Stopped, Failed To Connect To Any CDN List API", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    Log.Checking("API: Test #3 Done");

                    /*******************************/
                    /* Load CDN List                /
                    /*******************************/

                    if (InformationCache.CDNListStatus != "Loaded")
                    {
                        Log.Checking("LIST CORE: Moved to Function");
                        CDNListUpdater.GetList();
                    }
                }
            }
        }

        private static string OnlineListJson;

        private static bool RetriveJSON(string JSONUrl, string Function)
        {
            Log.Checking("JSON LIST: Retriving " + JSONUrl);
            try
            {
                FunctionStatus.TLS();
                Uri URLCall = new Uri(JSONUrl);
                ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                var Client = new WebClient
                {
                    Encoding = Encoding.UTF8
                };

                if (!WebCalls.Alternative()) { Client = new WebClientWithTimeout { Encoding = Encoding.UTF8 }; }
                else
                {
                    Client.Headers.Add("user-agent", "SBRW Launcher " +
                    Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                }
                
                try
                {
                    OnlineListJson = Client.DownloadString(URLCall);
                    Log.UrlCall("JSON LIST: Retrived " + JSONUrl);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("JSON LIST", null, Error, null, true);
                }
                finally
                {
                    if (Client != null)
                    {
                        Client.Dispose();
                    }
                }

                if (IsJSONValid.ValidJson(OnlineListJson))
                {
                    switch (Function)
                    {
                        case "SL":
                            ServerListUpdater.CachedJSONList = OnlineListJson;
                            break;
                        case "CDNL":
                            CDNListUpdater.CachedJSONList = OnlineListJson;
                            break;
                        default:
                            break;
                    }
                    Log.Completed("JSON LIST: Valid " + JSONUrl);

                    return true;
                }
                else
                {
                    Log.Completed("JSON LIST: Invalid " + JSONUrl);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (OnlineListJson != null)
                {
                    OnlineListJson = null;
                }
            }
        }
    }
}
