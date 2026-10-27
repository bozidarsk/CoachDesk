using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Home;
using static CoachDesk.Common.ApplicationConstants;

namespace CoachDesk.Web.Controllers;

public class HomeController : Controller
{
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	public async Task<IActionResult> Index()
	{
		if (signInManager.IsSignedIn(User))
		{
			var user = (await userManager.GetUserAsync(User))!;

			if (await userManager.IsInRoleAsync(user, CoachRole))
				return RedirectToAction(actionName: "Dashboard", controllerName: "Coach");

			if (await userManager.IsInRoleAsync(user, ClientRole))
				return RedirectToAction(actionName: "Dashboard", controllerName: "Client");

			return StatusCode(500);
		}

		return View();
	}

	public IActionResult Error(int id)
	{
		if (id < 400 || id > 599)
			id = 404;

		Response.StatusCode = id;

		var name = ReasonPhrases.GetReasonPhrase(id);

		return View(
			new ErrorViewModel()
			{
				Code = id,
				Name = string.IsNullOrEmpty(name) ? "Error" : name,
				Message = id switch
				{
					400 => "The request could not be understood.",
					401 => "You need to log in to see this page.",
					403 => "You don't have permission to see this page.",
					404 => "We couldn't find the page you were looking for.",
					>= 500 => "Something went wrong on our side. Please try again later.",
					_ => "Something went wrong with this request."
				},
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
			}
		);
	}

	public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
