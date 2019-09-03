using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business;
using Uno.AzureDevOps.Business.Entities;
using Uno.AzureDevOps.Business.Extensions;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Commands;
using Uno.AzureDevOps.Framework.Navigation;
using Uno.AzureDevOps.Framework.Storage;
using Uno.AzureDevOps.Framework.Tasks;
using Uno.AzureDevOps.Views.Content;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProjectPageViewModel : ViewModelBase, INotifyPropertyChanged
	{
		private const string CategoryInProgress = "InProgress";

		private readonly IStackNavigationService _navigationService;
		private readonly IVSTSRepository _vstsRepository;
		private readonly IUserPreferencesService _userPreferencesService;

		private string _currentView;
		private bool _loadingSuccess;
		private TeamProjectReference _project;
		private TeamSettingsIteration _selectedIteration;
		private WebApiTeam _selectedTeam;
		private AccountData _account;
		private List<WorkItemData> _workItemsData;
		private List<WorkItemType> _workItemsType;

		private int _daysRemaining;
		private int _totalWorkItems;
		private int _numberOfPbiInProgress;
		private int _numberOfBugInProgress;
		private int _numberOfAssignedWorkItem;
		private int _numberOfUnassignedWorkItem;
		private TeamSettingsIteration _currentIteration;

		private ITaskNotifier<List<RichWorkItem>> _iterationWorkItems;
		private ITaskNotifier<List<TeamSettingsIteration>> _iterations;
		private ITaskNotifier<List<TeamMember>> _teamMembers;
		private ITaskNotifier<List<WebApiTeam>> _teams;

		public ProjectPageViewModel()
		{
			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_vstsRepository = SimpleIoc.Default.GetInstance<IVSTSRepository>();
			_userPreferencesService = SimpleIoc.Default.GetInstance<IUserPreferencesService>();

			ToProjectItemDetailsPage = new RelayCommand<RichWorkItem>(workItem => _navigationService.ToProjectItemDetailsPage(workItem));
			ToOrganizationListPage = new RelayCommand(() => _navigationService.ToOrganizationListPage());

			ReloadPage = new AsyncCommand(async () => await LoadTeamsAndWorkItems());

			CurrentView = "Summary";
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
					IterationWorkItems = new TaskNotifier<List<RichWorkItem>>(GetAndComputeIterationWorkitems(_project, SelectedTeam, value));
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

		public ICommand ToOrganizationListPage { get; }

		public ICommand ToProjectListPage { get; }

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

		public AccountData Account
		{
			get => _account;
			set => Set(() => Account, ref _account, value);
		}

		public TeamProjectReference Project
		{
			get => _project;
			set => Set(() => Project, ref _project, value);
		}

		public int DaysRemaining
		{
			get => _daysRemaining;
			set => Set(() => DaysRemaining, ref _daysRemaining, value);
		}

		public int TotalWorkItems
		{
			get => _totalWorkItems;
			set => Set(() => TotalWorkItems, ref _totalWorkItems, value);
		}

		public int NumberOfPbiInProgress
		{
			get => _numberOfPbiInProgress;
			set => Set(() => NumberOfPbiInProgress, ref _numberOfPbiInProgress, value);
		}

		public int NumberOfBugInProgress
		{
			get => _numberOfBugInProgress;
			set => Set(() => NumberOfBugInProgress, ref _numberOfBugInProgress, value);
		}

		public int NumberOfAssignedWorkItem
		{
			get => _numberOfAssignedWorkItem;
			set => Set(() => NumberOfAssignedWorkItem, ref _numberOfAssignedWorkItem, value);
		}

		public int NumberOfUnassignedWorkItem
		{
			get => _numberOfUnassignedWorkItem;
			set => Set(() => NumberOfUnassignedWorkItem, ref _numberOfUnassignedWorkItem, value);
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
			var account = _userPreferencesService.GetPreferredAccount();
			_account = account;

			// if project is null, that means we navigate here from login page
			if (project == null)
			{
				if (_userPreferencesService.TryGetPreferredProjectId(out var projectId))
				{
					// Get account name to be able to do calls in vstsRepository
					_vstsRepository.SetVSTSAccount(_account.AccountName);
					project = await GetProject(projectId);
				}
			}
			else
			{
				// project is not null, navigation has been done from listview, saving it as preferred project
				_userPreferencesService.SavePreferredProject(project.Id);
			}

			Project = project;

			// SelectedProjectName = _project.Name;
			await LoadTeamsAndWorkItems();
		}

		private async Task<List<RichWorkItem>> GetAndComputeIterationWorkitems(TeamProjectReference project, WebApiTeam team, TeamSettingsIteration iteration)
		{
			var workItems = await GetIterationWorkItems(project, team, iteration);

			if (iteration.Attributes.TimeFrame == TimeFrame.Current)
			{
				await ComputeValues(workItems);
			}

			return workItems;
		}

		private async Task LoadTeamsAndWorkItems()
		{
			if (_selectedIteration != null)
			{
				IterationWorkItems = new TaskNotifier<List<RichWorkItem>>(GetAndComputeIterationWorkitems(Project, SelectedTeam, _selectedIteration));
			}

			await LoadTeams();
		}

		private async Task LoadTeams()
		{
			Teams = new TaskNotifier<List<WebApiTeam>>(GetTeams(Project));

			var teams = await Teams.Task;
			SelectedTeam = teams[0];
		}

		private async Task<List<WebApiTeam>> GetTeams(TeamProjectReference project)
		{
			return await _vstsRepository.GetTeams(project.Id);
		}

		/// <summary>
		/// Get a single project
		/// </summary>
		private async Task<TeamProjectReference> GetProject(Guid projectId)
		{
			return await _vstsRepository.GetTeamProjectReference(projectId);
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
			_workItemsData = workItems;

			var workItemTypes = await _vstsRepository.GetWorkItemTypes(project.Id);
			_workItemsType = workItemTypes;

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
			_currentIteration = SelectedIteration;

			// Failsafe for projects without a current sprint
			if (SelectedIteration == null)
			{
				SelectedIteration = result?.FirstOrDefault();
			}
		}

		/// <summary>
		/// Compute values for the currentsprint of the current project
		/// </summary>
		private async Task ComputeValues(List<RichWorkItem> workItems)
		{
			await ComputePbiValues(workItems);
			await ComputeBugValues();
			ComputeGeneralValues();
		}

		private async Task ComputeBugValues()
		{
			var bugWorkItems = _workItemsData
				.Where(w => "Bug".Equals(w.WorkItem.Fields["System.WorkItemType"] as string, StringComparison.OrdinalIgnoreCase))
				.ToList();

			if (bugWorkItems != null && bugWorkItems.Count > 0)
			{
				var bugStates = await _vstsRepository.GetWorkItemTypeStates(Project.Id, bugWorkItems.FirstOrDefault().WorkItemType);

				NumberOfBugInProgress = bugWorkItems
				.Where(bwi => bwi.State
					.Any(st => bugStates.Select(sc => sc)
					.Where(sc => sc.Category == CategoryInProgress)
					.Any(sc => sc.Name == bwi.State)))
				.Count();
			}
		}

		private async Task ComputePbiValues(List<RichWorkItem> workItems)
		{
			var iterationsWorkItems = workItems;
			if (iterationsWorkItems != null && iterationsWorkItems.Count > 0)
			{
				var pbiWorkItemType = iterationsWorkItems.FirstOrDefault().Type.Name;
				var pbiStates = await _vstsRepository.GetWorkItemTypeStates(Project.Id, pbiWorkItemType);

				NumberOfPbiInProgress = iterationsWorkItems
				.Where(iw => iw.Item.State
					.Any(st => pbiStates.Select(sc => sc)
					.Where(sc => sc.Category == CategoryInProgress)
					.Any(sc => sc.Name == iw.Item.State)))
				.Count();
			}
		}

		private void ComputeGeneralValues()
		{
			if (_workItemsData != null && _workItemsData.Count > 0)
			{
				TotalWorkItems = _workItemsData.Count;
				NumberOfAssignedWorkItem = _workItemsData.Where(wi => wi.AssignedTo != null).Count();
				NumberOfUnassignedWorkItem = TotalWorkItems - NumberOfAssignedWorkItem;

				var endDate = _currentIteration.Attributes.FinishDate;
				var startDate = _currentIteration.Attributes.StartDate;

				if (endDate != null && startDate != null)
				{
					var dateDifference = (TimeSpan)(endDate - startDate);
					DaysRemaining = dateDifference.Days;
				}
			}
		}
	}
}
