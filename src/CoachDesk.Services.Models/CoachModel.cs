namespace CoachDesk.Services.Models;

public class CoachModel
{
	public Guid Id;
	public string FullName { set; get; }
	public DateTime MemberSince { set; get; }
	public string? Bio { set; get; }
}
