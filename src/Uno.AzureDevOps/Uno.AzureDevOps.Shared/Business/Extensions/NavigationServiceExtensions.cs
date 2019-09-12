using System;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Presentation;
using Uno.AzureDevOps.Views.Content;

namespace Uno.AzureDevOps.Business.Extensions
{
	public static class NavigationServiceExtensions
	{
		public static void ToLoginPage(this IStackNavigationService navigationService) => navigationService.NavigateTo(nameof(LoginPage));

		public static void ToOrganizationListPage(this IStackNavigationService navigationService, bool isStackPop = false)
		{
			if (isStackPop)
			{
				navigationService.NavigateToAndRemoveSelf(nameof(OrganizationListPage));
			}
			else
			{
				navigationService.NavigateTo(nameof(OrganizationListPage));
			}
		}

		public static void ToProjectItemDetailsPage(this IStackNavigationService navigationService, RichWorkItem workItem, TeamProjectReference project)
			=> navigationService.NavigateTo(nameof(ProjectItemDetails), new Tuple<RichWorkItem, TeamProjectReference>(workItem, project));

		public static void ToProfilePage(this IStackNavigationService navigationService) => navigationService.NavigateTo(nameof(ProfilePage));

		public static void ToProjectListPage(this IStackNavigationService navigationService, AccountData account) => navigationService.NavigateTo(nameof(ProjectListPage), account);

		public static void ToProjectPage(this IStackNavigationService navigationService, TeamProjectReference project) => navigationService.NavigateTo(nameof(ProjectPage), project);

		public static void ToProjectPage(this IStackNavigationService navigationService) => navigationService.NavigateTo(nameof(ProjectPage));

		public static void ToAboutPage(this IStackNavigationService navigationService) => navigationService.NavigateTo(nameof(AboutPage));
	}
}
