using System;

namespace Uno.AzureDevOps.Client
{
	public class TeamIterationAttributes
	{
		public DateTime? StartDate { get; set; }

		public DateTime? FinishDate { get; set; }

		public TimeFrame? TimeFrame { get; set; }
	}
}
