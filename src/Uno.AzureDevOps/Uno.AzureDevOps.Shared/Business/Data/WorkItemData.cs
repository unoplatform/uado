using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Uno.AzureDevOps.Client;
using Windows.UI.Xaml.Data;

namespace Uno.AzureDevOps.Business.Entities
{
	[Bindable]
	public partial class WorkItemData
	{
		public WorkItemData(WorkItem workItem)
		{
			WorkItem = workItem;
		}

		public WorkItem WorkItem { get; }

		public IList<WorkItemRelation> Relations => WorkItem.Relations;

		public string AcceptanceCriteria => GetFieldValue<string>("Microsoft.VSTS.Common.AcceptanceCriteria");

		public string Details => GetDetailsFieldValue<string>("Microsoft.VSTS.TCM.SystemInfo");

		public string Description => GetFieldValue<string>("System.Description");

		public string ReproSteps => GetFieldValue<string>("Microsoft.VSTS.TCM.ReproSteps");

		public string Title => GetFieldValue<string>("System.Title");

		public IdentityRef AssignedTo => GetFieldValue<JObject>("System.AssignedTo")?.ToObject<IdentityRef>();

		public object Parent => GetFieldValue<object>("System.Parent");

		public string State => GetFieldValue<string>("System.State");

		public string StatusColor => GetFieldValue<string>("System.StatusColor");

		public string WorkItemType => GetFieldValue<string>("System.WorkItemType");

		public bool HasDescription => WorkItemType == "Product Backlog Item" || WorkItemType == "Task" || WorkItemType == "Impediment" || WorkItemType == "User Story";

		public bool HasDetails => WorkItemType == "Bug";

		public bool HasAcceptanceCriteria => WorkItemType == "Product Backlog Item" || WorkItemType == "User Story";

		public bool HasReproSteps => WorkItemType == "Bug";

		public string Url => WorkItem.Url;

		private T GetFieldValue<T>(string key)
			where T : class
		{
			return WorkItem.Fields.TryGetValue(key, out var result)
						   ? result as T
						   : default;
		}

		/// <summary>
		/// Specific GetFieldValue for the detail part of a bug
		/// Generic case would be key given (SystemInfo), but with some vsts templates its on key
		/// organization.Details which can be found in url.
		/// </summary>
		/// <typeparam name="T">return type</typeparam>
		/// <param name="key">Key to search</param>
		/// <returns>value</returns>
		private T GetDetailsFieldValue<T>(string key)
			where T : class
		{
			if (WorkItem.Fields.TryGetValue(key, out var result))
			{
				return result as T;
			}
			else
			{
				var organization = WorkItem.Url.Split('/')[3];
				var organizationBugDetailKey = organization + ".Details";

				return WorkItem.Fields.TryGetValue(organizationBugDetailKey, out var result2)
					? result2 as T : default;
			}
		}
	}
}
