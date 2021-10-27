using GameLauncher.App.Classes.LauncherCore.Global;
using SBRWCore.Classes.References;

namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    /// <summary>
    /// Global Account Save System
    /// </summary>
    /// <remarks>Used to set Values and Save them</remarks>
    class FileAccountSave
    {
        /// <summary>Account Format Information In Live Memory</summary>
        public static Format_Account Live_Data = new Format_Account();
        ///<value>Account File Information on Disk</value>
        private static IniFile AccountFile;
        ///<value>Used to Save Login Information when Remember me is Checked Marked</value>
        public static bool SaveLoginInformation = false;
        /// <summary>
        /// Null Safe Values Checker
        /// </summary>
        /// <remarks>Used to create, update, or remove Values before Critical Launcher Checks</remarks>
        public static void NullSafe()
        {
            AccountFile = new IniFile(Locations.RoamingAppDataFolder_Launcher_Account);

            if (!AccountFile.KeyExists("Server"))
            {
                AccountFile.Write("Server", Live_Data.Saved_Server_Address);
            }
            else
            {
                Live_Data.Saved_Server_Address = AccountFile.Read("Server");
            }

            if (!AccountFile.KeyExists("Hash"))
            {
                AccountFile.Write("Hash", Live_Data.Saved_Server_Hash_Version);
            }
            else
            {
                Live_Data.Saved_Server_Hash_Version = AccountFile.Read("Hash");
            }

            if (!AccountFile.KeyExists("AccountEmail"))
            {
                AccountFile.Write("AccountEmail", Live_Data.User_Raw_Email);
            }
            else
            {
                Live_Data.User_Raw_Email = AccountFile.Read("AccountEmail");
            }

            if (!AccountFile.KeyExists("AccountEmailHashed"))
            {
                AccountFile.Write("AccountEmailHashed", Live_Data.User_Hashed_Email);
            }
            else
            {
                Live_Data.User_Hashed_Email = AccountFile.Read("AccountEmailHashed");
            }

            if (AccountFile.KeyExists("Password"))
            {
                Live_Data.User_Raw_Password = AccountFile.Read("Password");
                AccountFile.DeleteKey("Password");
            }

            if (!AccountFile.KeyExists("PasswordHashed"))
            {
                AccountFile.Write("PasswordHashed", Live_Data.User_Hashed_Password);
            }
            else
            {
                Live_Data.User_Hashed_Password = AccountFile.Read("PasswordHashed");
            }

            if (!AccountFile.KeyExists("PasswordRaw"))
            {
                AccountFile.Write("PasswordRaw", Live_Data.User_Raw_Password);
            }
            else if (AccountFile.KeyExists("PasswordRaw"))
            {
                Live_Data.User_Raw_Password = AccountFile.Read("PasswordRaw");
            }

            AccountFile = new IniFile(Locations.RoamingAppDataFolder_Launcher_Account);
        }
        /// <summary>
        /// Account Information Saver
        /// </summary>
        /// <remarks>Used to create, update, or remove Values after a successful login</remarks>
        public static void Save()
        {
            AccountFile = new IniFile(Locations.RoamingAppDataFolder_Launcher_Account);

            if (!AccountFile.KeyExists("Server") || AccountFile.Read("Server") != Live_Data.Saved_Server_Address)
            {
                AccountFile.Write("Server", Live_Data.Saved_Server_Address);
            }

            if (!AccountFile.KeyExists("Hash") || AccountFile.Read("Hash") != Live_Data.Saved_Server_Hash_Version)
            {
                AccountFile.Write("Hash", Live_Data.Saved_Server_Hash_Version);
            }

            if (SaveLoginInformation)
            {
                if (!AccountFile.KeyExists("AccountEmail") || AccountFile.Read("AccountEmail") != Live_Data.User_Raw_Email)
                {
                    AccountFile.Write("AccountEmail", Live_Data.User_Raw_Email);
                }

                if (!AccountFile.KeyExists("AccountEmailHashed") || AccountFile.Read("AccountEmailHashed") != Live_Data.User_Hashed_Email)
                {
                    AccountFile.Write("AccountEmailHashed", Live_Data.User_Hashed_Email);
                }

                if (!AccountFile.KeyExists("PasswordHashed") || AccountFile.Read("PasswordHashed") != Live_Data.User_Hashed_Password)
                {
                    AccountFile.Write("PasswordHashed", Live_Data.User_Hashed_Password);
                }

                if (!AccountFile.KeyExists("PasswordRaw") || AccountFile.Read("PasswordRaw") != Live_Data.User_Raw_Password)
                {
                    AccountFile.Write("PasswordRaw", Live_Data.User_Raw_Password);
                }
            }
            else
            {
                AccountFile.Write("AccountEmail", string.Empty);
                AccountFile.Write("AccountEmailHashed", string.Empty);
                AccountFile.Write("PasswordHashed", string.Empty);
                AccountFile.Write("PasswordRaw", string.Empty);
            }

            AccountFile = new IniFile(Locations.RoamingAppDataFolder_Launcher_Account);
        }
    }
}
