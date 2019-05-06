#if NETFX_CORE
using Windows.Security.Credentials;

namespace Uno.AzureDevOps.Framework.Storage
{
	public partial class SecureStorage
	{
		private readonly PasswordVault _passwordVault = new PasswordVault();

		partial void DeleteValue(string key, ref bool isDeleted)
		{
			var credential = _passwordVault.Retrieve(UadoResourceId, key);

			if (credential != null)
			{
				_passwordVault.Remove(credential);

				isDeleted = true;
			}
			else
			{
				isDeleted = false;
			}
		}

		partial void GetValue(string key, ref string value)
		{
			try
			{
				var credential = _passwordVault.Retrieve(UadoResourceId, key);

				credential.RetrievePassword();

				value = credential?.Password;
			}
			catch
			{
				value = string.Empty;
			}
		}

		partial void SetValue(string key, string value, ref bool isSet)
		{
			try
			{
				_passwordVault.Add(new PasswordCredential()
				{
					Password = value,
					Resource = UadoResourceId,
					UserName = key
				});

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
