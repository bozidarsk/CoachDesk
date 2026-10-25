using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoachDesk.Data;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Controllers;

[Authorize(Roles = ClientRole)]
public class MembershipController : Controller
{
	private readonly IMembershipService membershipService;
	private readonly IMembershipPlanService membershipPlanService;
	private readonly IClientService clientService;
	private readonly UserManager<ApplicationUser> userManager;

	[HttpPost]
	public async Task<IActionResult> Purchase(Guid? membershipPlanId, string? returnUrl)
	{
		returnUrl ??= "/";

		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(membershipPlanId ?? default);

		TempData["MembershipPlanId"] = membershipPlanId;

		if (membershipPlan == null)
		{
			TempData["MembershipPurchaseError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		var user = (await userManager.GetUserAsync(User))!;
		var client = (await clientService.GetClientAsync(user))!;

		var membership = await membershipService.GetMembershipAsync(membershipPlanId: null, coachId: membershipPlan.CoachId, clientId: client.Id);

		if (membership?.Status == "Active")
		{
			TempData["MembershipPurchaseError"] = "You already have a membership from this coach.";

			return LocalRedirect(returnUrl);
		}
		else if (membership != null)
		{
			await membershipService.DeleteMembershipsAsync(membership.Id);
		}

		await membershipService.AddMembershipAsync(membershipPlan.Id, client.Id);

		return LocalRedirect(returnUrl);
	}

	public MembershipController(
		IMembershipService membershipService,
		IMembershipPlanService membershipPlanService,
		IClientService clientService,
		UserManager<ApplicationUser> userManager
	)
	{
		this.membershipService = membershipService;
		this.membershipPlanService = membershipPlanService;
		this.clientService = clientService;
		this.userManager = userManager;
	}
}
