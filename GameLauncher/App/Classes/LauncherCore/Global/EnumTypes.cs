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
        ///<summary><c>[Not Recommended]</c> Hash Version 1.0</summary>
        ///<remarks>Default Standard for https only servers</remarks>
        /// <value>Raw: Email and Password</value>
        H10,
        ///<summary>Hash Version 1.1</summary>
        /// <value>Raw: Email and MD5 Hash: Password</value>
        H11,
        ///<summary>Hash Version 1.2</summary>
        ///<remarks>Default Standard for http only servers</remarks>
        /// <value>Raw: Email and SHA Hash: Password</value>
        H12,
        ///<summary>Hash Version 1.3</summary>
        /// <value>Raw: Email and SHA256 Hash: Password</value>
        H13,
        ///<summary>Hash Version 2.0</summary>
        /// <value>MD5 Hashes: Email and Password</value>
        H20,
        ///<summary>Hash Version 2.1</summary>
        /// <value>SHA Hashes: Email and Password</value>
        H21,
        ///<summary>Hash Version 2.2</summary>
        /// <value>SHA256 Hashes: Email and Password</value>
        H22,
        ///<summary>Invalid Hash Version</summary>
        /// <value>Unknown Specified Hash Version</value>
        Unknown
    }
}
