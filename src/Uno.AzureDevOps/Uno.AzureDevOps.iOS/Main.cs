using UIKit;

namespace Uno.AzureDevOps.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		public static void Main(string[] args)
		{
			// This is required to change the default behavior of the public Uno.UI
			// which uses UWP styles by default.
			// This code needs to be executed prior here to anything else, otherwise its value
			// will be evaluated before we get the chance to change it.
			Uno.UI.FeatureConfiguration.Style.UseUWPDefaultStyles = false;

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, typeof(App));
		}
	}
}