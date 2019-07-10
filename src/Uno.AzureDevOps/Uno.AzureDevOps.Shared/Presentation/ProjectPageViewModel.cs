using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Commands;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Tasks;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProjectPageViewModel : ViewModelBase, INotifyPropertyChanged
	{
		private readonly IStackNavigationService _navigationService;
		private readonly IVSTSRepository _vstsRepository;

		private string _currentView;
		private bool _loadingSuccess;
		private string _selectedProjectName;
		private TeamProjectReference _project;
		private TeamSettingsIteration _selectedIteration;
		private WebApiTeam _selectedTeam;
		private ITaskNotifier<List<RichWorkItem>> _iterationWorkItems;
		private ITaskNotifier<List<TeamSettingsIteration>> _iterations;
		private ITaskNotifier<List<TeamMember>> _teamMembers;
		private ITaskNotifier<List<WebApiTeam>> _teams;

		public ProjectPageViewModel()
		{
			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_vstsRepository = SimpleIoc.Default.GetInstance<IVSTSRepository>();

			ToProjectItemDetailsPage = new RelayCommand<RichWorkItem>(workItem => _navigationService.ToProjectItemDetailsPage(workItem));
			ToProfilePage = new RelayCommand(() => _navigationService.ToProfilePage());

			ReloadPage = new AsyncCommand(async () => await LoadTeamsAndWorkItems());

			CurrentView = "Sprint";
		}

		public WebApiTeam SelectedTeam
		{
			get => _selectedTeam;
			set
			{
				if (_selectedTeam == null || _selectedTeam.Id != value.Id)
				{
					SelectedIteration = null;
					Iterations = new TaskNotifier<List<TeamSettingsIteration>>(GetIterations(_project, value));
					TeamMembers = new TaskNotifier<List<TeamMember>>(GetTeamMembers(_project, value));
				}

				Set(() => SelectedTeam, ref _selectedTeam, value);
			}
		}

		public TeamSettingsIteration SelectedIteration
		{
			get => _selectedIteration;
			set
			{
				if (_selectedIteration == null || _selectedIteration.Id != value?.Id)
				{
					IterationWorkItems = new TaskNotifier<List<RichWorkItem>>(GetIterationWorkItems(_project, SelectedTeam, value));
				}

				Set(() => SelectedIteration, ref _selectedIteration, value);
			}
		}

		public ITaskNotifier<List<RichWorkItem>> IterationWorkItems
		{
			get => _iterationWorkItems;
			set => Set(() => IterationWorkItems, ref _iterationWorkItems, value);
		}

		public ITaskNotifier<List<TeamSettingsIteration>> Iterations
		{
			get => _iterations;
			set => Set(() => Iterations, ref _iterations, value);
		}

		public ITaskNotifier<List<TeamMember>> TeamMembers
		{
			get => _teamMembers;
			set => Set(() => TeamMembers, ref _teamMembers, value);
		}

		public ITaskNotifier<List<WebApiTeam>> Teams
		{
			get => _teams;
			set => Set(() => Teams, ref _teams, value);
		}

		public ICommand ToProjectItemDetailsPage { get; }

		public ICommand ToProfilePage { get; }

		public ICommand ReloadPage { get; }

		public bool LoadingSuccess
		{
			get => _loadingSuccess;
			set => Set(() => LoadingSuccess, ref _loadingSuccess, value);
		}

		public string CurrentView
		{
			get => _currentView;
			set => Set(() => CurrentView, ref _currentView, value);
		}

		public string SelectedProjectName
		{
			get => _selectedProjectName;
			set => Set(() => SelectedProjectName, ref _selectedProjectName, value);
		}

		public void OnWorkItemClicked(RichWorkItem workItem)
		{
			ToProjectItemDetailsPage.Execute(workItem);
		}

		public void OnNavigatedFrom()
		{
			// Clear the list when navigating away
			IterationWorkItems = new TaskNotifier<List<RichWorkItem>>(Task.FromResult(new List<RichWorkItem>()));
		}

		public async Task OnNavigatedTo(TeamProjectReference project)
		{
			_project = project;
			SelectedProjectName = _project.Name;

			await LoadTeamsAndWorkItems();
		}

		private async Task LoadTeamsAndWorkItems()
		{
			if (_selectedIteration != null)
			{
				IterationWorkItems = new TaskNotifier<List<RichWorkItem>>(GetIterationWorkItems(_project, SelectedTeam, _selectedIteration));
			}

			await LoadTeams();
		}

		private async Task LoadTeams()
		{
			Teams = new TaskNotifier<List<WebApiTeam>>(GetTeams(_project));

			var teams = await Teams.Task;
			SelectedTeam = teams[0];
		}

		private async Task<List<WebApiTeam>> GetTeams(TeamProjectReference project)
		{
			return await _vstsRepository.GetTeams(project.Id);
		}

		private async Task<List<TeamMember>> GetTeamMembers(TeamProjectReference project, WebApiTeam team)
		{
			var members = await _vstsRepository.GetTeamMembers(project.Id, team.Id);

			return members
				.OrderBy(mem => mem.Identity.DisplayName)
				.ToList();
		}

		private async Task<List<RichWorkItem>> GetIterationWorkItems(TeamProjectReference project, WebApiTeam team, TeamSettingsIteration iteration)
		{
			if (team == null || iteration == null)
			{
				LoadingSuccess = true;
				return new List<RichWorkItem>();
			}

			var teamSettings = await _vstsRepository.GetTeamSettings(project.Id, team.Id);
			var workItems = await _vstsRepository.GetIterationWorkItems(project.Id, team.Id, iteration.Id);
			var workItemTypes = await _vstsRepository.GetWorkItemTypes(project.Id);

			if (teamSettings.BugsBehavior == BugsBehavior.AsRequirements)
			{
				workItems = workItems
					.Where(w => !"Task".Equals(w.WorkItem.Fields["System.WorkItemType"] as string, StringComparison.OrdinalIgnoreCase))
					.ToList();
			}
			else if (teamSettings.BugsBehavior == BugsBehavior.AsTasks)
			{
				workItems = workItems
					.Where(w => !"Task".Equals(w.WorkItem.Fields["System.WorkItemType"] as string, StringComparison.OrdinalIgnoreCase)
					&& !"Bug".Equals(w.WorkItem.Fields["System.WorkItemType"] as string, StringComparison.OrdinalIgnoreCase))
					.ToList();
			}

			LoadingSuccess = true;

			return workItems.Select(w => new RichWorkItem(
				w,
				workItemTypes.Where(it => it.Name.Equals(w.WorkItem.Fields["System.WorkItemType"])).FirstOrDefault(),
				project.Id)).ToList();
		}

		private async Task<List<TeamSettingsIteration>> GetIterations(TeamProjectReference project, WebApiTeam team)
		{
			var iterations = await _vstsRepository.GetIterations(project.Id, team.Id);
			SetSelectedIteration(iterations);

			return iterations;
		}

		private void SetSelectedIteration(List<TeamSettingsIteration> result)
		{
			SelectedIteration = result.Where(i => i.Attributes.TimeFrame == TimeFrame.Current).FirstOrDefault();

			// Failsafe for projects without a current sprint
			if (SelectedIteration == null)
			{
				SelectedIteration = result?.FirstOrDefault();
			}
		}
	}
}
