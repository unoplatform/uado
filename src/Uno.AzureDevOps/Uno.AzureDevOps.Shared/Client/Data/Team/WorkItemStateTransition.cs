using System.Collections.Generic;

namespace Uno.AzureDevOps.Client
{
	public class WorkItemStateTransition
	{
		public string To { get; set; }

		public IEnumerable<string> Actions { get; set; }
	}
}
