using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoachDesk.Data.Models;
using CoachDesk.Web.Models;
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

    public IActionResult Privacy()
    {
        return View();
    }

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
