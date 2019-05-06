using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class WorkItemType
	{
		public string Name { get; set; }

		public string ReferenceName { get; set; }

		public string Description { get; set; }

		public string Color { get; set; }

		public WorkItemIcon Icon { get; set; }

		public bool IsDisabled { get; set; }

		public string XmlForm { get; set; }

		public IEnumerable<WorkItemTypeFieldInstance> Fields { get; set; }

		public IEnumerable<WorkItemTypeFieldInstance> FieldInstances { get; set; }

		public IDictionary<string, WorkItemStateTransition[]> Transitions { get; set; }

		[JsonProperty("_links")]
		public ReferenceLinks Links { get; set; }
	}
}
