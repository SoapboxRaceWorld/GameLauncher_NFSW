using GameLauncher.App.Classes.Logger;
using System;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes
{
    class APIStatusChecker
    {
        public static API CheckStatus(string APIURI)
        {
            Log launcherLog = new Log("launcher.log");

            if (!string.IsNullOrEmpty(APIURI))
            {
                HttpWebResponse serverResponse = null;

                try
                {
                    HttpWebRequest requestAPIStatus = (HttpWebRequest)HttpWebRequest.Create(APIURI);
                    requestAPIStatus.AllowAutoRedirect = false; // Find out if this site is up and don't follow a redirector
                    requestAPIStatus.Method = "GET";
                    requestAPIStatus.UserAgent = "GameLauncher " + Application.ProductVersion + " (+https://github.com/SoapBoxRaceWorld/GameLauncher_NFSW)";
                    serverResponse = (HttpWebResponse)requestAPIStatus.GetResponse();
                    launcherLog.Info("CORE: " + APIURI + " is Online!");
                    return API.Online;
                    //Do something with response.Headers to find out information about the request
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        serverResponse = (HttpWebResponse)e.Response;

                        Console.Write("Errorcode: {0}\n", (int)serverResponse.StatusCode);
                        launcherLog.Error("CORE: " + APIURI + " has an Error! Status Code: " + (int)serverResponse.StatusCode);

                        if ((int)serverResponse.StatusCode == 404)
                        {
                            return API.NotFound;
                        }
                        else if ((int)serverResponse.StatusCode == 500)
                        {
                            return API.ServerError;
                        }
                        else if ((int)serverResponse.StatusCode == 502)
                        {
                            return API.ServerOverloaded;
                        }
                        else if ((int)serverResponse.StatusCode == 503)
                        {
                            return API.ServerUnavailable;
                        }
                        else if ((int)serverResponse.StatusCode == 504)
                        {
                            return API.GetWayTimeOut;
                        }
                        else if ((int)serverResponse.StatusCode == 520)
                        {
                            return API.Unknown;
                        }
                        else if ((int)serverResponse.StatusCode == 521)
                        {
                            return API.Offline;
                        }
                        else if ((int)serverResponse.StatusCode == 522)
                        {
                            return API.ConnectionTimeOut;
                        }
                        else if ((int)serverResponse.StatusCode == 523)
                        {
                            return API.OriginUnreachable;
                        }
                        else if ((int)serverResponse.StatusCode == 524)
                        {
                            return API.Timeout;
                        }
                        else if ((int)serverResponse.StatusCode == 525)
                        {
                            return API.SSLFailed;
                        }
                        else if ((int)serverResponse.StatusCode == 526)
                        {
                            return API.InvaildSSL;
                        }
                        //Set flag if there was a timeout or some other issues
                    }
                    else
                    {
                        Console.Write("------------");
                        Console.Write("Error: {0}", e.Status);
                        Console.Write("------------\n");
                        launcherLog.Error("CORE: " + APIURI + " is Offline!");
                        return API.Offline;
                    }
                }
                finally
                {
                    if (serverResponse != null)
                    {
                        serverResponse.Close();
                    }
                }
            }
            return API.Null;
        }
    }
    enum API
    {
        Offline,
        Online,
        BadRequest,
        Forbidden,
        NotFound,
        NotImplmented,
        ServerError,
        ServerOverloaded,
        ServerUnavailable,
        GetWayTimeOut,
        ConnectionTimeOut,
        OriginUnreachable,
        Timeout,
        SSLFailed,
        InvaildSSL,
        Unknown,
        Null
    }
}
