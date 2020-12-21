using System;

namespace GameLauncher.App.Classes.Auth
{
    class Tokens {
        public static String UserId         = String.Empty;
        public static String Warning        = String.Empty;
        public static String Error          = String.Empty;
        public static String Success        = String.Empty;
        public static String LoginToken     = String.Empty;

        public static String IPAddress      = String.Empty;
        public static String ServerName     = String.Empty;

        public static void Clear() {
            UserId = String.Empty;
            Warning = String.Empty;
            Error = String.Empty;
            Success = String.Empty;
            LoginToken = String.Empty;
            IPAddress = String.Empty;
            ServerName = String.Empty;
        }
    }
}
