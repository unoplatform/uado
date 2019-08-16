using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Client
{
	public interface IADOTeamClient
	{
		Task<List<TeamSettingsIteration>> GetIterations(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default);

		Task<IterationWorkItemCollection> GetIterationWorkItems(string accountName, Guid projectId, Guid teamId, Guid iterationId, string accessToken, CancellationToken ct = default);

		Task<List<TeamProjectReference>> GetProjects(
			string accountName,
			string accessToken,
			string continuationToken = null,
			int skip = 0,
			int offset = 1000,
			CancellationToken ct = default);

		Task<TeamProjectReference> GetProject(
			string accountName,
			Guid projectId,
			string accessToken,
			CancellationToken ct = default);

		Task<List<TeamMember>> GetTeamMembers(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default);

		Task<List<WebApiTeam>> GetTeams(string accountName, Guid projectId, string accessToken, CancellationToken ct = default);

		Task<TeamSetting> GetTeamSettings(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default);

		Task<WorkItem> GetWorkItem(string accountName, Guid projectId, int workItemId, string accessToken, CancellationToken ct = default);

		Task<List<WorkItem>> GetWorkItems(string accountName, int[] workItemIds, string accessToken, int limit = -1, CancellationToken ct = default);

		Task<List<WorkItemType>> GetWorkItemTypes(string accountName, Guid projectId, string accessToken, CancellationToken ct = default);

		Task UpdateWorkItem(string accountName, string userName, int workItemId, string accessToken, CancellationToken ct = default);
	}
}
