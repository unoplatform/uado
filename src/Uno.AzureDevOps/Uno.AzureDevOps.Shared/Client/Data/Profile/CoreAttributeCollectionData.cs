namespace Uno.AzureDevOps.Client
{
	public class CoreAttributeCollectionData
	{
		public CoreAttributeData<string> DisplayName { get; set; }

		public CoreAttributeData<string> PublicAlias { get; set; }

		public CoreAttributeData<string> EmailAddress { get; set; }

		public CoreAttributeData<string> UnconfirmedEmailAddress { get; set; }

		public CoreAttributeData<string> CountryName { get; set; }

		public CoreAttributeData<string> TermsOfServiceVersion { get; set; }

		public CoreAttributeData<string> TermsOfServiceAcceptDate { get; set; }

		public CoreAttributeData<ProfileAvatarData> Avatar { get; set; }

		public CoreAttributeData<string> ContactWithOffers { get; set; }
	}
}
