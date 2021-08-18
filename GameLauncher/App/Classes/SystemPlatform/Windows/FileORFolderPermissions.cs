using GameLauncher.App.Classes.LauncherCore.FileReadWrite;
using GameLauncher.App.Classes.LauncherCore.Logger;
using GameLauncher.App.Classes.SystemPlatform.Unix;
using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class FileORFolderPermissions
    {
        /* Sets File Permissions */
        public static void GiveEveryoneReadWriteFileAccess(string path)
        {
            if (!UnixOS.Detected())
            {
                try
                {
                    var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid,
                      null);

                    var accessRule = new FileSystemAccessRule(everyone,
                                                              FileSystemRights.FullControl,
                        InheritanceFlags.None, PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow);

                    var fileSecurity = File.GetAccessControl(path);
                    fileSecurity.AddAccessRule(accessRule);
                    File.SetAccessControl(path, fileSecurity);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("FILE PERMISSION", null, Error, null, true);
                }
            }
        }

        /* Sets Folder Permissions */
        public static void GiveEveryoneReadWriteFolderAccess(string path)
        {
            if (!UnixOS.Detected())
            {
                try
                {
                    DirectoryInfo Info = new DirectoryInfo(path);
                    DirectorySecurity FolderSecurity = Info.GetAccessControl();

                    SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    FolderSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl,
                        InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                        AccessControlType.Allow));

                    Directory.SetAccessControl(path, FolderSecurity);
                }
                catch (Exception Error)
                {
                    LogToFileAddons.OpenLog("FILE PERMISSION", null, Error, null, true);
                }
            }
        }

        /*** Checks if File or Folder Permissions is Set and returns a Boolen Value ***/

        /* Checks File Permissions */
        public static bool CheckIfFilePermissionIsSet(string path)
        {
            if (!UnixOS.Detected())
            {
                try
                {
                    var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                    var fileSecurity = File.GetAccessControl(path);
                    var acl = fileSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                    bool IsPermsGood = false;

                    foreach (FileSystemAccessRule rule in acl)
                    {
                        if (rule.IdentityReference.Value == everyone.Value && rule.AccessControlType == AccessControlType.Allow
                            && (rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                        {
                            IsPermsGood = true;
                        }
                    }

                    Log.Info("FILE PERMISSION: [" + path + "] Is permission set? -> " + IsPermsGood);

                    return IsPermsGood;
                }
                catch (Exception Error)
                {
                    Log.Error("FILE PERMISSION: [" + path + "] Encounterd an Error" + Error.Message);
                    Log.ErrorIC("FILE PERMISSION: " + Error.HResult);
                    Log.ErrorFR("FILE PERMISSION: " + Error.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /* Checks Folder Permissions */
        public static bool CheckIfFolderPermissionIsSet(string path)
        {
            if (!UnixOS.Detected())
            {
                try
                {
                    var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                    DirectoryInfo Info = new DirectoryInfo(path);
                    DirectorySecurity FolderSecurity = Info.GetAccessControl();

                    var acl = FolderSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                    bool IsPermsGood = false;

                    foreach (FileSystemAccessRule rule in acl)
                    {
                        if (rule.IdentityReference.Value == everyone.Value && rule.AccessControlType == AccessControlType.Allow
                            && (rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write)
                        {
                            IsPermsGood = true;
                        }
                    }

                    Log.Info("FOLDER PERMISSION: [" + path + "] Is permission set? -> " + IsPermsGood);

                    return IsPermsGood;
                }
                catch (Exception Error)
                {
                    Log.Error("FOLDER PERMISSION: [" + path + "] Encounterd an Error" + Error.Message);
                    Log.ErrorIC("FOLDER PERMISSION: " + Error.HResult);
                    Log.ErrorFR("FOLDER PERMISSION: " + Error.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /* Run The Functions to see if They need to be setup */

        public static void CheckLauncherPerms(string WhichFunction, string FileORFolderPath)
        {
            if (!UnixOS.Detected())
            {
                if (WhichFunction == "Folder")
                {
                    if (Directory.Exists(FileORFolderPath))
                    {
                        if (!CheckIfFolderPermissionIsSet(FileORFolderPath))
                        {
                            GiveEveryoneReadWriteFolderAccess(FileORFolderPath);
                        }
                    }
                }
                else if (WhichFunction == "File")
                {
                    if (File.Exists(FileORFolderPath))
                    {
                        if (!CheckIfFilePermissionIsSet(FileORFolderPath))
                        {
                            GiveEveryoneReadWriteFileAccess(FileORFolderPath);
                        }
                    }
                }
            }
        }
    }

    class FileORFolderPermissionsFunctions
    {
        public static void Folders()
        {
            try
            {
                /* Set Folder Permissions Here - DavidCarbon */
                if (FileSettingsSave.FilePermissionStatus != "Set" && !UnixOS.Detected())
                {
                    /* Launcher Folder */
                    FileORFolderPermissions.CheckLauncherPerms("Folder", Path.Combine(AppDomain.CurrentDomain.BaseDirectory));
                    /* Game Files Folder */
                    FileORFolderPermissions.CheckLauncherPerms("Folder", Path.Combine(FileSettingsSave.GameInstallation));
                    FileSettingsSave.FilePermissionStatus = "Set";
                }
                else
                {
                    Log.Core("PERMISSIONS: Checking File! It's value is " + FileSettingsSave.FilePermissionStatus);
                }
            }
            catch (Exception Error)
            {
                LogToFileAddons.OpenLog("PERMISSIONS", null, Error, null, true);
                FileSettingsSave.FilePermissionStatus = "Error";
            }

            FileSettingsSave.SaveSettings();
        }
    }
}
