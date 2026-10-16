using System.ComponentModel.DataAnnotations;

using static CoachDesk.Common.EntityValidation.User;

namespace CoachDesk.ViewModels;

public class RegisterInputModel
{
	[Required, EmailAddress, MinLength(EmailMinLength), MaxLength(EmailMaxLength)]
	public string Email { set; get; } = null!;

	[Required, MaxLength(FullNameMaxLength), Display(Name = "Full Name")]
	public string FullName { set; get; } = null!;

	[Required, DataType(DataType.Password), MinLength(PasswordMinLength), MaxLength(PasswordMaxLength)]
	public string Password { set; get; } = null!;

	[Display(Name = "Register as a Coach")]
	public bool RegisterAsCoach { set; get; } = false;

	[Display(Name = "Stay logged in")]
	public bool StayLoggedIn { set; get; } = false;
}
