#if NETFX_CORE
using System;
using Windows.UI.Xaml.Controls;
using _HttpBaseProtocolFilter = Windows.Web.Http.Filters.HttpBaseProtocolFilter;

namespace Uno.AzureDevOps.Views.Controls
{
	public partial class UnoWebView
	{
		partial void ClearCacheAndCookies()
		{
			var baseFilter = new _HttpBaseProtocolFilter();
			var clearCacheTask = WebView.ClearTemporaryWebDataAsync().AsTask();

			if (Uri.IsWellFormedUriString(SourceUri?.OriginalString, UriKind.Absolute))
			{
				foreach (var cookie in baseFilter.CookieManager.GetCookies(SourceUri))
				{
					baseFilter.CookieManager.DeleteCookie(cookie);
				}
			}

			if (Uri.IsWellFormedUriString(NavigatedUri?.OriginalString, UriKind.Absolute))
			{
				foreach (var cookie in baseFilter.CookieManager.GetCookies(NavigatedUri))
				{
					baseFilter.CookieManager.DeleteCookie(cookie);
				}
			}

			clearCacheTask.Wait();
		}
	}
}
#endif
