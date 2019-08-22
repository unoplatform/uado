using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.AzureDevOps.Presentation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Uno.AzureDevOps.Views.Controls
{
	public enum NavigationLevel
	{
		Organizations,
		Projects,
		Project
	}

	[SuppressMessage("", "SA1201", Justification = "Control properties")]
	[SuppressMessage("", "CA1720", Justification = "Control properties")]
	public sealed partial class SideMenu : UserControl
    {
        public SideMenu()
        {
            this.InitializeComponent();
			DataContext = new SideMenuViewModel();
		}

		public static readonly DependencyProperty NavigationLevelProperty = DependencyProperty.Register(
			"SourceUri",
			typeof(NavigationLevel?),
			typeof(SideMenu),
			new PropertyMetadata(default(NavigationLevel?), (d, e) => OnNavigationLevelChanged((SideMenu)d)));

		public NavigationLevel? NavLevel
		{
			get => (NavigationLevel)GetValue(NavigationLevelProperty);
			set => SetValue(NavigationLevelProperty, value);
		}

		private static void OnNavigationLevelChanged(SideMenu sideMenu)
		{
			if (!sideMenu.NavLevel.HasValue)
			{
				return;
			}

			var currentNavigationLevel = sideMenu.NavLevel.Value;

			if (currentNavigationLevel == NavigationLevel.Organizations)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Visible;
				sideMenu.TopMenuView.Visibility = Visibility.Collapsed;
			}
			else if (currentNavigationLevel == NavigationLevel.Projects)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Collapsed;
				sideMenu.TopMenuView.Visibility = Visibility.Visible;
			}
			else if (currentNavigationLevel == NavigationLevel.Project)
			{
				sideMenu.TopMenuLogoView.Visibility = Visibility.Collapsed;
				sideMenu.TopMenuView.Visibility = Visibility.Visible;
			}
		}
	}
}
