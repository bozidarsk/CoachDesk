namespace CoachDesk.Services.Models;

public class SessionModel
{
	public Guid Id { set; get; }
	public DateTime Date { set; get; }
	public TimeSpan Duration { set; get; }
	public Guid CoachId { set; get; }
	public Guid ClientId { set; get; }
}
