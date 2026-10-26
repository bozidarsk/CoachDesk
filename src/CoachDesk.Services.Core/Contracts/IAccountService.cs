namespace CoachDesk.Services.Core.Contracts;

public interface IAccountService
{
	bool IsEmailValid(string email);
	bool IsPasswordValid(string password);
}
