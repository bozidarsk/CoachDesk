namespace CoachDesk.Services.Models;

public class WorkPeriodModel
{
	public Guid Id { set; get; }
	public DayOfWeek DayOfWeek { set; get; }
	public TimeOnly Start { set; get; }
	public TimeOnly End { set; get; }
	public Guid CoachId { set; get; }
}
