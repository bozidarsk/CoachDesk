using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachDesk.Data.Models;

public class MembershipPlan
{
	[Key]
	public int Id { set; get; }

	[Required]
	public string Name { set; get; } = null!;

	[DataType(DataType.Currency), Column(TypeName = "decimal(19,4)")]
	public decimal Price { set; get; }

	public int? Count { set; get; }

	public TimeSpan? Duration { set; get; }
}
