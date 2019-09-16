namespace Uno.AzureDevOps {
	export class Auth {
		public static launch(htmlId: number, url: string) {

			const uadoTokenStorageKey = "__UadoToken";

			const cleanUp = () => {
				window.removeEventListener("message", eventListener);
				window.removeEventListener("storage", onStorageEvent);
				window.localStorage.removeItem(uadoTokenStorageKey);
			};

			const eventListener = (msgEvt: MessageEvent) => {
				if (msgEvt.origin !== window.location.origin) {
					const error =
						`Message with origin ${msgEvt.origin}ignored. Only origin ${window.location.origin
							} is allowed.`;
					console.error(error);
					return;
				}

				if (processData(msgEvt.data)) {

					try {
						// Try to close the other tab
						(msgEvt.source as Window).close();
					} catch (err) {
					}

					cleanUp();
				}
			};

			const onStorageEvent = (storageEvt: StorageEvent) => {
				if (storageEvt.key !== uadoTokenStorageKey) {
					return; // not a Uado Token
				}

				if (processData(storageEvt.newValue)) {
					cleanUp();
				}
			};

			const processData = (data: string): boolean => {

				if (typeof data !== "string") {
					return false;
				}

				if (data.indexOf("|") < 0) {
					return false;
				}

				const element = document.getElementById(`${htmlId}`);
				if (!element) {
					return false;
				}

				var securityTokensEvent = new CustomEvent("urlwithsecuritytokens",
					{
						detail: data
					});

				element.dispatchEvent(securityTokensEvent);

				return true;
			};

			// Listen to inter-tab "postMessage" event
			window.addEventListener("message", eventListener);

			// Listen to local storage
			window.addEventListener("storage", onStorageEvent, true);

			return "ok";
		}
	}
}
