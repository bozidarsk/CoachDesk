using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoachDesk.Data;
using CoachDesk.ViewModels;
using static CoachDesk.Common.EntityValidation.User;

namespace CoachDesk.Controllers;

public class AccountController : Controller
{
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	[Authorize]
	public IActionResult Index() => View();

	[HttpGet, AllowAnonymous]
	public IActionResult Register(string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		return View();
	}

	[HttpPost, AllowAnonymous]
	public async Task<IActionResult> Register(RegisterInputModel model, string? returnUrl)
	{
		returnUrl ??= "/";

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (!ValidateEmail(model.Email))
			ModelState.AddModelError(nameof(model.Email), "Invalid email address was provided.");

		if (!ValidatePassword(model.Password))
			ModelState.AddModelError(nameof(model.Password), $"Password must be between {PasswordMinLength} and {PasswordMaxLength} characters and contain letters and digits.");

		if (await userManager.FindByEmailAsync(model.Email) != null)
			ModelState.AddModelError(nameof(model.Email), "User with the same email address already exists.");

		if (!ModelState.IsValid)
			return View(model);

		var user = new ApplicationUser()
		{
			UserName = model.Email,
			Email = model.Email,
			FullName = model.FullName,
		};

		var result = await userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return View(model);
		}

		await signInManager.SignInAsync(user, isPersistent: model.StayLoggedIn);

		return LocalRedirect(returnUrl);
	}

	[HttpGet, AllowAnonymous]
	public IActionResult Login(string? returnUrl)
	{
		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		return View();
	}

	[HttpPost, AllowAnonymous]
	public async Task<IActionResult> Login(LoginInputModel model, string returnUrl)
	{
		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (!ValidateEmail(model.Email))
			ModelState.AddModelError(nameof(model.Email), "Invalid email address was provided.");

		if (!ValidatePassword(model.Password))
			ModelState.AddModelError(nameof(model.Password), $"Password must be between {PasswordMinLength} and {PasswordMaxLength} characters and contain letters and digits.");

		if (!ModelState.IsValid)
			return View(model);

		var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.StayLoggedIn, lockoutOnFailure: false);
		if (!result.Succeeded)
		{
			ModelState.AddModelError(string.Empty, "Invalid login attempt.");

			return View(model);
		}

		return LocalRedirect(returnUrl);
	}

	[Authorize]
	public async Task<IActionResult> Logout(string? returnUrl)
	{
		returnUrl ??= "/";

		await signInManager.SignOutAsync();

		return LocalRedirect(returnUrl);
	}

	public IActionResult AccessDenied() => View();

	private bool ValidateEmail(string email) => string.IsNullOrWhiteSpace(email) || Regex.IsMatch(email, "[a-zA-Z0-9_.]+@[a-zA-Z0-9_.]+");

	private bool ValidatePassword(string password)
	{
		if (string.IsNullOrEmpty(password))
			return false;

		if (password.Length < PasswordMinLength || password.Length > PasswordMaxLength)
			return false;

		if (!password.Any(x => char.IsDigit(x)))
			return false;

		return true;
	}

	public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
	{
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
