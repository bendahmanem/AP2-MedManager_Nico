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
		private readonly ILogger<CompteController> _logger;

		public CompteController(SignInManager<Medecin> gestionConnexion, UserManager<Medecin> gestionUtilisateurs, ILogger<CompteController> logger)
		{
			_gestionConnexion = gestionConnexion;
			_gestionUtilisateurs = gestionUtilisateurs;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Connexion()
		{
			_logger.LogInformation("Affichage de la page de connexion.");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Connexion(ConnexionViewModel modele)
		{
			_logger.LogInformation("Tentative de connexion pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);

			if (ModelState.IsValid)
			{
				try
				{
					var resultat = await _gestionConnexion.PasswordSignInAsync(modele.NomUtilisateur, modele.MotDePasse, modele.SeRappelerDeMoi, false);

					if (resultat.Succeeded)
					{
						_logger.LogInformation("Connexion réussie pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);
						return RedirectToAction("Index", "Tableaudebord");
					}

					_logger.LogWarning("Échec de connexion pour l'utilisateur {NomUtilisateur}. Mot de passe ou nom incorrect.", modele.NomUtilisateur);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Erreur lors de la tentative de connexion pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);
					ModelState.AddModelError("", "Une erreur est survenue lors de la tentative de connexion.");
				}
			}

			return View(modele);
		}

		public async Task<IActionResult> Deconnexion()
		{
			try
			{
				_logger.LogInformation("Déconnexion en cours.");
				await _gestionConnexion.SignOutAsync();
				_logger.LogInformation("Déconnexion réussie.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erreur lors de la tentative de déconnexion.");
				throw; // Optionnel, peut être géré si vous souhaitez afficher une page d'erreur.
			}

			return RedirectToAction("Index", "Accueil");
		}

		public IActionResult Inscription()
		{
			_logger.LogInformation("Affichage de la page d'inscription.");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Inscription(CompteViewModel modele)
		{
			_logger.LogInformation("Tentative d'inscription pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);

			if (ModelState.IsValid)
			{
				try
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
						_logger.LogInformation("Inscription réussie pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);
						await _gestionConnexion.SignInAsync(utilisateur, isPersistent: false);
						return RedirectToAction("Index", "Tableaudebord");
					}

					foreach (var erreur in resultat.Errors)
					{
						_logger.LogWarning("Erreur lors de l'inscription pour l'utilisateur {NomUtilisateur} : {Description}.", modele.NomUtilisateur, erreur.Description);
						ModelState.AddModelError(string.Empty, erreur.Description);
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Erreur lors de la tentative d'inscription pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);
					ModelState.AddModelError("", "Une erreur est survenue lors de la tentative d'inscription.");
				}
			}
			else
			{
				_logger.LogWarning("Échec de validation du modèle pour l'utilisateur {NomUtilisateur}.", modele.NomUtilisateur);
			}

			return View(modele);
		}
	}
}
