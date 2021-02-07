using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GameLauncher.App.Classes.SystemPlatform.Windows
{
    class FileORFolderPermissions
    {
        public static void GiveEveryoneReadWriteFileAccess(string path)
        {
            var everyone = new SecurityIdentifier(WellKnownSidType.WorldSid,
                                                  null);

            var accessRule = new FileSystemAccessRule(everyone,
                                                      FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow);

            var fileSecurity = File.GetAccessControl(path);
            fileSecurity.AddAccessRule(accessRule);
            File.SetAccessControl(path, fileSecurity);
        }

        public static void GiveEveryoneReadWriteFolderAccess(string path)
        {
            DirectoryInfo Info = new DirectoryInfo(path);
            DirectorySecurity FolderSecurity = Info.GetAccessControl();

            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

            FolderSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl,
                InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit,
                AccessControlType.Allow));

            Directory.SetAccessControl(path, FolderSecurity);
        }

        /* Checks if File or Folder Permissions is Set and returns a Boolen Value */

        public static bool CheckIfFilePermissionIsSet(string path)
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

            return IsPermsGood;
        }

        public static bool CheckIfFolderPermissionIsSet(string path)
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

            return IsPermsGood;
        }

        /* Run The Functions */
    }
}
