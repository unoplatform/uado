using System.Collections.Generic;

namespace Uno.AzureDevOps.Client
{
	public class WorkItemRelation
	{
		public string Rel { get; set; }

		public string Url { get; set; }

		public string Title { get; set; }

		public IDictionary<string, object> Attributes { get; set; }
	}
}
