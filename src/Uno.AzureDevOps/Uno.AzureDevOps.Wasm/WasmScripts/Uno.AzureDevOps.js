var Uno;
(function (Uno) {
    var AzureDevOps;
    (function (AzureDevOps) {
        class Auth {
            static launch(htmlId, url) {
                // Register event handler for postMessage()
                const eventListener = (msgEvt) => {
                    const data = msgEvt.data;
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
                        const error = `Message with origin ${msgEvt.origin}ignored. Only origin ${window.location.origin} is allowed.`;
                        console.error(error);
                        return;
                    }
                    var securityTokensEvent = new CustomEvent("urlwithsecuritytokens", {
                        detail: data
                    });
                    element.dispatchEvent(securityTokensEvent);
                    msgEvt.source.close();
                    // unsubscribe
                    window.removeEventListener("message", eventListener);
                };
                window.addEventListener("message", eventListener);
                return "ok";
            }
        }
        AzureDevOps.Auth = Auth;
    })(AzureDevOps = Uno.AzureDevOps || (Uno.AzureDevOps = {}));
})(Uno || (Uno = {}));
