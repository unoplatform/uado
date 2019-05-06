using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uno.AzureDevOps.Framework.Http;
using Uno.Extensions;

namespace Uno.AzureDevOps.Client
{
	public class ADOOrganizationClient : IADOOrganizationClient
	{
		private readonly IHttpRequestService _httpRequestService;

		public ADOOrganizationClient(IHttpRequestService httpRequestService)
		{
			_httpRequestService = httpRequestService ?? throw new ArgumentNullException(nameof(httpRequestService));
		}

		public async Task<List<AccountData>> GetOrganizations(string accessToken, string memberId, CancellationToken ct = default)
		{
			using (var builder = _httpRequestService.RequestBuilderFactory.Create())
			{
				var accountResponseData = await builder
					.SetDefaultADORequestValues(accessToken)
					.QueryParameter("memberId", memberId)
					.QueryParameter("includeOwner", "true")
					.Get<ADOCollectionData<AccountData>>("_apis/Accounts", ct);

				return accountResponseData.Value;
			}
		}
	}
}
