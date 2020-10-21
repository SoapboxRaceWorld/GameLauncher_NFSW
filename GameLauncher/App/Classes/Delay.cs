using System;
using System.Windows.Forms;

namespace GameLauncher.App.Classes {
    class Delay {
        public static void WaitSeconds(int sec) {
            if (sec < 1) return;
            DateTime _desired = DateTime.Now.AddSeconds(sec);
            while (DateTime.Now < _desired)
            {
                Application.DoEvents();
            }
        }

        /* Moved "AddMilliseconds" Code to Gist */
        /* https://gist.githubusercontent.com/DavidCarbon/97494268b0175a81a5f89a5e5aebce38/raw/15d5ef79b6c5e3dbca112a65b2a77623c5712411/Delay.cs */

    }
}
