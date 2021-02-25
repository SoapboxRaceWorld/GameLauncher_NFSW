using GameLauncher.App.Classes.Logger;
using GameLauncher.App.Classes.SystemPlatform.Linux;
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
            if (!DetectLinux.LinuxDetected())
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
                catch (Exception ex)
                {
                    Log.Error("FILE PERMISSION: " + ex.Message);
                }
            }
        }

        /* Sets Folder Permissions */
        public static void GiveEveryoneReadWriteFolderAccess(string path)
        {
            if (!DetectLinux.LinuxDetected())
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
                catch (Exception ex)
                {
                    Log.Error("FOLDER PERMISSION: " + ex.Message);
                }
            }
        }

        /*** Checks if File or Folder Permissions is Set and returns a Boolen Value ***/

        /* Checks File Permissions */
        public static bool CheckIfFilePermissionIsSet(string path)
        {
            if (!DetectLinux.LinuxDetected())
            {
                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                var fileSecurity = File.GetAccessControl(path);
                var acl = fileSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                bool IsPermsGood = false;

                foreach (FileSystemAccessRule rule in acl)
                {
                    if (rule.IdentityReference.Value == everyone.Value && rule.AccessControlType == AccessControlType.Allow
                        && (rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read)
                    {
                        IsPermsGood = true;
                    }
                }

                Log.Info("FILE PERMISSION: [" + path + "] Is permission set? -> " + IsPermsGood);

                return IsPermsGood;
            }
            else
            {
                return true;
            }
        }

        /* Checks Folder Permissions */
        public static bool CheckIfFolderPermissionIsSet(string path)
        {
            if (!DetectLinux.LinuxDetected())
            {
                var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                DirectoryInfo Info = new DirectoryInfo(path);
                DirectorySecurity FolderSecurity = Info.GetAccessControl();

                var acl = FolderSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));

                bool IsPermsGood = false;

                foreach (FileSystemAccessRule rule in acl)
                {
                    if (rule.IdentityReference.Value == everyone.Value && rule.AccessControlType == AccessControlType.Allow
                        && (rule.FileSystemRights & FileSystemRights.Read) == FileSystemRights.Read)
                    {
                        IsPermsGood = true;
                    }
                }

                Log.Info("FOLDER PERMISSION: [" + path + "] Is permission set? -> " + IsPermsGood);

                return IsPermsGood;
            }
            else
            {
                return true;
            }
        }

        /* Run The Functions to see if They need to be setup */

        public static void CheckLauncherPerms(string WhichFunction, string FileORFolderPath)
        {
            if (!DetectLinux.LinuxDetected())
            {
                if (WhichFunction == "Folder")
                {
                    if (Directory.Exists(FileORFolderPath))
                    {
                        if (CheckIfFolderPermissionIsSet(FileORFolderPath) == false)
                        {
                            GiveEveryoneReadWriteFolderAccess(FileORFolderPath);
                        }
                    }
                }
                else if (WhichFunction == "File")
                {
                    if (File.Exists(FileORFolderPath))
                    {
                        if (CheckIfFilePermissionIsSet(FileORFolderPath) == false)
                        {
                            GiveEveryoneReadWriteFileAccess(FileORFolderPath);
                        }
                    }
                }
            }
        }
    }
}
