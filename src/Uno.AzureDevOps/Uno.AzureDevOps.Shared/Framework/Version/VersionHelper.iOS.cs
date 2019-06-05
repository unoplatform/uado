#if __IOS__
using System;
using System.Diagnostics;
using UIKit;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	public static partial class VersionHelper
	{
		private static string GetAppVersion()
		{
			return UIApplication.SharedApplication.Delegate.GetType().Assembly.GetName().Version.ToString(4);
		}

		private static string GetBuildNumber()
		{
			string buildNumber = null;

			var versionInfo = FileVersionInfo.GetVersionInfo(UIApplication.SharedApplication.Delegate.GetType().Assembly.Location);

			buildNumber = versionInfo.FileVersion;

			return $"({buildNumber})";
		}
	}
}
#endif
