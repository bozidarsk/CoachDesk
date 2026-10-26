using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.Data.Common;
using CoachDesk.Data.Models;
using CoachDesk.Services.Core;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICoachService, CoachService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMembershipPlanService, MembershipPlanService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IWorkPeriodService, WorkPeriodService>();
builder.Services.AddScoped<ISearchService, SearchService>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
	{
		options.SignIn.RequireConfirmedAccount = false;
		options.Password.RequiredLength = EntityValidation.User.PasswordMinLength;
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = false;
		options.Password.RequireUppercase = false;
		options.Password.RequireNonAlphanumeric = false;
		options.User.RequireUniqueEmail = true;
	}
)
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
	options.LogoutPath = "/Account/Logout";
	options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		foreach (var role in new[] { CoachRole, ClientRole })
			if (!await roleManager.RoleExistsAsync(role))
				await roleManager.CreateAsync(new IdentityRole(role));
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
).WithStaticAssets();

app.Run();
