using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CoachDesk.Data.Models;

public class Client
{
	[Key]
	public Guid Id { set; get; }

	[ForeignKey(nameof(User))]
	public string? UserId { set; get; }
	[DeleteBehavior(DeleteBehavior.SetNull)]
	public virtual ApplicationUser? User { set; get; }

	public virtual ICollection<Session> BookedSessions { set; get; } = [];

	public virtual ICollection<Membership> ActiveMemberships { set; get; } = [];
}
