using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Uno.AzureDevOps.Framework.Http;
using Uno.Extensions;

namespace Uno.AzureDevOps.Client
{
	public class AzureADOAuthClient : IOAuthClient
	{
		private readonly ImmutableDictionary<string, string> _authenticationRequestBody;

		private readonly ImmutableDictionary<string, string> _refreshTokenRequestBody;

		private readonly IHttpRequestService _httpRequestService;

		public AzureADOAuthClient(IHttpRequestService httpRequestService, IApplicationContext applicationContext)
		{
			_httpRequestService = httpRequestService ?? throw new ArgumentNullException(nameof(httpRequestService));

			var baseRequestBody = new Dictionary<string, string>
			{
				{ "client_assertion_type", ClientConstants.ClientAssertionType },
				{ "client_assertion", applicationContext.AuthApplicationSecret },
				{ "redirect_uri", applicationContext.AuthRedirectUrl }
			}.ToImmutableDictionary();

			_authenticationRequestBody = baseRequestBody.Add("grant_type", ClientConstants.AuthorizationCodeGrantType);
			_refreshTokenRequestBody = baseRequestBody.Add("grant_type", ClientConstants.RefreshTokenGrantType);
		}

		public async Task<OAuthResponseData> GetAuthData(string authenticationCode, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var responseStream = await builder
					.RequestBody(_authenticationRequestBody.Add("assertion", authenticationCode))
					.GetResponseStream(ClientConstants.TokenEndpoint, HttpMethod.Post, ct);
				return GetResponse(responseStream);
			}
		}

		public async Task<OAuthResponseData> RefreshToken(string refreshToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var responseStream = await builder
					.RequestBody(_refreshTokenRequestBody.Add("assertion", refreshToken))
					.GetResponseStream(ClientConstants.TokenEndpoint, HttpMethod.Post, ct);
				return GetResponse(responseStream);
			}
		}

		private OAuthResponseData GetResponse(Stream httpResponse)
		{
			try
			{
				var response = httpResponse.ReadToEnd();

				return new OAuthResponseData()
				{
					Auth = JsonConvert.DeserializeObject<OAuthData>(response),
				};
			}
			catch (WebException ex)
			{
				using (var errorResponse = ex.Response)
				{
					var response = errorResponse.GetResponseStream().ReadToEnd();

					return new OAuthResponseData()
					{
						ErrorData = JsonConvert.DeserializeObject<OAuthErrorData>(response),
					};
				}
			}
			catch (Exception ex)
			{
				return new OAuthResponseData()
				{
					ErrorData = new OAuthErrorData()
					{
						Error = ex.Message,
					},
				};
			}
		}
	}
}
