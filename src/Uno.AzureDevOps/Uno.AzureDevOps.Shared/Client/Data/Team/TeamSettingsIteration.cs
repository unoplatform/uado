using System;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class TeamSettingsIteration
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public TeamIterationAttributes Attributes { get; set; }

		public string Url { get; set; }

		[JsonProperty("_links")]
		public ReferenceLinks Links { get; set; }
	}
}
