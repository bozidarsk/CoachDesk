namespace CoachDesk.Services.Models;

public class CoachModel
{
	public Guid Id;
	public string FullName { set; get; } = null!;
	public DateTime MemberSince { set; get; }
	public string? Bio { set; get; }
}
