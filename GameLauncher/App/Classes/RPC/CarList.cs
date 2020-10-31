using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class CarList {
        public static String remoteCarList = String.Empty;

        public static string GetCarName(string id) {
            // Let's load the "Cached From Server" version first
            if (remoteCarList != String.Empty) {
                dynamic dynJson = JsonConvert.DeserializeObject(remoteCarList);

                foreach (var item in dynJson) {
                    if (item.carid == id) {
                        return item.carname;
                    }
                }
            }

            // If we don't have a Server version, load "default" version
            if (remoteCarList == String.Empty) {
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
