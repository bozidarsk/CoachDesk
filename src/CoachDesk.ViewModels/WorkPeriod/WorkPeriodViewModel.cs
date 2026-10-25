namespace CoachDesk.ViewModels.WorkPeriod;

public class WorkPeriodViewModel
{
	public Guid Id { set; get; }
	public DayOfWeek DayOfWeek { set; get; }
	public TimeOnly Start { set; get; }
	public TimeOnly End { set; get; }

	public TimeSpan Duration => End - Start;
}
