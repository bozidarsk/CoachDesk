using System.ComponentModel.DataAnnotations;

namespace CoachDesk.ViewModels.Account;

public class AccountUpdateProfileInputModel
{
	[Required, Display(Name = "Full name")]
	public string FullName { set; get; } = null!;

	public string? Bio { set; get; }
}
