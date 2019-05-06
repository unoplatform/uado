using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class WorkItem
	{
		public int? Id { get; set; }

		public int? Rev { get; set; }

		public string Url { get; set; }

		public IDictionary<string, object> Fields { get; set; }

		public IList<WorkItemRelation> Relations { get; set; }

		[JsonProperty("_links")]
		public ReferenceLinks Links { get; set; }
	}
}
