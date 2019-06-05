#if __ANDROID__
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.App;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	public static partial class VersionHelper
	{
		private static string GetAppVersion()
		{
			return  Application.Context.GetType().Assembly.GetName().Version.ToString(4);
		}

		private static string GetBuildNumber()
		{
			string buildNumber = null;

			var attribute = Application.Context.GetType().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();

			if (attribute != null)
			{
				buildNumber = attribute.Version;
			}

			return $"({buildNumber})";
		}
	}
}
#endif
