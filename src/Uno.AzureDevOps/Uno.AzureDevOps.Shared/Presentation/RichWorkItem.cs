using System;
using Uno.AzureDevOps.Business.Entities;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Presentation
{
	[Windows.UI.Xaml.Data.Bindable]
	public class RichWorkItem
	{
		public RichWorkItem(WorkItemData item, WorkItemType type, Guid projectId)
		{
			Item = item;
			Type = type;
			ProjectId = projectId;
		}

		public WorkItemData Item { get; }

		public WorkItemType Type { get; }

		public Guid ProjectId { get; }
	}
}
