#if __IOS__
using System;
using Foundation;
using UIKit;
using WebKit;

namespace Uno.AzureDevOps.Views.Controls
{
	partial class UnoWebView
    {
		partial void ClearCacheAndCookies()
		{
			NSUrlCache.SharedCache.RemoveAllCachedResponses();

			if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
			{
				WKWebsiteDataStore.DefaultDataStore.HttpCookieStore.GetAllCookies((cookies) =>
				{
					foreach (var cookie in cookies)
					{
						WKWebsiteDataStore.DefaultDataStore.HttpCookieStore.DeleteCookie(cookie, null);
					}
				});
			}
			else
			{
				foreach (var cookie in NSHttpCookieStorage.SharedStorage.Cookies)
				{
					NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);
				}
			}
		}
	}
}
#endif
