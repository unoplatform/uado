function getMobileOperatingSystem() {

    var userAgent = navigator.userAgent;

    if (/android/i.test(userAgent)) {
        return "Android";
    }

    // iOS detection from: http://stackoverflow.com/a/9039885/177710
    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        return "iOS";
    }

    return "unknown";
}


function RedirectToStores() {

    let os = getMobileOperatingSystem();
    let unoPlatform = "https://platform.uno/code-samples/";

    if (os == "Android") {
        if (confirm("Download the application for a better experience.")) {
            window.location.href = "https://play.app.goo.gl/?link=https://play.google.com/store/apps/details?id=uno.platform.azuredevops&ddl=1&pcampaignid=web_ddl_1";
        } else {
            window.location.href = unoPlatform;
        }
    } else if (os == "iOS") {
        if (confirm("Download the application for a better experience.")) {
            window.location.href = "https://apps.apple.com/fr/app/uado/id1472198775";
        } else {
            window.location.href = unoPlatform;
        }
    }
}

RedirectToStores();
