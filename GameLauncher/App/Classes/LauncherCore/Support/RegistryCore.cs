using GameLauncher.App.Classes.LauncherCore.Logger;
using Microsoft.Win32;
using System;
using System.IO;

namespace GameLauncher.App.Classes.LauncherCore.Support
{
    class RegistryCore
    {
        /// <summary>
        /// Reads a key from the Windows Registry.
        /// </summary>
        /// <returns>Registry Key Value or if it Doesn't Exist it Returns a String.Empty Value</returns>
        /// <param name="Key_Name">Registry Key Entry</param>
        public static string Read(string Key_Name)
        {
            string subKey = Path.Combine("SOFTWARE", "Soapbox Race World", "Launcher");

            RegistryKey sk = null;
            try
            {
                sk = Registry.LocalMachine.OpenSubKey(subKey, false) ?? null;
                if (sk == null)
                    return string.Empty;
                else
                    return sk.GetValue(Key_Name).ToString();
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("READ REGISTRYKEY", null, Error, null, true);
                return string.Empty;
            }
            finally
            {
                if (sk != null)
                {
                    sk.Close();
                    sk.Dispose();
                }
            }
        }
        /// <summary>
        /// Reads a key from the Windows Registry.
        /// </summary>
        /// <returns>Registry Key Value or if it Doesn't Exist it Returns a String.Empty Value</returns>
        /// <param name="Key_Name">Registry Key Entry</param>
        /// <param name="Key_Path">Registry Key Path</param>
        public static string Read(string Key_Name, string Key_Path)
        {
            RegistryKey sk = null;
            try
            {
                sk = Registry.LocalMachine.OpenSubKey(Key_Path, false) ?? null;
                if (sk == null)
                    return string.Empty;
                else
                    return sk.GetValue(Key_Name).ToString();
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("READ REGISTRYKEY", null, Error, null, true);
                return string.Empty;
            }
            finally
            {
                if (sk != null)
                {
                    sk.Close();
                    sk.Dispose();
                }
            }
        }
        /// <summary>
        /// Writes a key to the Windows Registry.
        /// </summary>
        /// <param name="Key_Name">Registry Key Entry</param>
        /// <param name="Key_Value">Inner Value to write to for Key Entry</param>
        public static void Write(string Key_Name, string Key_Value)
        {
            string subKey = Path.Combine("SOFTWARE", "Soapbox Race World", "Launcher");
            RegistryKey sk = null;

            try
            {
                sk = Registry.LocalMachine.CreateSubKey(subKey, true);
                sk.SetValue(Key_Name, Key_Value);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WRITE REGISTRYKEY", null, Error, null, true);
            }
            finally
            {
                if (sk != null)
                {
                    sk.Close();
                    sk.Dispose();
                }
            }
        }
        /// <summary>
        /// Writes a key to the Windows Registry.
        /// </summary>
        /// <param name="Key_Name">Registry Key Entry</param>
        /// <param name="Key_Value">Inner Value to write to for Key Entry</param>
        /// <param name="Key_Path">Registry Key Path</param>
        public static void Write(string Key_Name, string Key_Value, string Key_Path)
        {
            RegistryKey sk = null;

            try
            {
                sk = Registry.LocalMachine.CreateSubKey(Key_Path, true);
                sk.SetValue(Key_Name, Key_Value);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WRITE REGISTRYKEY", null, Error, null, true);
            }
            finally
            {
                if (sk != null)
                {
                    sk.Close();
                    sk.Dispose();
                }
            }
        }
        /// <summary>
        /// Writes a key to the Windows Registry.
        /// </summary>
        /// <param name="Key_Name">Registry Key Entry</param>
        /// <param name="Key_Value">Inner Value to write to for Key Entry</param>
        /// <param name="Key_Path">Registry Key Path</param>
        public static void Write(string Key_Name, object Key_Value, string Key_Path)
        {
            RegistryKey sk = null;

            try
            {
                sk = Registry.LocalMachine.CreateSubKey(Key_Path, true);
                sk.SetValue(Key_Name, Key_Value);
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("WRITE REGISTRYKEY", null, Error, null, true);
            }
            finally
            {
                if (sk != null)
                {
                    sk.Close();
                    sk.Dispose();
                }
            }
        }
    }
}
