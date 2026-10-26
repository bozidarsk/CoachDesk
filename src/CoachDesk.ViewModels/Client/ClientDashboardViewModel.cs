using CoachDesk.ViewModels.Membership;
using CoachDesk.ViewModels.Session;

namespace CoachDesk.ViewModels.Client;

public class ClientDashboardViewModel
{
	public Guid ClientId { get; set; }
	public string FullName { get; set; } = null!;

	public SessionBookViewModel SessionBooking { set; get; } = null!;
	public IReadOnlyList<MembershipViewModel> Memberships { get; set; } = null!;
	public IReadOnlyList<SessionViewModel> UpcomingSessions { get; set; } = null!;
}
