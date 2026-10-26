using CoachDesk.Data.Models;

namespace CoachDesk.Services.Core.Contracts;

public interface IAccountService
{
	Task ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword);
	Task SetUserNameAsync(ApplicationUser user, string password);
	Task SetEmailAsync(ApplicationUser user, string email);
	Task SetFullNameAsync(ApplicationUser user, string fullName);
	Task SetBioAsync(ApplicationUser user, string? bio);
	bool IsEmailValid(string email);
	bool IsPasswordValid(string password);
}
