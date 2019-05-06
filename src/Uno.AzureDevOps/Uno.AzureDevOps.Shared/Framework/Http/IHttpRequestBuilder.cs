using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Uno.AzureDevOps.Framework.Http
{
	public interface IHttpRequestBuilder : IDisposable
	{
		/// <summary>
		/// This is the absolute base uri for the service
		/// </summary>
		Uri BaseUri { get; set; }

		/// <summary>
		/// This is the address of the call, relative to the BaseUri.
		/// </summary>
		/// <remarks>
		/// When set to an absolute Uri, the BaseUri will be ignored.
		/// Can contain Query string
		/// </remarks>
		Uri Address { get; set; }

		/// <summary>
		/// The ContentType of the request payload, when set as a stream
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// The headers of the request
		/// </summary>
		IDictionary<string, string> Headers { get; }

		/// <summary>
		/// The method used for the request
		/// </summary>
		HttpMethod HttpMethod { get; set; }

		/// <summary>
		/// The payload body of the request, if used.
		/// </summary>
		/// <remarks>
		/// Could be a string, a stream or a dictionary
		/// </remarks>
		object RequestBody { get; set; }

		/// <summary>
		/// Query parameters of the request.
		/// </summary>
		/// <remarks>
		/// Will be merged to Query of the Address property, if any.
		/// </remarks>
		IDictionary<string, string> QueryParameters { get; }

		/// <summary>
		/// The service owner of this builder
		/// </summary>
		IHttpRequestService Service { get; }

		/// <summary>
		/// Create a HttpRequestMessage from the current state of the builder
		/// </summary>
		HttpRequestMessage Build();
	}
}
