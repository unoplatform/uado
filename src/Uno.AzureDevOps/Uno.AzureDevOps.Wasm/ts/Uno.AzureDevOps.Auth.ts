namespace Uno.AzureDevOps {
	export class Auth {
		public static launch(htmlId: number, url: string) {

			// Register event handler for postMessage()
			const eventListener = (msgEvt: MessageEvent) => {
				const data = <string>msgEvt.data;

				if (typeof data !== "string") {
					return;
				}

				if (data.indexOf("|") < 0) {
					return;
				}

				const element = document.getElementById(`${htmlId}`);
				if (!element) {
					window.removeEventListener("message", eventListener);
					return;
				}

				if (msgEvt.origin !== window.location.origin) {
					const error =
						`Message with origin ${msgEvt.origin}ignored. Only origin ${window.location.origin
							} is allowed.`;
					console.error(error);
					return;
				}

				var securityTokensEvent = new CustomEvent("urlwithsecuritytokens",
					{
						detail: data
					});

				element.dispatchEvent(securityTokensEvent);

				(msgEvt.source as Window).close();

				// unsubscribe
				window.removeEventListener("message", eventListener);
			};
			window.addEventListener("message", eventListener);

			return "ok";
		}
	}
}
