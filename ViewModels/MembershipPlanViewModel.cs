using System.ComponentModel.DataAnnotations;
using static CoachDesk.Common.EntityValidation.MembershipPlan;

namespace CoachDesk.ViewModels;

public class MembershipPlanViewModel
{
	[Required, MinLength(NameMinLength), MaxLength(NameMaxLength)]
	public string Name { set; get; } = null!;

	[DataType(DataType.Currency)]
	public decimal Price { set; get; }

	[Display(Name = "Number of sessions")]
	public int? Count { set; get; }

	[Display(Name = "Valid for (days)")]
	public int? Duration { set; get; }

	public Guid Id { set; get; }
	public int ActiveClients { set; get; }
	public bool OwnerViewing { set; get; }
	public bool ClientViewing { set; get; }
	public bool Active { set; get; }
}
