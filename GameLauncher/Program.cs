using System;
using System.Diagnostics;
using System.IO;

namespace GameLauncher
{
    internal class Program
    {
        static void Main()
        {
            try
            {
                if (File.Exists("SBRW.Launcher.exe"))
                {
                    Process.Start("SBRW.Launcher.exe");
                }
                else if (File.Exists("GameLauncherUpdater.exe"))
                {
                    Process.Start("GameLauncherUpdater.exe");
                }
                else
                {
                    Console.WriteLine("No Soapbox Race World Launcher or Updater is Present on System");
                }
            }
            catch (Exception Error)
            {
                if (Error != null)
                {
                    Console.WriteLine("Error Message: " + Error.Message);
                    Console.WriteLine("Error Message: " + Error.ToString());
                    if (Error.InnerException != null)
                    {
                        Console.WriteLine("\n\n");
                        Console.WriteLine("Error Message: " + Error.InnerException.Message);
                        Console.WriteLine("Error Message: " + Error.InnerException.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Error Is Null");
                }
            }
        }
    }
}