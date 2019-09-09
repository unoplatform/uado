using System.Diagnostics.CodeAnalysis;
using Uno.AzureDevOps.Presentation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238
namespace Uno.AzureDevOps.Views.Content
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ProfilePage : Page
	{
		public ProfilePage()
		{
			InitializeComponent();
			DataContext = new ProfilePageViewModel();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);
			LargeViewNavigation.MenuVisibility = Visibility.Collapsed;
		}
	}
}
