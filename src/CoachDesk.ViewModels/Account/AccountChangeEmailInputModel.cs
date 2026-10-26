using System.ComponentModel.DataAnnotations;

namespace CoachDesk.ViewModels.Account;

public class AccountChangeEmailInputModel
{
	public string? CurrentEmail { set; get; }

	[Required, DataType(DataType.Password), Display(Name = "Current password")]
	public string EmailCurrentPassword { set; get; } = null!;

	[Required, EmailAddress, Display(Name = "New email")]
	public string NewEmail { set; get; } = null!;

}
