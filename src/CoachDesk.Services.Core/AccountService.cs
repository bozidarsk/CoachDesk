using System.Text.RegularExpressions;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Data.Common.EntityValidation.User;

namespace CoachDesk.Services.Core;

public class AccountService : IAccountService
{
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
}
