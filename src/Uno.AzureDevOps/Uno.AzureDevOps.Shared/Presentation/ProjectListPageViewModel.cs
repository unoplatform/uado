using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Tasks;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProjectListPageViewModel : ViewModelBase, INotifyPropertyChanged
	{
		private readonly IStackNavigationService _navigationService;
		private readonly IVSTSRepository _vstsRepository;
		private readonly IUserPreferencesService _userPreferencesService;

		private ITaskNotifier<List<TeamProjectReference>> _projects;
		private AccountData _account;

		public ProjectListPageViewModel()
		{
			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_vstsRepository = SimpleIoc.Default.GetInstance<IVSTSRepository>();
			_userPreferencesService = SimpleIoc.Default.GetInstance<IUserPreferencesService>();

			ToProjectPage = new RelayCommand<TeamProjectReference>(project => _navigationService.ToProjectPage(project));

			ReloadPage = new RelayCommand(() => Projects = new TaskNotifier<List<TeamProjectReference>>(GetProjects()));

			ToProfilePage = new RelayCommand(() => _navigationService.ToProfilePage());
		}

		public ICommand ToProjectPage { get; }

		public ICommand ToProfilePage { get; }

		public ICommand ReloadPage { get; }

		public ITaskNotifier<List<TeamProjectReference>> Projects
		{
			get => _projects;
			set => Set(nameof(Projects), ref _projects, value);
		}

		public AccountData Account
		{
			get => _account;
			set => Set(() => Account, ref _account, value);
		}

		public void OnNavigatedTo(AccountData account)
		{
			Account = account;

			// Saving the accountName to be able to retrieve it later
			_userPreferencesService.SavePreferredAccountName(account.AccountName);
			_vstsRepository.SetVSTSAccount(_account.AccountName);

			if (Projects == null)
			{
				Projects = new TaskNotifier<List<TeamProjectReference>>(GetProjects());
			}
		}

		public void NavigateToProjectPage(TeamProjectReference project)
		{
			ToProjectPage.Execute(project);
		}

		private async Task<List<TeamProjectReference>> GetProjects()
		{
			return await _vstsRepository.GetTeamProjectReferences();
		}
	}
}
