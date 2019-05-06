using System.Collections.Generic;
using System.Collections.Immutable;

namespace Uno.AzureDevOps.Client
{
	public static class ClientConstants
	{
		public const string AzureDevOpsRepositoryPath = "https://dev.azure.com/";
		public const string BaseAzureDevOpsApiUrl = "https://app.vssps.visualstudio.com/";

		public const string AuthorizationCodeGrantType = "urn:ietf:params:oauth:grant-type:jwt-bearer";
		public const string AuthorizationResponseType = "Assertion";
		public const string AuthorizationEndpoint = "oauth2/authorize";
		public const string ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
		public const string RefreshTokenGrantType = "refresh_token";
		public const string TokenEndpoint = "oauth2/token";

		public const string BaseAuthorizationUrl = BaseAzureDevOpsApiUrl + AuthorizationEndpoint;

		public static readonly IDictionary<string, (string applicationId, string clientSecret, string redirectUrl, string scopes)> ApplicationRegistrations =
			new Dictionary<string, (string, string, string, string)>
			{
				{
					// Uado (Wasm Prod + others)
					// https://app.vsaex.visualstudio.com/app/view?clientId=4937f798-d0ba-47e6-a614-dc83beda919b (nv-devops)
					"uado.platform.uno", (
						"4937F798-D0BA-47E6-A614-DC83BEDA919B",
#if DEBUG
						"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiIzNDA5MWZhZC1iZDVhLTQ5MWUtYTIyOS1iYzg4ZjliODViMzEiLCJjc2kiOiJhZDAyYjk3Zi01YTZmLTQ3NzQtYTE2NC0wOWMzZDU1ZjQ1ZDEiLCJuYW1laWQiOiIyZDdmNmRmNC1iMzE0LTYzMzUtOTUwYy1iYjgyM2U0NzkwMzEiLCJpc3MiOiJhcHAudnN0b2tlbi52aXN1YWxzdHVkaW8uY29tIiwiYXVkIjoiYXBwLnZzdG9rZW4udmlzdWFsc3R1ZGlvLmNvbSIsIm5iZiI6MTU1NjU2NDM5NCwiZXhwIjoxNzE0NDE3MTk0fQ.1ADPCHLXJgj9ALHX7Zam1FeBMlFOF6QpVDI3sxIe7FRc4KSHpblZzV0yJ7AkHa8UbkYSktA0uEF_DX4V7RnUt-Er7m-0wqF7RAkPR92R3HR79o47nt8_gyknggKskvbaKiUyNAoHsrIAuMV3v1PPKvnBw5dj-zqp2mAgYolW9I128_yyRuC2PvaYug8EjCbDg8F4GZ15J8ZxIqUj9itEPY8H017vG9RgjlTHqbiAc8sZVNjCWiyrxDM73u0dhhA2iYOFBkESrwO-euJN_7q3QLm5UAY7fJnwnAvqoU4leib9qrD1aMC3FiP0ZVpoDgCe9sy2sFdArX-sAJMyoWegOg",
#else
						"--production-secret--",
#endif
						"https://uado.platform.uno/logincompleted.html",
						"vso.build vso.code vso.connected_server vso.dashboards vso.extension vso.graph vso.identity vso.notification_manage vso.packaging vso.project vso.release vso.symbols vso.wiki vso.work_write"
					)
				},
#if __WASM__
				{
					// Uado - Dev
					// https://app.vsaex.visualstudio.com/app/view?clientId=34091fad-bd5a-491e-a229-bc88f9b85b31 (nv-devops)
					"localhost", (
						"34091FAD-BD5A-491E-A229-BC88F9B85B31",
						"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiIzNDA5MWZhZC1iZDVhLTQ5MWUtYTIyOS1iYzg4ZjliODViMzEiLCJjc2kiOiJhZDAyYjk3Zi01YTZmLTQ3NzQtYTE2NC0wOWMzZDU1ZjQ1ZDEiLCJuYW1laWQiOiIyZDdmNmRmNC1iMzE0LTYzMzUtOTUwYy1iYjgyM2U0NzkwMzEiLCJpc3MiOiJhcHAudnN0b2tlbi52aXN1YWxzdHVkaW8uY29tIiwiYXVkIjoiYXBwLnZzdG9rZW4udmlzdWFsc3R1ZGlvLmNvbSIsIm5iZiI6MTU1NjU2NDM5NCwiZXhwIjoxNzE0NDE3MTk0fQ.1ADPCHLXJgj9ALHX7Zam1FeBMlFOF6QpVDI3sxIe7FRc4KSHpblZzV0yJ7AkHa8UbkYSktA0uEF_DX4V7RnUt-Er7m-0wqF7RAkPR92R3HR79o47nt8_gyknggKskvbaKiUyNAoHsrIAuMV3v1PPKvnBw5dj-zqp2mAgYolW9I128_yyRuC2PvaYug8EjCbDg8F4GZ15J8ZxIqUj9itEPY8H017vG9RgjlTHqbiAc8sZVNjCWiyrxDM73u0dhhA2iYOFBkESrwO-euJN_7q3QLm5UAY7fJnwnAvqoU4leib9qrD1aMC3FiP0ZVpoDgCe9sy2sFdArX-sAJMyoWegOg",
						"https://localhost:44306/logincompleted.html",
						"vso.build vso.code vso.connected_server vso.dashboards vso.extension vso.graph vso.identity vso.notification_manage vso.packaging vso.project vso.release vso.symbols vso.wiki vso.work_write"
					)
				},
				{
					// Uado - Staging
					// https://app.vsaex.visualstudio.com/app/view?clientId=4937f798-d0ba-47e6-a614-dc83beda919b (nv-devops)
					"uno-azuredevops-staging.azurewebsites.net", (
						"8326FEDF-8996-4B07-BE85-01F6DDFE42F2",
						"--staging-secret--",
						"https://uno-azuredevops-staging.azurewebsites.net/logincompleted.html",
						"vso.build vso.code vso.connected_server vso.dashboards vso.extension vso.graph vso.identity vso.notification_manage vso.packaging vso.project vso.release vso.symbols vso.wiki vso.work_write"
					)
				},
#endif
			};
	}
}
