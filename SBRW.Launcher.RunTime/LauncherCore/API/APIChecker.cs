using SBRW.Launcher.RunTime.LauncherCore.Global;
using SBRW.Launcher.RunTime.LauncherCore.Languages.Visual_Forms;
using SBRW.Launcher.RunTime.LauncherCore.Lists;
using SBRW.Launcher.RunTime.LauncherCore.Logger;
using SBRW.Launcher.Core.Cache;
using SBRW.Launcher.Core.Extension.Api_;
using SBRW.Launcher.Core.Extension.Validation_.Json_.Newtonsoft_;
using SBRW.Launcher.Core.Extension.Web_;
using SBRW.Launcher.Core.Discord.RPC_;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Cache;
using System.Threading.Tasks;

namespace SBRW.Launcher.RunTime.LauncherCore.APICheckers
{
    class APIChecker
    {
        public static string StatusStrings(APIStatus Error)
        {
            switch (Error)
            {
                case APIStatus.CacheEntryNotFound:
                    return Translations.Database("APIChecker_CacheEntryNotFound");
                case APIStatus.ConnectFailure:
                    return Translations.Database("APIChecker_ConnectFailure");
                case APIStatus.ConnectionClosed:
                    return Translations.Database("APIChecker_ConnectionClosed");
                case APIStatus.KeepAliveFailure:
                    return Translations.Database("APIChecker_KeepAliveFailure");
                case APIStatus.MessageLengthLimitExceeded:
                    return Translations.Database("APIChecker_MessageLengthLimitExceeded");
                case APIStatus.NameResolutionFailure:
                    return Translations.Database("APIChecker_NameResolutionFailure");
                case APIStatus.Pending:
                    return Translations.Database("APIChecker_Pending");
                case APIStatus.PipelineFailure:
                    return Translations.Database("APIChecker_PipelineFailure");
                case APIStatus.NotFound:
                    return Translations.Database("APIChecker_NotFound");
                case APIStatus.ServerError:
                    return Translations.Database("APIChecker_ServerError");
                case APIStatus.ServerOverloaded:
                    return Translations.Database("APIChecker_ServerOverloaded");
                case APIStatus.ServerUnavailable:
                    return Translations.Database("APIChecker_ServerUnavailable");
                case APIStatus.GetWayTimeOut:
                    return Translations.Database("APIChecker_GetWayTimeOut");
                case APIStatus.ConnectionTimeOut:
                    return Translations.Database("APIChecker_ConnectionTimeOut");
                case APIStatus.OriginUnreachable:
                    return Translations.Database("APIChecker_OriginUnreachable");
                case APIStatus.Timeout:
                    return Translations.Database("APIChecker_Timeout");
                case APIStatus.SSLFailed:
                    return Translations.Database("APIChecker_SSLFailed");
                case APIStatus.InvaildSSL:
                    return Translations.Database("APIChecker_InvaildSSL");
                case APIStatus.UnknownStatusCode:
                    return Translations.Database("APIChecker_UnknownStatusCode");
                case APIStatus.ProxyNameResolutionFailure:
                    return Translations.Database("APIChecker_ProxyNameResolutionFailure");
                case APIStatus.ReceiveFailure:
                    return Translations.Database("APIChecker_ReceiveFailure");
                case APIStatus.RequestCanceled:
                    return Translations.Database("APIChecker_RequestCanceled");
                case APIStatus.RequestProhibitedByCachePolicy:
                    return Translations.Database("APIChecker_RequestProhibitedByCachePolicy");
                case APIStatus.SecureChannelFailure:
                    return Translations.Database("APIChecker_SecureChannelFailure");
                case APIStatus.SendFailure:
                    return Translations.Database("APIChecker_SendFailure");
                case APIStatus.ServerProtocolViolation:
                    return Translations.Database("APIChecker_ServerProtocolViolation");
                case APIStatus.Success:
                    return Translations.Database("APIChecker_Success");
                case APIStatus.TrustFailure:
                    return Translations.Database("APIChecker_TrustFailure");
                case APIStatus.Unknown:
                case APIStatus.UnknownError:
                    return Translations.Database("APIChecker_Unknown");
                case APIStatus.Null:
                    return Translations.Database("APIChecker_Null");
                case APIStatus.Online:
                    return Translations.Database("APIChecker_Online");
                default:
                    return Translations.Database("APIChecker_Offline");
            }
        }
    }

    class VisualsAPIChecker
    {
        public static bool UnitedSL { get; set; }
        public static bool UnitedCDNL { get; set; }
        public static APIStatus UnitedSC { get; set; } = APIStatus.Unknown;
        public static bool UnitedAPI() => (UnitedSL && UnitedCDNL);

        public static bool CarbonSL { get; set; }
        public static bool CarbonCDNL { get; set; }
        public static APIStatus CarbonSC { get; set; } = APIStatus.Unknown;
        public static bool CarbonAPI() => (CarbonSL && CarbonCDNL);

        public static bool CarbonTwoSL { get; set; }
        public static bool CarbonTwoCDNL { get; set; }
        public static APIStatus CarbonTwoSC { get; set; } = APIStatus.Unknown;
        public static bool CarbonAPITwo() => (CarbonTwoSL && CarbonTwoCDNL);

        public static bool Local_Cached_SL { get; set; }
        public static bool Local_Cached_CDNL { get; set; }
        public static APIStatus Local_Cached_SC { get; set; } = APIStatus.Unknown;
        public static bool Local_Cached_API() => (Local_Cached_SL && Local_Cached_CDNL);

        public static bool GitHubAPI { get; set; }
        public static APIStatus GitHubAPISC { get; set; } = APIStatus.Unknown;

        public static async void PingAPIStatus()
        {
            LogToFileAddons.Parent_Log_Screen(1, "API", "Checking Status");
            if (!InsiderKit.EnableInsiderDeveloper.Allowed())
            {
                LogToFileAddons.Parent_Log_Screen(2, "API", "Checking WorldUnited Status");
                await Task.Run(() => 
                {
                    switch (UnitedSC = API_Core.StatusCheck(URLs.Main + "/serverlist.json", 15))
                    {
                        case APIStatus.Online:
                            UnitedSL = RetrieveJSON(URLs.Main + "/serverlist.json", "SL", UnitedSC);
                            if (UnitedSL) { UnitedCDNL = RetrieveJSON(URLs.Main + "/cdn_list.json", "CDNL", UnitedSC); }
                            LogToFileAddons.Parent_Log_Screen(3, "API", "WorldUnited Status Check");
                            break;
                        default:
                            LogToFileAddons.Parent_Log_Screen(3, "API", "WorldUnited Status Check");
                            break;
                    }
                });
            }
            else
            {
                UnitedSC = APIStatus.NotImplmented;
            }

            if (!UnitedAPI())
            {
                LogToFileAddons.Parent_Log_Screen(2, "API", "Checking DavidCarbon Status");
                await Task.Run(() =>
                {
                    switch (CarbonSC = API_Core.StatusCheck(URLs.Static + "/serverlist.json", 15))
                    {
                        case APIStatus.Online:
                            if (!UnitedSL) { CarbonSL = RetrieveJSON(URLs.Static + "/serverlist.json", "SL", CarbonSC); }
                            else { CarbonSL = true; }
                            if (!UnitedCDNL) { CarbonCDNL = RetrieveJSON(URLs.Static + "/cdnlist.json", "CDNL", CarbonSC); }
                            else { CarbonCDNL = true; }
                            LogToFileAddons.Parent_Log_Screen(3, "API", "DavidCarbon Status Check");
                            break;
                        default:
                            LogToFileAddons.Parent_Log_Screen(3, "API", "DavidCarbon Status Check");
                            break;
                    }
                });
            }
            else
            {
                CarbonSL = CarbonCDNL = true;
            }

            if (!CarbonAPI())
            {
                LogToFileAddons.Parent_Log_Screen(2, "API", "Checking DavidCarbon [Second] Status");
                await Task.Run(() =>
                {
                    switch (CarbonTwoSC = API_Core.StatusCheck(URLs.Static_Alt + "/serverlist.json", 15))
                    {
                        case APIStatus.Online:
                            if (!CarbonSL) { CarbonTwoSL = RetrieveJSON(URLs.Static_Alt + "/serverlist.json", "SL", CarbonTwoSC); }
                            else { CarbonTwoSL = true; }
                            if (!CarbonCDNL) { CarbonTwoCDNL = RetrieveJSON(URLs.Static_Alt + "/cdnlist.json", "CDNL", CarbonTwoSC); }
                            else { CarbonTwoCDNL = true; }
                            LogToFileAddons.Parent_Log_Screen(3, "API", "DavidCarbon [Second] Status Check");
                            break;
                        default:
                            LogToFileAddons.Parent_Log_Screen(3, "API", "DavidCarbon [Second] Status Check");
                            break;
                    }
                });
            }
            else
            {
                CarbonTwoSL = CarbonTwoCDNL = true;
            }

            if (!CarbonAPITwo())
            {
                LogToFileAddons.Parent_Log_Screen(2, "API", "Checking Local Cache");
                await Task.Run(() =>
                {
                    string Launcher_Data_Folder = Path.Combine("Launcher_Data", "JSON", "Lists");
                    string Server_List_Cache = Path.Combine(Launcher_Data_Folder, "Game_Servers.json");
                    string CDN_List_Cache = Path.Combine(Launcher_Data_Folder, "Content_Delivery_Networks.json");

                    if (File.Exists(Server_List_Cache))
                    {
                        if (File.ReadAllText(Server_List_Cache).Valid_Json())
                        {
                            Local_Cached_SL = RetrieveJSON(Server_List_Cache, "SL", Local_Cached_SC, true, File.ReadAllText(Server_List_Cache));
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "API", "Invalid Game Servers Cache File");
                        }
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "API", "No Game Servers Cache File Found");
                    }

                    if (File.Exists(CDN_List_Cache))
                    {
                        if (File.ReadAllText(CDN_List_Cache).Valid_Json())
                        {
                            Local_Cached_CDNL = RetrieveJSON(CDN_List_Cache, "CDNL", Local_Cached_SC, true, File.ReadAllText(CDN_List_Cache));
                        }
                        else
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "API", "Invalid Content Delivery Networks Cache File");
                        }
                    }
                    else
                    {
                        LogToFileAddons.Parent_Log_Screen(5, "API", "No Content Delivery Networks Cache File Found");
                    }
                });
            }
            else
            {
                Local_Cached_SL = Local_Cached_CDNL = true;
            }

            LogToFileAddons.Parent_Log_Screen(2, "API", "Checking API Results");
            /* Check If Launcher Failed to Connect to any APIs */
            if (!Local_Cached_API())
            {
                Presence_Launcher.Status(0, "Launcher Encountered API Errors");

                if (MessageBox.Show(null, Translations.Database("VisualsAPIChecker_TextBox_No_API"),
                    Translations.Database("VisualsAPIChecker_TextBox_No_API_P2"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    FunctionStatus.LauncherForceClose = true;
                }
                else
                {
                    LogToFileAddons.Parent_Log_Screen(4, "API PROMPT CHECK", 
                        "User has Bypassed 'No Internet Connection' Check and will Continue");
                    MessageBox.Show(Translations.Database("VisualsAPIChecker_TextBox_No_API_P3"),
                        Translations.Database("VisualsAPIChecker_TextBox_No_API_P4"));
                }
            }
            LogToFileAddons.Parent_Log_Screen(3, "API", "Done Checking API Results");

            if (FunctionStatus.LauncherForceClose)
            {
                FunctionStatus.ErrorCloseLauncher("Closing From API Check Error", false);
            }
            else
            {
                FunctionStatus.IsVisualAPIsChecked = true;

                LogToFileAddons.Parent_Log_Screen(1, "LIST CORE", "Moved to Function");
                /* (Start Process) Check ServerList Status */
                ServerListUpdater.GetList();
            }
        }

#pragma warning disable CS8618
        private static string OnlineListJson { get; set; }
#pragma warning restore CS8618

        private static bool RetrieveJSON(string JSONUrl, string Function, APIStatus API_Name, bool ByPass_Online = false, string ByPass_List = "")
        {
            LogToFileAddons.Parent_Log_Screen(2, "JSON LIST", "Retriving JSON LIST " + JSONUrl);
            try
            {
                if (!ByPass_Online)
                {
                    Uri URLCall = new Uri(JSONUrl);
#pragma warning disable SYSLIB0014 // Type or member is obsolete
                    ServicePointManager.FindServicePoint(URLCall).ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                    var Client = new WebClient
                    {
                        Encoding = Encoding.UTF8,
                        CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore)
                    };
#pragma warning restore SYSLIB0014 // Type or member is obsolete

                    if (!Launcher_Value.Launcher_Alternative_Webcalls()) 
                    { 
                        Client = new WebClientWithTimeout { Encoding = Encoding.UTF8, CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore) }; 
                    }
                    else
                    {
                        Client.Headers.Add("user-agent", "SBRW Launcher " +
                        Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)");
                    }

                    try
                    {
                        OnlineListJson = Client.DownloadString(URLCall);
                        API_Name = APIStatus.Online;
                        LogToFileAddons.Parent_Log_Screen(6, "JSON LIST", "Retrieved " + JSONUrl);
                    }
                    catch (WebException Error)
                    {
                        APIStatus API_Status = API_Core.StatusCodes(JSONUrl, Error, Error.Response as HttpWebResponse);
                        API_Name = API_Status;

                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "JSON LIST", Error.InnerException.Message);
                        }

                        return false;
                    }
                    catch (Exception Error)
                    {
                        LogToFileAddons.OpenLog("JSON LIST", string.Empty, Error, string.Empty, true);
                        API_Name = APIStatus.Unknown;

                        if (Error.InnerException != null && !string.IsNullOrWhiteSpace(Error.InnerException.Message))
                        {
                            LogToFileAddons.Parent_Log_Screen(5, "JSON LIST", Error.InnerException.Message, false, true);
                        }

                        return false;
                    }
                    finally
                    {
                        Client?.Dispose();

                        #if !(RELEASE_UNIX || DEBUG_UNIX) 
                        GC.Collect(); 
                        #endif
                    }
                }
                else
                {
                    API_Name = APIStatus.Online;
                    OnlineListJson = ByPass_List;
                }

                if (OnlineListJson.Valid_Json())
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
                    LogToFileAddons.Parent_Log_Screen(3, "JSON LIST", "Valid " + JSONUrl);

                    return true;
                }
                else
                {
                    LogToFileAddons.Parent_Log_Screen(3, "JSON LIST", "Invalid " + JSONUrl);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(OnlineListJson))
                {
                    OnlineListJson = string.Empty;
                }

                #if !(RELEASE_UNIX || DEBUG_UNIX) 
                GC.Collect(); 
                #endif
            }
        }
    }
}
