using Newtonsoft.Json;

namespace Uno.AzureDevOps.Client
{
	public class OAuthErrorData
	{
		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }

		[JsonProperty("error_codes")]
		public int[] ErrorCodes { get; set; }

		[JsonProperty("timestamp")]
		public string Timestamp { get; set; }

		[JsonProperty("trace_id")]
		public string TraceId { get; set; }

		[JsonProperty("correlation_id")]
		public string CorrelationId { get; set; }
	}
}
