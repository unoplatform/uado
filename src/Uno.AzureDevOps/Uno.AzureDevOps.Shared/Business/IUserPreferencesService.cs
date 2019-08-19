using System;
using System.Collections.Generic;
using System.Text;
using Uno.AzureDevOps.Client;

namespace Uno.AzureDevOps.Business
{
	public interface IUserPreferencesService
	{
		bool HasPreferredProject();

		bool HasPreferredAccountName();

		void SavePreferredProject(Guid projectId);

		void SavePreferredAccountName(string accountName);

		bool TryGetPreferredProjectId(out Guid projectId);

		string GetPreferredAccountName();
	}
}
