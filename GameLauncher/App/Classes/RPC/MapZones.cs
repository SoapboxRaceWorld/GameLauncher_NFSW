using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class MapZones {
        public static string getZoneName(string color) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.zones.json"));

            foreach (var item in dynJson) {
                if (item.color == color) {
                    return item.name;
                }
            }

            return null;
        }
    }
}
