using System;

namespace Uno.AzureDevOps.Client
{
	public class WebApiTeam
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string Description { get; set; }

		public string IdentityUrl { get; set; }

		public string ProjectName { get; set; }

		public Guid ProjectId { get; set; }
	}
}
