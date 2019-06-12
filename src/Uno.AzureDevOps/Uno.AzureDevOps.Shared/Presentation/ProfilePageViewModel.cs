using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business.Authentication;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Framework.AppVersion;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Tasks;
using Windows.System;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProfilePageViewModel : ViewModelBase
	{
		private readonly IStackNavigationService _navigationService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IVSTSRepository _vstsRespository;
		private string _appVersion;

		public ProfilePageViewModel()
		{
			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_authenticationService = SimpleIoc.Default.GetInstance<IAuthenticationService>();
			_vstsRespository = SimpleIoc.Default.GetInstance<IVSTSRepository>();

			User = new TaskNotifier<UserProfile>(_vstsRespository.GetUserProfile());
			Logout = new RelayCommand(() => _authenticationService.Logout());
			ToAboutPage = new RelayCommand(() => _navigationService.ToAboutPage());
			ReloadPage = new RelayCommand(() => ReloadPageCommand());

			NavigateToSourceCode = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://github.com/nventive/Uado")));

			AppVersion = VersionHelper.GetAppVersionWithBuildNumber;
		}

		public ITaskNotifier<UserProfile> User { get; private set; }

		public ICommand ReloadPage { get; }

		public ICommand Logout { get; }

		public ICommand ToAboutPage { get; }

		public ICommand NavigateToSourceCode { get; }

		public string AppVersion
		{
			get => _appVersion;
			set => Set(() => AppVersion, ref _appVersion, value);
		}

		private void ReloadPageCommand()
		{
			User = new TaskNotifier<UserProfile>(_vstsRespository.GetUserProfile());
		}
	}
}
