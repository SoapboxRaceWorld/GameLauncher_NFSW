using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameLauncher.App.Classes.GPU {
    class NVIDIA : GPU {
        public string _driverVersion = String.Empty;
        public string _manufacturer = String.Empty;
        ManagementObjectSearcher objvide;
        public NVIDIA() : base() {
            objvide = new ManagementObjectSearcher("select * from Win32_VideoController");
        }

        public override string DriverVersion() {
            if (_driverVersion != String.Empty) return _driverVersion;

            foreach (ManagementObject obj in objvide.Get()) {
                string split_and_replace = obj["DriverVersion"].ToString().Replace(".", "");
                _driverVersion = split_and_replace.Substring(split_and_replace.Length - 5).Insert(3, ".");
            }

            return _driverVersion;
        }
    }
}
