using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using static CoachDesk.Common.EntityValidation.MembershipPlan;

namespace CoachDesk.Data.Models;

public class MembershipPlan
{
	[Key]
	public Guid Id { set; get; }

	[Required, MinLength(NameMinLength), MaxLength(NameMaxLength)]
	public string Name { set; get; } = null!;

	[DataType(DataType.Currency), Column(TypeName = "decimal(19,4)")]
	public decimal Price { set; get; }

	public int? Count { set; get; }

	public int? Duration { set; get; }

	[ForeignKey(nameof(Coach))]
	public Guid? CoachId { set; get; }
	[DeleteBehavior(DeleteBehavior.SetNull)]
	public virtual Coach? Coach { set; get; }
}
