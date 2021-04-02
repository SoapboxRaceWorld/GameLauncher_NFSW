namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileAccountSave
    {
        public static IniFile accountFile = new IniFile("Account.ini");

        public static string UserRawEmail = accountFile.Read("AccountEmail");

        public static string UserHashedPassword = accountFile.Read("PasswordHashed");

        public static string UserRawPassword = accountFile.Read("PasswordRaw");

        public static string ChoosenGameServer = accountFile.Read("Server");

        public static void NullSafeAccount()
        {
            if (accountFile.KeyExists("Password"))
            {
                UserHashedPassword = accountFile.Read("Password");
                accountFile.DeleteKey("Password");
            }

            if (!accountFile.KeyExists("Server"))
            {
                accountFile.Write("Server", string.Empty);
            }

            if (!accountFile.KeyExists("AccountEmail"))
            {
                accountFile.Write("AccountEmail", string.Empty);
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

            if (!accountFile.KeyExists("PasswordHashed") || accountFile.Read("PasswordHashed") != UserHashedPassword)
            {
                accountFile.Write("PasswordHashed", UserHashedPassword);
            }

            if (!accountFile.KeyExists("PasswordRaw") || accountFile.Read("PasswordRaw") != UserRawPassword)
            {
                accountFile.Write("PasswordRaw", UserRawPassword);
            }

            accountFile = new IniFile("Account.ini");
        }
    }
}
