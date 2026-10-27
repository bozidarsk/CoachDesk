using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoachDesk.Data.Models;
using CoachDesk.ViewModels.Account;
using CoachDesk.Services.Core.Contracts;
using static CoachDesk.Common.ApplicationConstants;
using static CoachDesk.Data.Common.EntityValidation.User;

namespace CoachDesk.Web.Controllers;

[Authorize]
public class AccountController : Controller
{
	private readonly IAccountService accountService;
	private readonly ICoachService coachService;
	private readonly IClientService clientService;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly SignInManager<ApplicationUser> signInManager;

	public async Task<IActionResult> Index()
	{
		return View(await CreateViewModel());
	}

	[HttpGet, AllowAnonymous]
	public IActionResult Register(string? returnUrl, bool? registerAsCoach)
	{
		if (signInManager.IsSignedIn(User))
			return Forbid();

		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		return View(
			new AccountRegisterInputModel()
			{
				RegisterAsCoach = registerAsCoach ?? false,
			}
		);
	}

	[HttpPost, AllowAnonymous]
	public async Task<IActionResult> Register(AccountRegisterInputModel model, string? returnUrl)
	{
		if (signInManager.IsSignedIn(User))
			return Forbid();

		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (!accountService.IsEmailValid(model.Email))
			ModelState.AddModelError(nameof(model.Email), "Invalid email address was provided.");

		if (!accountService.IsPasswordValid(model.Password))
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
			MemberSince = DateTime.UtcNow,
		};

		var result = await userManager.CreateAsync(user, model.Password);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return View(model);
		}

		if (model.RegisterAsCoach)
			await coachService.AddCoachAsync(user.Id);
		else
			await clientService.AddClientAsync(user.Id);

		await userManager.AddToRoleAsync(user, model.RegisterAsCoach ? CoachRole : ClientRole);
		await signInManager.SignInAsync(user, isPersistent: model.StayLoggedIn);

		return LocalRedirect(returnUrl);
	}

	[HttpGet, AllowAnonymous]
	public IActionResult Login(string? returnUrl)
	{
		if (signInManager.IsSignedIn(User))
			return Forbid();

		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		return View();
	}

	[HttpPost, AllowAnonymous]
	public async Task<IActionResult> Login(AccountLoginInputModel model, string returnUrl)
	{
		if (signInManager.IsSignedIn(User))
			return Forbid();

		returnUrl ??= "/";

		ViewData["ReturnUrl"] = returnUrl;

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(model);
		}

		if (!accountService.IsEmailValid(model.Email))
			ModelState.AddModelError(nameof(model.Email), "Invalid email address was provided.");

		if (!accountService.IsPasswordValid(model.Password))
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

	[HttpGet]
	public async Task<IActionResult> Logout(string? returnUrl)
	{
		returnUrl ??= "/";

		await signInManager.SignOutAsync();

		return LocalRedirect(returnUrl);
	}

	[HttpPost]
	public async Task<IActionResult> UpdateProfile(AccountUpdateProfileInputModel model)
	{
		var viewModel = await CreateViewModel();

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(nameof(Index), viewModel);
		}

		var user = (await userManager.GetUserAsync(User))!;

		await accountService.SetFullNameAsync(user, model.FullName);
		await accountService.SetBioAsync(user, model.Bio);

		return View(nameof(Index), viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> ChangeEmail(AccountChangeEmailInputModel model)
	{
		var viewModel = await CreateViewModel();

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(nameof(Index), viewModel);
		}

		var user = (await userManager.GetUserAsync(User))!;

		if (!accountService.IsEmailValid(model.NewEmail))
			ModelState.AddModelError(nameof(model.NewEmail), "Invalid email address was provided.");

			if (await userManager.FindByEmailAsync(model.NewEmail) != null)
			ModelState.AddModelError(nameof(model.NewEmail), "User with the same email address already exists.");

		if (!await userManager.CheckPasswordAsync(user, model.EmailCurrentPassword))
			ModelState.AddModelError(nameof(model.EmailCurrentPassword), "Wrong password.");

		if (!ModelState.IsValid)
			return View(nameof(Index), viewModel);

		await accountService.SetUserNameAsync(user, model.NewEmail);
		await accountService.SetEmailAsync(user, model.NewEmail);

		return View(nameof(Index), viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> ChangePassword(AccountChangePasswordInputModel model)
	{
		var viewModel = await CreateViewModel();

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(nameof(Index), viewModel);
		}

		var user = (await userManager.GetUserAsync(User))!;

		if (!accountService.IsPasswordValid(model.NewPassword))
			ModelState.AddModelError(nameof(model.NewPassword), $"Password must be between {PasswordMinLength} and {PasswordMaxLength} characters and contain letters and digits.");

		if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
			ModelState.AddModelError(nameof(model.CurrentPassword), "Wrong password.");

		if (!ModelState.IsValid)
			return View(nameof(Index), viewModel);

		await accountService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

		return View(nameof(Index), viewModel);
	}

	[HttpPost]
	public async Task<IActionResult> Delete(AccountDeleteInputModel model)
	{
		var viewModel = await CreateViewModel();

		if (!ModelState.IsValid)
		{
			ModelState.AddModelError(string.Empty, "Invalid request data was provided.");

			return View(nameof(Index), viewModel);
		}

		if (!model.Confirm)
			ModelState.AddModelError(nameof(model.Confirm), "Tick the box to confirm.");

		var user = (await userManager.GetUserAsync(User))!;

		if (!await userManager.CheckPasswordAsync(user, model.Password))
			ModelState.AddModelError(nameof(model.Password), "Wrong password.");

		if (!ModelState.IsValid)
			return View(nameof(Index), viewModel);

		await signInManager.SignOutAsync();
		await userManager.DeleteAsync(user);

		return LocalRedirect("/");
	}

	private async Task<AccountIndexViewModel> CreateViewModel()
	{
		var user = (await userManager.GetUserAsync(User))!;

		Guid profileId;
		string profileController;

		if (await userManager.IsInRoleAsync(user, CoachRole))
		{
			var coach = (await coachService.GetCoachAsync(user))!;

			profileId = coach.Id;
			profileController = "Coach";
		}
		else if (await userManager.IsInRoleAsync(user, ClientRole))
		{
			var client = (await clientService.GetClientAsync(user))!;

			profileId = client.Id;
			profileController = "Client";
		}
		else
		{
			throw new InvalidOperationException();
		}

		var viewModel = new AccountIndexViewModel()
		{
			ProfileId = profileId,
			ProfileController = profileController,
			Email = user.Email!,
			MemberSince = user.MemberSince,
		};

		viewModel.UpdateProfile.FullName = user.FullName;
		viewModel.UpdateProfile.Bio = user.Bio;

		viewModel.ChangeEmail.CurrentEmail = user.Email!;

		return viewModel;
	}

	public AccountController(
		IAccountService accountService,
		ICoachService coachService,
		IClientService clientService,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager
	)
	{
		this.accountService = accountService;
		this.coachService = coachService;
		this.clientService = clientService;
		this.userManager = userManager;
		this.signInManager = signInManager;
	}
}
