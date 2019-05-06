using System.Collections.Generic;

namespace Uno.AzureDevOps.Client
{
	public class WorkItemTypeFieldInstance : WorkItemFieldReference
	{
		public WorkItemFieldReference Field { get; set; }

		public string HelpText { get; set; }

		public bool AlwaysRequired { get; set; }

		public IEnumerable<WorkItemFieldReference> DependentFields { get; set; }

		public bool IsIdentity { get; set; }

		public string DefaultValue { get; set; }

		public string[] AllowedValues { get; set; }
	}
}
