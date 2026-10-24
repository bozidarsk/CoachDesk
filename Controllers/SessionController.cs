using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoachDesk.Data;
using CoachDesk.ViewModels;
using static CoachDesk.Common.ApplicationConstants;
using Microsoft.EntityFrameworkCore;
using CoachDesk.Data.Models;

namespace CoachDesk.Controllers;

public class SessionController : Controller
{
	private readonly ApplicationDbContext dbContext;

	[HttpPost, Authorize(Roles = ClientRole)]
	public async Task<IActionResult> Book(SessionInputModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		if (!ModelState.IsValid)
		{
			TempData["SessionBookError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		if (!await dbContext.Coaches.AnyAsync(x => x.Id == model.CoachId))
		{
			TempData["SessionBookError"] = $"No coach with id '{model.CoachId}' was found.";

			return LocalRedirect(returnUrl);
		}

		if (!await dbContext.Clients.AnyAsync(x => x.Id == model.ClientId))
		{
			TempData["SessionBookError"] = $"No client with id '{model.ClientId}' was found.";

			return LocalRedirect(returnUrl);
		}

		// cannot do it in one AnyAsync call because it fails to translate it to sql
		IReadOnlyList<Session> coachSessions = await dbContext.Sessions.Where(x => x.CoachId == model.CoachId).ToListAsync();

		foreach (var x in coachSessions)
		{
			if (model.Date >= x.Date && model.Date < x.Date + x.Duration)
			{
				TempData["SessionBookError"] = "There is a booked session during the requested time period.";

				return LocalRedirect(returnUrl);
			}
		}

		var membership = await dbContext.Memberships
			.Include(x => x.MembershipPlan)
			.Where(x => x.MembershipPlan.CoachId == model.CoachId && x.ClientId == model.ClientId)
			.SingleOrDefaultAsync()
		;

		if (membership == null)
		{
			TempData["SessionBookError"] = "You don't have a membership for this coach, purchase one first.";

			return LocalRedirect(returnUrl);
		}

		if (membership.MembershipPlan.Duration != null && membership.ActivatedOn.AddDays((int)membership.MembershipPlan.Duration) < DateTime.UtcNow)
		{
			TempData["SessionBookError"] = "Your membership has expired, purchase a new one first.";

			return LocalRedirect(returnUrl);
		}

		if (membership.MembershipPlan.Count != null && membership.TimesUsed >= (int)membership.MembershipPlan.Count)
		{
			TempData["SessionBookError"] = "You have reached the limit of sessions for your membership, purchase a new one first.";

			return LocalRedirect(returnUrl);
		}

		membership.TimesUsed++;

		await dbContext.Sessions.AddAsync(
			new()
			{
				Date = model.Date,
				Duration = model.Duration,
				CoachId = model.CoachId,
				ClientId = model.ClientId,
			}
		);

		await dbContext.SaveChangesAsync();

		return LocalRedirect(returnUrl);
	}

	[HttpPost, Authorize(Roles = CoachOrClientRole)]
	public async Task<IActionResult> Cancel(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		var session = await dbContext.Sessions.SingleOrDefaultAsync(x => x.Id == id);

		if (session == null)
		{
			TempData["SessionCancelError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		if (DateTime.UtcNow >= session.Date && DateTime.UtcNow < session.Date + session.Duration)
		{
			TempData["SessionCancelError"] = "Cannot cancel a session in progress.";

			return LocalRedirect(returnUrl);
		}

		await dbContext.Sessions
			.Where(x => x.Id == session.Id)
			.ExecuteDeleteAsync()
		;

		return LocalRedirect(returnUrl);
	}

	public SessionController(ApplicationDbContext dbContext)
	{
		this.dbContext = dbContext;
	}
}
