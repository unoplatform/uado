using System.Diagnostics.CodeAnalysis;
using Uno.AzureDevOps.Client;
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
	public sealed partial class ProjectPage : Page
	{
		public ProjectPage()
		{
			InitializeComponent();
			DataContext = new ProjectPageViewModel();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			(DataContext as ProjectPageViewModel).OnNavigatedFrom();

			base.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			(DataContext as ProjectPageViewModel).OnNavigatedTo(e.Parameter as TeamProjectReference);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			base.OnNavigatedTo(e);
		}

		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			(DataContext as ProjectPageViewModel).OnWorkItemClicked(e.ClickedItem as RichWorkItem);
		}
	}
}
