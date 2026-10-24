using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoachDesk.Data;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Controllers;

[Authorize(Roles = ClientRole)]
public class MembershipController : Controller
{
	private readonly ApplicationDbContext dbContext;
	private readonly UserManager<ApplicationUser> userManager;

	[HttpPost]
	public async Task<IActionResult> Purchase(Guid? membershipPlanId, string? returnUrl)
	{
		returnUrl ??= "/";

		var membershipPlan = await dbContext.MembershipPlans.SingleOrDefaultAsync(x => x.Id == membershipPlanId);

		TempData["MembershipPlanId"] = membershipPlanId;

		if (membershipPlan == null)
		{
			TempData["MembershipError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		var user = (await userManager.GetUserAsync(User))!;
		var client = await dbContext.Clients.SingleAsync(x => x.UserId == user.Id);

		if (await dbContext.Memberships.AnyAsync(x =>
				x.ClientId == client.Id && x.MembershipPlan.CoachId == membershipPlan.CoachId &&
				!((x.MembershipPlan.Duration != null) ? x.ActivatedOn.AddDays((int)x.MembershipPlan.Duration) < DateTime.UtcNow : false)
			)
		)
		{
			TempData["MembershipError"] = "You already have a membership from this coach.";

			return LocalRedirect(returnUrl);
		}

		if (await dbContext.Memberships.AnyAsync(x =>
				x.ClientId == client.Id && x.MembershipPlan.CoachId == membershipPlan.CoachId &&
				((x.MembershipPlan.Duration != null) ? x.ActivatedOn.AddDays((int)x.MembershipPlan.Duration) < DateTime.UtcNow : false)
			)
		)
		{
			await dbContext.Memberships
				.Where(x =>
					x.ClientId == client.Id && x.MembershipPlan.CoachId == membershipPlan.CoachId &&
					((x.MembershipPlan.Duration != null) ? x.ActivatedOn.AddDays((int)x.MembershipPlan.Duration) < DateTime.UtcNow : false)
				)
				.ExecuteDeleteAsync()
			;
		}

		await dbContext.Memberships.AddAsync(
			new()
			{
				ActivatedOn = DateTime.UtcNow,
				MembershipPlanId = (Guid)membershipPlanId!,
				ClientId = client.Id,
			}
		);

		await dbContext.SaveChangesAsync();

		return LocalRedirect(returnUrl);
	}

	public MembershipController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		this.dbContext = dbContext;
		this.userManager = userManager;
	}
}
