using System;
using System.Collections.Generic;
using System.Text;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Storage;

namespace Uno.AzureDevOps.Business
{
	///<summary>
	/// Holds all logic linked with user preferences (i.e locally saved data)
	///</summary>
	public partial class UserPreferencesService : IUserPreferencesService
	{
		private const string ProjectStorageKey = "Uado.SavedProject";
		private const string AccountStorageKey = "Uado.SavedAccount";

		private readonly ISecureStorage _secureStorage;

		public UserPreferencesService(ISecureStorage secureStorage)
		{
			_secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
		}

		public bool HasPreferredProject()
		{
			return _secureStorage.HasKey(ProjectStorageKey);
		}

		public bool HasPreferredAccount()
		{
			return _secureStorage.HasKey(AccountStorageKey);
		}

		public void SavePreferredProject(Guid projectId)
		{
			if (!HasPreferredProject())
			{
				_secureStorage.SetValue(ProjectStorageKey, projectId.ToString());
			}
			else
			{
				var savedProjectId = _secureStorage.GetValue<string>(ProjectStorageKey);
				if (savedProjectId != projectId.ToString())
				{
					_secureStorage.Delete(ProjectStorageKey);
					_secureStorage.SetValue(ProjectStorageKey, projectId.ToString());
				}
			}
		}

		public bool TryGetPreferredProjectId(out Guid projectGuid)
		{
			return Guid.TryParse(_secureStorage.GetValue<string>(ProjectStorageKey), out projectGuid);
		}

		public AccountData GetPreferredAccount()
		{
			return _secureStorage.GetValue<AccountData>(AccountStorageKey);
		}

		public void SavePreferredAccount(AccountData accountData)
		{
			if (!HasPreferredAccount())
			{
				_secureStorage.SetValue(AccountStorageKey, accountData);
			}
			else
			{
				var savedAccount = _secureStorage.GetValue<AccountData>(AccountStorageKey);
				if (savedAccount != accountData)
				{
					_secureStorage.Delete(AccountStorageKey);
					_secureStorage.SetValue(AccountStorageKey, accountData);
				}
			}
		}
	}
}
