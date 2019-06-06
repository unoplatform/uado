using System;
using System.Collections.Generic;
using System.Text;

namespace Uno.AzureDevOps.Client.Helpers
{
	public static class EncodingHelper
	{
		public static string Decode(string stringToDecode)
		{
			byte[] data = Convert.FromBase64String(stringToDecode);
			string decodedKey = Encoding.UTF8.GetString(data);
			return decodedKey;
		}
	}
}
