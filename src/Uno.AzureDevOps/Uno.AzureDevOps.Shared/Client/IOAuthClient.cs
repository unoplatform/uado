using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Client
{
	public interface IOAuthClient
	{
		Task<OAuthResponseData> GetAuthData(string authenticationCode, CancellationToken ct = default);

		Task<OAuthResponseData> RefreshToken(string refreshToken, CancellationToken ct = default);
	}
}
