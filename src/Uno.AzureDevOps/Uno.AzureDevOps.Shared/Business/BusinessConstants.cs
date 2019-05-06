using System.Diagnostics.CodeAnalysis;

namespace Uno.AzureDevOps
{
	[SuppressMessage("Nesting types", "CA1034", Justification = "Wanted behavior")]
	public static class BusinessConstants
	{
		public static class DoubleBack
		{
			public const string SettingsKey = "DoubleBackShown";
			public const int MinimumDepth = 1;
		}

		public static class WorkItemRelationType
		{
			public const string Parent = "System.LinkTypes.Hierarchy-Reverse";
			public const string Child = "System.LinkTypes.Hierarchy-Forward";
			public const string Attachment = "AttachedFile";
		}
	}
}
