using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Droid
{
	[Activity(
		Theme = "@style/splashscreen",
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
		WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
	)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			// Switch to app theme on activity start
			base.SetTheme(Resource.Style.AppTheme);
			base.OnCreate(bundle);
			//Init Xamarin essentials is needed for Android only
			Xamarin.Essentials.Platform.Init(this, bundle);

#if !DEBUG
			AppCenter.Start(ClientConstants.AppCenterSecret,
							   typeof(Analytics), typeof(Crashes));
#endif
		}
	}
}
