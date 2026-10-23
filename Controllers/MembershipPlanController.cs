using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using CoachDesk.ViewModels;
using static CoachDesk.Common.ApplicationConstants;
using static CoachDesk.Common.EntityValidation.MembershipPlan;

namespace CoachDesk.Controllers;

[Authorize(Roles = CoachRole)]
public class MembershipPlanController : Controller
{
	private readonly ApplicationDbContext dbContext;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	[AllowAnonymous]
	public async Task<IActionResult> Details(Guid? id)
	{
		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == id);

		if (membershipPlan == null)
			return BadRequest();

		bool ownerViewing = false;
		bool clientViewing = false;

		if (signInManager.IsSignedIn(User))
		{
			var user = (await userManager.GetUserAsync(User))!;

			if (await userManager.IsInRoleAsync(user, CoachRole))
			{
				var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

				ownerViewing = coach.Id == membershipPlan.CoachId;
			}
			else if (await userManager.IsInRoleAsync(user, ClientRole))
			{
				clientViewing = true;
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
				ActiveClients = await dbContext.Memberships.Where(x => x.MembershipPlanId == membershipPlan.Id).CountAsync(),
				OwnerViewing = ownerViewing,
				ClientViewing = clientViewing,
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
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		coach.MembershipPlans.Add(
			new()
			{
				Name = model.Name,
				Price = model.Price,
				Count = model.Count,
				Duration = model.Duration,
				CoachId = coach.Id,
			}
		);

		await dbContext.SaveChangesAsync();

		return LocalRedirect(returnUrl);
	}

	[HttpGet]
	public async Task<IActionResult> Edit(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == id);

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
				ActiveClients = await dbContext.Memberships.Where(x => x.MembershipPlanId == membershipPlan.Id).CountAsync(),
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
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == model.Id);

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

		membershipPlan.Name = model.Name;
		membershipPlan.Price = model.Price;
		membershipPlan.Count = model.Count;
		membershipPlan.Duration = model.Duration;

		await dbContext.SaveChangesAsync();

		return LocalRedirect(returnUrl);
	}

	[HttpGet]
	public async Task<IActionResult> Delete(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		var user = (await userManager.GetUserAsync(User))!;
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == id);

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
				ActiveClients = await dbContext.Memberships.Where(x => x.MembershipPlanId == membershipPlan.Id).CountAsync(),
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
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == model.Id);

		if (membershipPlan == null || coach.Id != membershipPlan.CoachId)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		var activeClients = await dbContext.Memberships.Where(x => x.MembershipPlanId == model.Id).CountAsync();

		if (activeClients > 0)
		{
			ModelState.AddModelError(string.Empty, "There are clients that hold this plan, so it cannot be deleted.");

			return View(model);
		}

		await dbContext.MembershipPlans.Where(x => x.Id == model.Id).ExecuteDeleteAsync();

		return LocalRedirect(returnUrl);
	}

	public MembershipPlanController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		this.dbContext = dbContext;
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
