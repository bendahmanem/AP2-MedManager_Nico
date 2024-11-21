using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using MedManager.ViewModel.Compte;


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
		public async Task<IActionResult> Connexion(ConnexionViewModel modele)
		{
			if (ModelState.IsValid)
			{
				var resultat = await _gestionConnexion.PasswordSignInAsync(modele.NomUtilisateur, modele.MotDePasse, modele.SeRappelerDeMoi, false);

				if (resultat.Succeeded)
				{
					return RedirectToAction("Index", "Tableaudebord");
				}

				ModelState.AddModelError("", "Le nom d'utilisateur ou le mot de passe que vous avez saisi est incorrect. Veuillez réessayer.");
			}

			return View(modele);
		}

		public async Task<IActionResult> Deconnexion()
		{
			await _gestionConnexion.SignOutAsync();

			return RedirectToAction("Index", "Accueil");
		}

		public IActionResult Inscription()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Inscription(CompteViewModel modele)
		{
			if (ModelState.IsValid)
			{
				var utilisateur = new Medecin
				{
                    UserName = modele.NomUtilisateur,
					Email = modele.Email,
					Nom = modele.Nom,
					Prenom = modele.Prenom,
					Ville = modele.Ville,
					Adresse = modele.Adresse,
					Faculte = modele.Faculte,
					Specialite = modele.Specialite,
                    NumeroTel = modele.MotDePasse,
				};

                var resultat = await _gestionUtilisateurs.CreateAsync(utilisateur, modele.MotDePasse);

				if (resultat.Succeeded)
				{
					await _gestionConnexion.SignInAsync(utilisateur, isPersistent: false);
					return RedirectToAction("Index", "Tableaudebord");
				}

				foreach (var erreur in resultat.Errors)
				{
					ModelState.AddModelError(string.Empty, erreur.Description);
				}
			}

			return View(modele);
		}
	}
}
