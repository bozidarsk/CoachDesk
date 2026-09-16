using Microsoft.AspNetCore.Mvc;

namespace CoachDesk.Controllers;

public class LoginController : Controller
{
	public IActionResult Index()
	{
		ViewData["Title"] = "Log in";

		return View();
	}

	[HttpPost]
	public IActionResult Submit()
	{
		return Redirect("/");
	}
}
