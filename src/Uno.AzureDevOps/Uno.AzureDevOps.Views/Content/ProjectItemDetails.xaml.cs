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
	[SuppressMessage("", "CA1801", Justification = "Event handler")]
	public sealed partial class ProjectItemDetails : Page
	{
		public ProjectItemDetails()
		{
			InitializeComponent();
			DataContext = new ProjectItemDetailsPageViewModel();
		}

		// TODO replace pragma with suppressmessage
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			(DataContext as ProjectItemDetailsPageViewModel).OnNavigatedTo(e.Parameter as RichWorkItem);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			base.OnNavigatedTo(e);
		}

		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			(DataContext as ProjectItemDetailsPageViewModel).OnWorkItemClicked(e.ClickedItem as RichWorkItem);
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			if (LargeViewNavigation.MenuVisibility == Visibility.Collapsed)
			{
				LargeViewNavigation.MenuVisibility = Visibility.Visible;
				LargeViewNavigation.SetValue(Grid.ColumnProperty, 0);
				LargeViewNavigation.SetValue(Grid.RowProperty, 0);
				LargeViewNavigation.SetValue(Grid.RowSpanProperty, 2);
				ContentView.SetValue(Grid.ColumnProperty, 0);
			}
			else
			{
				LargeViewNavigation.MenuVisibility = Visibility.Collapsed;
				LargeViewNavigation.SetValue(Grid.RowProperty, 1);
				LargeViewNavigation.SetValue(Grid.RowSpanProperty, 1);
			}
		}
	}
}
