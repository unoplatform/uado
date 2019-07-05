#if __IOS__
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

			// Calling both methods as there are cookies in both SharedStorage & HttpCookieStore even if ios >= 11
			var cookieStorage = NSHttpCookieStorage.SharedStorage;
			foreach (var cookie in cookieStorage.Cookies)
			{
				cookieStorage.DeleteCookie(cookie);
			}

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
		}
	}
}
#endif
