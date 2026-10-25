using CoachDesk.ViewModels.WorkPeriod;

namespace CoachDesk.ViewModels.Session;

public class SessionBookViewModel
{
	public Guid CoachId { set; get; }
	public Guid ClientId { set; get; }
	public string? ClientMembershipStatus { set; get; }
	public IReadOnlyList<SessionViewModel> CoachUpcomingSessions { set; get; } = null!;
	public IReadOnlyList<WorkPeriodViewModel> CoachWorkPeriods { set; get; } = null!;
}
