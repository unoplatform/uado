namespace Uno.AzureDevOps.Client
{
	public class WorkItemLink
	{
		public string Rel { get; set; }

		public WorkItemReference Source { get; set; }

		public WorkItemReference Target { get; set; }
	}
}
