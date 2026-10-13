using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoachDesk.Data.Models;

public class Session
{
	[Key]
	public int Id { set; get; }

	[Required]
	public DateTime Date { set; get; }

	[Required]
	public TimeOnly Duration { set; get; }

	[Required, ForeignKey(nameof(Coach))]
	public int CoachId { set; get; }
	[DeleteBehavior(DeleteBehavior.NoAction)]
	public virtual Coach Coach { set; get; } = null!;

	[Required, ForeignKey(nameof(Client))]
	public int ClientId { set; get; }
	[DeleteBehavior(DeleteBehavior.NoAction)]
	public virtual Client Client { set; get; } = null!;
}
