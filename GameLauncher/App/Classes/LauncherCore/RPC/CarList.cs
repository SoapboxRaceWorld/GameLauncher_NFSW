using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Support;
using Newtonsoft.Json;
using System;

namespace GameLauncher.App.Classes.LauncherCore.RPC
{
    class CarsList
    {
        public static String remoteCarsList = String.Empty;

        public static string GetCarName(string id)
        {
            /* Let's load the "Cached From Server" version first */
            if (remoteCarsList != String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(remoteCarsList));

                foreach (var item in dynJson)
                {
                    if (item.carid == id)
                    {
                        return item.carname;
                    }
                }
            }

            /* If we don't have a Server version, load "default" version */
            if (remoteCarsList == String.Empty)
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Strings.Encode(ExtractResource.AsString("GameLauncher.App.Classes.LauncherCore.RPC.JSON.cars.json")));

                foreach (var item in dynJson)
                {
                    if (item.carid == id)
                    {
                        return item.carname;
                    }
                }
            }

            /* And if it's not found, do this instead */
            return "Addon Car (" + id + ")";
        }
    }
}
