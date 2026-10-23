using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachDesk.Data.Models;

public class Membership
{
	[Key]
	public Guid Id { set; get; }

	[Required]
	public DateTime ActivatedOn { set; get; }

	public int TimesUsed { set; get; } = 0;

	[Required, ForeignKey(nameof(MembershipPlan))]
	public Guid MembershipPlanId { set; get; }
	public virtual MembershipPlan MembershipPlan { set; get; } = null!;

	[Required, ForeignKey(nameof(Client))]
	public Guid ClientId { set; get; }
	public virtual Client Client { set; get; } = null!;
}
