using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Uno.AzureDevOps.Droid
{
	[Activity(
		MainLauncher = true,
		ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
		WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
	)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			//Init Xamarin essentials is needed for Android only
			Xamarin.Essentials.Platform.Init(this, bundle);
		}
	}
}

