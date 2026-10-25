using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachDesk.Data.Models;

public class WorkPeriod
{
	[Key]
	public Guid Id { set; get; }

	[Required]
	public DayOfWeek DayOfWeek { set; get; }

	[Required]
	public TimeOnly Start { set; get; }

	[Required]
	public TimeOnly End { set; get; }

	[Required, ForeignKey(nameof(Coach))]
	public Guid CoachId { set; get; }
	public virtual Coach Coach { set; get; } = null!;
}
