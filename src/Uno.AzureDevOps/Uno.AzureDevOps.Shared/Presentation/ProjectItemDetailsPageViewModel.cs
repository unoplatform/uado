using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;

#if !__WASM__
using Connectivity = Xamarin.Essentials;
#endif

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class ProjectItemDetailsPageViewModel : ViewModelBase
	{
		private readonly IStackNavigationService _navigationService;
		private readonly IVSTSRepository _vstsRepository;

		private ITaskNotifier<List<RichWorkItem>> _childrenWorkItems;
		private ITaskNotifier<RichWorkItem> _parentWorkItem;
		private ITaskNotifier<RichWorkItem> _workItem;
		private ITaskNotifier<UserProfile> _userProfile;
		private string _pageTitle;
		private bool _showDoubleBackTip;
		private bool _hasRelatedWork;
		private bool _hasMoreRelatedItems;
		private bool _hasParent;
		private bool _canAssignToMe;

		public ProjectItemDetailsPageViewModel()
		{
			_navigationService = SimpleIoc.Default.GetInstance<IStackNavigationService>();
			_vstsRepository = SimpleIoc.Default.GetInstance<IVSTSRepository>();

			AssignToMe = new AsyncCommand(async () => await AssignToMeAndRefresh());
			ToProfilePage = new RelayCommand(() => _navigationService.ToProfilePage());
			ReloadPage = new RelayCommand(() => ReloadPageCommand());
			ToProjectItemDetailsPage = new RelayCommand<RichWorkItem>(workItem => _navigationService.ToProjectItemDetailsPage(workItem));
			ToParentProjectItemDetailsPage = new RelayCommand(() => OnWorkItemClicked(ParentWorkItem.Result));
			ViewMore = new AsyncCommand(async () => await LaunchBrowserWithWorkItemUri());
			HideDoubleBackTip = new RelayCommand(async () => await HideDoubleBackTipCommand());
		}

		public ITaskNotifier<RichWorkItem> WorkItem
		{
			get => _workItem;
			set => Set(() => WorkItem, ref _workItem, value);
		}

		public string PageTitle
		{
			get => _pageTitle;
			set => Set(() => PageTitle, ref _pageTitle, value);
		}

		public bool CanAssignToMe
		{
			get => _canAssignToMe;
			set => Set(() => CanAssignToMe, ref _canAssignToMe, value);
		}

		public bool ShowDoubleBackTip
		{
			get => _showDoubleBackTip;
			set => Set(() => ShowDoubleBackTip, ref _showDoubleBackTip, value);
		}

		public bool HasRelatedWork
		{
			get => _hasRelatedWork;
			set => Set(() => HasRelatedWork, ref _hasRelatedWork, value);
		}

		public bool HasMoreRelatedItems
		{
			get => _hasMoreRelatedItems;
			set => Set(() => HasMoreRelatedItems, ref _hasMoreRelatedItems, value);
		}

		public bool HasParent
		{
			get => _hasParent;
			set => Set(() => HasParent, ref _hasParent, value);
		}

		public ITaskNotifier<List<RichWorkItem>> ChildrenWorkItems
		{
			get => _childrenWorkItems;
			set => Set(() => ChildrenWorkItems, ref _childrenWorkItems, value);
		}

		public ITaskNotifier<RichWorkItem> ParentWorkItem
		{
			get => _parentWorkItem;
			set => Set(() => ParentWorkItem, ref _parentWorkItem, value);
		}

		public ITaskNotifier<UserProfile> UserProfile
		{
			get => _userProfile;
			set => Set(() => UserProfile, ref _userProfile, value);
		}

		public ICommand ReloadPage { get; }

		public ICommand HideDoubleBackTip { get; }

		public ICommand AssignToMe { get; }

		public ICommand ViewMore { get; }

		public ICommand ToProfilePage { get; }

		public ICommand ToParentProjectItemDetailsPage { get; }

		public ICommand ToProjectItemDetailsPage { get; }

		public void OnWorkItemClicked(RichWorkItem workItem)
		{
			ChildrenWorkItems = null;
			ToProjectItemDetailsPage.Execute(workItem);
		}

		public async Task OnNavigatedTo(RichWorkItem workItem)
		{
			WorkItem = new TaskNotifier<RichWorkItem>(Task.FromResult(workItem));

			var workItemType = workItem.Item.WorkItemType == "Product Backlog Item" ? "PBI" : workItem.Item.WorkItemType;

			PageTitle = workItemType + " " + workItem.Item.WorkItem.Id;

			await LoadUserProfile(workItem);

			LoadRelatedWorkItems();

			// Show the "double-back" info if we have reached the minimum child depth in the back stack
			ShowDoubleBackTip = !IsDoubleBackTipShown()
				&& _navigationService
				.BackStack
				.Where(stackEntry => stackEntry.SourcePageType == typeof(Views.Content.ProjectItemDetails))
				.Count() >= BusinessConstants.DoubleBack.MinimumDepth;
		}

		public async Task LaunchBrowserWithWorkItemUri()
		{
			var workItem = await WorkItem.Task;
			var targetUrl = workItem.Item.Url.Replace("_apis/wit/workItems", "_workitems/edit/");

			await Launcher.LaunchUriAsync(new Uri(targetUrl));
		}

		public async Task AssignToMeAndRefresh()
		{
#if __WASM__
			// MessageDialog not fully implemented yet for Uno/Wasm
			// https://github.com/nventive/Uno/issues/124

			await Task.Yield();

			const string js = "(confirm(\"Assign this work item to yourself?\") ? \"ok\" : \"cancel\")";
			var r = Uno.Foundation.WebAssemblyRuntime.InvokeJS(js);
			if(r == "ok")
			{
				AssignToMeHandler(null);
			}
#else
			var messageDialog = new MessageDialog("Assign this work item to yourself?");

			messageDialog.Commands.Add(new UICommand("Cancel", null));
			messageDialog.Commands.Add(new UICommand("Assign to me", new UICommandInvokedHandler(this.AssignToMeHandler)));

			messageDialog.CancelCommandIndex = 0;
			messageDialog.DefaultCommandIndex = 1;

			await messageDialog.ShowAsync();
#endif
		}

		private async void AssignToMeHandler(IUICommand command)
		{
			// As AssignToMe has no return type we try catch to safely call endpoints
			try
			{
				var workItem = await WorkItem.Task;
				await _vstsRepository.AssignToMe(workItem.Item.WorkItem);
				var updatedWorkItem = await _vstsRepository.GetWorkItem(workItem.ProjectId, workItem.Item.WorkItem.Id.Value);

				WorkItem = new TaskNotifier<RichWorkItem>(Task.FromResult(new RichWorkItem(updatedWorkItem, workItem.Type, workItem.ProjectId)));
				CanAssignToMe = false;
			}
			catch (Exception e)
			{
				Console.Error.Write(e.Message + " " + e.InnerException);
				var errorMessage = "An error occured, please try again";
#if !__WASM__
				if (Connectivity.Connectivity.NetworkAccess != Connectivity.NetworkAccess.Internet)
				{
					errorMessage = "No connection detected, please check your device settings to ensure a network connection is setup.";
				}
#endif
				var messageDialog = new MessageDialog(errorMessage);
				await messageDialog.ShowAsync();
			}
		}

		private async Task<List<RichWorkItem>> GetRelatedWorkItems()
		{
			var workItem = await WorkItem.Task;
			var relations = workItem.Item.Relations ?? new List<WorkItemRelation>();

			var relationType = relations.Where(r => r.Rel == BusinessConstants.WorkItemRelationType.Child);

			var relatedWorkItems = await GetRelatedRichWorkItems(workItem.ProjectId, relationType);

			HasRelatedWork = relatedWorkItems.Items.Count > 0;

			HasMoreRelatedItems = relatedWorkItems.IsExceedingMaxItems;

			return relatedWorkItems.Items;
		}

		private async Task<RichWorkItem> GetParentWorkItem()
		{
			var workItem = await WorkItem.Task;
			var relations = workItem.Item.Relations ?? new List<WorkItemRelation>();

			var relationType = relations.Where(r => r.Rel == BusinessConstants.WorkItemRelationType.Parent);

			var relatedWorkItems = await GetRelatedRichWorkItems(workItem.ProjectId, relationType);

			HasParent = relatedWorkItems.Items.Count > 0;

			return relatedWorkItems.Items.FirstOrDefault();
		}

		private async Task<UserProfile> GetUserProfile()
		{
			return await _vstsRepository.GetUserProfile();
		}

		private async Task LoadUserProfile(RichWorkItem workItem)
		{
			UserProfile = new TaskNotifier<UserProfile>(GetUserProfile());
			var userProfile = await UserProfile.Task;

			CanAssignToMe = !userProfile.Name.Equals(workItem.Item?.AssignedTo?.DisplayName);
		}

		[SuppressMessage("Brackets and parenthesis issue", "SA1009", Justification = "Syntax is fine like that, if this is correct it creates another issue that recreates this issue afterwards")]
		private async Task<(List<RichWorkItem> Items, bool IsExceedingMaxItems)> GetRelatedRichWorkItems(Guid projectId, IEnumerable<WorkItemRelation> itemRelations)
		{
			var itemIds = itemRelations?.Select(r => int.TryParse(r.Url.Split('/').LastOrDefault(), out var parsed) ? parsed : (int?)null);

			if (itemIds?.Any() == true)
			{
				// We use a limit of MaxItems + 1 so that we can return a list greater than MaxItems
				// if they exist (needed for the UI to show a Load More option)
				var limit = PresentationConstants.RelatedWorkItem.MaxItems + 1;
				var workItems = await _vstsRepository.GetWorkItems(itemIds, limit);
				var richWorkItems = workItems?.Select(i => new RichWorkItem(i, null, projectId)) ?? new List<RichWorkItem>();

				return (richWorkItems.Take(PresentationConstants.RelatedWorkItem.MaxItems).ToList(),
					richWorkItems.Count() > PresentationConstants.RelatedWorkItem.MaxItems);
			}

			return (new List<RichWorkItem>(), false);
		}

		private void LoadRelatedWorkItems()
		{
			ChildrenWorkItems = new TaskNotifier<List<RichWorkItem>>(GetRelatedWorkItems());
			ParentWorkItem = new TaskNotifier<RichWorkItem>(GetParentWorkItem());
		}

		private void ReloadPageCommand()
		{
			LoadRelatedWorkItems();
		}

		private async Task HideDoubleBackTipCommand()
		{
			await SetDoubleBackTipAsShown();
		}

		private async Task SetDoubleBackTipAsShown()
		{
			await ApplicationData.Current.LocalFolder.CreateFileAsync($"{BusinessConstants.DoubleBack.SettingsKey}", CreationCollisionOption.ReplaceExisting);
			ShowDoubleBackTip = false;
		}

		[SuppressMessage("", "CA1822", Justification = "Wanted behavior")]
		private bool IsDoubleBackTipShown()
		{
			return File.Exists(ApplicationData.Current.LocalFolder.Path + $"/{BusinessConstants.DoubleBack.SettingsKey}");
		}
	}
}
