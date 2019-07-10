using System;

namespace Uno.AzureDevOps.Client
{
	public class TeamIterationAttributes
	{
		public DateTimeOffset? StartDate { get; set; }

		public DateTimeOffset? FinishDate { get; set; }

		public TimeFrame? TimeFrame { get; set; }
	}
}
