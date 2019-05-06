using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.AzureDevOps.Framework.Http;
using Uno.Extensions;

namespace Uno.AzureDevOps.Client
{
	public class ADOProfileClient : IADOProfileClient
	{
		private readonly IHttpRequestService _httpRequestService;

		public ADOProfileClient(IHttpRequestService httpRequestService)
		{
			_httpRequestService = httpRequestService ?? throw new ArgumentNullException(nameof(httpRequestService));
		}

		public async Task<ProfileData> GetProfile(string accessToken, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var response = await builder
					.SetDefaultADORequestValues(accessToken)
					.QueryParameter("details", "true")
					.Get<ProfileData>("_apis/profile/profiles/me", ct);

				return response;
			}
		}
	}
}
