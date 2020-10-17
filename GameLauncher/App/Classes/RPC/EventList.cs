using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.App.Classes.RPC {
    class EventList {
        public static String remoteEvent = String.Empty;

        public static string getEventName(int id) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

            foreach (var item in dynJson) {
                if (item.id == id) {
                    return item.trackname;
                }
            }

            //Let's load all the content from cached thing
            if(remoteEvent != String.Empty) {
                try { 
                    dynamic dynJson2 = JsonConvert.DeserializeObject(remoteEvent);

                    foreach (var item in dynJson2) {
                        if (item.id == id) {
                            return item.trackname;
                        }
                    }
                } catch { }
            }

            return "EVENT:"+id;
        }

        public static string getEventType(int id) {
            dynamic dynJson = JsonConvert.DeserializeObject(ExtractResource.AsString("GameLauncher.App.Classes.RPC.JSON.events.json"));

            foreach (var item in dynJson) {
                if (item.id == id) {
                    return item.type;
                }
            }

            //Let's load all the content from cached thing
            if (remoteEvent != String.Empty) {
                try {
                    dynamic dynJson2 = JsonConvert.DeserializeObject(remoteEvent);

                    foreach (var item in dynJson2) {
                        if (item.id == id) {
                            return item.type;
                        }
                    }
                } catch { }
            }

            return "gamemode_freeroam";
        }
    }
}
