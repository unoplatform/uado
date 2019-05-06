using System;

namespace Uno.AzureDevOps.Client
{
	public class TeamProjectReference
	{
		public Guid Id { get; set; }

		public string Abbreviation { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }

		public ProjectState State { get; set; }

		public long Revision { get; set; }

		public ProjectVisibility Visibility { get; set; }

		public string DefaultTeamImageUrl { get; set; }
	}
}
