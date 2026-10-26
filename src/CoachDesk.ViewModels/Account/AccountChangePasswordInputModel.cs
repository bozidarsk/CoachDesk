using System.ComponentModel.DataAnnotations;
using static CoachDesk.Data.Common.EntityValidation.User;

namespace CoachDesk.ViewModels.Account;

public class AccountChangePasswordInputModel
{
	[Required, DataType(DataType.Password), Display(Name = "Current password")]
	public string CurrentPassword { set; get; } = null!;

	[Required, MinLength(PasswordMinLength), MaxLength(PasswordMaxLength), DataType(DataType.Password), Display(Name = "New password")]
	public string NewPassword { set; get; } = null!;
}
