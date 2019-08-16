using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uno.AzureDevOps.Business.Entities;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Business.VSTS
{
	public interface IVSTSRepository
	{
		void SetVSTSAccount(string accountName);

		Task<List<TeamProjectReference>> GetTeamProjectReferences(CancellationToken ct = default);

		Task<TeamProjectReference> GetTeamProjectReference(Guid projectId, CancellationToken ct = default);

		Task<List<AccountData>> GetOrganizations(CancellationToken ct = default);

		Task<List<WebApiTeam>> GetTeams(Guid projectId, CancellationToken ct = default);

		Task<TeamSetting> GetTeamSettings(Guid projectId, Guid teamId, CancellationToken ct = default);

		Task<List<TeamMember>> GetTeamMembers(Guid projectId, Guid teamId, CancellationToken ct = default);

		Task<List<TeamSettingsIteration>> GetIterations(Guid projectId, Guid teamId, CancellationToken ct = default);

		Task<List<WorkItemType>> GetWorkItemTypes(Guid projectId, CancellationToken ct = default);

		Task<List<WorkItemData>> GetIterationWorkItems(Guid projectId, Guid teamId, Guid iterationId, CancellationToken ct = default);

		Task<WorkItemData> GetWorkItem(Guid projectId, int workItemId, CancellationToken ct = default);

		Task<List<WorkItemData>> GetWorkItems(IEnumerable<int?> workItemIds, int limit = -1, CancellationToken ct = default);

		Task AssignToMe(WorkItem workItem, CancellationToken ct = default);

		Task<UserProfile> GetUserProfile(CancellationToken ct = default);
	}
}
