using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Views;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace Uno.AzureDevOps.Framework.Navigation
{
	public delegate void OnNavigatingEventHandler(IStackNavigationService sender, OnNavigatingEventArgs args);

	public delegate void BackButtonPressEventHandler(IStackNavigationService sender, BackRequestedEventArgs args);

	public delegate void BackButtonDoublePressEventHandler(IStackNavigationService sender, BackRequestedEventArgs args);

	public interface IStackNavigationService : INavigationService
	{
		event BackButtonPressEventHandler BackButtonPressed;

		event BackButtonDoublePressEventHandler BackButtonDoublePressed;

		event OnNavigatingEventHandler OnNavigating;

		IList<PageStackEntry> BackStack { get; }

		void NavigateToAndClearStack(string pageKey, object parameter = null);

		void NavigateToAndRemoveSelf(string pageKey, object parameter = null);

		void GoBackTo(string pageKey);
	}
}
