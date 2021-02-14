using GameLauncher.App.Classes.LauncherCore.Global;
using GameLauncher.App.Classes.Logger;
using System;
using System.Net;
using System.Windows.Forms;

namespace GameLauncher.App.Classes
{
    class APIStatusChecker
    {
        public static APIStatus CheckStatus(string APIURI)
        {
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
                    Log.Info("CORE: " + APIURI + " is Online!");
                    return APIStatus.Online;
                    //Do something with response.Headers to find out information about the request
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
                        //Set flag if there was a timeout or some other issues
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
                    }
                }
            }
            return APIStatus.Null;
        }
    }
}
