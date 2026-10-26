using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Client;
using CoachDesk.ViewModels.Session;
using CoachDesk.ViewModels.Membership;
using CoachDesk.ViewModels.WorkPeriod;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Web.Controllers;

[Authorize(Roles = ClientRole)]
public class ClientController : Controller
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
		var client = await clientService.GetClientAsync(id ?? default);

		if (client == null)
			return BadRequest();

		bool isLoggedIn = false;
		bool ownerViewing = false;
		bool coachOfClientViewing = false;
		Guid? coachId = null;

		if (signInManager.IsSignedIn(User))
		{
			var user = (await userManager.GetUserAsync(User))!;

			isLoggedIn = true;

			if (await userManager.IsInRoleAsync(user, CoachRole))
			{
				var currentCoach = (await coachService.GetCoachAsync(user))!;

				coachOfClientViewing = (await coachService.GetClientsAsync(currentCoach.Id)).Any(x => x.Id == client.Id);
				coachId = currentCoach.Id;
			}
			else if (await userManager.IsInRoleAsync(user, ClientRole))
			{
				var currentClient = (await clientService.GetClientAsync(user))!;

				ownerViewing = client.Id == currentClient.Id;
			}
		}

		var initials = client.FullName
			.Split(' ', StringSplitOptions.RemoveEmptyEntries)
			.Select(x => char.ToUpper(x[0]))
			.ToList()
		;

		var memberships = (await clientService.GetMembershipsAsync(client.Id))
			.Select(async x =>
				{
					var membershipPlan = (await membershipPlanService.GetMembershipPlanAsync(x.MembershipPlanId))!;
					var coach = (await coachService.GetCoachAsync(membershipPlan.CoachId))!;

					return new MembershipViewModel()
					{
						Id = x.Id,
						PlanName = membershipPlan.Name,
						CoachId = membershipPlan.CoachId,
						CoachName = coach.FullName,
						SessionsIncluded = membershipPlan.Count,
						SessionsUsed = x.TimesUsed,
						ExpiresOn = (membershipPlan.Duration != null) ? x.ActivatedOn.AddDays((int)membershipPlan.Duration) : null,
						Status = x.Status,
					};
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var upcomingSessions = (await clientService.GetUpcomingSessionsAsync(client.Id, coachId))
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

		return View(
			new ClientIndexViewModel()
			{
				Id = client.Id,
				FullName = client.FullName,
				Bio = client.Bio,
				MemberSince = client.MemberSince,
				Initials = $"{initials[0]}{initials[^1]}",
				IsLoggedIn = isLoggedIn,
				OwnerViewing = ownerViewing,
				CoachOfClientViewing = coachOfClientViewing,
				Memberships = memberships,
				UpcomingSessions = upcomingSessions,
			}
		);
	}

	public async Task<IActionResult> Dashboard(Guid? selectedCoachId)
	{
		var user = (await userManager.GetUserAsync(User))!;
		var client = (await clientService.GetClientAsync(user))!;

		var selectedCoach = await coachService.GetCoachAsync(selectedCoachId ?? default);

		var memberships = (await clientService.GetMembershipsAsync(client.Id))
			.Select(async x =>
				{
					var membershipPlan = (await membershipPlanService.GetMembershipPlanAsync(x.MembershipPlanId))!;
					var coach = (await coachService.GetCoachAsync(membershipPlan.CoachId))!;

					return new MembershipViewModel()
					{
						Id = x.Id,
						PlanName = membershipPlan.Name,
						CoachId = membershipPlan.CoachId,
						CoachName = coach.FullName,
						SessionsIncluded = membershipPlan.Count,
						SessionsUsed = x.TimesUsed,
						ExpiresOn = (membershipPlan.Duration != null) ? x.ActivatedOn.AddDays((int)membershipPlan.Duration) : null,
						Status = x.Status,
					};
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		var upcomingSessions = (await clientService.GetUpcomingSessionsAsync(client.Id, null))
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

		return View(
			new ClientDashboardViewModel()
			{
				ClientId = client.Id,
				FullName = client.FullName,
				Memberships = memberships,
				UpcomingSessions = upcomingSessions,
				SessionBooking = (selectedCoach != null)
					? new()
					{
						CoachId = selectedCoach.Id,
						ClientId = client.Id,
						ClientMembershipStatus = (await membershipService.GetMembershipAsync(membershipPlanId: default, coachId: selectedCoach.Id, clientId: client.Id))?.Status,
						CoachUpcomingSessions = (await coachService.GetUpcomingSessionsAsync(selectedCoach.Id, null))
							.Select(x =>
								new SessionViewModel()
								{
									Date = x.Date,
									Duration = x.Duration,
								}
							)
							.ToList(),
						CoachWorkPeriods = (await coachService.GetWorkPeriodsAsync(selectedCoach.Id))
							.Select(x =>
								new WorkPeriodViewModel()
								{
									DayOfWeek = x.DayOfWeek,
									Start = x.Start,
									End = x.End,
								}
							)
							.ToList(),
						CoachesDropdown = memberships
							.DistinctBy(x => x.CoachId)
							.Select(x => (x.CoachId, x.CoachName))
							.ToList(),
					}
					: new()
					{
						CoachId = default,
						ClientId = default,
						ClientMembershipStatus = null,
						CoachUpcomingSessions = [],
						CoachWorkPeriods = [],
						CoachesDropdown = memberships
							.DistinctBy(x => x.CoachId)
							.Select(x => (x.CoachId, x.CoachName))
							.ToList(),
					},
			}
		);
	}

	public ClientController(
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
