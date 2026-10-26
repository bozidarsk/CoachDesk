namespace CoachDesk.ViewModels.Account;

public class AccountIndexViewModel
{
	public Guid ProfileId { set; get; }
	public string ProfileController { set; get; } = null!;
	public string Email { set; get; } = null!;
	public DateTime MemberSince { set; get; }

	public AccountUpdateProfileInputModel UpdateProfile { set; get; } = new();
	public AccountChangeEmailInputModel ChangeEmail { set; get; } = new();
	public AccountChangePasswordInputModel ChangePassword { set; get; } = new();
	public AccountDeleteInputModel Delete { set; get; } = new();
}
