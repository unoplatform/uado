using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Framework.Http
{
	public class HttpRequestService : IHttpRequestService, IHttpRequestBuilderFactory
	{
		private readonly HttpClient _httpClient;
		private readonly HttpStatusCode[] _unauthorizedAccessResponseCodes;

		public HttpRequestService(HttpClient client, params HttpStatusCode[] unauthorizedAccessResponseCodes)
		{
			if (!Uri.IsWellFormedUriString(client.BaseAddress.OriginalString, UriKind.Absolute))
			{
				throw new ArgumentException(message: $"Invalid request url {client.BaseAddress}", paramName: nameof(client));
			}

			_httpClient = client ?? throw new ArgumentNullException(nameof(client));
			_unauthorizedAccessResponseCodes = unauthorizedAccessResponseCodes;
			BaseUri = client.BaseAddress;
		}

		public Uri BaseUri { get; set; }

		public IHttpRequestBuilderFactory RequestBuilderFactory => this;

		public async Task<Stream> SendRequest(IHttpRequestBuilder requestBuilder, CancellationToken ct = default)
		{
			var request = requestBuilder.Build();

			var httpResponse = await _httpClient.SendAsync(request, ct);

			if (_unauthorizedAccessResponseCodes?.Any(code => code == httpResponse.StatusCode) == true)
			{
				throw new UnauthorizedAccessException("Unauthorized request");
			}

			var responseStream = await httpResponse.Content
				.ReadAsStreamAsync()
				.ConfigureAwait(false);

			return responseStream;
		}

		public IHttpRequestBuilder Create()
		{
			return new HttpJsonRequestBuilder(BaseUri, this);
		}
	}
}
