namespace CoachDesk.ViewModels;

public class CoachDashboardViewModel
{
	public Guid CoachId { set; get; }
	public string FullName { set; get; } = null!;
	public double WeeklyHours { set; get; }
	public IReadOnlyList<SessionViewModel> UpcomingSessions { set; get; } = null!;
	public IReadOnlyList<CoachClientRowViewModel> Clients { set; get; } = null!;
	public IReadOnlyList<WorkPeriodViewModel> WorkPeriods { set; get; } = null!;
	public IReadOnlyList<MembershipPlanViewModel> MembershipPlans { set; get; } = null!;
}
