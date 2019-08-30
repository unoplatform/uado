using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Uno.AzureDevOps.Views.Behaviors
{
	[SuppressMessage("", "SA1201", Justification = "Behavior")]
	[SuppressMessage("", "CA1720", Justification = "Behavior")]
	public static class FullscreenSideMenuBehavior
	{
		public static Visibility GetMenuVisibility(CommandBar d) => (Visibility)d.GetValue(MenuVisibilityProperty);

		public static void SetMenuVisibility(CommandBar d, Visibility value) => d.SetValue(MenuVisibilityProperty, value);

		public static readonly DependencyProperty MenuVisibilityProperty =
			DependencyProperty.RegisterAttached(
				"MenuVisibility",
				typeof(Visibility),
				typeof(FullscreenSideMenuBehavior),
				new PropertyMetadata(Visibility.Collapsed, HandlePropertyChanges));

		public static bool GetIsFullscreenMenu(CommandBar d) => (bool)d.GetValue(IsFullscreenMenuProperty);

		public static void SetIsFullscreenMenu(CommandBar d, bool value) => d.SetValue(IsFullscreenMenuProperty, value);

		public static readonly DependencyProperty IsFullscreenMenuProperty =
			DependencyProperty.RegisterAttached(
				"IsFullscreenMenu",
				typeof(bool),
				typeof(FullscreenSideMenuBehavior),
				new PropertyMetadata(false, HandlePropertyChanges));

		private static void HandlePropertyChanges(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var commandBar = d as CommandBar;

			// On a narrow screen when the side menu opens, the CommandBar will collapse
			// to allow the side menu to become full screen
			commandBar.Visibility = (GetIsFullscreenMenu(commandBar) && GetMenuVisibility(commandBar) == Visibility.Visible)
				? Visibility.Collapsed
				: Visibility.Visible;
		}
	}
}
