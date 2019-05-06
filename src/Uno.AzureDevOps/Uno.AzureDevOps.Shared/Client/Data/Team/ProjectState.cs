namespace Uno.AzureDevOps.Client
{
	public enum ProjectState
	{
		Unchanged = -2,
		All = -1,
		New = 0,
		WellFormed = 1,
		Deleting = 2,
		CreatePending = 3,
		Deleted = 4
	}
}
