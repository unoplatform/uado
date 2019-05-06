using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class TeamSetting
	{
		public TeamSettingsIteration BacklogIteration { get; set; }

		public BugsBehavior BugsBehavior { get; set; }

		public DayOfWeek[] WorkingDays { get; set; }

		public IDictionary<string, bool> BacklogVisibilities { get; set; }

		public TeamSettingsIteration DefaultIteration { get; set; }

		public string DefaultIterationMacro { get; set; }

		public string Url { get; set; }

		[JsonProperty("_links")]
		public ReferenceLinks Links { get; set; }
	}
}
