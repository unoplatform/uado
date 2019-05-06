using System;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class ProfileData
	{
		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("publicAlias")]

		public string PublicAlias { get; set; }

		[JsonProperty("emailAddress")]

		public string EmailAddress { get; set; }

		[JsonProperty("coreRevision")]

		public int CoreRevision { get; set; }

		[JsonProperty("timeStamp")]

		public DateTime TimeStamp { get; set; }

		[JsonProperty("id")]

		public string Id { get; set; }

		[JsonProperty("Revision")]
		public int Revision { get; set; }

		[JsonProperty("coreAttributes")]
		public CoreAttributeCollectionData CoreAttributes { get; set; }
	}
}
