#if __WASM__
using System.Collections.Generic;

namespace Uno.AzureDevOps.Framework.Storage
{
	public partial class SecureStorage
	{
		private Dictionary<string, string> _storage = new Dictionary<string, string>();

		partial void DeleteValue(string key, ref bool isDeleted)
		{
			if (_storage.ContainsKey(key))
			{
				_storage.Remove(key);

				isDeleted = true;
			}
			else
			{
				isDeleted = false;
			}
		}

		partial void GetValue(string key, ref string value)
		{
			if (_storage.ContainsKey(key))
			{
				value = _storage[key];
			}
			else
			{
				value = string.Empty;
			}
		}

		partial void SetValue(string key, string value, ref bool isSet)
		{
			if (!_storage.ContainsKey(key))
			{
				_storage.Add(key, value);
			}

			_storage[key] = value;
			isSet = true;
		}
	}
}
#endif
