var Uno;
(function (Uno) {
    var UI;
    (function (UI) {
        var UnoWebViewHandler = /** @class */ (function () {
            function UnoWebViewHandler() {
            }
            UnoWebViewHandler.log = function () {
                var message = "UnoWebViewHandler was logged";
                console.log(message);
                return message;
            };
            return UnoWebViewHandler;
        }());
        UI.UnoWebViewHandler = UnoWebViewHandler;
    })(UI = Uno.UI || (Uno.UI = {}));
})(Uno || (Uno = {}));
//# sourceMappingURL=UnoWebView.js.map