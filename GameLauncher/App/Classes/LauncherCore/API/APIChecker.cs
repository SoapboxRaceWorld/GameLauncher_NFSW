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
        public static APIStatus CheckStatus(string URI, int Timer)
        {
            if (!string.IsNullOrWhiteSpace(URI))
            {
                HttpWebResponse ServerResponse = null;
                HttpWebRequest URLConnection = null;

                try
                {
                    Log.Checking("Checking Status: ".ToUpper() + URI);
                    FunctionStatus.TLS();
                    Uri ConvertedAPIURI = new Uri(URI);
                    /* Releases Connection Socket after 30 seconds */
                    ServicePointManager.FindServicePoint(ConvertedAPIURI).ConnectionLeaseTimeout = (int)TimeSpan.FromSeconds(Timer + 1).TotalMilliseconds;
                    URLConnection = (HttpWebRequest)WebRequest.Create(ConvertedAPIURI);
                    URLConnection.AllowAutoRedirect = false; /* Find out if this site is up and don't follow a redirector */
                    URLConnection.Method = "GET";
                    URLConnection.UserAgent = "SBRW Launcher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    URLConnection.Timeout = (int)TimeSpan.FromSeconds(Timer).TotalMilliseconds;
                    URLConnection.KeepAlive = false;

                    try
                    {
                        ServerResponse = (HttpWebResponse)URLConnection.GetResponse();
                        Log.Completed("Checking Status: ".ToUpper() + URI + " [Is Online]");
                        return APIStatus.Online;
                        /* Do something with response.Headers to find out information about the request */
                    }
                    catch (WebException Error)
                    {
                        Log.Completed("Checking Status: ".ToUpper() + URI + " [WebException]");
                        return StatusCodes(URI, Error, (HttpWebResponse)Error.Response);
                    }
                }
                catch (Exception Error)
                {
                    Log.Completed("Checking Status: ".ToUpper() + URI + " [Exception]");
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
        public static bool UnitedSL = false;
        public static bool UnitedCDNL = false;
        public static bool UnitedAPI() => (UnitedSL && UnitedCDNL);

        public static bool CarbonSL = false;
        public static bool CarbonCDNL = false;
        public static bool CarbonAPI() => (CarbonSL && CarbonCDNL);

        public static bool CarbonTwoSL = false;
        public static bool CarbonTwoCDNL = false;
        public static bool CarbonAPITwo() => (CarbonTwoSL && CarbonTwoCDNL);

        public static bool WOPLSL = false;
        public static bool WOPLCDNL = false;
        public static bool WOPLAPI() => (WOPLSL && WOPLCDNL);

        public static bool GitHubAPI = true;

        public static void PingAPIStatus()
        {
            Log.Checking("API: Checking Status");
            Log.Checking("API Status: WorldUnited");
            switch (APIChecker.CheckStatus(URLs.Main + "/serverlist.json", 15))
            {
                case APIStatus.Online:
                    UnitedSL = RetriveJSON(URLs.Main + "/serverlist.json", "SL");
                    if (UnitedSL) { UnitedCDNL = RetriveJSON(URLs.Main + "/cdn_list.json", "CDNL"); }
                    Log.Completed("API Status: WorldUnited");
                    break;
                default:
                    Log.Completed("API Status: WorldUnited");
                    break;
            }

            if (!UnitedAPI())
            {
                Log.Checking("API Status: DavidCarbon");
                switch (APIChecker.CheckStatus(URLs.Static + "/serverlist.json", 15))
                {
                    case APIStatus.Online:
                        if (!UnitedSL) { CarbonSL = RetriveJSON(URLs.Static + "/serverlist.json", "SL"); }
                        else { CarbonSL = true; }
                        if (!UnitedCDNL) { CarbonCDNL = RetriveJSON(URLs.Static + "/cdn_list.json", "CDNL"); }
                        else { CarbonCDNL = true; }
                        Log.Completed("API Status: DavidCarbon");
                        break;
                    default:
                        Log.Completed("API Status: DavidCarbon");
                        break;
                }
            }
            else
            {
                CarbonSL = true;
                CarbonCDNL = true;
            }

            if (!CarbonAPI())
            {
                Log.Checking("API Status: DavidCarbon [Second]");
                switch (APIChecker.CheckStatus(URLs.Static_Alt + "/serverlist.json", 15))
                {
                    case APIStatus.Online:
                        if (!CarbonSL) { CarbonTwoSL = RetriveJSON(URLs.Static_Alt + "/serverlist.json", "SL"); }
                        else { CarbonTwoSL = true; }
                        if (!CarbonCDNL) { CarbonTwoCDNL = RetriveJSON(URLs.Static_Alt + "/cdn_list.json", "CDNL"); }
                        else { CarbonTwoCDNL = true; }
                        Log.Completed("API Status: DavidCarbon [Second]");
                        break;
                    default:
                        Log.Completed("API Status: DavidCarbon [Second]");
                        break;
                }
            }
            else
            {
                CarbonTwoSL = true;
                CarbonTwoCDNL = true;
            }

            if (!CarbonAPITwo())
            {
                Log.Checking("API Status: WorldOnline");
                switch (APIChecker.CheckStatus(URLs.WOPL + "/serverlist.json", 15))
                {
                    case APIStatus.Online:
                        if (!CarbonTwoSL) { WOPLSL = RetriveJSON(URLs.WOPL + "/serverlist.json", "SL"); }
                        else { WOPLSL = true; }
                        if (!CarbonTwoCDNL) { WOPLCDNL = RetriveJSON(URLs.WOPL + "/cdn_list.json", "CDNL"); }
                        else { WOPLCDNL = true; }
                        Log.Completed("API Status: WorldOnline");
                        break;
                    default:
                        Log.Completed("API Status: WorldOnline");
                        break;
                }
            }
            else
            {
                WOPLSL = true;
                WOPLCDNL = true;
            }

            Log.Completed("API: Checking Status Done");

            Log.Checking("API: Test #2");

            /* Check If Launcher Failed to Connect to any APIs */
            if (!WOPLAPI())
            {
                DiscordLauncherPresense.Status("Start Up", "Launcher Encountered API Errors");

                DialogResult restartAppNoApis = MessageBox.Show(null, "Unable to Connect to any Server List API. Please check your connection." +
                "\n \nClick Yes to Close Launcher \nor \nClick No Continue", "GameLauncher has Stopped, Failed To Connect to any Server List API",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (restartAppNoApis == DialogResult.No)
                {
                    MessageBox.Show("Please keep in Mind Launcher might crash past this point. Make sure to resolve the issue next time",
                        "GameLauncher will continue, despite failing To Connect To API");
                    Log.Warning("PRE-CHECK: User has Bypassed 'No Internet Connection' Check and will Continue");
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
                    return false;
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
