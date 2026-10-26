using CoachDesk.ViewModels.WorkPeriod;
using CoachDesk.ViewModels.MembershipPlan;

namespace CoachDesk.ViewModels.Search;

public class SearchCoachesViewModel
{
	public Guid Id { set; get; }
	public string FullName { set; get; } = null!;
	public string Initials { set; get; } = null!;
	public string? Bio { set; get; }
	public int ActiveClients { set; get; }
	public IReadOnlyList<WorkPeriodViewModel> WorkPeriods { set; get; } = null!;
	public IReadOnlyList<MembershipPlanViewModel> MatchingPlans { set; get; } = null!;
}
