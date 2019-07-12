using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business.Authentication;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Tasks;
using Xamarin.Essentials;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class LoginPageViewModel : ViewModelBase
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly IStackNavigationService _navigationService;
		private readonly string _azureADLoginUrl;

		private Uri _sourceUri;
		private Uri _navigatedUri;
		private bool _isConnected;
		private ITaskNotifier<bool> _isAuthenticating;

		public LoginPageViewModel()
		{
			Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_authenticationService = SimpleIoc.Default.GetInstance<IAuthenticationService>();

			ReloadPage = new RelayCommand(() => ReloadPageCommand());
			IsAuthenticating = new TaskNotifier<bool>(Task.FromResult(false));

			var applicationContext = SimpleIoc.Default.GetInstance<IApplicationContext>();

			_azureADLoginUrl = $"{ClientConstants.BaseAuthorizationUrl}?client_id={applicationContext.AuthApplicationId}" +
			$"&response_type={ClientConstants.AuthorizationResponseType}&scope={applicationContext.AuthScopes}&redirect_uri={applicationContext.AuthRedirectUrl}";

			if (Connectivity.NetworkAccess == NetworkAccess.Internet)
			{
				IsConnected = true;
				SourceUri = new Uri(_azureADLoginUrl);
			}

		}

		public ICommand ReloadPage { get; }

		public Uri SourceUri
		{
			get => _sourceUri;
			set => Set(nameof(SourceUri), ref _sourceUri, value);
		}

		public bool IsConnected
		{
			get => _isConnected;
			set => Set(nameof(IsConnected), ref _isConnected, value);
		}

		public ITaskNotifier<bool> IsAuthenticating
		{
			get => _isAuthenticating;
			private set => Set(nameof(IsAuthenticating), ref _isAuthenticating, value);
		}

		public Uri NavigatedUri
		{
			get => _navigatedUri;
			set
			{
				Set(nameof(NavigatedUri), ref _navigatedUri, value);

				if (_navigatedUri?.OriginalString?.Contains("code=") ?? false)
				{
					OnAuthenticatedUri(_navigatedUri);
				}

				if (_navigatedUri?.OriginalString?.Contains("error=access_denied") ?? false)
				{
					SourceUri = new Uri(_azureADLoginUrl);
				}
			}
		}

		private void ReloadPageCommand()
		{
			SourceUri = new Uri(_azureADLoginUrl);
			IsAuthenticating = new TaskNotifier<bool>(Task.FromResult(false));
		}

		private void OnAuthenticatedUri(Uri uri)
		{
			var parsed = HttpUtility.ParseQueryString(uri.Query);
			var authenticationCode = parsed["code"];

			IsAuthenticating = new TaskNotifier<bool>(_authenticationService.Login(authenticationCode)
				.ContinueWith(
					task =>
					{
						var result = true;
						if (task.Exception != null)
						{
							task.Exception.Handle(ex =>
							{
								SourceUri = null;
								NavigatedUri = null;

								// We don't flag exception handled so that the TaskNotifier can complete to a Faulted state
								return false;
							});

							SourceUri = new Uri(_azureADLoginUrl);

							result = false;
						}
						else
						{
							_navigationService.ToOrganizationListPage(true);
						}

						return result;
					},
					CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously,
					TaskScheduler.Default));
		}

		void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
			IsConnected = e.NetworkAccess == NetworkAccess.Internet;

			if (IsConnected)
			{
				SourceUri = new Uri(_azureADLoginUrl);
				IsAuthenticating = new TaskNotifier<bool>(Task.FromResult(false));
			}
		}
	}
}
