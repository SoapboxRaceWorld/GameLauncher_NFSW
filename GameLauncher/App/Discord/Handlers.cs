using GameLauncher.App.Classes.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Discord {
    class Handlers {
        private static Handlers instance = null;
        public static Handlers Instance {
            get {
                if (instance == null)
                {
                    instance = new Handlers();
                }
                return instance;
            }
        }

        public void ErrorCallback(int errorcode, string errormsg) {
            MessageBox.Show("Error " + errorcode + ": " + errormsg);
        }

        public void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser) {
            Log.Debug(string.Format("Discord: connected to {0}#{1}: {2}", connectedUser.username, connectedUser.discriminator, connectedUser.userId));
            System.Windows.Forms.Application.DoEvents();
        }
    }
}
