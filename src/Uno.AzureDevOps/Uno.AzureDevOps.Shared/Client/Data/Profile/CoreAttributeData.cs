using System;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class CoreAttributeData<T>
	{
		[JsonProperty("Descriptor")]
		public CoreAttributeDescriptor Descriptor { get; set; }

		[JsonProperty("value")]
		public T Value { get; set; }

		[JsonProperty("timeStamp")]
		public DateTimeOffset TimeStamp { get; set; }
	}
}
