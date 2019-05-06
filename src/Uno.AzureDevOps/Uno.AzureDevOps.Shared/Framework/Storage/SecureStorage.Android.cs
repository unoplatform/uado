#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Security;
using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using Uno.UI;
using Windows.Storage;

namespace Uno.AzureDevOps.Framework.Storage
{
	public partial class SecureStorage
	{
		partial void DeleteValue(string key, ref bool isDeleted)
		{
			try
			{
				using (var preferences = PreferenceManager.GetDefaultSharedPreferences(ContextHelper.Current))
				{
					if (preferences.Contains(key))
					{
						using (var editor = preferences.Edit())
						{
							editor.Remove(key);
							editor.Commit();
							isDeleted = true;
						}
					}
					else
					{
						isDeleted = false;
					}
				}

			}
			catch
			{
				isDeleted = false;
			}
		}

		partial void GetValue(string key, ref string value)
		{
			using (var preferences = PreferenceManager.GetDefaultSharedPreferences(ContextHelper.Current))
			{
				var storedValue = preferences.GetString(key, string.Empty);

				if (string.IsNullOrWhiteSpace(storedValue))
				{
					value = storedValue;
				}
				else
				{
					value = Encoding.UTF8.GetString(Convert.FromBase64String(storedValue));
				}
			}
		}

		partial void SetValue(string key, string value, ref bool isSet)
		{
			try
			{
				using (var preferences = PreferenceManager.GetDefaultSharedPreferences(ContextHelper.Current))
				{
					using (var editor = preferences.Edit())
					{
						var base64Value = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
						editor.PutString(key, base64Value);
						editor.Commit();
					}
				}

				isSet = true;
			}
			catch
			{
				isSet = false;
			}
		}
	}
}
#endif
