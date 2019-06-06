#if __WASM__
using System;
using System.Reflection;

namespace Uno.AzureDevOps.Framework.AppVersion
{
	//Not working for wasm application.
	public static partial class VersionHelper
	{
		private static string GetBuildNumber()
		{
			return null;
		}

		private static string GetAppVersion()
		{
			return null;
		}
	}
}
#endif
