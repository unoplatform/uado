using System;
using System.Collections.Generic;
using System.Linq;

namespace Uno.AzureDevOps.Client
{
	public class ApplicationContext : IApplicationContext
	{
		private readonly (string applicationId, string clientSecret, string redirectUrl, string scopes) _current;

		public ApplicationContext()
		{
#if __WASM__
			var hostname = Uno.Foundation.WebAssemblyRuntime.InvokeJS("(function(){ return window.location.hostname; })();")?.ToLowerInvariant();
			if (ClientConstants.ApplicationRegistrations.TryGetValue(hostname, out var current))
			{
				BaseUrl = hostname;
				_current = current;
			}
			else
			{
				Console.Error.WriteLine($"Context not found for hostname {hostname}.");
			}
#else
			(BaseUrl, _current) = ClientConstants.ApplicationRegistrations.Single();
#endif
		}

		public string BaseUrl { get; }

		public string AuthApplicationId => _current.applicationId;

		public string AuthApplicationSecret => _current.clientSecret;

		public string AuthRedirectUrl => _current.redirectUrl;

		public string AuthScopes => _current.scopes;
	}
}
