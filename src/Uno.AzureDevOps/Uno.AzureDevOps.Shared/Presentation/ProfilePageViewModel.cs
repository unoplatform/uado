using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Framework.Tasks;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProfilePageViewModel : ViewModelBase
	{
		private readonly IVSTSRepository _vstsRespository;
		private ITaskNotifier<UserProfile> _userProfile;

		public ProfilePageViewModel()
		{
			_vstsRespository = SimpleIoc.Default.GetInstance<IVSTSRepository>();

			UserProfile = new TaskNotifier<UserProfile>(_vstsRespository.GetUserProfile());
			ReloadPage = new RelayCommand(() => ReloadPageCommand());
		}

		public ITaskNotifier<UserProfile> UserProfile
		{
			get => _userProfile;
			set => Set(() => UserProfile, ref _userProfile, value);
		}

		public ICommand ReloadPage { get; }

		private void ReloadPageCommand()
		{
			UserProfile = new TaskNotifier<UserProfile>(_vstsRespository.GetUserProfile());
		}
	}
}
