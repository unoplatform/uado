using System;
using Newtonsoft.Json;
using Uno.Serialization;

namespace Uno.AzureDevOps.Client
{
	[Windows.UI.Xaml.Data.Bindable]
	public class AccountData
	{
		[JsonProperty("accountId")]
		public string AccountId { get; set; }

		[JsonProperty("accountUri")]
		public string AccountUri { get; set; }

		[JsonProperty("accountName")]
		public string AccountName { get; set; }
	}
}
