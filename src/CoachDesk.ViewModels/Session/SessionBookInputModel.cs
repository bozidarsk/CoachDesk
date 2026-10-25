using System.ComponentModel.DataAnnotations;

namespace CoachDesk.ViewModels.Session;

public class SessionBookInputModel
{
	[Required]
	public Guid CoachId { set; get; }

	[Required]
	public Guid ClientId { set; get; }

	[Required]
	public DateTime Date { set; get; }

	[Required]
	public TimeSpan Duration { set; get; }
}
