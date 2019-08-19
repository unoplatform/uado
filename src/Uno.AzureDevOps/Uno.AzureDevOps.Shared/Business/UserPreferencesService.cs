using System;
using System.Collections.Generic;
using System.Text;
using Uno.AzureDevOps.Business.VSTS;
using Uno.AzureDevOps.Client;
using Uno.AzureDevOps.Framework.Storage;

namespace Uno.AzureDevOps.Business
{
	public partial class UserPreferencesService : IUserPreferencesService
	{
		private const string ProjectStorageKey = "Uado.SavedProject";
		private const string AccountStorageKey = "Uado.SavedAccount";

		private readonly ISecureStorage _secureStorage;

		public UserPreferencesService(ISecureStorage secureStorage)
		{
			_secureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
		}

		/// <summary>
		/// Returns if a preferred project exists for the current user
		/// </summary>
		/// <returns></returns>
		public bool HasPreferredProject()
		{
			return _secureStorage.HasKey(ProjectStorageKey);
		}

		/// <summary>
		/// Returns if a saved account exists for the current user
		/// </summary>
		/// <returns></returns>
		public bool HasPreferredAccountName()
		{
			return _secureStorage.HasKey(AccountStorageKey);
		}

		/// <summary>
		/// Set the preferredProject.
		/// If one already exists remove it and save the new one.
		/// </summary>
		/// <param name="projectId"></param>
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

		/// <summary>
		/// Parses the stored string projectId to Guid
		/// </summary>
		/// <param name="projectGuid"></param>
		/// <returns></returns>
		public bool TryGetPreferredProjectId(out Guid projectGuid)
		{
			return Guid.TryParse(_secureStorage.GetValue<string>(ProjectStorageKey), out projectGuid);
		}

		public string GetPreferredAccountName()
		{
			return _secureStorage.GetValue<string>(AccountStorageKey);
		}

		/// <summary>
		/// Set the preferredAccountName
		/// If one already exists, remove it and save the new one
		/// </summary>
		/// <param name="accountName"></param>
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
