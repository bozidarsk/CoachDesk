using CoachDesk.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CoachDesk.Data;

public class ApplicationDbContext : DbContext
{
	public virtual DbSet<UserCredentials> UserCredentials { set; get; }
	public virtual DbSet<Coach> Coaches { set; get; }
	public virtual DbSet<Client> Clients { set; get; }
	public virtual DbSet<Membership> Memberships { set; get; }
	public virtual DbSet<Session> Sessions { set; get; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
}
