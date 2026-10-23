using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoachDesk.Data;

public class ApplicationUser : IdentityUser
{
	[Required]
	public string FullName { set; get; } = null!;

	[Required]
	public DateTime MemberSince { set; get; }

	public string? Bio { set; get; } = null;
}
