using System.Net.Http;

#if __WASM__
using Uno.UI.Wasm;
#elif __ANDROID__
using Xamarin.Android.Net;
#endif

namespace Uno.AzureDevOps.Framework
{
	public class UnoHttpClientHandler :
#if __WASM__
		WasmHttpHandler
#elif __IOS__
		NSUrlSessionHandler
#elif __ANDROID__
		AndroidClientHandler
#else
		HttpClientHandler
#endif
	{
	}
}
