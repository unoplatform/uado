using Uno.AzureDevOps.Presentation;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Uno.AzureDevOps.Views.Content
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page
	{
		public LoginPage()
		{
			InitializeComponent();
			DataContext = new LoginPageViewModel();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Force portrait for webview (nventive logo is cropped in landscape)
			DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
			base.OnNavigatedTo(e);
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
		{
			DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
			base.OnNavigatingFrom(e);
		}
	}
}
