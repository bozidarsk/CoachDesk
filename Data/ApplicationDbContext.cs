using Microsoft.EntityFrameworkCore;

namespace CoachDesk.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
}
