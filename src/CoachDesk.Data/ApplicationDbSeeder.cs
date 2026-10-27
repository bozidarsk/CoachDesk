using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CoachDesk.Data.Models;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Data;

public static class ApplicationDbSeeder
{
	public const string DevPassword = "1234567890";

	public static async Task SeedRolesAsync(IServiceProvider services)
	{
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

		foreach (var role in new[] { CoachRole, ClientRole })
			if (!await roleManager.RoleExistsAsync(role))
				await roleManager.CreateAsync(new IdentityRole(role));
	}

	public static async Task SeedDemoDataAsync(IServiceProvider services)
	{
		var dbContext = services.GetRequiredService<ApplicationDbContext>();
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

		if (await dbContext.Coaches.AnyAsync() || await dbContext.Clients.AnyAsync())
			return;

		async Task<string> CreateUser(string email, string fullName, string? bio, string role)
		{
			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true,
				FullName = fullName,
				Bio = bio
			};

			var result = await userManager.CreateAsync(user, DevPassword);

			if (!result.Succeeded)
				throw new InvalidOperationException($"Could not seed {email}: " + string.Join("; ", result.Errors.Select(e => e.Description)));

			await userManager.AddToRoleAsync(user, role);

			return user.Id;
		}

		async Task<Coach> AddCoach(string email, string fullName, string? bio)
		{
			var coach = new Coach { UserId = await CreateUser(email, fullName, bio, CoachRole) };

			dbContext.Coaches.Add(coach);

			return coach;
		}

		async Task<Client> AddClient(string email, string fullName, string? bio = null)
		{
			var client = new Client { UserId = await CreateUser(email, fullName, bio, ClientRole) };

			dbContext.Clients.Add(client);

			return client;
		}

		void Hours(Coach coach, DayOfWeek day, int from, int to) =>
			dbContext.WorkPeriods.Add(
				new WorkPeriod()
				{
					Coach = coach,
					DayOfWeek = day,
					Start = new TimeOnly(from, 0),
					End = new TimeOnly(to, 0),
				}
			)
		;

		MembershipPlan Offer(Coach coach, string name, decimal price, int? sessions, int? days)
		{
			var plan = new MembershipPlan()
			{
				Name = name,
				Price = price,
				Count = sessions,
				Duration = days,
			};

			coach.MembershipPlans.Add(plan);

			return plan;
		}

		void Buy(Client client, MembershipPlan plan, int activatedDaysAgo, int timesUsed) =>
			client.ActiveMemberships.Add(
				new Membership()
				{
					MembershipPlan = plan,
					ActivatedOn = DateTime.Today.AddDays(-activatedDaysAgo),
					TimesUsed = timesUsed,
				}
			)
		;

		void Book(Coach coach, Client client, DateTime at) =>
			dbContext.Sessions.Add(
				new Session()
				{
					Coach = coach,
					Client = client,
					Date = at,
					Duration = TimeSpan.FromHours(1),
				}
			)
		;


		var marta = await AddCoach(
			"marta.keller@coachdesk.test",
			"Marta Keller",
			"Strength and conditioning coach. I teach beginners the big lifts safely and help experienced lifters get past plateaus."
		);

		Hours(marta, DayOfWeek.Monday, 9, 13);
		Hours(marta, DayOfWeek.Wednesday, 14, 18);
		Hours(marta, DayOfWeek.Friday, 9, 13);

		var martaStarter = Offer(marta, "Starter, 4 sessions", 80m, 4, 30);
		var martaMonthly = Offer(marta, "Monthly, 12 sessions", 210m, 12, 30);

		Offer(marta, "Unlimited month", 320m, null, 30);

		var daniel = await AddCoach(
			"daniel.okafor@coachdesk.test",
			"Daniel Okafor",
			"Yoga and mobility. Calm sessions for stiff hips and desk-bound backs."
		);

		Hours(daniel, DayOfWeek.Tuesday, 8, 12);
		Hours(daniel, DayOfWeek.Thursday, 8, 12);
		Hours(daniel, DayOfWeek.Thursday, 16, 20);
		Hours(daniel, DayOfWeek.Saturday, 10, 14);

		var danielSingle = Offer(daniel, "Single session", 25m, 1, null);
		var danielPass = Offer(daniel, "10-session pass", 220m, 10, 90);

		var sofia = await AddCoach("sofia.lindqvist@coachdesk.test", "Sofia Lindqvist", null);

		foreach (var day in new[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday })
			Hours(sofia, day, 17, 20);

		var sofiaCheckIn = Offer(sofia, "Weekly check-in, 8 sessions", 160m, 8, 60);
		var sofiaPack = Offer(sofia, "Open-ended 20-pack", 380m, 20, null);

		var alex = await AddClient("alex.novak@coachdesk.test", "Alex Novak", "Getting back into training after a long break.");

		Buy(alex, martaMonthly, activatedDaysAgo: 10, timesUsed: 3);
		Book(marta, alex, Past(DayOfWeek.Monday, 10));
		Book(marta, alex, Upcoming(DayOfWeek.Monday, 10));
		Book(marta, alex, Upcoming(DayOfWeek.Wednesday, 15));
		Buy(alex, danielSingle, activatedDaysAgo: 12, timesUsed: 1);
		Book(daniel, alex, Past(DayOfWeek.Saturday, 10));

		var jamie = await AddClient("jamie.reyes@coachdesk.test", "Jamie Reyes");

		Buy(jamie, martaStarter, activatedDaysAgo: 27, timesUsed: 2);
		Book(marta, jamie, Past(DayOfWeek.Wednesday, 14));
		Book(marta, jamie, Past(DayOfWeek.Friday, 9));
		Buy(jamie, sofiaPack, activatedDaysAgo: 20, timesUsed: 0);

		var priya = await AddClient("priya.nair@coachdesk.test", "Priya Nair", "Yoga, mostly for my back.");

		Buy(priya, danielPass, activatedDaysAgo: 14, timesUsed: 4);
		Book(daniel, priya, Past(DayOfWeek.Tuesday, 9));
		Book(daniel, priya, Past(DayOfWeek.Thursday, 8));
		Book(daniel, priya, Upcoming(DayOfWeek.Tuesday, 8));
		Book(daniel, priya, Upcoming(DayOfWeek.Thursday, 16));

		var tomas = await AddClient("tomas.berg@coachdesk.test", "Tomas Berg");

		Buy(tomas, sofiaCheckIn, activatedDaysAgo: 75, timesUsed: 5);

		for (var weeksBack = 3; weeksBack <= 7; weeksBack++)
			Book(sofia, tomas, Past(DayOfWeek.Monday, 17).AddDays(-7 * weeksBack));

		await AddClient("nina.costa@coachdesk.test", "Nina Costa");

		await dbContext.SaveChangesAsync();
	}

	private static DateTime Upcoming(DayOfWeek day, int hour)
	{
		var daysAhead = ((int)day - (int)DateTime.Today.DayOfWeek + 7) % 7;

		if (daysAhead == 0)
			daysAhead = 7;

		return DateTime.Today
			.AddDays(daysAhead)
			.AddHours(hour)
		;
	}

	private static DateTime Past(DayOfWeek day, int hour)
	{
		var daysAgo = ((int)DateTime.Today.DayOfWeek - (int)day + 7) % 7;

		if (daysAgo == 0)
			daysAgo = 7;

		return DateTime.Today
			.AddDays(-daysAgo)
			.AddHours(hour)
		;
	}
}
