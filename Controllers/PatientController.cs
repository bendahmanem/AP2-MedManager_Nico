 using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;
using MedManager.Data;
using Microsoft.EntityFrameworkCore;
using MedManager.ViewModel.PatientVM;
using Microsoft.AspNetCore.Mvc.Abstractions;
using X.PagedList;
using System.Data.Common;
using Microsoft.CodeAnalysis.CSharp;
using NuGet.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace MedManager.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Medecin> _userManager;
        private readonly ILogger<Patient> _logger;

        public PatientController(ApplicationDbContext dbContext, UserManager<Medecin> userManager, ILogger<Patient> logger)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        //Revoir ici l'appel à la base de données pour la construction de l'objet médecin. N'appeller que le nécessaire ! 
        public async Task<IActionResult> Index(int? page, string Filtre)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Connexion", "Compte");
                }

                string id = user.Id;

                Medecin? medecin = await _dbContext.Users
                                        .Include(u => u.Patients)
                                        .ThenInclude(p => p.Allergies)
                                        .Include(u => u.Ordonnances)
                                        .FirstOrDefaultAsync(m => m.Id == id);

                if (medecin == null)
                {
                    return RedirectToAction("Error");
                }

                
                var patients = medecin.Patients.AsQueryable();
                if (!string.IsNullOrEmpty(Filtre))
                {
                    patients = patients.Where(p => p.Nom.ToUpper().Contains(Filtre.ToUpper()) || p.Prenom.ToUpper().Contains(Filtre.ToUpper()));
                }

                int TaillePage = 9;
                int NombrePage = (page ?? 1);
                var ListePagineesPatients = patients.ToPagedList(NombrePage, TaillePage);

                var viewModel = new IndexPatientViewModel
                {
                    medecin = medecin,
                    Patients = ListePagineesPatients
				};

                
                ViewData["FiltreActuel"] = Filtre;

                return View(viewModel);
            }
			catch (DbException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la récupération des données.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue.");
				return RedirectToAction("Error");
			}

		}


		[HttpGet]
		public async Task<IActionResult> Ajouter(string id)
		{
			try
			{
				var viewModel = new PatientViewModel
				{
					Allergies = await _dbContext.Allergies.ToListAsync(),
					Antecedents = await _dbContext.Antecedents.ToListAsync(),
					IdMedecin = id,
				};
				return View(viewModel);
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la récupération des allergies ou des antécédents.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue.");
				return RedirectToAction("Error");
			}
		}


		[HttpPost]
		public async Task<IActionResult> Ajouter(PatientViewModel model, IFormFile? photo)
		{
			try
			{
				Medecin? medecin = await _dbContext.Users.FirstOrDefaultAsync(m => m.Id == model.IdMedecin);

				if (ModelState.IsValid)
				{
					var patient = new Patient
					{
						Nom = model.patient.Nom,
						Prenom = model.patient.Prenom,
						NuméroSécuritéSociale = model.patient.NuméroSécuritéSociale,
						DateNaissance = model.patient.DateNaissance,
						Taille = model.patient.Taille,
						Poids = model.patient.Poids,
						Adresse = model.patient.Adresse,
						Ville = model.patient.Ville,
						Sexe = model.patient.Sexe,
						MedecinID = model.IdMedecin,
						medecin = medecin,
					};

					if (photo != null && photo.Length > 0)
					{
						using (var memoryStream = new MemoryStream())
						{
							await photo.CopyToAsync(memoryStream);
							patient.Photo = memoryStream.ToArray();
						}
					}

					if (model.SelectedAllergieIds != null)
					{
						var selectedAllergies = await _dbContext.Allergies
							.Where(a => model.SelectedAllergieIds.Contains(a.AllergieId))
							.ToListAsync();
						foreach (var allergie in selectedAllergies)
						{
							patient.Allergies.Add(allergie);
						}
					}

					if (model.SelectedAntecedentIds != null)
					{
						var selectedAntecedents = await _dbContext.Antecedents
							.Where(a => model.SelectedAntecedentIds.Contains(a.AntecedentId))
							.ToListAsync();
						foreach (var antecedent in selectedAntecedents)
						{
							patient.Antecedents.Add(antecedent);
						}
					}

					await _dbContext.Patients.AddAsync(patient);
					await _dbContext.SaveChangesAsync();
					TempData["SuccessMessage"] = "Le patient a été ajouté avec succès.";
					return RedirectToAction("Index", "Patient");
				}

				return View(model);
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la mise à jour de la base de données.");
				TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'ajout du patient. Veuillez réessayer.";
				return RedirectToAction("Ajouter", new { id = model.IdMedecin });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de l'ajout du patient.");
				TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
				return RedirectToAction("Ajouter", new { id = model.IdMedecin });
			}
		}



		public async Task<IActionResult> Supprimer(int id)
		{
			try
			{
				Patient? patientToDelete = await _dbContext.Patients.FindAsync(id);
				if (patientToDelete != null)
				{
					_dbContext.Patients.Remove(patientToDelete);
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", "Patient");
				}
				return NotFound();
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la suppression du patient.");
				TempData["ErrorMessage"] = "Une erreur s'est produite lors de la suppression du patient. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de la suppression du patient.");
				TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
		}


		[HttpGet]
		public async Task<IActionResult> Editer(int id)
		{
			try
			{
				Patient? patient = await _dbContext.Patients
													.Include(p => p.Allergies)
													.Include(p => p.Antecedents)
													.FirstOrDefaultAsync(p => p.PatientId == id);
				var user = await _userManager.GetUserAsync(User);

				if (patient == null || user == null)
				{
					return NotFound();
				}

				string idMedecin = user.Id;

				var viewModel = new PatientViewModel
				{
					IdMedecin = idMedecin,
					patient = patient,
					Allergies = await _dbContext.Allergies.ToListAsync(),
					Antecedents = await _dbContext.Antecedents.ToListAsync(),
					SelectedAllergieIds = patient.Allergies.Select(a => a.AllergieId).ToList() ?? new List<int>(),
					SelectedAntecedentIds = patient.Antecedents.Select(a => a.AntecedentId).ToList() ?? new List<int>()
				};
				return View(viewModel);
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la récupération des informations du patient.");
				TempData["ErrorMessage"] = "Une erreur s'est produite lors de la récupération des informations du patient. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de la récupération des informations du patient.");
				TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
		}


		[HttpPost]
		public async Task<IActionResult> Editer(int id, PatientViewModel viewModel)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var patient = await _dbContext.Patients
						.Include(p => p.Antecedents)
						.Include(p => p.Allergies)
						.FirstOrDefaultAsync(p => p.PatientId == id);

					if (patient == null)
						return NotFound();

					patient.Nom = viewModel.patient.Nom;
					patient.Prenom = viewModel.patient.Prenom;
					patient.Adresse = viewModel.patient.Adresse;
					patient.DateNaissance = viewModel.patient.DateNaissance;
					patient.Ville = viewModel.patient.Ville;
					patient.Sexe = viewModel.patient.Sexe;
					patient.NuméroSécuritéSociale = viewModel.patient.NuméroSécuritéSociale;
					patient.Poids = viewModel.patient.Poids;
					patient.Taille = viewModel.patient.Taille;
					patient.Photo = viewModel.patient.Photo;

					patient.Allergies.Clear();
					if (viewModel.SelectedAllergieIds != null)
					{
						var selectedAllergies = await _dbContext.Allergies
							.Where(a => viewModel.SelectedAllergieIds.Contains(a.AllergieId))
							.ToListAsync();
						foreach (var allergie in selectedAllergies)
						{
							patient.Allergies.Add(allergie);
						}
					}

					patient.Antecedents.Clear();
					if (viewModel.SelectedAntecedentIds != null)
					{
						var selectedAntecedents = await _dbContext.Antecedents
							.Where(a => viewModel.SelectedAntecedentIds.Contains(a.AntecedentId))
							.ToListAsync();
						foreach (var antecedent in selectedAntecedents)
						{
							patient.Antecedents.Add(antecedent);
						}
					}

					_dbContext.Entry(patient).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();

					return RedirectToAction("Index", "Patient");
				}
				return View(viewModel);
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la mise à jour du patient.");
				TempData["ErrorMessage"] = "Une erreur s'est produite lors de la mise à jour du patient. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de la mise à jour du patient.");
				TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
		}



		public async Task<IActionResult> Detail(int id)
		{
			try
			{
				Patient? patient = await _dbContext.Patients
					.Include(p => p.Ordonnances)
						.ThenInclude(o => o.Medicaments)
					.Include(p => p.Allergies)
					.Include(p => p.Antecedents)
					.FirstOrDefaultAsync(p => p.PatientId == id);

				if (patient == null)
				{
					return NotFound(); 
				}

				return View(patient);
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la récupération des détails du patient.");
				TempData["ErrorMessage"] = "Une erreur s'est produite lors de la récupération des détails. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de la récupération des détails du patient.");
				TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
				return RedirectToAction("Index", "Patient");
			}
		}


		public async Task<IActionResult> GetPhoto(int id)
		{
			try
			{
				var patient = await _dbContext.Patients.FindAsync(id);
				if (patient == null || patient.Photo == null)
				{
					return NotFound(); 
				}

				return File(patient.Photo, "image/jpeg"); 
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "Une erreur s'est produite lors de la récupération de la photo du patient.");
				return RedirectToAction("Error"); 
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Une erreur inattendue est survenue lors de la récupération de la photo du patient.");
				return RedirectToAction("Error"); 
			}
		}

	}
}

