using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using MedManager.ViewModel.Account;

namespace MedManager.Controllers
{
	public class CompteController : Controller
	{
		private readonly SignInManager<Medecin> _gestionConnexion;
		private readonly UserManager<Medecin> _gestionUtilisateurs;
		private readonly ILogger<Medecin> _logger;

		public CompteController(SignInManager<Medecin> gestionConnexion, UserManager<Medecin> gestionUtilisateurs, ILogger<Medecin> logger)
		{
			_gestionConnexion = gestionConnexion;
			_gestionUtilisateurs = gestionUtilisateurs;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Connexion()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Connexion(LoginViewModel modele)
		{
			if (ModelState.IsValid)
			{
				var resultat = await _gestionConnexion.PasswordSignInAsync(modele.UserName, modele.Password, modele.RememberMe, false);

				if (resultat.Succeeded)
				{
					return RedirectToAction("Index", "Medecin");
				}

				ModelState.AddModelError(string.Empty, "Tentative de connexion invalide.");
			}

			return RedirectToAction("Index", "Medecin");
		}

		public async Task<IActionResult> Deconnexion()
		{
			await _gestionConnexion.SignOutAsync();

			return RedirectToAction("Index", "Medecin");
		}

		public IActionResult Inscription()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Inscription(RegisterViewModel modele)
		{
			if (ModelState.IsValid)
			{
				var utilisateur = new Medecin
				{
					UserName = modele.UserName,
					Email = modele.Email,
					Nom = modele.Nom,
					Prenom = modele.Prenom,
					Ville = modele.Ville,
					Adresse = modele.Adresse,
					Faculte = modele.Faculte,
					Specialite = modele.Specialite,
					NumTel = modele.NumTel,
				};

				var resultat = await _gestionUtilisateurs.CreateAsync(utilisateur, modele.Password);

				if (resultat.Succeeded)
				{
					await _gestionConnexion.SignInAsync(utilisateur, isPersistent: false);
					return RedirectToAction("Index", "Medecin");
				}

				foreach (var erreur in resultat.Errors)
				{
					ModelState.AddModelError(string.Empty, erreur.Description);
				}
			}

			return RedirectToAction("Connexion", "Compte");
		}
	}
}
