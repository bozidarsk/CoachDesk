using Microsoft.AspNetCore.Mvc;

namespace CoachDesk.Controllers;

public class SignupController : Controller
{
	public IActionResult Index()
	{
		ViewData["Title"] = "Sign up";

		return View();
	}

	public IActionResult Coach()
	{
		ViewData["Title"] = "Sign up (as Coach)";

		return View();
	}

	[HttpPost]
	public IActionResult Submit()
	{
		return Redirect("/");
	}
}
