using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class CarList {
        public static String remoteCarList = String.Empty;

        public static string getCarName(string id) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.cars.json"));

            foreach (var item in dynJson) {
                if (item.carid == id) {
                    return item.carname;
                }
            }

            if (remoteCarList != String.Empty) {
                try {
                    dynamic dynJson2 = JsonConvert.DeserializeObject(remoteCarList);

                    foreach (var item in dynJson2) {
                        if (item.id == id) {
                            return item.carname;
                        }
                    }
                } catch { }
            }

            return "Traffic Car ("+id+")";
        }
    }
}
