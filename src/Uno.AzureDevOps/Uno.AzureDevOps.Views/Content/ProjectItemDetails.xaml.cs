using System.Diagnostics.CodeAnalysis;
using Uno.AzureDevOps.Presentation;
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

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			(DataContext as ProjectItemDetailsPageViewModel).OnNavigatedTo(e.Parameter as RichWorkItem);
			base.OnNavigatedTo(e);
		}

		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			(DataContext as ProjectItemDetailsPageViewModel).OnWorkItemClicked(e.ClickedItem as RichWorkItem);
		}
	}
}
