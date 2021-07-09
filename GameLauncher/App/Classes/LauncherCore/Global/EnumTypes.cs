namespace GameLauncher.App.Classes.LauncherCore.Global
{
    /* Used with CheckFolder Function in FunctionStatus.cs
    /* Or
    * Other Conditionals */
    enum FolderType
    {
        IsTempFolder,
        IsUsersFolders,
        IsProgramFilesFolder,
        IsWindowsFolder,
        IsSameAsLauncherFolder,
        IsRootFolder,
        Unknown
    }

    /* Used with CheckStatus Function in APIStatusChecker.cs
    /* Or
     * Other Conditionals */
    enum APIStatus
    {
        Offline,
        Online,
        BadRequest,
        Forbidden,
        NotFound,
        NotImplmented,
        ServerError,
        ServerOverloaded,
        ServerUnavailable,
        GetWayTimeOut,
        ConnectionTimeOut,
        OriginUnreachable,
        Timeout,
        SSLFailed,
        InvaildSSL,
        Unknown,
        Null
    }
    /* Used with CheckHash Function in MainScreen.cs and RegisterScreen.cs
    /* Or
    * Other Conditionals */
    /// <summary>
    /// (Name Only) Hash Standard Name that is used for Checking functions
    /// </summary>
    /// <returns>Hash Standard Name</returns>
    enum AuthHash
    {
        H10,
        H11,
        H12,
        H13,
        Unknown
    }
}
