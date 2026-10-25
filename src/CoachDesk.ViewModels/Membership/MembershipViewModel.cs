namespace CoachDesk.ViewModels.Membership;

public class MembershipViewModel
{
	public Guid Id { get; set; }
	public string PlanName { get; set; } = null!;
	public Guid CoachId { get; set; }
	public string CoachName { get; set; } = null!;
	public int? SessionsIncluded { get; set; }
	public int SessionsUsed { get; set; }
	public DateTime? ExpiresOn { get; set; }
	public string? Status { set; get; }
}
