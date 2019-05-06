namespace Uno.AzureDevOps.Client
{
	public interface IApplicationContext
	{
		string BaseUrl { get; }

		string AuthApplicationId { get; }

		string AuthApplicationSecret { get; }

		string AuthRedirectUrl { get; }

		string AuthScopes { get; }
	}
}