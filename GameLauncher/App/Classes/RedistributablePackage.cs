using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

// based on https://github.com/bitbeans/RedistributableChecker/blob/master/RedistributableChecker/RedistributablePackage.cs
namespace GameLauncher.App.Classes
{
	/// <summary>
	/// Microsoft Visual C++ Redistributable Package Versions
	/// </summary>
	public enum RedistributablePackageVersion
	{
		VC2015to2019x86,
		VC2015to2019x64,
	};

	/// <summary>
	///	Class to detect installed Microsoft Redistributable Packages.
	/// </summary>
	/// <see cref="//https://stackoverflow.com/questions/12206314/detect-if-visual-c-redistributable-for-visual-studio-2012-is-installed"/>
	public static class RedistributablePackage
	{
		/// <summary>
		/// Check if a Microsoft Redistributable Package is installed.
		/// </summary>
		/// <param name="redistributableVersion">The package version to detect.</param>
		/// <returns><c>true</c> if the package is installed, otherwise <c>false</c></returns>
		public static bool IsInstalled(RedistributablePackageVersion redistributableVersion)
		{
			try
			{
				switch (redistributableVersion)
				{
					case RedistributablePackageVersion.VC2015to2019x86:
						var parametersVc2015to2019x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
						if (parametersVc2015to2019x86 == null) return false;
						var vc2015to2019x86Version = parametersVc2015to2019x86.GetValue("Version");
						if (((string)vc2015to2019x86Version).StartsWith("14.2"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2015to2019x64:
						var parametersVc2015to2019x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
						if (parametersVc2015to2019x64 == null) return false;
						var vc2015to2019x64Version = parametersVc2015to2019x64.GetValue("Version");
						if (((string)vc2015to2019x64Version).StartsWith("14.2"))
						{
							return true;
						}
						break;
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
