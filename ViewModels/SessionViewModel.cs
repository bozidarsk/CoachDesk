namespace CoachDesk.ViewModels;

public class SessionViewModel
{
	public Guid Id { get; set; }
	public string CoachFullName { set; get; } = null!;
	public string ClientFullName { set; get; } = null!;
	public DateTime Date { set; get; }
	public TimeSpan Duration { set; get; }
	public bool CanCancel { get; set; }
}
