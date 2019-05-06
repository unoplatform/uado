using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Client
{
	public interface IADOOrganizationClient
	{
		Task<List<AccountData>> GetOrganizations(string accessToken, string memberId, CancellationToken ct = default);
	}
}
