using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Coach;
using CoachDesk.ViewModels.Membership;
using CoachDesk.ViewModels.MembershipPlan;
using CoachDesk.ViewModels.Session;
using CoachDesk.ViewModels.WorkPeriod;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Controllers;

[Authorize(Roles = CoachRole)]
public class CoachController : Controller
{
	private readonly ApplicationDbContext dbContext;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	[AllowAnonymous]
	public async Task<IActionResult> Index(Guid? id)
	{
		var coach = await dbContext.Coaches.Include(x => x.User).SingleOrDefaultAsync(x => x.Id == id);

		if (coach == null)
			return BadRequest();

		bool isLoggedIn = false;
		bool ownerViewing = false;
		bool clientViewing = false;
		Guid? clientId = null;

		if (signInManager.IsSignedIn(User))
		{
			var user = (await userManager.GetUserAsync(User))!;

			isLoggedIn = true;

			if (await userManager.IsInRoleAsync(user, CoachRole))
			{
				var currentCoach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

				ownerViewing = coach.Id == currentCoach.Id;
			}
			else if (await userManager.IsInRoleAsync(user, ClientRole))
			{
				var currentClient = await dbContext.Clients.SingleAsync(x => x.UserId == user.Id);

				clientViewing = true;
				clientId = currentClient.Id;
			}
		}

		var initials = coach.User.FullName
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Select(x => char.ToUpper(x[0]))
			.ToList()
		;

		return View(
			new CoachIndexViewModel()
			{
				Id = coach.Id,
				FullName = coach.User.FullName,
				IsLoggedIn = isLoggedIn,
				OwnerViewing = ownerViewing,
				ClientViewing = clientViewing,
				Bio = coach.User.Bio,
				MemberSince = coach.User.MemberSince,
				Initials = $"{initials[0]}{initials[^1]}",
				MembershipPlans = await GetAllMembershipPlans(coachId: coach.Id, clientId: clientId),
				ActiveClients = (await GetAllActiveClients(coach.Id)).Count,
				UpcomingSessions = await GetAllUpcomingSessions(coachId: coach.Id, clientId: null),
				WorkPeriods = await GetAllWorkPeriods(coach.Id),
				ViewingClientId = clientId,
				ViewingClientMembershipStatus = (clientId != null) ? GetMembershipStatus(await dbContext.Memberships.Include(x => x.MembershipPlan).SingleOrDefaultAsync(x => x.ClientId == clientId && x.MembershipPlan.CoachId == coach.Id)) : null,
				ViewingClientUpcomingSessions = clientViewing ? await GetAllUpcomingSessions(coachId: coach.Id, clientId: clientId) : null,
			}
		);
	}

	public async Task<IActionResult> Dashboard()
	{
		var user = (await userManager.GetUserAsync(User))!;
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var workPeriods = await GetAllWorkPeriods(coach.Id);

		return View(
			new CoachDashboardViewModel()
			{
				CoachId = coach.Id,
				FullName = user.FullName,
				WeeklyHours = workPeriods.Sum(x => x.Duration.TotalHours),
				Clients = await GetAllActiveClients(coach.Id),
				MembershipPlans = await GetAllMembershipPlans(coach.Id, null),
				UpcomingSessions = await GetAllUpcomingSessions(coachId: coach.Id, clientId: null),
				WorkPeriods = workPeriods,
			}
		);
	}

	private async Task<IReadOnlyList<CoachClientRowViewModel>> GetAllActiveClients(Guid coachId) => await dbContext.Clients
		.Where(x => x.ActiveMemberships.Any(x => x.MembershipPlan.CoachId == coachId))
		.Select(x => new { Client = x, Membership = x.ActiveMemberships.FirstOrDefault(x => x.MembershipPlan.CoachId == coachId) })
		.Select(x =>
			new CoachClientRowViewModel()
			{
				ClientId = x.Client.Id,
				FullName = x.Client.User.FullName,
				ActivePlanName = (x.Membership != null) ? x.Membership.MembershipPlan.Name : null,
				MembershipExpiresOn = (x.Membership != null && x.Membership.MembershipPlan.Duration != null) ? x.Membership.ActivatedOn.AddDays((int)x.Membership.MembershipPlan.Duration) : null,
				SessionsUsed = (x.Membership != null) ? x.Membership.TimesUsed : 0,
				SessionsIncluded = (x.Membership != null) ? x.Membership.MembershipPlan.Count : null,
			}
		)
		.ToListAsync()
	;

	private async Task<IReadOnlyList<MembershipPlanViewModel>> GetAllMembershipPlans(Guid coachId, Guid? clientId)
	{
		var result = await dbContext.MembershipPlans
			.Where(x => x.CoachId == coachId)
			.Select(x =>
				new MembershipPlanViewModel()
				{
					Id = x.Id,
					Name = x.Name,
					Price = x.Price,
					Duration = x.Duration,
					Count = x.Count,
					ActiveClients = x.ActiveMemberships.Count,
				}
			)
			.ToListAsync()
		;

		for (int i = 0; i < result.Count; i++)
			result[i].MembershipStatus = (clientId != null)
				? GetMembershipStatus(await dbContext.Memberships.Include(x => x.MembershipPlan).SingleOrDefaultAsync(x => x.MembershipPlanId == result[i].Id && x.ClientId == clientId && x.MembershipPlan.CoachId == coachId))
				: MembershipStatus.Unknown
			;

		return result;
	}

	private async Task<IReadOnlyList<SessionViewModel>> GetAllUpcomingSessions(Guid coachId, Guid? clientId) => await dbContext.Sessions
		.Where(x => ((clientId == null) ? x.CoachId == coachId : x.CoachId == coachId && x.ClientId == clientId) && x.Date >= DateTime.UtcNow)
		.Select(x =>
			new SessionViewModel()
			{
				Id = x.Id,
				CoachFullName = x.Coach.User.FullName,
				ClientFullName = x.Client.User.FullName,
				Date = x.Date,
				Duration = x.Duration,
				CanCancel = true,
			}
		)
		.ToListAsync()
	;

	private async Task<IReadOnlyList<WorkPeriodViewModel>> GetAllWorkPeriods(Guid coachId) => await dbContext.WorkPeriods
		.Where(x => x.CoachId == coachId)
		.Select(x =>
			new WorkPeriodViewModel()
			{
				Id = x.Id,
				DayOfWeek = x.DayOfWeek,
				Start = x.Start,
				End = x.End,
			}
		)
		.ToListAsync()
	;

	private static MembershipStatus GetMembershipStatus(Membership? membership)
	{
		if (membership == null)
			return MembershipStatus.Unknown;

		if ((membership.MembershipPlan.Duration != null) ? membership.ActivatedOn.AddDays((int)membership.MembershipPlan.Duration) <= DateTime.UtcNow : false)
			return MembershipStatus.Expired;

		if ((membership.MembershipPlan.Count != null) ? membership.TimesUsed >= membership.MembershipPlan.Count : false)
			return MembershipStatus.OutOfSessions;

		return MembershipStatus.Active;
	}

	public CoachController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		this.dbContext = dbContext;
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
