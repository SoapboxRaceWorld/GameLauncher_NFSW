// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Allowed within API/Updater Checks", Scope = "namespaceanddescendants", Target = "~N:SBRW.Launcher.RunTime.LauncherCore.LauncherUpdater")]
[assembly: SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Server JSON uses lowerCase", Scope = "namespaceanddescendants", Target = "~N:SBRW.Launcher.RunTime.LauncherCore.ModNet.JSON")]
[assembly: SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Inline Pragma's needed for multi-build", Scope = "module")]
