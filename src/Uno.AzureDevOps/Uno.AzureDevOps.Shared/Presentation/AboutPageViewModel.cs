using System;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using Uno.AzureDevOps.Framework.Commands;
using Windows.System;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class AboutPageViewModel : ViewModelBase
	{
		public AboutPageViewModel()
		{
			NavigateToSourceCode = new AsyncCommand(async () => await Launcher.LaunchUriAsync(new Uri("https://platform.uno")));
		}

		public ICommand NavigateToSourceCode { get; }
	}
}
