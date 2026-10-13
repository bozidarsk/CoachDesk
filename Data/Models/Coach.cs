using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachDesk.Data.Models;

public class Coach
{
	[Key]
	public int Id { set; get; }

	[Required, ForeignKey(nameof(UserCredentials))]
	public int UserCredentialsId { set; get; }
	public virtual UserCredentials UserCredentials { set; get; } = null!;

	public virtual ICollection<Session> BookedSessions { set; get; } = [];

	public virtual ICollection<Membership> Memberships { set; get; } = [];
}
