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
		VC2005x86,
		VC2005x64,
		VC2008x86,
		VC2008x64,
		VC2010x86,
		VC2010x64,
		VC2012x86,
		VC2012x64,
		VC2013x86,
		VC2013x64,
		VC2015x86,
		VC2015x64,
		VC2017x86,
		VC2017x64,
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
						if (((string)vc2015to2019x86Version).StartsWith("14"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2015to2019x64:
						var parametersVc2015to2019x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DevDiv\VC\Servicing\14.0\RuntimeMinimum", false);
						if (parametersVc2015to2019x64 == null) return false;
						var vc2015to2019x64Version = parametersVc2015to2019x64.GetValue("Version");
						if (((string)vc2015to2019x64Version).StartsWith("14"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2017x86:
						var paths2017x86 = new List<string>
						{
							@"Installer\Dependencies\,,x86,14.0,bundle",
							@"Installer\Dependencies\VC,redist.x86,x86,14.16,bundle" //changed in 14.16.x
						};
						foreach (var path in paths2017x86)
						{
							var parametersVc2017x86 = Registry.ClassesRoot.OpenSubKey(path, false);
							if (parametersVc2017x86 == null) continue;
							var vc2017x86Version = parametersVc2017x86.GetValue("Version");
							if (vc2017x86Version == null) return false;
							if (((string)vc2017x86Version).StartsWith("14"))
							{
								return true;
							}
						}
						break;
					case RedistributablePackageVersion.VC2017x64:
						var paths2017x64 = new List<string>
						{
							@"Installer\Dependencies\,,amd64,14.0,bundle",
							@"Installer\Dependencies\VC,redist.x64,amd64,14.16,bundle" //changed in 14.16.x
						};
						foreach (var path in paths2017x64)
						{
							var parametersVc2017x64 = Registry.ClassesRoot.OpenSubKey(path, false);
							if (parametersVc2017x64 == null) continue;
							var vc2017x64Version = parametersVc2017x64.GetValue("Version");
							if (vc2017x64Version == null) return false;
							if (((string)vc2017x64Version).StartsWith("14"))
							{
								return true;
							}
						}
						break;
					case RedistributablePackageVersion.VC2015x86:
						var parametersVc2015x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{e2803110-78b3-4664-a479-3611a381656a}", false);
						if (parametersVc2015x86 == null) return false;
						var vc2015x86Version = parametersVc2015x86.GetValue("Version");
						if (((string)vc2015x86Version).StartsWith("14"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2015x64:
						var parametersVc2015x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{d992c12e-cab2-426f-bde3-fb8c53950b0d}", false);
						if (parametersVc2015x64 == null) return false;
						var vc2015x64Version = parametersVc2015x64.GetValue("Version");
						if (((string)vc2015x64Version).StartsWith("14"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2013x86:
						var parametersVc2013x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{f65db027-aff3-4070-886a-0d87064aabb1}", false);
						if (parametersVc2013x86 == null) return false;
						var vc2013x86Version = parametersVc2013x86.GetValue("Version");
						if (((string)vc2013x86Version).StartsWith("12"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2013x64:
						var parametersVc2013x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{050d4fc8-5d48-4b8f-8972-47c82c46020f}", false);
						if (parametersVc2013x64 == null) return false;
						var vc2013x64Version = parametersVc2013x64.GetValue("Version");
						if (((string)vc2013x64Version).StartsWith("12"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2012x86:
						var parametersVc2012x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{33d1fd90-4274-48a1-9bc1-97e33d9c2d6f}", false);
						if (parametersVc2012x86 == null) return false;
						var vc2012x86Version = parametersVc2012x86.GetValue("Version");
						if (((string)vc2012x86Version).StartsWith("11"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2012x64:
						var parametersVc2012x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Dependencies\{ca67548a-5ebe-413a-b50c-4b9ceb6d66c6}", false);
						if (parametersVc2012x64 == null) return false;
						var vc2012x64Version = parametersVc2012x64.GetValue("Version");
						if (((string)vc2012x64Version).StartsWith("11"))
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2010x86:
						var parametersVc2010x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1D5E3C0FEDA1E123187686FED06E995A", false);
						if (parametersVc2010x86 == null) return false;
						var vc2010x86Version = parametersVc2010x86.GetValue("Version");
						if ((int)vc2010x86Version > 1)
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2010x64:
						var parametersVc2010x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1926E8D15D0BCE53481466615F760A7F", false);
						if (parametersVc2010x64 == null) return false;
						var vc2010x64Version = parametersVc2010x64.GetValue("Version");
						if ((int)vc2010x64Version > 1)
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2008x86:
						var parametersVc2008x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\6E815EB96CCE9A53884E7857C57002F0", false);
						if (parametersVc2008x86 == null) return false;
						var vc2008x86Version = parametersVc2008x86.GetValue("Version");
						if ((int)vc2008x86Version > 1)
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2008x64:
						var parametersVc2008x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\67D6ECF5CD5FBA732B8B22BAC8DE1B4D", false);
						if (parametersVc2008x64 == null) return false;
						var vc2008x64Version = parametersVc2008x64.GetValue("Version");
						if ((int)vc2008x64Version > 1)
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2005x86:
						var parametersVc2005x86 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\c1c4f01781cc94c4c8fb1542c0981a2a", false);
						if (parametersVc2005x86 == null) return false;
						var vc2005x86Version = parametersVc2005x86.GetValue("Version");
						if ((int)vc2005x86Version > 1)
						{
							return true;
						}
						break;
					case RedistributablePackageVersion.VC2005x64:
						var parametersVc2005x64 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\Installer\Products\1af2a8da7e60d0b429d7e6453b3d0182", false);
						if (parametersVc2005x64 == null) return false;
						var vc2005x64Version = parametersVc2005x64.GetValue("Version");
						if ((int)vc2005x64Version > 1)
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
