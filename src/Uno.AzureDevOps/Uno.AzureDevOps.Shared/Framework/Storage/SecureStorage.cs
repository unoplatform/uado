using System;
using Newtonsoft.Json;

namespace Uno.AzureDevOps.Framework.Storage
{
	public partial class SecureStorage : ISecureStorage
	{
		private const string UadoResourceId = "Uno.AzureDevOps";

		public bool Delete(string key)
		{
			ValidateKey(key);

			var isDeleted = false;

			DeleteValue(key, ref isDeleted);

			return isDeleted;
		}

		public T GetValue<T>(string key)
		{
			ValidateKey(key);

			var retrievedValue = string.Empty;

			GetValue(key, ref retrievedValue);

			if (string.IsNullOrWhiteSpace(retrievedValue))
			{
				return default;
			}

			return JsonConvert.DeserializeObject<T>(retrievedValue);
		}

		public bool HasKey(string key)
		{
			return GetValue<dynamic>(key) != null;
		}

		public bool SetValue<T>(string key, T value)
			where T : class
		{
			ValidateKey(key);

			var serializedValue = JsonConvert.SerializeObject(value);
			var isSet = false;

			SetValue(key, serializedValue, ref isSet);

			return isSet;
		}

		private void ValidateKey(string key)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				throw new ArgumentException("Please provide a non empty storage key", nameof(key));
			}
		}

		partial void DeleteValue(string key, ref bool isDeleted);

		partial void GetValue(string key, ref string value);

		partial void SetValue(string key, string value, ref bool isSet);
	}
}
