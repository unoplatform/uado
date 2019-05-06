using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.Extensions;

namespace Uno.AzureDevOps.Framework.Http
{
	public static partial class HttpRequestBuilderExtensions
	{
		public static IHttpRequestBuilder ContentType(this IHttpRequestBuilder requestBuilder, string name)
		{
			requestBuilder.ContentType = name;

			return requestBuilder;
		}

		public static IHttpRequestBuilder Header(this IHttpRequestBuilder requestBuilder, string name, string value)
		{
			requestBuilder.Headers[name] = value;

			return requestBuilder;
		}

		public static IHttpRequestBuilder RequestBody(this IHttpRequestBuilder requestBuilder, Stream requestBody)
		{
			requestBuilder.RequestBody = requestBody;

			return requestBuilder;
		}

		public static IHttpRequestBuilder RequestBody(this IHttpRequestBuilder requestBuilder, IDictionary<string, string> requestBody)
		{
			requestBuilder.RequestBody = requestBody;

			return requestBuilder;
		}

		public static IHttpRequestBuilder RequestBody(this IHttpRequestBuilder requestBuilder, string requestBody, string contentType = null)
		{
			if (contentType != null)
			{
				requestBuilder.ContentType = contentType;
			}

			requestBuilder.RequestBody = requestBody;

			return requestBuilder;
		}

		public static IHttpRequestBuilder QueryParameter(this IHttpRequestBuilder requestBuilder, string name, string value)
		{
			requestBuilder.QueryParameters[name] = value;

			return requestBuilder;
		}

		public static IHttpRequestBuilder SetBaseUri(this IHttpRequestBuilder requestBuilder, string baseUri)
		{
			requestBuilder.BaseUri = new Uri(baseUri);

			return requestBuilder;
		}

		public static IHttpRequestBuilder SetBaseUri(this IHttpRequestBuilder requestBuilder, Uri baseUri)
		{
			requestBuilder.BaseUri = baseUri;

			return requestBuilder;
		}

		public static Task<T> Delete<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, HttpMethod.Delete, ct);
		}

		public static Task<T> Get<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, HttpMethod.Get, ct);
		}

		public static Task<T> Head<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, HttpMethod.Head, ct);
		}

		public static Task<T> Post<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, HttpMethod.Post, ct);
		}

		public static Task<T> Put<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, HttpMethod.Put, ct);
		}

		public static Task<T> Patch<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, CancellationToken ct = default)
		{
			return requestBuilder.Send<T>(actionUrl, new HttpMethod("PATCH"), ct);
		}

		public static async Task<Stream> GetResponseStream(this IHttpRequestBuilder requestBuilder, string actionUrl, HttpMethod method, CancellationToken ct)
		{
			if (!Uri.IsWellFormedUriString(actionUrl, UriKind.Relative))
			{
				throw new ArgumentException("Invalid request endpoint action", nameof(actionUrl));
			}

			if (actionUrl.StartsWith("/", StringComparison.OrdinalIgnoreCase) || actionUrl.Contains("?", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Please provide endpoint action url without parameters and without a starting /", nameof(actionUrl));
			}

			requestBuilder.Address =
				requestBuilder.Address == null
					? new Uri(actionUrl, UriKind.RelativeOrAbsolute)
					: new Uri(requestBuilder.Address, actionUrl);

			requestBuilder.HttpMethod = method;

			var stream = await requestBuilder.Service.SendRequest(requestBuilder, ct);
			return stream;
		}

		private static async Task<T> Send<T>(this IHttpRequestBuilder requestBuilder, string actionUrl, HttpMethod method, CancellationToken ct = default)
		{
			var stream = await GetResponseStream(requestBuilder, actionUrl, method, ct);

			return JsonConvert.DeserializeObject<T>(stream.ReadToEnd());
		}
	}
}
