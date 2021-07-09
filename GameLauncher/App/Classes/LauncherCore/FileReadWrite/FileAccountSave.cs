namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    /// <summary>
    /// Global Account Save System
    /// </summary>
    /// <remarks>Used to set Values and Save them</remarks>
    class FileAccountSave
    {
        ///<value>Account File Information on Disk</value>
        public static IniFile accountFile = new IniFile("Account.ini");
        ///<value>Used to Save Login Information when Remember me is Checked Marked</value>
        public static bool SaveLoginInformation = false;

        public static string UserRawEmail = accountFile.Read("AccountEmail");

        public static string UserHashedEmail = accountFile.Read("AccountEmailHashed");

        public static string UserHashedPassword = accountFile.Read("PasswordHashed");

        public static string UserRawPassword = accountFile.Read("PasswordRaw");

        public static string ChoosenGameServer = accountFile.Read("Server");

        public static string SavedGameServerHash = accountFile.Read("Hash");

        /// <summary>
        /// Null Safe Values Checker
        /// </summary>
        /// <remarks>Used to create, update, or remove Values before Critical Launcher Checks</remarks>
        public static void NullSafeAccount()
        {
            if (!accountFile.KeyExists("Server"))
            {
                accountFile.Write("Server", string.Empty);
            }

            if (!accountFile.KeyExists("Hash"))
            {
                accountFile.Write("Hash", string.Empty);
            }

            if (!accountFile.KeyExists("AccountEmail"))
            {
                accountFile.Write("AccountEmail", string.Empty);
            }

            if (!accountFile.KeyExists("AccountEmailHashed"))
            {
                accountFile.Write("AccountEmailHashed", string.Empty);
            }

            if (accountFile.KeyExists("Password"))
            {
                UserHashedPassword = accountFile.Read("Password");
                accountFile.DeleteKey("Password");
            }

            if (!accountFile.KeyExists("PasswordHashed"))
            {
                accountFile.Write("PasswordHashed", string.Empty);
            }

            if (!accountFile.KeyExists("PasswordRaw"))
            {
                accountFile.Write("PasswordRaw", string.Empty);
            }

            accountFile = new IniFile("Account.ini");
        }

        /// <summary>
        /// Account Information Saver
        /// </summary>
        /// <remarks>Used to create, update, or remove Values after a successful login</remarks>
        public static void SaveAccount()
        {
            if (!accountFile.KeyExists("Server") || accountFile.Read("Server") != ChoosenGameServer)
            {
                accountFile.Write("Server", ChoosenGameServer);
            }

            if (!accountFile.KeyExists("Hash") || accountFile.Read("Hash") != SavedGameServerHash)
            {
                accountFile.Write("Hash", SavedGameServerHash);
            }

            if (SaveLoginInformation)
            {
                if (!accountFile.KeyExists("AccountEmail") || accountFile.Read("AccountEmail") != UserRawEmail)
                {
                    accountFile.Write("AccountEmail", UserRawEmail);
                }

                if (!accountFile.KeyExists("AccountEmailHashed") || accountFile.Read("AccountEmailHashed") != UserHashedEmail)
                {
                    accountFile.Write("AccountEmailHashed", UserHashedEmail);
                }

                if (!accountFile.KeyExists("PasswordHashed") || accountFile.Read("PasswordHashed") != UserHashedPassword)
                {
                    accountFile.Write("PasswordHashed", UserHashedPassword);
                }

                if (!accountFile.KeyExists("PasswordRaw") || accountFile.Read("PasswordRaw") != UserRawPassword)
                {
                    accountFile.Write("PasswordRaw", UserRawPassword);
                }
            }
            else
            {
                accountFile.Write("AccountEmail", string.Empty);
                accountFile.Write("AccountEmailHashed", string.Empty);
                accountFile.Write("PasswordHashed", string.Empty);
                accountFile.Write("PasswordRaw", string.Empty);
            }

            accountFile = new IniFile("Account.ini");
        }
    }
}
