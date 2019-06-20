#if !__WASM__
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.App;
using AppInfo = Xamarin.Essentials.AppInfo;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	public static partial class VersionHelper
	{
		private static string GetAppVersion()
		{
			return AppInfo.VersionString;
		}

		private static string GetBuildNumber()
		{
			return $"({AppInfo.BuildString})";
		}
	}
}
#endif
