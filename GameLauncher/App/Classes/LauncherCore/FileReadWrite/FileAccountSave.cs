namespace GameLauncher.App.Classes.LauncherCore.FileReadWrite
{
    class FileAccountSave
    {
        public static IniFile accountFile = new IniFile("Account.ini");

        public static string UserRawEmail = accountFile.Read("AccountEmail");

        public static string UserHashedPassword = accountFile.Read("Password");

        public static string ChoosenGameServer = accountFile.Read("Server");

        public static void SaveChoosenServer()
        {
            if (accountFile.Read("Server") != ChoosenGameServer)
            {
                accountFile.Write("Server", ChoosenGameServer);
            }

            accountFile = new IniFile("Account.ini");
        }

        public static void SaveAccount()
        {
            if (accountFile.Read("AccountEmail") != UserRawEmail)
            {
                accountFile.Write("AccountEmail", UserRawEmail);
            }

            if (accountFile.Read("Password") != UserHashedPassword)
            {
                accountFile.Write("Password", UserHashedPassword);
            }

            accountFile = new IniFile("Account.ini");
        }
    }
}
