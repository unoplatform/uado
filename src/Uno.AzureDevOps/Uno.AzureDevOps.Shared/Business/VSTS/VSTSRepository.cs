using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.AzureDevOps.Business.Authentication;
using Uno.AzureDevOps.Business.Entities;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Business.VSTS
{
	public class VSTSRepository : IVSTSRepository
	{
		private readonly IADOProfileClient _profileClient;
		private readonly IADOOrganizationClient _organizationClient;
		private readonly IADOTeamClient _teamClient;
		private readonly IAuthenticationService _authenticationService;

		private string _accountName;

		public VSTSRepository(
			IADOProfileClient profileClient,
			IADOOrganizationClient organizationClient,
			IADOTeamClient teamClient,
			IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
			_profileClient = profileClient ?? throw new ArgumentNullException(nameof(profileClient));
			_teamClient = teamClient ?? throw new ArgumentNullException(nameof(teamClient));
			_organizationClient = organizationClient ?? throw new ArgumentNullException(nameof(organizationClient));
		}

		public void SetVSTSAccount(string accountName)
		{
			_accountName = accountName;
		}

		public Task<List<TeamProjectReference>> GetTeamProjectReferences(CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var projects = await _teamClient.GetProjects(_accountName, authData.AccessToken);

					return projects.OrderBy(project => project.Name).ToList();
				},
				ct);
		}

		public Task<TeamSetting> GetTeamSettings(Guid projectId, Guid teamId, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var setting = await _teamClient.GetTeamSettings(_accountName, projectId, teamId, authData.AccessToken);

					return setting;
				},
				ct);
		}

		public Task<List<WebApiTeam>> GetTeams(Guid projectId, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var teams = await _teamClient.GetTeams(_accountName, projectId, authData.AccessToken);

					return teams;
				},
				ct);
		}

		public Task<List<TeamMember>> GetTeamMembers(Guid projectId, Guid teamId, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var members = await _teamClient.GetTeamMembers(_accountName, projectId, teamId, authData.AccessToken);

					return members;
				},
				ct);
		}

		public Task<List<TeamSettingsIteration>> GetIterations(Guid projectId, Guid teamId, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var iterations = await _teamClient.GetIterations(_accountName, projectId, teamId, authData.AccessToken);

					return iterations;
				},
				ct);
		}

		public Task<List<WorkItemType>> GetWorkItemTypes(Guid projectId, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var workItemTypes = await _teamClient.GetWorkItemTypes(_accountName, projectId, authData.AccessToken);

					return workItemTypes;
				},
				ct);
		}

		public async Task<List<WorkItemData>> GetWorkItems(IEnumerable<int?> workItemIds, int limit = -1, CancellationToken ct = default)
		{
			return await _authenticationService.AuthenticatedExecution(async (token, authData) =>
			{
				var workItems = await _teamClient.GetWorkItems(_accountName, workItemIds.Where(i => i.HasValue).Select(i => i.Value).ToArray(), authData.AccessToken, limit);

				return workItems.Select(workItem => new WorkItemData(workItem)).ToList();
			});
		}

		public async Task<List<WorkItemData>> GetIterationWorkItems(Guid projectId, Guid teamId, Guid iterationId, CancellationToken ct = default)
		{
			return await _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var workItems = new List<WorkItem>();
					var iterationWorkItems = await _teamClient.GetIterationWorkItems(_accountName, projectId, teamId, iterationId, authData.AccessToken);

					if (iterationWorkItems.WorkItemRelations.Any())
					{
						var skip = 0;
						const int batchSize = 100;
						var workItemIds = Array.Empty<int>();

						do
						{
							workItemIds = iterationWorkItems
								.WorkItemRelations
								.Skip(skip)
								.Take(batchSize)
								.Select(wir => wir.Target.Id)
								.ToArray();

							if (workItemIds.Any())
							{
								var items = await _teamClient.GetWorkItems(_accountName, workItemIds, authData.AccessToken);
								workItems.AddRange(items);
							}

							skip += batchSize;
						}
						while (workItemIds.Count() == batchSize);
					}

					return workItems.Select(workItem => new WorkItemData(workItem)).ToList();
				},
				ct);
		}

		public async Task<WorkItemData> GetWorkItem(Guid projectId, int workItemId, CancellationToken ct = default)
		{
			return await _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var workItem = await _teamClient.GetWorkItem(_accountName, projectId, workItemId, authData.AccessToken);

					return new WorkItemData(workItem);
				},
				ct);
		}

		public Task AssignToMe(WorkItem workItem, CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var profileData = await _profileClient.GetProfile(authData.AccessToken, ct);

					await _teamClient.UpdateWorkItem(_accountName, profileData.DisplayName, workItem.Id.Value, authData.AccessToken);
				},
				ct);
		}

		public Task<List<AccountData>> GetOrganizations(CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					// authData does not contain the correct id for memberId, so profileData is necessary.
					var profileData = await _profileClient.GetProfile(authData.AccessToken, ct);
					return await _organizationClient.GetOrganizations(authData.AccessToken, profileData.Id, ct);
				},
				ct);
		}

		public Task<UserProfile> GetUserProfile(CancellationToken ct = default)
		{
			return _authenticationService.AuthenticatedExecution(
				async (token, authData) =>
				{
					var profileData = await _profileClient.GetProfile(authData.AccessToken, ct);

					if (string.IsNullOrEmpty(_accountName))
					{
						var organizations = await GetOrganizations(ct);
						SetVSTSAccount(organizations?.FirstOrDefault()?.AccountName);
					}

					return new UserProfile()
					{
						Email = profileData.EmailAddress,
						Image = new MemoryStream(Convert.FromBase64String(FixBase64ForImage(profileData.CoreAttributes.Avatar.Value.Value))),
						Name = profileData.DisplayName,
					};
				},
				ct);
		}

		private string FixBase64ForImage(string base64Image)
		{
			return new StringBuilder(base64Image, base64Image.Length)
				.Replace("\r\n", string.Empty)
				.Replace(" ", string.Empty)
				.ToString();
		}
	}
}
