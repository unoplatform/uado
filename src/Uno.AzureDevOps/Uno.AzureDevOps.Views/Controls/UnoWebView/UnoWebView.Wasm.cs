#if __WASM__
using System;
using Windows.UI.Xaml;
using Uno.Foundation;
using Windows.UI.Xaml.Controls;
using Uno.Extensions;

namespace Uno.AzureDevOps.Views.Controls
{
	partial class UnoWebView
	{
		partial void SetWebViewControl()
		{
			InternalWebView = new WasmPseudoWebView(this, SourceUri.OriginalString);
			OnNavigationCompleted(null, null);
		}
	}

	public class WasmPseudoWebView : Control
	{
		private readonly UnoWebView _webView;
		private string _sourceUrl;

		public WasmPseudoWebView(UnoWebView webView, string sourceUrl)
			: base("a")
		{
			_webView = webView;
			_sourceUrl = sourceUrl;
		}

		protected override void OnLoaded()
		{
			this.RegisterHtmlCustomEventHandler("urlwithsecuritytokens", OnSecurityTokens);
			WebAssemblyRuntime.InvokeJS($"Uno.AzureDevOps.Auth.launch({HtmlId})");

			// SetStyle pointer-events makes sure that HTML tag (WasmWebView) is interactive
			SetStyle("pointer-events", "all");
			SetAttribute("target", "_blank");
			SetAttribute("href", _sourceUrl);
		}

		private void OnSecurityTokens(object sender, HtmlCustomEventArgs e)
		{
			var parts = e.Detail.Split('|');
			if(parts.Length == 0)
			{
				return;
			}

			switch (parts[0])
			{
				case "canceled":
					break;
				case "error":
					break;
				case "nav":
					_webView.NavigatedUri = new Uri(parts[1]);
					break;
			}
		}
	}
}
#endif
