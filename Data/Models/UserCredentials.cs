using System.ComponentModel.DataAnnotations;

namespace CoachDesk.Data.Models;

public class UserCredentials
{
	[Key]
	public int Id { set; get; }

	[Required]
	public string Username { set; get; } = null!;

	[Required]
	public ulong PasswordHash { set; get; }

	[Required, EmailAddress]
	public string Email { set; get; } = null!;

	[Required]
	public string Name { set; get; } = null!;
}
