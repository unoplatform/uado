using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.AzureDevOps.Framework.Http;
using Uno.Extensions;
using Uno.Extensions.Specialized;

namespace Uno.AzureDevOps.Client
{
	public class ADOTeamClient : IADOTeamClient
	{
		private readonly IHttpRequestService _httpRequestService;

		public ADOTeamClient(IHttpRequestService httpRequestService)
		{
			_httpRequestService = httpRequestService ?? throw new ArgumentNullException(nameof(httpRequestService));
		}

		public async Task<List<TeamSettingsIteration>> GetIterations(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var iterations = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<ADOCollectionData<TeamSettingsIteration>>($"{accountName}/{projectId.ToStringInvariant()}/{teamId.ToStringInvariant()}/_apis/work/teamsettings/iterations/", ct);

				return iterations.Value;
			}
		}

		public async Task<IterationWorkItemCollection> GetIterationWorkItems(
			string accountName,
			Guid projectId,
			Guid teamId,
			Guid iterationId,
			string accessToken,
			CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var iterationItems = await builder
					.Header("Authorization", "Bearer " + accessToken)
					.Get<IterationWorkItemCollection>(
						$"{accountName}/{projectId.ToStringInvariant()}/{teamId.ToStringInvariant()}/_apis/work/teamsettings/iterations/{iterationId.ToStringInvariant()}/workitems/",
						ct);

				return iterationItems;
			}
		}

		public async Task<List<TeamProjectReference>> GetProjects(
			string accountName,
			string accessToken,
			string continuationToken = null,
			int skip = 0,
			int offset = 1000,
			CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var projectReferences = await builder
					.SetDefaultADORequestValues(accessToken)
					.QueryParameter("stateFilter", "All")
					.QueryParameter("$top", offset.ToStringInvariant())
					.QueryParameter("$skip", skip.ToStringInvariant())
					.QueryParameter("getDefaultTeamImageUrl", "true")
					.Get<ADOCollectionData<TeamProjectReference>>($"{accountName}/_apis/projects", ct);

				return projectReferences.Value;
			}
		}

		public async Task<TeamProjectReference> GetProject(
			string accountName,
			Guid projectId,
			string accessToken,
			CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var projectReference = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<TeamProjectReference>($"{accountName}/_apis/projects/{projectId}", ct);

				return projectReference;
			}
		}

		public async Task<List<TeamMember>> GetTeamMembers(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var projectReferences = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<ADOCollectionData<TeamMember>>(
						$"{accountName}/_apis/projects/{projectId.ToStringInvariant()}/teams/{teamId.ToStringInvariant()}/members/",
						ct);

				return projectReferences.Value;
			}
		}

		public async Task<List<WebApiTeam>> GetTeams(string accountName, Guid projectId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var teams = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<ADOCollectionData<WebApiTeam>>(
						$"{accountName}/_apis/projects/{projectId.ToStringInvariant()}/teams/", ct);

				return teams.Value;
			}
		}

		public async Task<TeamSetting> GetTeamSettings(string accountName, Guid projectId, Guid teamId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var teamSettings = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<TeamSetting>(
						$"{accountName}/{projectId.ToStringInvariant()}/{teamId.ToStringInvariant()}/_apis/work/teamsettings/",
						ct);

				return teamSettings;
			}
		}

		public async Task<WorkItem> GetWorkItem(string accountName, Guid projectId, int workItemId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var workItem = await builder
					.SetDefaultADORequestValues(accessToken)
					.QueryParameter("$expand", "All")
					.Get<WorkItem>(
						$"{accountName}/{projectId.ToStringInvariant()}/_apis/wit/workItems/{workItemId.ToStringInvariant()}/",
						ct);

				return workItem;
			}
		}

		public async Task<List<WorkItem>> GetWorkItems(string accountName, int[] workItemIds, string accessToken, int limit = -1, CancellationToken ct = default)
		{
			if (workItemIds.Any())
			{
				// If a limit was specified, use it on the work items Id's list
				var ids = (limit > -1)
					? string.Join(",", workItemIds.Take(limit))
					: string.Join(",", workItemIds);

				using (var builder = _httpRequestService.RequestBuilderFactory.Create())
				{
					var workItems = await builder
						.SetDefaultADORequestValues(accessToken)
						.QueryParameter("$expand", "All")
						.QueryParameter("ids", ids)
						.Get<ADOCollectionData<WorkItem>>($"{accountName}/_apis/wit/workItems", ct);

					if (workItems?.Value != null)
					{
						return workItems.Value;
					}
				}
			}

			return new List<WorkItem>();
		}

		public async Task<List<WorkItemType>> GetWorkItemTypes(string accountName, Guid projectId, string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var types = await builder
					.SetDefaultADORequestValues(accessToken)
					.Get<ADOCollectionData<WorkItemType>>(
						$"{accountName}/{projectId.ToStringInvariant()}/_apis/wit/workItemTypes/", ct);

				return types.Value;
			}
		}

		public async Task UpdateWorkItem(string accountName, string userName, int workItemId, string accessToken, CancellationToken ct = default)
		{
			var payload = new[]
			{
				new
				{
					from = string.Empty,
					op = "Add",
					path = "/fields/System.AssignedTo",
					value = userName
				}
			};

			var requestBody = JsonConvert.SerializeObject(payload);

			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var wi = await builder
					.SetDefaultADORequestValues(accessToken)
					.ContentType("application/json-patch+json")
					.RequestBody(requestBody)
					.Patch<WorkItem>($"{accountName}/_apis/wit/workitems/{workItemId.ToStringInvariant()}/", ct);
			}
		}
	}
}
