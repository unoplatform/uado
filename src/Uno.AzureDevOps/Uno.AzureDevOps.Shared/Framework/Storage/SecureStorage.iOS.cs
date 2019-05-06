#if __IOS__
using Foundation;
using Security;

namespace Uno.AzureDevOps.Framework.Storage
{
	public partial class SecureStorage
	{
		partial void DeleteValue(string key, ref bool isDeleted)
		{
			var value = string.Empty;
			GetValue(key, ref value);

			if (string.IsNullOrWhiteSpace(value))
			{
				isDeleted = false;
			}
			else
			{
				var record = CreateRecord(key);
				SecKeyChain.Remove(record);

				isDeleted = true;
			}
		}

		partial void GetValue(string key, ref string value)
		{
			var query = CreateRecord(key);
			var statusCode = SecStatusCode.InvalidQuery;
			var storedValue = SecKeyChain.QueryAsRecord(query, out statusCode);

			if (statusCode == SecStatusCode.Success)
			{
				value = storedValue.ValueData.ToString();
			}
			else
			{
				value = string.Empty;
			}
		}

		partial void SetValue(string key, string value, ref bool isSet)
		{
			try
			{
				var newRecord = CreateRecord(key, value);
				var isDeleted = false;

				DeleteValue(key, ref isDeleted);

				var status = SecKeyChain.Add(newRecord);

				isSet = status == SecStatusCode.Success;
			}
			catch
			{
				isSet = false;
			}
		}

		private SecRecord CreateRecord(string key, string value = null)
		{
			var record = new SecRecord(SecKind.GenericPassword) { Account = key, };

			if (!string.IsNullOrWhiteSpace(value))
			{
				record.ValueData = NSData.FromString(value);
			}

			return record;
		}
	}
}
#endif
