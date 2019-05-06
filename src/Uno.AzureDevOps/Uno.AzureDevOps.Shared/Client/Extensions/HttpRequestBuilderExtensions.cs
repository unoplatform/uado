using Uno.AzureDevOps.Framework.Http;

namespace Uno.AzureDevOps.Client
{
	public static partial class HttpRequestBuilderExtensions
	{
		public static IHttpRequestBuilder SetDefaultADORequestValues(this IHttpRequestBuilder builder, string accessToken)
		{
			return builder
				.QueryParameter("api-version", "5.0")
				.Header("Authorization", "Bearer " + accessToken);
		}
	}
}
