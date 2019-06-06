#if NETFX_CORE
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Uno.Extensions;
using Windows.UI.Xaml;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	public static partial class VersionHelper
	{
		private static string GetAppVersion()
		{
			string appVersion;

			try
			{
				appVersion = Application.Current.GetType().GetAssembly().GetName().Version.ToString(4);
			}
			catch
			{
				// Application.Current throws when running inside a background task
				appVersion = null;
			}

			return appVersion;
		}

		private static string GetBuildNumber()
		{
			string buildNumber = null;

			var attribute = Application.Current.GetType().GetAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>();
			if (attribute == null)
			{
				return buildNumber;
			}

			buildNumber = attribute.Version;

			return $"({buildNumber})";
		}
	}
}
#endif
