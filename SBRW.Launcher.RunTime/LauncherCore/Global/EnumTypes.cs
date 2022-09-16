namespace SBRW.Launcher.RunTime.LauncherCore.Global
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
        Unknown,
        Invalid
    }
    /* Used with CheckHash Function in MainScreen.cs and RegisterScreen.cs
    /* Or
    * Other Conditionals */
    ///<summary>(Name Only) Hash Standard Name that is used for Checking functions</summary>
    ///<returns>Hash Standard Name</returns>
    enum AuthHash
    {
        ///<summary><c>[Not Recommended]</c> Hash Version 1.0</summary>
        ///<remarks>Default Standard for https only servers</remarks>
        ///<value>Raw: Email and Password</value>
        H10,
        ///<summary>Hash Version 1.1</summary>
        ///<value>Raw: Email and MD5 Hash: Password</value>
        H11,
        ///<summary>Hash Version 1.2</summary>
        ///<remarks>Default Standard for http only servers</remarks>
        ///<value>Raw: Email and SHA Hash: Password</value>
        H12,
        ///<summary>Hash Version 1.3</summary>
        ///<value>Raw: Email and SHA256 Hash: Password</value>
        H13,
        ///<summary>Hash Version 2.0</summary>
        ///<value>MD5 Hashes: Email and Password</value>
        H20,
        ///<summary>Hash Version 2.1</summary>
        ///<value>SHA Hashes: Email and Password</value>
        H21,
        ///<summary>Hash Version 2.2</summary>
        ///<value>SHA256 Hashes: Email and Password</value>
        H22,
        ///<summary>Invalid Hash Version</summary>
        ///<value>Unknown Specified Hash Version</value>
        Unknown
    }

    /// <summary>
    /// Identifies the operating system, or platform, supported by an assembly.
    /// </summary>
    enum PlatformIDPort
    {
        /// <summary>
        /// The operating system is Win32s. This value is no longer in use.
        /// </summary>
        Win32S = 0,
        /// <summary>
        /// The operating system is Windows 95 or Windows 98. This value is no longer in use.
        /// </summary>
        Win32Windows = 1,
        /// <summary>
        /// The operating system is Windows NT or later.
        /// </summary>
        Win32NT = 2,
        /// <summary>
        /// The operating system is Windows CE. This value is no longer in use.
        /// </summary>
        WinCE = 3,
        /// <summary>
        /// The operating system is Unix.
        /// </summary>
        Unix = 4,
        /// <summary>
        /// The development platform is Xbox 360. This value is no longer in use.
        /// </summary>
        Xbox = 5,
        /// <summary>
        /// The operating system is Macintosh. This value was returned by Silverlight. On .NET Core, its replacement is Unix.
        /// </summary>
        MacOSX = 6,
        /// <summary>
        /// The operating system is Unix. This value was returned by Mono CLR 1.x runtime.
        /// </summary>
        MonoLegacy = 128,
        /// <summary>
        /// Unable to detect operating system. This value is used as a fail safe.
        /// </summary>
        Unknown = 2017
    }
}
