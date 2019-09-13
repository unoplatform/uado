using System.Diagnostics.CodeAnalysis;
using Uno.AzureDevOps.Views.Content;
using Uno.AzureDevOps.Views.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Uno.AzureDevOps.Views.Behaviors
{
	[SuppressMessage("", "SA1201", Justification = "Behavior")]
	[SuppressMessage("", "CA1720", Justification = "Behavior")]
	public static class FullscreenSideMenuBehavior
	{
		public static Visibility GetMenuVisibility(PageHeader d) => (Visibility)d.GetValue(MenuVisibilityProperty);

		public static void SetMenuVisibility(PageHeader d, Visibility value) => d.SetValue(MenuVisibilityProperty, value);

		public static readonly DependencyProperty MenuVisibilityProperty =
			DependencyProperty.RegisterAttached(
				"MenuVisibility",
				typeof(Visibility),
				typeof(FullscreenSideMenuBehavior),
				new PropertyMetadata(Visibility.Collapsed, HandlePropertyChanges));

		public static bool GetIsFullscreenMenu(PageHeader d) => (bool)d.GetValue(IsFullscreenMenuProperty);

		public static void SetIsFullscreenMenu(PageHeader d, bool value) => d.SetValue(IsFullscreenMenuProperty, value);

		public static readonly DependencyProperty IsFullscreenMenuProperty =
			DependencyProperty.RegisterAttached(
				"IsFullscreenMenu",
				typeof(bool),
				typeof(FullscreenSideMenuBehavior),
				new PropertyMetadata(false, HandlePropertyChanges));

		private static void HandlePropertyChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			// On a narrow screen when the side menu opens, the CommandBar will collapse
			// to allow the side menu to become full screen
			if (d is PageHeader header && header?.FindName("PART_CommandBarView") is CommandBar bar)
			{
				bar.Visibility = (GetIsFullscreenMenu(header) && GetMenuVisibility(header) == Visibility.Visible)
					? Visibility.Collapsed
					: Visibility.Visible;
			}
		}
	}
}
