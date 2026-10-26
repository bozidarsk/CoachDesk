using CoachDesk.ViewModels.Membership;
using CoachDesk.ViewModels.Session;

namespace CoachDesk.ViewModels.Client;

public class ClientIndexViewModel
{
	public Guid Id { set; get; }
	public string FullName { set; get; } = null!;
	public string? Bio { set; get; }
	public DateTime MemberSince { set; get; }
	public string Initials { set; get; } = null!;

	public bool IsLoggedIn { set; get; }
	public bool OwnerViewing { set; get; }
	public bool CoachOfClientViewing { set; get; }

	public IReadOnlyList<MembershipViewModel> Memberships { get; set; } = [];
	public IReadOnlyList<SessionViewModel> UpcomingSessions { get; set; } = [];
}
