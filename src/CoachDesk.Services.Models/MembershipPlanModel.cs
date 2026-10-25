namespace CoachDesk.Services.Models;

public class MembershipPlanModel
{
	public Guid Id { set; get; }
	public string Name { set; get; }
	public decimal Price { set; get; }
	public int? Count { set; get; }
	public int? Duration { set; get; }
	public Guid CoachId { set; get; }
}
