using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Uno.AzureDevOps.Framework.Http
{
	public interface IHttpRequestService
	{
		Uri BaseUri { get; set; }

		IHttpRequestBuilderFactory RequestBuilderFactory { get; }

		Task<Stream> SendRequest(IHttpRequestBuilder request, CancellationToken ct = default);
	}
}
