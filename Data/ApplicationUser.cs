using Microsoft.AspNetCore.Identity;

namespace CoachDesk.Data;

public class ApplicationUser : IdentityUser
{
	public required string FullName { set; get; }
}
