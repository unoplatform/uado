using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Views.Content;

namespace Uno.AzureDevOps.Views
{
	[SuppressMessage("", "CA1716", Justification = "Stylistic choice")]
	public static class Module
	{
		public static void Initialize(ISimpleIoc serviceProvider)
		{
			serviceProvider.Register<IStackNavigationService>(() =>
			{
				var navigationService = new StackNavigationService();
				navigationService.Configure(nameof(LoginPage), typeof(LoginPage));
				navigationService.Configure(nameof(AboutPage), typeof(AboutPage));
				navigationService.Configure(nameof(OrganizationListPage), typeof(OrganizationListPage));
				navigationService.Configure(nameof(ProfilePage), typeof(ProfilePage));
				navigationService.Configure(nameof(ProjectListPage), typeof(ProjectListPage));
				navigationService.Configure(nameof(ProjectPage), typeof(ProjectPage));
				navigationService.Configure(nameof(ProjectItemDetails), typeof(ProjectItemDetails));

				return navigationService;
			});
		}
	}
}
