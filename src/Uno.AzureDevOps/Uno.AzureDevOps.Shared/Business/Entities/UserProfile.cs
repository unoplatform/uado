using System.IO;

namespace Uno.AzureDevOps
{
	public class UserProfile
	{
		public string Name { get; set; }

		public string Email { get; set; }

		public Stream Image { get; set; }
	}
}
