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

#if DEBUG
						"76E4354C-BBC6-46D3-B294-49A27C89ACBA",
						"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiI3NmU0MzU0Yy1iYmM2LTQ2ZDMtYjI5NC00OWEyN2M4OWFjYmEiLCJjc2kiOiIxNTYyNTZkNC02NTExLTQxMzMtYmJkNS05M2Q0N2FmMjIxYmYiLCJuYW1laWQiOiIyZDdmNmRmNC1iMzE0LTYzMzUtOTUwYy1iYjgyM2U0NzkwMzEiLCJpc3MiOiJhcHAudnN0b2tlbi52aXN1YWxzdHVkaW8uY29tIiwiYXVkIjoiYXBwLnZzdG9rZW4udmlzdWFsc3R1ZGlvLmNvbSIsIm5iZiI6MTU1OTgzMjE1OSwiZXhwIjoxNzE3Njg0OTU5fQ.nxA8Zz710MVpNTac_ohHm9hhwRzqMjNy7KLR5vKKbycP-uRY85UvcaVFaOHKDPW1S6CC7AiOuHuvK4Aus0DBMYRPpxVkKiErdUCm_TAe8LQpZArZH6I0uoUYPoHiS4x1nJvdT92awIeThaCrtQJ0aXlJp0onuQ1iKj5opUYTUdQ1SUGzpq1Y79s7bhynnkVt3XXwrpVKbyLdxc-_XGkdMU6DEjbEzVPWQGoHDhy7BlMxLxNej15hGHfNfclyovBw5qo69C0wMjZCjg8N7M5tE98RiOzWLheOghJHQGz1GUYEXucYwGXqcAdk5sYsLL0dwcpGof-Si34OSMGphFT9pA",
						"https://localhost:44306/logincompleted.html",
#else
						"4937F798-D0BA-47E6-A614-DC83BEDA919B",
						"--production-secret--",
						"https://uado.platform.uno/logincompleted.html",
#endif
						"vso.build vso.code vso.connected_server vso.dashboards vso.extension vso.graph vso.identity vso.notification_manage vso.packaging vso.project vso.release vso.symbols vso.wiki vso.work_write"
					)
				},
#if __WASM__
				{
					// Uado - Dev
					// https://app.vsaex.visualstudio.com/app/view?clientId=34091fad-bd5a-491e-a229-bc88f9b85b31 (nv-devops)
					"localhost", (
						"76E4354C-BBC6-46D3-B294-49A27C89ACBA",
						"eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Im9PdmN6NU1fN3AtSGpJS2xGWHo5M3VfVjBabyJ9.eyJjaWQiOiI3NmU0MzU0Yy1iYmM2LTQ2ZDMtYjI5NC00OWEyN2M4OWFjYmEiLCJjc2kiOiIxNTYyNTZkNC02NTExLTQxMzMtYmJkNS05M2Q0N2FmMjIxYmYiLCJuYW1laWQiOiIyZDdmNmRmNC1iMzE0LTYzMzUtOTUwYy1iYjgyM2U0NzkwMzEiLCJpc3MiOiJhcHAudnN0b2tlbi52aXN1YWxzdHVkaW8uY29tIiwiYXVkIjoiYXBwLnZzdG9rZW4udmlzdWFsc3R1ZGlvLmNvbSIsIm5iZiI6MTU1OTgzMjE1OSwiZXhwIjoxNzE3Njg0OTU5fQ.nxA8Zz710MVpNTac_ohHm9hhwRzqMjNy7KLR5vKKbycP-uRY85UvcaVFaOHKDPW1S6CC7AiOuHuvK4Aus0DBMYRPpxVkKiErdUCm_TAe8LQpZArZH6I0uoUYPoHiS4x1nJvdT92awIeThaCrtQJ0aXlJp0onuQ1iKj5opUYTUdQ1SUGzpq1Y79s7bhynnkVt3XXwrpVKbyLdxc-_XGkdMU6DEjbEzVPWQGoHDhy7BlMxLxNej15hGHfNfclyovBw5qo69C0wMjZCjg8N7M5tE98RiOzWLheOghJHQGz1GUYEXucYwGXqcAdk5sYsLL0dwcpGof-Si34OSMGphFT9pA",
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
