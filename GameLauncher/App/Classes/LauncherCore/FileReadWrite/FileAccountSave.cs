namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileAccountSave
    {
        public static IniFile accountFile = new IniFile("Account.ini");

        public static string UserRawEmail = accountFile.Read("AccountEmail");

        public static string UserHashedPassword = accountFile.Read("Password");

        public static string ChoosenGameServer = accountFile.Read("Server");

        public static void NullSafeAccount()
        {
            if (!accountFile.KeyExists("Server"))
            {
                accountFile.Write("Server", string.Empty);
            }

            if (!accountFile.KeyExists("AccountEmail"))
            {
                accountFile.Write("AccountEmail", string.Empty);
            }

            if (!accountFile.KeyExists("Password"))
            {
                accountFile.Write("Password", string.Empty);
            }

            accountFile = new IniFile("Account.ini");
        }

        public static void SaveAccount()
        {
            if (!accountFile.KeyExists("Server") || accountFile.Read("Server") != ChoosenGameServer)
            {
                accountFile.Write("Server", ChoosenGameServer);
            }

            if (!accountFile.KeyExists("AccountEmail") || accountFile.Read("AccountEmail") != UserRawEmail)
            {
                accountFile.Write("AccountEmail", UserRawEmail);
            }

            if (!accountFile.KeyExists("Password") || accountFile.Read("Password") != UserHashedPassword)
            {
                accountFile.Write("Password", UserHashedPassword);
            }

            accountFile = new IniFile("Account.ini");
        }
    }
}
