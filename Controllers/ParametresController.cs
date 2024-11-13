using MedManager.Data;
using MedManager.Models;
using MedManager.ViewModel.Compte;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Controllers
{
	public class ParametresController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<Medecin> _userManager;
		private readonly ILogger<Patient> _logger;
		public ParametresController(ApplicationDbContext dbContext, UserManager<Medecin> userManager, ILogger<Patient> logger)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);

			if (user == null)
				return NotFound();

			var medecin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);


			var modele = new InscriptionViewModel
			{
				NomUtilisateur = medecin.UserName,
				Prenom = medecin.Prenom,
				Nom = medecin.Nom,
				Adresse = medecin.Adresse,
				Ville = medecin.Ville,
				Faculte = medecin.Faculte,
				NumeroTel = medecin.NumeroTel,
				Email = medecin.Email,
				MotDePasse = medecin.PasswordHash,
			};
			return View(modele);
		}

		[HttpPost]
		public async Task<IActionResult> Index(InscriptionViewModel model)
		{
			if(!ModelState.IsValid)
			{
                return View(model);
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var medecin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

			if (medecin == null)
				return NotFound();

			medecin.Nom = model.Nom;
			medecin.Prenom = model.Prenom;
			medecin.Adresse = model.Adresse;
			medecin.NormalizedEmail = model.Email;
			medecin.NormalizedUserName = model.NomUtilisateur;
			medecin.Ville = model.Ville;
			medecin.Faculte = model.Faculte;
			medecin.NumeroTel = model.NumeroTel;
			medecin.Email = model.Email;
			medecin.UserName = model.NomUtilisateur;

			var passwordHasher = new PasswordHasher<Medecin>();
			medecin.PasswordHash = passwordHasher.HashPassword(medecin, model.MotDePasse);

			_dbContext.Update(medecin);
			await _dbContext.SaveChangesAsync();


            return View(model);
		}
	}
}

