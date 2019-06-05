using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	public static partial class VersionHelper
	{
		public static string GetAppVersionWithBuildNumber => $"{GetAppVersion()} {GetBuildNumber()}";
	}
}
