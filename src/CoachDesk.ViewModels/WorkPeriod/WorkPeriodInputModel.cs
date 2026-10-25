using System.ComponentModel.DataAnnotations;

namespace CoachDesk.ViewModels.WorkPeriod;

public class WorkPeriodInputModel
{
	[Required]
	public DayOfWeek DayOfWeek { set; get; }

	[Required]
	public TimeOnly Start { set; get; }

	[Required]
	public TimeOnly End { set; get; }
}
