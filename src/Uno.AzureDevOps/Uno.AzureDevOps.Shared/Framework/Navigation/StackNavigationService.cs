#pragma warning disable
// ****************************************************************************
// <copyright file="NavigationService.cs" company="GalaSoft Laurent Bugnion">
// Copyright © GalaSoft Laurent Bugnion 2009-2016
// Copyright © nventive Willer Travassos 2019
// </copyright>
// ****************************************************************************
// <author>Laurent Bugnion</author>
// <email>laurent@galasoft.ch</email>
// <date>02.10.2014</date>
// <project>GalaSoft.MvvmLight</project>
// <web>http://www.mvvmlight.net</web>
// <license>
// See license.txt in this solution or http://www.galasoft.ch/license_MIT.txt
// </license>
// ****************************************************************************
// <author>Willer Travasos</author>
// <email>willer.travassos@nventive.com</email>
// <date>04.10.2019</date>
// <project>Uno.AzureDevOps</project>
// ****************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Uno.AzureDevOps.Framework.Navigation
{
	/// <summary>
	/// Windows 10 UWP implementation of <see cref="INavigationService"/>.
	/// </summary>
	////[ClassInfo(typeof(INavigationService))]
	public sealed class StackNavigationService : IStackNavigationService, IDisposable
	{
		/// <summary>
		/// The key that is returned by the <see cref="CurrentPageKey"/> property
		/// when the current Page is the root page.
		/// </summary>
		public const string RootPageKey = "-- ROOT --";

		/// <summary>
		/// The key that is returned by the <see cref="CurrentPageKey"/> property
		/// when the current Page is not found.
		/// This can be the case when the navigation wasn't managed by this NavigationService,
		/// for example when it is directly triggered in the code behind, and the
		/// NavigationService was not configured for this page type.
		/// </summary>
		public const string UnknownPageKey = "-- UNKNOWN --";

		private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
		private readonly int _doublePressTolerance;

		private int _pressedCounter = 0;

		private SystemNavigationManager _systemNavigationManager;
		private Frame _currentFrame;

		public StackNavigationService(bool isBackRequestHandler = true, int doublePressTolerance = 200)
		{
			if (doublePressTolerance < 100)
			{
				throw new ArgumentException("Please provide a longer double press tolerance than 100 ms", nameof(doublePressTolerance));
			}
			else if (doublePressTolerance >= 900)
			{
				throw new ArgumentException("Please provide a shorter double press tolerance than 900 ms", nameof(doublePressTolerance));
			}

			if (isBackRequestHandler)
			{
				_systemNavigationManager = SystemNavigationManager.GetForCurrentView();
				_systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
				_systemNavigationManager.BackRequested += OnBackRequested;
			}

			_doublePressTolerance = doublePressTolerance;
		}

		public event BackButtonPressEventHandler BackButtonPressed;

		public event BackButtonDoublePressEventHandler BackButtonDoublePressed;

		/// <summary>
		/// Gets or sets the Frame that should be use for the navigation.
		/// If this is not set explicitly, then (Frame)Window.Current.Content is used.
		/// </summary>
		public Frame CurrentFrame
		{
			get => _currentFrame ?? (_currentFrame = (Frame)Window.Current.Content);

			set => _currentFrame = value;
		}

		/// <summary>
		/// Gets the key corresponding to the currently displayed page.
		/// </summary>
		public string CurrentPageKey
		{
			get
			{
				lock (_pagesByKey)
				{
					if (CurrentFrame.BackStackDepth == 0)
					{
						return RootPageKey;
					}

					if (CurrentFrame.Content == null)
					{
						return UnknownPageKey;
					}

					var currentType = CurrentFrame.Content.GetType();

					if (_pagesByKey.All(p => p.Value != currentType))
					{
						return UnknownPageKey;
					}

					var item = _pagesByKey.FirstOrDefault(
						i => i.Value == currentType);

					return item.Key;
				}
			}
		}

		public IList<PageStackEntry> BackStack
		{
			get => CurrentFrame.BackStack;
		}

		/// <summary>
		/// Gets a value indicating whether if the CurrentFrame can navigate backwards.
		/// </summary>
		public bool CanGoBack => CurrentFrame.CanGoBack;

		/// <summary>
		/// Gets a value indicating whether the CurrentFrame can navigate forward.
		/// </summary>
		public bool CanGoForward => CurrentFrame.CanGoForward;

		/// <summary>
		/// Check if the CurrentFrame can navigate forward, and if yes, performs
		/// a forward navigation.
		/// </summary>
		public void GoForward()
		{
			if (CurrentFrame.CanGoForward)
			{
				CurrentFrame.GoForward();
			}
		}

		/// <summary>
		/// If possible, discards the current page and displays the previous page
		/// on the navigation stack.
		/// </summary>
		public void GoBack()
		{
			if (CurrentFrame.CanGoBack)
			{
				CurrentFrame.GoBack();
			}
		}

		/// <summary>
		/// Displays a new page corresponding to the given key.
		/// Make sure to call the <see cref="Configure"/>
		/// method first.
		/// </summary>
		/// <param name="pageKey">The key corresponding to the page
		/// that should be displayed.</param>
		/// <exception cref="ArgumentException">When this method is called for
		/// a key that has not been configured earlier.</exception>
		public void NavigateTo(string pageKey)
		{
			NavigateTo(pageKey, null);
		}

		/// <summary>
		/// Displays a new page corresponding to the given key,
		/// and passes a parameter to the new page.
		/// Make sure to call the <see cref="Configure"/>
		/// method first.
		/// </summary>
		/// <param name="pageKey">The key corresponding to the page
		/// that should be displayed.</param>
		/// <param name="parameter">The parameter that should be passed
		/// to the new page.</param>
		/// <exception cref="ArgumentException">When this method is called for
		/// a key that has not been configured earlier.</exception>
		public void NavigateTo(string pageKey, object parameter)
		{
			lock (_pagesByKey)
			{
				if (!_pagesByKey.ContainsKey(pageKey))
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.InvariantCulture,
							"No such page: {0}. Did you forget to call NavigationService.Configure?",
							pageKey),
						nameof(pageKey));
				}

				CurrentFrame.Navigate(_pagesByKey[pageKey], parameter);
			}
		}

		/// <summary>
		/// Adds a key/page pair to the navigation service.
		/// </summary>
		/// <param name="key">The key that will be used later
		/// in the <see cref="NavigateTo(string)"/> or <see cref="NavigateTo(string, object)"/> methods.</param>
		/// <param name="pageType">The type of the page corresponding to the key.</param>
		public void Configure(string key, Type pageType)
		{
			lock (_pagesByKey)
			{
				if (_pagesByKey.ContainsKey(key))
				{
					throw new ArgumentException("This key is already used: " + key);
				}

				if (_pagesByKey.Any(p => p.Value == pageType))
				{
					throw new ArgumentException(
						"This type is already configured with key " + _pagesByKey.First(p => p.Value == pageType).Key);
				}

				_pagesByKey.Add(
					key,
					pageType);
			}
		}

		/// <summary>
		/// Gets the key corresponding to a given page type.
		/// </summary>
		/// <param name="page">The type of the page for which the key must be returned.</param>
		/// <returns>The key corresponding to the page type.</returns>
		public string GetKeyForPage(Type page)
		{
			lock (_pagesByKey)
			{
				if (_pagesByKey.ContainsValue(page))
				{
					return _pagesByKey.FirstOrDefault(p => p.Value == page).Key;
				}
				else
				{
					throw new ArgumentException($"The page '{page.Name}' is unknown by the NavigationService");
				}
			}
		}

		/*********************************************************************************************************
		 *
		 * Added functionality
		 *
		 *********************************************************************************************************/

		/// <summary>
		/// Displays a new page corresponding to the given key,
		/// and passes a parameter to the new page.
		/// Clears the back and forward stacks of the frame, so new page
		/// becomes the new root of navigation
		/// </summary>
		/// <param name="pageKey">The key corresponding to the page
		/// that should be displayed.</param>
		/// <param name="parameter">The parameter that should be passed
		/// to the new page.</param>
		public void NavigateToAndClearStack(string pageKey, object parameter = null)
		{
			NavigateTo(pageKey, parameter);

			lock (_pagesByKey)
			{
				CurrentFrame.BackStack.Clear();
				CurrentFrame.ForwardStack.Clear();
			}
		}

		/// <summary>
		/// Displays a new page corresponding to the given key,
		/// and passes a parameter to the new page.
		/// Removes reference of the calling page from the navigation stack
		/// </summary>
		/// <param name="pageKey">The key corresponding to the page
		/// that should be displayed.</param>
		/// <param name="parameter">The parameter that should be passed
		/// to the new page.</param>
		public void NavigateToAndRemoveSelf(string pageKey, object parameter = null)
		{
			NavigateTo(pageKey, parameter);

			lock (_pagesByKey)
			{
				var selectedEntry = CurrentFrame.BackStack?.LastOrDefault();

				if (selectedEntry != null)
				{
					var pageIndex = CurrentFrame.BackStack.IndexOf(selectedEntry);

					if (pageIndex > -1)
					{
						CurrentFrame.BackStack.RemoveAt(pageIndex);
					}
				}
				else
				{
					throw new ArgumentException($"Unable to remove self before reaching from page {pageKey}");
				}
			}
		}

		/// <summary>
		/// If possible, discards all pages until it matches the provide pageKey.
		/// If pageKey is not in the navigation stack, fire navigation failed event
		/// </summary>
		/// <param name="pageKey">The key corresponding to the page
		/// that should be displayed.</param>
		public void GoBackTo(string pageKey)
		{
			lock (_pagesByKey)
			{
				var selectedEntry = CurrentFrame.BackStack?.FirstOrDefault(stackEntry => string.Equals(stackEntry.SourcePageType.Name, pageKey, StringComparison.InvariantCulture));

				if (selectedEntry != null)
				{
					var lastItem = CurrentFrame.BackStack.LastOrDefault();

					while (!string.Equals(lastItem.SourcePageType.Name, selectedEntry.SourcePageType.Name, StringComparison.InvariantCulture))
					{
						if (CurrentFrame.CanGoBack)
						{
							CurrentFrame.GoBack();
						}
						else
						{
							break;
						}

						lastItem = CurrentFrame.BackStack.LastOrDefault();
					}

					if (CurrentFrame.CanGoBack)
					{
						CurrentFrame.GoBack();
					}

					CurrentFrame.ForwardStack.Clear();
				}
				else
				{
					throw new ArgumentException($"The page '{pageKey}' is not in the current navigation stack");
				}
			}
		}

		public void Dispose()
		{
			if (_systemNavigationManager != null)
			{
				_systemNavigationManager.BackRequested -= OnBackRequested;
				_systemNavigationManager = null;
			}

			foreach (BackButtonPressEventHandler handler in BackButtonPressed.GetInvocationList())
			{
				BackButtonPressed -= handler;
			}

			foreach (BackButtonDoublePressEventHandler handler in BackButtonDoublePressed.GetInvocationList())
			{
				BackButtonDoublePressed -= handler;
			}
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			if (CurrentFrame.CanGoBack)
			{
				if (Interlocked.Increment(ref _pressedCounter) == 1)
				{
					Task.Delay(_doublePressTolerance)
						.ContinueWith(
							_ =>
							{
								if (_pressedCounter > 1)
								{
									BackButtonDoublePressed?.Invoke(this, e);
								}
								else
								{
									BackButtonPressed?.Invoke(this, e);
								}

								Interlocked.Exchange(ref _pressedCounter, 0);
							},
						CancellationToken.None,
						TaskContinuationOptions.ExecuteSynchronously,
						TaskScheduler.Default);
				}

				e.Handled = true;
			}
		}
	}
}
