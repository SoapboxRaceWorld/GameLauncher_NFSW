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
        BadGateway,
        Forbidden,
        NotFound,
        NotImplmented,
        ServerError,
        ServerUnavailable,
        GetWayTimeOut,
        ConnectionTimeOut,
        OriginUnreachable,
        OriginError,
        Timeout,
        SSLFailed,
        InvaildSSL,
        Unauthorized,
        Unknown,
        Null
    }
}
