﻿using System;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
#if !NETFX_CORE && !__WASM__
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
#endif
using Microsoft.Extensions.Logging;
using Uno.AzureDevOps.Business;
using Uno.AzureDevOps.Business.Authentication;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Storage;
using Uno.AzureDevOps.Views.Content;
using Uno.Extensions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UICommand = Windows.UI.Popups.UICommand;
#if __IOS__
using UIKit;
using Xamarin.Essentials;
#endif
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Uno.AzureDevOps
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="App"/> class.
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
#if XAMARIN
            Uno.UI.FeatureConfiguration.Font.IgnoreTextScaleFactor = true;
#endif

#if DEBUG
			InitializeLogging();
#endif

			InitializeComponent();
			Suspending += OnSuspending;
		}

		public static SimpleIoc ServiceProvider { get; } = SimpleIoc.Default;

		public static async System.Threading.Tasks.Task RunOnDispatcher(Action action)
		{
			await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
		}

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="e">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (Windows.UI.Xaml.Window.Current.Content is Frame rootFrame)
			{
			}
			else
			{
				Module.Initialize(ServiceProvider);
				Views.Module.Initialize(ServiceProvider);

#if !NETFX_CORE && !__WASM__ && !DEBUG

				AppCenter.Start(ClientConstants.AppCenterSecret,
                   typeof(Analytics), typeof(Crashes));
#endif

				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					// TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				Windows.UI.Xaml.Window.Current.Content = rootFrame;
			}

			if (e.PrelaunchActivated == false)
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					ExecuteFirstNavigation();
				}

				// Ensure the current window is active
				Windows.UI.Xaml.Window.Current.Activate();
			}
		}

		private static async void OnLoggedOut(LoggedOutEventArgs args)
		{
			if (args.Exception is UnauthorizedAccessException)
			{
				var dialog = new MessageDialog("Unable to verify your credentials. You must log in.");

				dialog.Commands.Add(new UICommand("OK"));

				await dialog.ShowAsync();
			}

			ServiceProvider.GetInstance<IStackNavigationService>().NavigateToAndClearStack(nameof(LoginPage));
		}

		private static void InitializeLogging()
		{
#if DEBUG
			// Logging is disabled by default for release builds, as it incurs a significant
			// initialization cost from Microsoft.Extensions.Logging setup. If startup performance
			// is a concern for your application, keep this disabled. If you're running on web or 
			// desktop targets, you can use url or command line parameters to enable it.
			//
			// For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

			var factory = LoggerFactory.Create(builder =>
			{
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
				builder.AddDebug();
#else
                builder.AddConsole();
#endif

				// Exclude logs below this level
				builder.SetMinimumLevel(LogLevel.Information);

				// Default filters for Uno Platform namespaces
				builder.AddFilter("Uno", LogLevel.Warning);
				builder.AddFilter("Windows", LogLevel.Warning);
				builder.AddFilter("Microsoft", LogLevel.Warning);

				// Generic Xaml events
				// builder.AddFilter("Windows.UI.Xaml", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.UIElement", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.FrameworkElement", LogLevel.Trace );

				// Layouter specific messages
				// builder.AddFilter("Windows.UI.Xaml.Controls", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Controls.Panel", LogLevel.Debug );

				// builder.AddFilter("Windows.Storage", LogLevel.Debug );

				// Binding related messages
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );
				// builder.AddFilter("Windows.UI.Xaml.Data", LogLevel.Debug );

				// Binder memory references tracking
				// builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

				// RemoteControl and HotReload related
				// builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

				// Debug JS interop
				// builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
			});

			global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
			//global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
		}
		private static void ExecuteFirstNavigation()
		{
			var navigationService = ServiceProvider.GetInstance<IStackNavigationService>();
			var authenticationService = ServiceProvider.GetInstance<IAuthenticationService>();
			var userPreferencesService = ServiceProvider.GetInstance<IUserPreferencesService>();

			navigationService.OnNavigating += async (s, e) => await RunOnDispatcher(() =>
			{
#if __IOS__
				var loginPageKey = nameof(LoginPage);

				var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

				statusBar.ForegroundColor = e.PageKey == loginPageKey ? Windows.UI.Colors.Black : Windows.UI.Colors.White;
#endif
			});

			if (authenticationService.IsAuthenticated())
			{
				// Redirect user to project detail page (both are mandatory for API calls later)
				if (userPreferencesService.HasPreferredProject() && userPreferencesService.HasPreferredAccount())
				{
					navigationService.ToProjectPage();
				}
				else
				{
					navigationService.ToOrganizationListPage();
				}
			}
			else
			{
				navigationService.ToLoginPage();
			}

			// NOTE: Any event below do not have to be unhooked, since they have to stay connected throughout the app lifetime
			authenticationService.LoggedOut += OnLoggedOut;
			navigationService.BackButtonPressed += async (s, e) => await RunOnDispatcher(s.GoBack);
			navigationService.BackButtonDoublePressed += async (s, e) => await RunOnDispatcher(() =>
			{
				if (s is StackNavigationService navService)
				{
					var projectPageKey = nameof(ProjectPage);

					if (navService.CurrentFrame.BackStack.Any(entry => entry.SourcePageType.Name == projectPageKey))
					{
						s.GoBackTo(projectPageKey);
					}
					else if (navService.CanGoBack)
					{
						s.GoBack();
					}
				}
			});
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		private void OnNavigationFailed(object sender, NavigationFailedEventArgs e) => throw new Exception("Failed to load Page " + e.SourcePageType.FullName, e.Exception);

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();

			// TODO: Save application state and stop any background activity
			deferral.Complete();
		}

#if __IOS__
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
		{
			var idiom = DeviceInfo.Idiom;

			if (idiom == DeviceIdiom.Tablet)
			{
				return UIInterfaceOrientationMask.Landscape;
			}
			else if (idiom == DeviceIdiom.Phone)
			{
				return UIInterfaceOrientationMask.Portrait;
			}
			else
			{
				return UIInterfaceOrientationMask.Portrait;
			}
		}
#endif
	}
}
