using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CoachDesk.ViewModels.Session;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Web.Controllers;

public class SessionController : Controller
{
	private readonly ISessionService sessionService;
	private readonly ICoachService coachService;
	private readonly IClientService clientService;
	private readonly IMembershipService membershipService;

	[HttpPost, Authorize(Roles = ClientRole)]
	public async Task<IActionResult> Book(SessionInputModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		if (!ModelState.IsValid)
		{
			TempData["SessionBookError"] = "Invalid request data was provided.";

			return LocalRedirect(returnUrl);
		}

		if (await coachService.GetCoachAsync(model.CoachId) == null)
		{
			TempData["SessionBookError"] = $"No coach with id '{model.CoachId}' was found.";

			return LocalRedirect(returnUrl);
		}

		if (await clientService.GetClientAsync(model.ClientId) == null)
		{
			TempData["SessionBookError"] = $"No client with id '{model.ClientId}' was found.";

			return LocalRedirect(returnUrl);
		}

		if (!await coachService.IsAvailable(model.CoachId, model.Date, model.Duration))
		{
			TempData["SessionBookError"] = "The coach is not available during the requested time period.";

			return LocalRedirect(returnUrl);
		}

		var membership = await membershipService.GetMembershipAsync(membershipPlanId: null, coachId: model.CoachId, clientId: model.ClientId);

		if (membership == null)
		{
			TempData["SessionBookError"] = "You don't have a membership for this coach, purchase one first.";

			return LocalRedirect(returnUrl);
		}

		if (membership.Status == "Expired")
		{
			TempData["SessionBookError"] = "Your membership has expired, purchase a new one first.";

			return LocalRedirect(returnUrl);
		}

		if (membership.Status == "OutOfSessions")
		{
			TempData["SessionBookError"] = "You have reached the limit of sessions for your membership, purchase a new one first.";

			return LocalRedirect(returnUrl);
		}

		await membershipService.UseMembersipAsync(membership.Id);

		await sessionService.AddSessionAsync(
			date: model.Date,
			duration: model.Duration,
			coachId: model.CoachId,
			clientId: model.ClientId
		);

		return LocalRedirect(returnUrl);
	}

	[HttpPost, Authorize(Roles = CoachOrClientRole)]
	public async Task<IActionResult> Cancel(Guid? id, string? returnUrl)
	{
		returnUrl ??= "/";

		var session = await sessionService.GetSessionAsync(id ?? default);

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

		await sessionService.DeleteSessionsAsync(session.Id);

		return LocalRedirect(returnUrl);
	}

	public SessionController(
		ISessionService sessionService,
		ICoachService coachService,
		IClientService clientService,
		IMembershipService membershipService
	)
	{
		this.sessionService = sessionService;
		this.coachService = coachService;
		this.clientService = clientService;
		this.membershipService = membershipService;
	}
}
