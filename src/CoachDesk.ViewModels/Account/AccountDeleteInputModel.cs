using System.ComponentModel.DataAnnotations;

namespace CoachDesk.ViewModels.Account;

public class AccountDeleteInputModel
{
	[Required, DataType(DataType.Password)]
	public string Password { set; get; } = null!;

	public bool Confirm { set; get; }
}
