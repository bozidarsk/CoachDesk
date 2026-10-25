using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data;
using CoachDesk.ViewModels.WorkPeriod;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Controllers;

[Authorize(Roles = CoachRole)]
public class WorkPeriodController : Controller
{
	private readonly IWorkPeriodService workPeriodService;
	private readonly ICoachService coachService;
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
		var coach = (await coachService.GetCoachAsync(user))!;

		await workPeriodService.AddWorkPeriodAsync(
			dayOfWeek: model.DayOfWeek,
			start: model.Start,
			end: model.End,
			coachId: coach.Id
		);

		return LocalRedirect(returnUrl);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		var user = (await userManager.GetUserAsync(User))!;
		var coach = (await coachService.GetCoachAsync(user))!;

		var workPeriod = await workPeriodService.GetWorkPeriodAsync(id ?? default);

		if (workPeriod == null || coach.Id != workPeriod.CoachId)
		{
			TempData["WorkPeriodDeleteError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		await workPeriodService.DeleteWorkPeriodsAsync((Guid)id!);

		return LocalRedirect(returnUrl);
	}

	public WorkPeriodController(IWorkPeriodService workPeriodService, ICoachService coachService, UserManager<ApplicationUser> userManager)
	{
		this.workPeriodService = workPeriodService;
		this.coachService = coachService;
		this.userManager = userManager;
	}
}
