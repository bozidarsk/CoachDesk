using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data;
using CoachDesk.ViewModels;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Controllers;

[Authorize(Roles = CoachRole)]
public class WorkPeriodController : Controller
{
	private readonly ApplicationDbContext dbContext;
	private readonly UserManager<ApplicationUser> userManager;

	[HttpPost]
	public async Task<IActionResult> Create(WorkPeriodInputModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		if (!ModelState.IsValid)
		{
			TempData["WorkPeriodCreateError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		if (model.Start >= model.End)
		{
			TempData["WorkPeriodCreateError"] = "Start time must be less than End time.";

			return LocalRedirect(returnUrl);
		}

		var user = (await userManager.GetUserAsync(User))!;
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		coach.WorkPeriods.Add(
			new()
			{
				DayOfWeek = model.DayOfWeek,
				Start = model.Start,
				End = model.End,
				CoachId = coach.Id,
			}
		);

		await dbContext.SaveChangesAsync();

		return LocalRedirect(returnUrl);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		var user = (await userManager.GetUserAsync(User))!;
		var coach = await dbContext.Coaches.SingleAsync(x => x.UserId == user.Id);

		var workPeriod = await dbContext.WorkPeriods.SingleOrDefaultAsync(x => x.Id == id);

		if (workPeriod == null || coach.Id != workPeriod.CoachId)
		{
			TempData["WorkPeriodDeleteError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		await dbContext.WorkPeriods.Where(x => x.Id == id).ExecuteDeleteAsync();

		return LocalRedirect(returnUrl);
	}

	public WorkPeriodController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
	{
		this.dbContext = dbContext;
		this.userManager = userManager;
	}
}
