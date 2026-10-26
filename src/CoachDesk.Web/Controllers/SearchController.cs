using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Search;
using CoachDesk.ViewModels.WorkPeriod;
using CoachDesk.ViewModels.MembershipPlan;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Web.Controllers;

public class SearchController : Controller
{
	private readonly ISearchService searchService;
	private readonly ICoachService coachService;
	private readonly IClientService clientService;
	private readonly IMembershipPlanService membershipPlanService;
	private readonly IMembershipService membershipService;
	private readonly UserManager<ApplicationUser> userManager;

	[HttpGet, Authorize(Roles = ClientRole)]
	public async Task<IActionResult> Coaches(SearchCoachesInputModel model)
	{
		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (model.MinPrice != null && model.MaxPrice != null && model.MinPrice > model.MaxPrice)
		{
			ModelState.AddModelError(string.Empty, "Min price must be less than or equal to max price.");

			return View(model);
		}

		if (model.MinDurationDays != null && model.MinDurationDays <= 0)
		{
			ModelState.AddModelError(string.Empty, "Number of valid for days must be greater than 0.");

			return View(model);
		}

		if (model.MinSessions != null && model.MinSessions <= 0)
		{
			ModelState.AddModelError(string.Empty, "Number of sessions included must be greater than 0.");

			return View(model);
		}

		if (model.AvailableFrom != null && model.AvailableTo != null && model.AvailableFrom >= model.AvailableTo)
		{
			ModelState.AddModelError(string.Empty, "Available from time must be less than available to time.");

			return View(model);
		}

		if (model.Page <= 0)
		{
			ModelState.AddModelError(string.Empty, "Page number must be greater than 0.");

			return View(model);
		}

		if (!ModelState.IsValid)
			return View(model);

		var user = (await userManager.GetUserAsync(User))!;
		var client = (await clientService.GetClientAsync(user))!;

		var coaches = model.Days.Any()
			? model.Days
				.Select(async x => await searchService.SearchCoaches(model.Name, x, model.AvailableFrom, model.AvailableTo))
				.Select(x => x.Result)
				.SelectMany(x => x)
				.DistinctBy(x => x.Id)
				.ToList()
			: await searchService.SearchCoaches(model.Name, null, model.AvailableFrom, model.AvailableTo)
		;

		var results = coaches
			.Select(async coach =>
				{
					var initials = coach.FullName
						.Split(' ', StringSplitOptions.RemoveEmptyEntries)
						.Select(x => char.ToUpper(x[0]))
						.ToList()
					;

					return new SearchCoachesViewModel()
					{
						Id = coach.Id,
						FullName = coach.FullName,
						Initials = $"{initials[0]}{initials[^1]}",
						Bio = coach.Bio,
						ActiveClients = await coachService.CountClientsAsync(coach.Id),
						MatchingPlans = (await searchService.SearchCoachMembershipPlans(coach.Id, model.MinPrice, model.MaxPrice, model.MinDurationDays, model.MinSessions))
							.Select(async membershipPlan =>
								new MembershipPlanViewModel()
								{
									Id = membershipPlan.Id,
									Name = membershipPlan.Name,
									Price = membershipPlan.Price,
									Duration = membershipPlan.Duration,
									Count = membershipPlan.Count,
									ActiveClients = await membershipPlanService.CountMembershipsAsync(membershipPlan.Id),
									MembershipStatus = (await membershipService.GetMembershipAsync(membershipPlanId: membershipPlan.Id, coachId: coach.Id, clientId: client.Id))?.Status,
									OwnerViewing = false,
									ClientViewing = true,
								}
							)
							.Select(x => x.Result)
							.ToList(),
						WorkPeriods = (await coachService.GetWorkPeriodsAsync(coach.Id))
							.Select(workPeriod =>
								new WorkPeriodViewModel()
								{
									Id = workPeriod.Id,
									DayOfWeek = workPeriod.DayOfWeek,
									Start = workPeriod.Start,
									End = workPeriod.End,
								}
							)
							.ToList(),
					};
				}
			)
			.Select(x => x.Result)
			.ToList()
		;

		model.Results = results
			.Skip((model.Page - 1) * SearchCoachesPageSize)
			.Take(SearchCoachesPageSize)
			.ToList()
		;

		model.TotalPages = (((results.Count - 1) / SearchCoachesPageSize) + 1);
		model.TotalResults = results.Count;

		return View(model);
	}

	public SearchController(
		ISearchService searchService,
		ICoachService coachService,
		IClientService clientService,
		IMembershipPlanService membershipPlanService,
		IMembershipService membershipService,
		UserManager<ApplicationUser> userManager
	)
	{
		this.searchService = searchService;
		this.coachService = coachService;
		this.clientService = clientService;
		this.membershipPlanService = membershipPlanService;
		this.membershipService = membershipService;
		this.userManager = userManager;
	}
}
