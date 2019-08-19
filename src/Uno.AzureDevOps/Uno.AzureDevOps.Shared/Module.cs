using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using GalaSoft.MvvmLight.Ioc;
using Uno.AzureDevOps.Business;
using Uno.AzureDevOps.Business.Authentication;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework;
using Uno.AzureDevOps.Framework.Http;
using Uno.AzureDevOps.Framework.Storage;

namespace Uno.AzureDevOps
{
	[SuppressMessage("", "CA1716", Justification = "Stylistic choice")]
	public static class Module
	{
		public static void Initialize(SimpleIoc serviceProvider)
		{
			var adoUnauthorizedCodes = new[] { HttpStatusCode.NonAuthoritativeInformation, HttpStatusCode.Unauthorized };
			var unoHttpClientHandler = new UnoHttpClientHandler();

			// iOS & wasm httpclient enabled by default
#if NETFX_CORE || __ANDROID__
			unoHttpClientHandler.AutomaticDecompression = DecompressionMethods.GZip;
#endif

			// Client wire-up
			serviceProvider.Register(() => new HttpClient(unoHttpClientHandler, false) { BaseAddress = new Uri(ClientConstants.BaseAzureDevOpsApiUrl) }, "VSSPS");

			serviceProvider.Register(() => new HttpClient(unoHttpClientHandler, false) { BaseAddress = new Uri(ClientConstants.AzureDevOpsRepositoryPath) }, "Repo");

			serviceProvider.Register<IHttpRequestService>(
				() => new HttpRequestService(
					serviceProvider.GetInstance<HttpClient>("VSSPS"),
					adoUnauthorizedCodes),
				"VSSPSService");

			serviceProvider.Register<IHttpRequestService>(
				() => new HttpRequestService(
					serviceProvider.GetInstance<HttpClient>("Repo"),
					adoUnauthorizedCodes),
				"RepoService");

			serviceProvider.Register<IApplicationContext>(() => new ApplicationContext());

			serviceProvider.Register<IOAuthClient>(
				() => new AzureADOAuthClient(
						serviceProvider.GetInstanceWithoutCaching<IHttpRequestService>("VSSPSService"),
						serviceProvider.GetInstanceWithoutCaching<IApplicationContext>()));

			serviceProvider.Register<IADOProfileClient>(() => new ADOProfileClient(serviceProvider.GetInstanceWithoutCaching<IHttpRequestService>("VSSPSService")));

			serviceProvider.Register<IADOOrganizationClient>(() => new ADOOrganizationClient(serviceProvider.GetInstanceWithoutCaching<IHttpRequestService>("VSSPSService")));

			serviceProvider.Register<IADOTeamClient>(() => new ADOTeamClient(serviceProvider.GetInstanceWithoutCaching<IHttpRequestService>("RepoService")));

			// Business wire-up
			serviceProvider.Register<ISecureStorage>(() => new SecureStorage());

			serviceProvider.Register<IAuthenticationService>(() => new AuthenticationService(
				serviceProvider.GetInstanceWithoutCaching<IOAuthClient>(),
				serviceProvider.GetInstance<ISecureStorage>()));

			serviceProvider.Register<IVSTSRepository>(() => new VSTSRepository(
				serviceProvider.GetInstanceWithoutCaching<IADOProfileClient>(),
				serviceProvider.GetInstanceWithoutCaching<IADOOrganizationClient>(),
				serviceProvider.GetInstanceWithoutCaching<IADOTeamClient>(),
				serviceProvider.GetInstance<IAuthenticationService>()));

			serviceProvider.Register<IUserPreferencesService>(() => new UserPreferencesService(
				serviceProvider.GetInstance<ISecureStorage>()));
		}
	}
}
