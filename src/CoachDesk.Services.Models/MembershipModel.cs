namespace CoachDesk.Services.Models;

public class MembershipModel
{
	public Guid Id { set; get; }
	public DateTime ActivatedOn { set; get; }
	public int TimesUsed { set; get; }
	public Guid MembershipPlanId { set; get; }
	public Guid ClientId { set; get; }
	public string? Status { set; get; }
}
