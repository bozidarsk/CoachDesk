namespace CoachDesk.ViewModels.Coach;

public class CoachClientRowViewModel
{
	public Guid ClientId { get; set; }
	public string FullName { get; set; } = "";
	public string? ActivePlanName { get; set; }
	public DateTime? MembershipExpiresOn { get; set; }
	public int SessionsUsed { get; set; }
	public int? SessionsIncluded { get; set; }
	public DateTime? NextSession { get; set; }
}
