using System;
using System.Collections.Generic;
using System.Text;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Business
{
	public interface IUserPreferencesService
	{
		/// <summary>
		/// Returns if a preferred project exists for the current user
		/// </summary>
		/// <returns>boolean</returns>
		bool HasPreferredProject();

		/// <summary>
		/// Returns if a saved account exists for the current user
		/// </summary>
		/// <returns>boolean</returns>
		bool HasPreferredAccount();

		/// <summary>
		/// Save the preferredProject.
		/// If one already exists remove it and save the new one.
		/// </summary>
		/// <param name="projectId">The Guid of the selected proejct</param>
		void SavePreferredProject(Guid projectId);

		/// <summary>
		/// Save the preferredAccountData
		/// If one already exists, remove it and save the new one
		/// </summary>
		/// <param name="accountData"></param>
		void SavePreferredAccount(AccountData accountData);

		/// <summary>
		/// Get & parses the stored string projectId to Guid
		/// </summary>
		/// <param name="projectGuid"></param>
		/// <returns>boolean</returns>
		bool TryGetPreferredProjectId(out Guid projectId);

		AccountData GetPreferredAccount();
	}
}
