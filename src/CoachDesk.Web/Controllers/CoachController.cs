using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Coach;
using CoachDesk.ViewModels.MembershipPlan;
using CoachDesk.ViewModels.Session;
using CoachDesk.ViewModels.WorkPeriod;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Web.Controllers;

[Authorize(Roles = CoachRole)]
public class CoachController : Controller
{
	private readonly ICoachService coachService;
	private readonly IClientService clientService;
	private readonly IMembershipPlanService membershipPlanService;
	private readonly IMembershipService membershipService;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	[AllowAnonymous]
	public async Task<IActionResult> Index(Guid? id)
	{
		var coach = await coachService.GetCoachAsync(id ?? default);

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
				var currentCoach = (await coachService.GetCoachAsync(user))!;

				ownerViewing = coach.Id == currentCoach.Id;
			}
			else if (await userManager.IsInRoleAsync(user, ClientRole))
			{
				var currentClient = (await clientService.GetClientAsync(user))!;

				clientViewing = true;
				clientId = currentClient.Id;
			}
		}

		var initials = coach.FullName
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Select(x => char.ToUpper(x[0]))
			.ToList()
		;

		var membershipPlans = (await coachService.GetMembershipPlansAsync(coach.Id))
			.Select(async x =>
				new MembershipPlanViewModel()
				{
					Id = x.Id,
					Name = x.Name,
					Price = x.Price,
					Count = x.Count,
					Duration = x.Duration,
					ActiveClients = await membershipPlanService.CountMembershipsAsync(x.Id),
					MembershipStatus = isLoggedIn ? (await membershipService.GetMembershipAsync(membershipPlanId: x.Id, coachId: coach.Id, clientId: clientId))?.Status : null,
					OwnerViewing = ownerViewing,
					ClientViewing = clientViewing,
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var upcomingSessions = (await coachService.GetUpcomingSessionsAsync(coach.Id, null))
			.Select(async x =>
				new SessionViewModel()
				{
					Id = x.Id,
					CoachFullName = (await coachService.GetCoachAsync(x.CoachId))!.FullName,
					ClientFullName = (await clientService.GetClientAsync(x.ClientId))!.FullName,
					Date = x.Date,
					Duration = x.Duration,
					CanCancel = true,
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var viewingClientUpcomingSessions = clientViewing
			? (await coachService.GetUpcomingSessionsAsync(coach.Id, clientId))
				.Select(async x =>
					new SessionViewModel()
					{
						Id = x.Id,
						CoachFullName = (await coachService.GetCoachAsync(x.CoachId))!.FullName,
						ClientFullName = (await clientService.GetClientAsync(x.ClientId))!.FullName,
						Date = x.Date,
						Duration = x.Duration,
						CanCancel = true,
					}
				)
				.Select(x => x.Result)
				.ToList()
			: null
		;

		var workPeriods = (await coachService.GetWorkPeriodsAsync(coach.Id))
			.Select(x =>
				new WorkPeriodViewModel()
				{
					Id = x.Id,
					DayOfWeek = x.DayOfWeek,
					Start = x.Start,
					End = x.End,
				}
			)
			.ToList()
		;

		return View(
			new CoachIndexViewModel()
			{
				Id = coach.Id,
				FullName = coach.FullName,
				IsLoggedIn = isLoggedIn,
				OwnerViewing = ownerViewing,
				ClientViewing = clientViewing,
				Bio = coach.Bio,
				MemberSince = coach.MemberSince,
				Initials = $"{initials[0]}{initials[^1]}",
				MembershipPlans = membershipPlans,
				ActiveClients = (await coachService.GetClientsAsync(coach.Id)).Count,
				UpcomingSessions = upcomingSessions,
				WorkPeriods = workPeriods,
				ViewingClientId = clientId,
				ViewingClientMembershipStatus = clientViewing ? (await membershipService.GetMembershipAsync(membershipPlanId: null, coachId: coach.Id, clientId: clientId))?.Status : null,
				ViewingClientUpcomingSessions = clientViewing ? viewingClientUpcomingSessions : null,
			}
		);
	}

	public async Task<IActionResult> Dashboard()
	{
		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var clients = (await coachService.GetClientsAsync(coach.Id))
			.Select(async x =>
				{
					var membership = await membershipService.GetMembershipAsync(membershipPlanId: null, coachId: coach.Id, clientId: x.Id);
					var membershipPlan = (membership != null) ? await membershipPlanService.GetMembershipPlanAsync(membership.MembershipPlanId) : null;

					return new CoachClientRowViewModel()
					{
						FullName = x.FullName,
						ClientId = x.Id,
						ActivePlanName = membershipPlan?.Name,
						MembershipExpiresOn = membership?.ActivatedOn.AddDays(membershipPlan?.Duration ?? 0),
						SessionsUsed = membership?.TimesUsed ?? 0,
						SessionsIncluded = membershipPlan?.Count,
						NextSession = (await coachService.GetUpcomingSessionsAsync(coach.Id, withClientId: x.Id)).FirstOrDefault()?.Date,
					};
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var membershipPlans = (await coachService.GetMembershipPlansAsync(coach.Id))
			.Select(async x =>
				new MembershipPlanViewModel()
				{
					Id = x.Id,
					Name = x.Name,
					Price = x.Price,
					Count = x.Count,
					Duration = x.Duration,
					ActiveClients = await membershipPlanService.CountMembershipsAsync(x.Id),
					MembershipStatus = null,
					OwnerViewing = true,
					ClientViewing = false,
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var upcomingSessions = (await coachService.GetUpcomingSessionsAsync(coach.Id, withClientId: null))
			.Select(async x =>
				new SessionViewModel()
				{
					Id = x.Id,
					CoachFullName = (await coachService.GetCoachAsync(x.CoachId))!.FullName,
					ClientFullName = (await clientService.GetClientAsync(x.ClientId))!.FullName,
					Date = x.Date,
					Duration = x.Duration,
					CanCancel = true,
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var workPeriods = (await coachService.GetWorkPeriodsAsync(coach.Id))
			.Select(x =>
				new WorkPeriodViewModel()
				{
					Id = x.Id,
					DayOfWeek = x.DayOfWeek,
					Start = x.Start,
					End = x.End,
				}
			)
			.ToList()
		;

		return View(
			new CoachDashboardViewModel()
			{
				CoachId = coach.Id,
				FullName = user.FullName,
				WeeklyHours = workPeriods.Sum(x => x.Duration.TotalHours),
				Clients = clients,
				MembershipPlans = membershipPlans,
				UpcomingSessions = upcomingSessions,
				WorkPeriods = workPeriods,
			}
		);
	}

	public CoachController(
		ICoachService coachService,
		IClientService clientService,
		IMembershipPlanService membershipPlanService,
		IMembershipService membershipService,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager
	)
	{
		this.coachService = coachService;
		this.clientService = clientService;
		this.membershipPlanService = membershipPlanService;
		this.membershipService = membershipService;
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
