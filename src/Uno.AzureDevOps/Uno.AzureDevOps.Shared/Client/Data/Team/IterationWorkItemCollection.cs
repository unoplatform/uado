using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class IterationWorkItemCollection
	{
		public string Url { get; set; }

		[JsonProperty("_links")]
		public ReferenceLinks Links { get; set; }

		public IEnumerable<WorkItemLink> WorkItemRelations { get; set; }
	}
}
