using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Uno.AzureDevOps.Presentation
{
	[SuppressMessage("Nesting types", "CA1034", Justification = "Wanted behavior")]
	public static class PresentationConstants
	{
		public static class RelatedWorkItem
		{
			// Determines the maximum number of related items to show in the details page
			public const int MaxItems = 6;
		}
	}
}
