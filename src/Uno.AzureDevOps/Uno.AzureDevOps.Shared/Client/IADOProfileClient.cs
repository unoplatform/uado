using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Client
{
	public interface IADOProfileClient
	{
		Task<ProfileData> GetProfile(string accessToken, CancellationToken ct = default);
	}
}
