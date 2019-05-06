#if NETFX_CORE
#elif __ANDROID__ || __IOS__ || __WASM__
using Visibility = Windows.UI.Xaml.Visibility;
#else
using System.Windows;
#endif

namespace Uno.AzureDevOps.Views.Converters
{
	public enum VisibilityOnEnumerableHasAny
	{
		Visible,
		Collapsed
	}
}
