using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data;
using CoachDesk.ViewModels.MembershipPlan;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;
using static CoachDesk.Common.EntityValidation.MembershipPlan;

namespace CoachDesk.Controllers;

[Authorize(Roles = CoachRole)]
public class MembershipPlanController : Controller
{
	private readonly IMembershipPlanService membershipPlanService;
	private readonly IMembershipService membershipService;
	private readonly ICoachService coachService;
	private readonly IClientService clientService;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	[AllowAnonymous]
	public async Task<IActionResult> Details(Guid? id)
	{
		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(id ?? default);

		if (membershipPlan == null)
			return BadRequest();

		bool ownerViewing = false;
		bool clientViewing = false;
		Guid? clientId = null;

		if (signInManager.IsSignedIn(User))
		{
			var user = (await userManager.GetUserAsync(User))!;

			if (await userManager.IsInRoleAsync(user, CoachRole))
			{
				var coach = (await coachService.GetCoachAsync(user))!;

				ownerViewing = coach.Id == membershipPlan.CoachId;
			}
			else if (await userManager.IsInRoleAsync(user, ClientRole))
			{
				var client = (await clientService.GetClientAsync(user))!;

				clientViewing = true;
				clientId = client.Id;
			}
		}

		return View(
			new MembershipPlanViewModel()
			{
				Id = membershipPlan.Id,
				Name = membershipPlan.Name,
				Price = membershipPlan.Price,
				Count = membershipPlan.Count,
				Duration = membershipPlan.Duration,
				ActiveClients = await membershipPlanService.CountMembershipsAsync(membershipPlan.Id),
				OwnerViewing = ownerViewing,
				ClientViewing = clientViewing,
				MembershipStatus = clientViewing ? (await membershipService.GetMembershipAsync(membershipPlanId: membershipPlan.Id, coachId: null, clientId: clientId))?.Status : null,
			}
		);
	}

	[HttpGet]
	public IActionResult Create(string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> Create(MembershipPlanViewModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (model.Name.Length < NameMinLength || model.Name.Length > NameMaxLength)
			ModelState.AddModelError(nameof(model.Name), $"Name must be between {NameMinLength} and {NameMaxLength} characters.");

		if (model.Price < 0)
			ModelState.AddModelError(nameof(model.Price), "Price must be a positive number.");

		if (model.Count == null && model.Duration == null)
			ModelState.AddModelError(string.Empty, $"One of '{nameof(model.Count)}' or '{nameof(model.Duration)}' must be specified.");

		if (model.Count <= 0)
			ModelState.AddModelError(nameof(model.Count), "Count must be a positive number.");

		if (!ModelState.IsValid)
			return View(model);

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		await membershipPlanService.AddMembershipPlanAsync(
			name: model.Name,
			price: model.Price,
			count: model.Count,
			duration: model.Duration,
			coachId: coach.Id
		);

		return LocalRedirect(returnUrl);
	}

	[HttpGet]
	public async Task<IActionResult> Edit(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(id ?? default);

		if (membershipPlan == null || coach.Id != membershipPlan.CoachId)
			return BadRequest();

		return View(
			new MembershipPlanViewModel()
			{
				Id = membershipPlan.Id,
				Name = membershipPlan.Name,
				Price = membershipPlan.Price,
				Count = membershipPlan.Count,
				Duration = membershipPlan.Duration,
				ActiveClients = await membershipPlanService.CountMembershipsAsync(membershipPlan.Id),
				OwnerViewing = true,
				ClientViewing = false,
			}
		);
	}

	[HttpPost]
	public async Task<IActionResult> Edit(MembershipPlanViewModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(model.Id);

		if (!ModelState.IsValid || membershipPlan == null || coach.Id != membershipPlan.CoachId)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (model.Name.Length < NameMinLength || model.Name.Length > NameMaxLength)
			ModelState.AddModelError(nameof(model.Name), $"Name must be between {NameMinLength} and {NameMaxLength} characters.");

		if (model.Price < 0)
			ModelState.AddModelError(nameof(model.Price), "Price must be a positive number.");

		if (model.Count == null && model.Duration == null)
			ModelState.AddModelError(string.Empty, $"One of '{nameof(model.Count)}' or '{nameof(model.Duration)}' must be specified.");

		if (model.Count <= 0)
			ModelState.AddModelError(nameof(model.Count), "Count must be a positive number.");

		if (!ModelState.IsValid)
			return View(model);

		await membershipPlanService.EditMembershipPlanAsync(
			id: membershipPlan.Id,
			name: model.Name,
			price: model.Price,
			count: model.Count,
			duration: model.Duration
		);

		return LocalRedirect(returnUrl);
	}

	[HttpGet]
	public async Task<IActionResult> Delete(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(id ?? default);

		if (membershipPlan == null || coach.Id != membershipPlan.CoachId)
			return BadRequest();

		return View(
			new MembershipPlanViewModel()
			{
				Id = membershipPlan.Id,
				Name = membershipPlan.Name,
				Price = membershipPlan.Price,
				Count = membershipPlan.Count,
				Duration = membershipPlan.Duration,
				ActiveClients = await membershipPlanService.CountMembershipsAsync(membershipPlan.Id),
				OwnerViewing = true,
				ClientViewing = false,
			}
		);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(MembershipPlanViewModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var membershipPlan = await membershipPlanService.GetMembershipPlanAsync(model.Id);

		if (membershipPlan == null || coach.Id != membershipPlan.CoachId)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		var activeClients = await membershipPlanService.CountMembershipsAsync(membershipPlan.Id);

		if (activeClients > 0)
		{
			ModelState.AddModelError(string.Empty, "There are clients that hold this plan, so it cannot be deleted.");

			return View(model);
		}

		await membershipPlanService.DeleteMembershipPlansAsync(membershipPlan.Id);

		return LocalRedirect(returnUrl);
	}

	public MembershipPlanController(
		IMembershipPlanService membershipPlanService,
		IMembershipService membershipService,
		ICoachService coachService,
		IClientService clientService,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager
	)
	{
		this.membershipPlanService = membershipPlanService;
		this.membershipService = membershipService;
		this.coachService = coachService;
		this.clientService = clientService;
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
