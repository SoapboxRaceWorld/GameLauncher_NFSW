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
        /* Protocol Error Codes */
        /// <summary>The Remote Web server is down</summary>
        /// <remarks>Occurs when the origin web server refuses connections</remarks>
        Offline,
        /// <summary>The Remote Web server is up</summary>
        /// <remarks>Server Is Up and Running</remarks>
        Online,
        /// <summary>Client Error Side</summary>
        /// <remarks>Server cannot or will not Process the Request</remarks>
        BadRequest,
        /// <summary>No Permission</summary>
        /// <remarks>Server aknowleged the request but refused authorization</remarks>
        Forbidden,
        /// <summary>Not Found</summary>
        /// <remarks>Server can find the requested resource</remarks>
        NotFound,
        /// <summary>Not Supported</summary>
        /// <remarks>Server does not support the functionality required to fulfill the request</remarks>
        NotImplmented,
        /// <summary>Not Supported</summary>
        /// <remarks>Server does not support the functionality required to fulfill the request</remarks>
        ServerError,
        ServerOverloaded,
        ServerUnavailable,
        GetWayTimeOut,
        ConnectionTimeOut,
        OriginUnreachable,
        ///<summary>No response was received during the time-out period for a request.</summary>
        Timeout,
        SSLFailed,
        InvaildSSL,
        ///<summary>Unknown Error was Encountered</summary>
        ///<remarks>Occurs when the origin server returns an empty, unknown, or unexpected response to Cloudflare.</remarks>
        Unknown,
        ///<summary>Provided URI was Null</summary>
        Null,
        ///<summary>Unknown Web Status Code Provided</summary>
        UnknownStatusCode,
        /* WebClient Error Codes */
        ///<summary>No error was encountered.</summary>
        Success,
        ///<summary>The name resolver service could not resolve the host name.</summary>
        NameResolutionFailure,
        ///<summary>The remote service point could not be contacted at the transport level.</summary>
        ConnectFailure,
        ///<summary>A complete response was not received from the remote server.</summary>
        ReceiveFailure,
        ///<summary>A complete request could not be sent to the remote server.</summary>
        SendFailure,
        ///<summary>The request was a pipelined request and the connection was closed before the response was received.</summary>
        PipelineFailure,
        ///<summary>The request was canceled, the System.Net.WebRequest.Abort method was called, or an unclassifiable error occurred. 
        ///This is the default value for System.Net.WebException.Status.</summary>  
        RequestCanceled,
        ///<summary>The response received from the server was complete but indicated a protocol-level error. 
        ///For example, an HTTP protocol error such as 401 Access Denied would use this status.</summary>
        ProtocolError,
        ///<summary>The connection was prematurely closed.</summary>
        ConnectionClosed,
        ///<summary>A server certificate could not be validated.</summary>
        TrustFailure,
        ///<summary>An error occurred while establishing a connection using SSL.</summary>
        SecureChannelFailure,
        ///<summary>The server response was not a valid HTTP response.</summary>
        ServerProtocolViolation,
        ///<summary>The connection for a request that specifies the Keep-alive header was closed unexpectedly.</summary>
        KeepAliveFailure,
        ///<summary>An internal asynchronous request is pending.</summary>
        Pending,
        ///<summary>The name resolver service could not resolve the proxy host name.</summary>
        ProxyNameResolutionFailure,
        ///<summary>An exception of unknown type has occurred.</summary>
        UnknownError,
        ///<summary>A message was received that exceeded the specified limit when sending a request or receiving a response from the server.</summary>
        MessageLengthLimitExceeded,
        ///<summary>The specified cache entry was not found.</summary>
        CacheEntryNotFound,
        ///<summary>The request was not permitted by the cache policy. 
        ///In general, this occurs when a request is not cacheable and the effective policy prohibits sending the request to the server. 
        ///You might receive this status if a request method implies the presence of a request body, a request method requires direct interaction with 
        ///the server, or a request contains a conditional header.</summary>
        RequestProhibitedByCachePolicy,
        ///<summary>This request was not permitted by the proxy.</summary>
        RequestProhibitedByProxy
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
    /// <summary>
    /// Identifies the Values of the Status State
    /// </summary>
    enum SecurityCenterCodes
    {
        /// <summary>Unknown Status</summary>
        /// <remarks>User hasn't ran any checks</remarks>
        Unknown = 0,
        /// <summary>Firewall Exclusions is current</summary>
        Firewall_Updated = 2,
        /// <summary>Firewall Exclusions had encounterd an Error</summary>
        Firewall_Error = 6,
        /// <summary>Firewall Exclusions is not current</summary>
        /// <remarks>Game Path had changed and User hasn't updated Exclusions</remarks>
        Firewall_Outdated = 8,
        /// <summary>Defender Exclusions is current</summary>
        Defender_Updated = 10,
        /// <summary>Defender Exclusions had encounterd an Error</summary>
        Defender_Error = 12,
        /// <summary>Defender Exclusions is not current</summary>
        /// <remarks>Game Path had changed and User hasn't updated Exclusions</remarks>
        Defender_Outdated = 14,
        /// <summary>Unix System</summary>
        /// <remarks>All Functions should not continue past Seeing the Screen</remarks>
        Unix = 16,
        /// <summary>Permssions has not been Set</summary>
        Permissions_NotSet = 18,
        /// <summary>Permssions is Set</summary>
        Permissions_Updated = 20,
        /// <summary>Permssions had encounterd an Error</summary>
        Permissions_Error = 22,
        /// <summary>Defender Exclusions is not current</summary>
        /// <remarks>Game Path had changed and User hasn't updated Permissions</remarks>
        Permissions_Outdated = 24
    }
}
