using Newtonsoft.Json;
using System;
using GameLauncher.App.Classes.Logger;

namespace GameLauncher.App.Classes.RPC
{
    class CarsList {
        public static String remoteCarsList = String.Empty;

        public static string GetCarName(string id) {
            // Let's load the "Cached From Server" version first
            if (remoteCarsList != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteCarsList);

                foreach (var item in dynJson) {
                    if (item.carid == id) {
                        return item.carname;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteCarsList == String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.cars.json"));

                foreach (var item in dynJson) {
                    if (item.carid == id) {
                        return item.carname;
                    }
                }
            }

            // And if it's not found, do this instead
            return "Traffic Car ("+id+")";
        }
    }
}
