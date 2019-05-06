using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class CoreAttributeDescriptor
	{
		[JsonProperty("attributeName")]
		public string AttributeName { get; set; }

		[JsonProperty("containerName")]
		public string ContainerName { get; set; }
	}
}
