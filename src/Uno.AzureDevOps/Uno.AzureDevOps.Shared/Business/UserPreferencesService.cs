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

		public bool HasPreferredAccountName()
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

		public string GetPreferredAccountName()
		{
			return _secureStorage.GetValue<string>(AccountStorageKey);
		}

		public void SavePreferredAccountName(string accountName)
		{
			if (!HasPreferredAccountName())
			{
				_secureStorage.SetValue(AccountStorageKey, accountName);
			}
			else
			{
				var savedAccountName = _secureStorage.GetValue<string>(AccountStorageKey);
				if (savedAccountName != accountName)
				{
					_secureStorage.Delete(AccountStorageKey);
					_secureStorage.SetValue(AccountStorageKey, accountName);
				}
			}
		}
	}
}
