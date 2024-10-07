using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using MedManager.ViewModel.Account;

namespace MedManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Medecin> _signInManager;
        private readonly UserManager<Medecin> _userManager;
		private readonly ILogger<Medecin> _logger;

		public AccountController(SignInManager<Medecin> signInManager, UserManager<Medecin> userManager, ILogger<Medecin> logger)
		{
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;

		}

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false ) ;

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Medecin");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return RedirectToAction("Index", "Medecin");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Medecin");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Medecin
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Nom = model.Nom,
                    Prenom = model.Prenom,
                    Ville = model.Ville
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Medecin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
