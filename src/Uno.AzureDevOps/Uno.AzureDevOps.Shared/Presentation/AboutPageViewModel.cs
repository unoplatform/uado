using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Commands;
using Windows.System;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class AboutPageViewModel : ViewModelBase
	{
		public AboutPageViewModel()
		{
			NavigateToUnoPlatform = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri(ClientConstants.UnoPlatformUrl)));
			NavigateToSourceCode = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri(ClientConstants.GitHubUadoUrl)));
			NavigateToPrivacyPolicy = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri(ClientConstants.PrivacyPolicyUrl)));
			NavigateToTermsAndConditions = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri(ClientConstants.TermsAndConditionsUrl)));
		}

		public ICommand NavigateToSourceCode { get; }

		public ICommand NavigateToPrivacyPolicy { get; }

		public ICommand NavigateToTermsAndConditions { get; }

		public ICommand NavigateToUnoPlatform { get; }
	}
}
