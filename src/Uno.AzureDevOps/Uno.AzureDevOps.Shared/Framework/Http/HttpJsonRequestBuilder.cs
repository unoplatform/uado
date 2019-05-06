using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Uno.AzureDevOps.Framework.Http
{
	public class HttpJsonRequestBuilder : IHttpRequestBuilder, IDisposable
	{
		private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

		private Uri _baseUri;

		public HttpJsonRequestBuilder(Uri baseUri, IHttpRequestService service, Dictionary<string, string> defaultParameters = null)
		{
			BaseUri = baseUri;
			Service = service;
			QueryParameters = defaultParameters ?? new Dictionary<string, string>();
		}

		public Uri BaseUri
		{
			get => _baseUri;
			set
			{
				if (!value.IsAbsoluteUri)
				{
					throw new InvalidOperationException("Base uri must be absolute.");
				}

				_baseUri = value;
			}
		}

		public IHttpRequestService Service { get; }

		public Uri Address { get; set; }

		public string ContentType { get; set; }

		public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

		public HttpMethod HttpMethod { get; set; }

		public object RequestBody { get; set; }

		public IDictionary<string, string> QueryParameters { get; }

		public HttpRequestMessage Build()
		{
			var finalUriWithParameters = BuildUri();

			var message = new HttpRequestMessage()
			{
				Method = HttpMethod,
				RequestUri = finalUriWithParameters
			};

			SetHeaders(message);
			SetContent(message);

			return message;
		}

		public void Dispose()
		{
			(RequestBody as IDisposable)?.Dispose();
		}

		private static Uri GetParameterizedUri(Uri baseUri, IDictionary<string, string> parameters)
		{
			if (parameters == null || parameters.Count == 0)
			{
				return baseUri;
			}

			var builder = new UriBuilder(baseUri);

			var queryString = HttpUtility.ParseQueryString(builder.Query);

			foreach (var parameter in parameters.Where(parameter => parameter.Value != null))
			{
				queryString[parameter.Key] = parameter.Value;
			}

			builder.Query = queryString.ToString();

			return builder.Uri;
		}

		private Uri BuildUri()
		{
			var finalUri = new Uri(BaseUri, Address);

			var parameterizedUri = QueryParameters.Any()
				? GetParameterizedUri(finalUri, QueryParameters)
				: finalUri;

			return parameterizedUri;
		}

		private void SetHeaders(HttpRequestMessage message)
		{
			foreach (var header in Headers)
			{
				if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value))
				{
					if (!message.Content.Headers.TryAddWithoutValidation(header.Key, header.Value))
					{
						throw new InvalidOperationException($"Unable to add header {header.Key}={header.Value})");
					}
				}
			}
		}

		private void SetContent(HttpRequestMessage message)
		{
			if (RequestBody != null)
			{
				if (RequestBody is Stream stream)
				{
					if (string.IsNullOrWhiteSpace(ContentType))
					{
						throw new InvalidOperationException("ContentType must be set.");
					}

					var streamContent = new StreamContent(stream);
					streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
					message.Content = streamContent;
				}
				else if (RequestBody is string str)
				{
					if (string.IsNullOrWhiteSpace(ContentType))
					{
						throw new InvalidOperationException("ContentType must be set.");
					}

					var streamContent = new StringContent(str, Utf8);
					streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(ContentType);
					message.Content = streamContent;
				}
				else if (RequestBody is IDictionary<string, string> dictionary)
				{
					var streamContent = new FormUrlEncodedContent(dictionary);
					message.Content = streamContent;
				}
				else
				{
					throw new InvalidOperationException($"Unsupported content type {RequestBody.GetType()}.");
				}
			}
			else
			{
				message.Content = null;
			}
		}
	}
}
