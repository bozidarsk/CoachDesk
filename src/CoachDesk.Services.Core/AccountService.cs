using System.Text.RegularExpressions;
using CoachDesk.Data;
using CoachDesk.Data.Models;
using CoachDesk.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static CoachDesk.Data.Common.EntityValidation.User;

namespace CoachDesk.Services.Core;

public class AccountService : IAccountService
{
	private readonly ApplicationDbContext dbContext;
	private readonly UserManager<ApplicationUser> userManager;

	public async Task ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword)
	{
		await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
	}

	public async Task SetUserNameAsync(ApplicationUser user, string userName)
	{
		await userManager.SetUserNameAsync(user, userName);
	}

	public async Task SetEmailAsync(ApplicationUser user, string email)
	{
		await userManager.SetEmailAsync(user, email);
	}

	public async Task SetFullNameAsync(ApplicationUser user, string fullName)
	{
		var found = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == user.Id);

		if (found != null)
		{
			found.FullName = fullName;

			await dbContext.SaveChangesAsync();
		}
	}

	public async Task SetBioAsync(ApplicationUser user, string? bio)
	{
		var found = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == user.Id);

		if (found != null)
		{
			found.Bio = bio;

			await dbContext.SaveChangesAsync();
		}
	}

	public bool IsEmailValid(string email)
	{
		return string.IsNullOrWhiteSpace(email) || Regex.IsMatch(email, "[a-zA-Z0-9_.]+@[a-zA-Z0-9_.]+");
	}

	public bool IsPasswordValid(string password)
	{
		if (string.IsNullOrEmpty(password))
			return false;

		if (password.Length < PasswordMinLength || password.Length > PasswordMaxLength)
			return false;

		if (!password.Any(x => char.IsDigit(x)))
			return false;

		return true;
	}

	public AccountService(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		this.dbContext = dbContext;
		this.userManager = userManager;
	}
}
