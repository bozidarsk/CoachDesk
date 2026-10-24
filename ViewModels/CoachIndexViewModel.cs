namespace CoachDesk.ViewModels;

public class CoachIndexViewModel
{
	public Guid Id { set; get; }
	public string FullName { set; get; } = null!;
	public string? Bio { set; get; }
	public DateTime MemberSince { set; get; }
	public string Initials { set; get; } = null!;

	public int ActiveClients { set; get; }
	public IReadOnlyList<MembershipPlanViewModel> MembershipPlans { set; get; } = null!;

	public bool IsLoggedIn { set; get; }
	public bool OwnerViewing { set; get; }
	public bool ClientViewing { set; get; }

	public Guid? ViewingClientId { set; get; }
	public IReadOnlyList<SessionViewModel>? ViewingClientUpcomingSessions { set; get; }

	public IReadOnlyList<SessionViewModel> UpcomingSessions { set; get; } = null!;
	public IReadOnlyList<WorkPeriodViewModel> WorkPeriods { set; get; } = null!;
}
