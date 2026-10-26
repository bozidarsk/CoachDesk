namespace CoachDesk.Common;

public static class ApplicationConstants
{
	public const string CoachRole = "Coach";
	public const string ClientRole = "Client";
	public const string CoachOrClientRole = $"{CoachRole},{ClientRole}";
	public const int SearchCoachesPageSize = 5;
}
