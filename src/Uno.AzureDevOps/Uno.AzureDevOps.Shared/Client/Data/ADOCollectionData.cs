using System.Collections.Generic;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class ADOCollectionData<T>
	{
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("value")]
		public List<T> Value { get; set; }
	}
}
