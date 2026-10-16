using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CoachDesk.Data.Models;

namespace CoachDesk.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public virtual DbSet<Coach> Coaches { set; get; }
	public virtual DbSet<Client> Clients { set; get; }
	public virtual DbSet<Membership> Memberships { set; get; }
	public virtual DbSet<Session> Sessions { set; get; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
}
