#if __ANDROID__

using System;

namespace Uno.AzureDevOps.Views.Controls
{
	partial class UnoWebView
    {
		partial void ClearCacheAndCookies()
		{
			Android.Webkit.CookieManager.Instance.RemoveAllCookie();
			Android.Webkit.CookieManager.Instance.Flush();
		}
	}
}
#endif
