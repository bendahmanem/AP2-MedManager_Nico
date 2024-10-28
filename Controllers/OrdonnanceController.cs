using MedManager.Data;
using MedManager.Models;
using MedManager.ViewModel.OrdonnanceVM;
using MedManager.ViewModel.PatientVM;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Data.Common;
using X.PagedList;

namespace MedManager.Controllers
{
	public class OrdonnanceController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<Medecin> _userManager;
		private readonly ILogger<Patient> _logger;
		private readonly string _pdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");

		public OrdonnanceController(ApplicationDbContext dbContext, UserManager<Medecin> userManager, ILogger<Patient> logger)
		{
			_logger = logger;
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index(int? page, string searchString)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return RedirectToAction("Login", "Account");
				}

				string id = user.Id;

				Medecin? medecin = await _dbContext.Users
					.Include(u => u.Ordonnances)
						.ThenInclude(o => o.Patient)
					.Include(u => u.Ordonnances)
						.ThenInclude(o => o.Medicaments)
					.FirstOrDefaultAsync(m => m.Id == id);

				if (medecin == null)
				{
					return RedirectToAction("Error");
				}


				var ordonnances = medecin.Ordonnances.AsQueryable();
				if (!string.IsNullOrEmpty(searchString))
				{
					ordonnances = ordonnances.Where(p => p.Patient.Nom.ToUpper().Contains(searchString.ToUpper()) || p.Patient.Prenom.ToUpper().Contains(searchString.ToUpper()));
				}

				int pageSize = 9;
				int pageNumber = (page ?? 1);
				var OrdonnancePagedList = ordonnances.ToPagedList(pageNumber, pageSize);

				var viewModel = new IndexOrdonnanceViewModel
				{
					medecin = medecin,
					Ordonnances = OrdonnancePagedList
				};


				ViewData["CurrentFilter"] = searchString;

				return View(viewModel);
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "An error occurred while updating the database.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}

		[HttpGet]
		public async Task<IActionResult> Ajouter()
		{
			try
			{
				var MedecinId = _userManager.GetUserId(User);
				if (MedecinId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				var medecin = await _dbContext.Users
					.Include(u => u.Patients)
					.FirstOrDefaultAsync(u => u.Id == MedecinId);
				if (medecin == null)
					return NotFound();

				var viewModel = new OrdonnanceViewModel
				{
					Medicaments = await _dbContext.Medicaments.ToListAsync(),
					patients = medecin.Patients
				};
				return View("Action", viewModel);
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "An error occurred while updating the database.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Ajouter(OrdonnanceViewModel model)
		{
			if (ModelState.IsValid)
			{
				var MedecinId = _userManager.GetUserId(User);
				if (MedecinId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				var medecin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == MedecinId);

				if (medecin == null)
					return NotFound();

				var ordonnance = new Ordonnance
				{
					DateDebut = model.DateDebut,
					DateFin = model.DateFin,
					InfoSupplementaire = model.InfoSupplementaire,
					Medecin = medecin,
					PatientId = model.PatientId,
					Medicaments = new List<Medicament>(),
					MedecinId = MedecinId,
				};

				if (model.MedicamentIdSelectionnes != null)
				{
					var MedicamentsSelectionnes = await _dbContext.Medicaments
							.Where(m => model.MedicamentIdSelectionnes.Contains(m.MedicamentId))
							.ToListAsync();
					foreach (var medicament in MedicamentsSelectionnes)
					{
						ordonnance.Medicaments.Add(medicament);
					}
				}

				await _dbContext.Ordonnances.AddAsync(ordonnance);
				await _dbContext.SaveChangesAsync();
				return RedirectToAction("Index", "Ordonnance");
			}
			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Editer(int id)
		{
			try
			{
				var MedecinId = _userManager.GetUserId(User);
				if (MedecinId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				Ordonnance ordonnance = await _dbContext.Ordonnances
						.Include(o => o.Medicaments)
						.Include(o => o.Patient)
						.FirstOrDefaultAsync(o => o.OrdonnanceId == id);

				if (ordonnance == null)
					return NotFound();

				List<int> MedicamentsSelectionnes = new List<int>();
				foreach (var m in ordonnance.Medicaments)
				{
					MedicamentsSelectionnes.Add(m.MedicamentId);

				}

				OrdonnanceViewModel model = new OrdonnanceViewModel
				{
					OrdonnanceId = ordonnance.OrdonnanceId,
					DateDebut = ordonnance.DateDebut,
					DateFin = ordonnance.DateFin,
					InfoSupplementaire = ordonnance.InfoSupplementaire,
					MedicamentIdSelectionnes = MedicamentsSelectionnes,
					Medicaments = await _dbContext.Medicaments.ToListAsync(),
					patients = await _dbContext.Patients.Where(p => p.medecin.Id == MedecinId).ToListAsync(),
					PatientId = ordonnance.Patient.PatientId
				};

				return View("Action", model);
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "An error occurred while updating the database.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Editer(OrdonnanceViewModel model)
		{
			try
			{
				var MedecinId = _userManager.GetUserId(User);
				if (MedecinId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				if (ModelState.IsValid)
				{
					var ordonannce = await _dbContext.Ordonnances
						.Include(p => p.Medicaments)
						.Include(p => p.Patient)
						.FirstOrDefaultAsync(o => o.OrdonnanceId == model.OrdonnanceId);
					var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

					if (ordonannce == null || patient == null)
					{
						return NotFound();
					}

					ordonannce.DateDebut = model.DateDebut;
					ordonannce.DateFin = model.DateFin;
					ordonannce.Patient = patient;
					ordonannce.InfoSupplementaire = model.InfoSupplementaire;

					ordonannce.Medicaments.Clear();
					if (model.MedicamentIdSelectionnes != null)
					{
						var MedicamentSelectionne = await _dbContext.Medicaments
							.Where(m => model.MedicamentIdSelectionnes.Contains(m.MedicamentId))
							.ToListAsync();

						foreach (var medicament in MedicamentSelectionne)
						{
							ordonannce.Medicaments.Add(medicament);
						}
					}
					_dbContext.Entry(ordonannce).State = EntityState.Modified;
					model.Medicaments = await _dbContext.Medicaments.ToListAsync();
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("index", "Ordonnance");
				}
				else
					return RedirectToAction("Error");
			}
			catch (DbException ex)
			{
				_logger.LogError(ex, "An error occurred while updating the database.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}


		public async Task<IActionResult> Supprimer(int id)
		{
			try
			{
				Ordonnance? OrdonnanceSupprimee = await _dbContext.Ordonnances.FindAsync(id);
				if (OrdonnanceSupprimee != null)
				{
					_dbContext.Ordonnances.Remove(OrdonnanceSupprimee);
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", "Ordonnance");
				}
				return NotFound();
			}
			catch (DbException ex)

			{
				_logger.LogError(ex, "An error occurred while deleting the patient.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}

		public async Task<IActionResult> GenererPdf(int ordonnanceId, int patientId)
		{
			try
			{

				var MedecinId = _userManager.GetUserId(User);
				if (MedecinId == null)
				{
					return RedirectToAction("Login", "Account");
				}

				var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
				var medecin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == MedecinId);
				var ordonnance = await _dbContext.Ordonnances
					.Include(o => o.Medicaments)
					.FirstOrDefaultAsync(o => o.OrdonnanceId == ordonnanceId);

				if (patient != null && medecin != null && ordonnance != null)
				{
					string fileName = $"Ordonnance_{patient.NomComplet}_{ordonnanceId}.pdf";
					string filePath = Path.Combine(_pdfDirectory, fileName);

					OrdonnancePdfGenerateur pdfGenerateur = new OrdonnancePdfGenerateur();

					pdfGenerateur.GenerateOrdonnance(filePath, medecin, patient, ordonnance);

					if (!System.IO.File.Exists(filePath))
					{
						return NotFound("Le fichier PDF n'a pas pu être créé.");
					}

					var fileBytes = System.IO.File.ReadAllBytes(filePath);
					return File(fileBytes, "application/pdf", fileName);
				}
				else
					return NotFound();
			}
			catch (DbException ex)

			{
				_logger.LogError(ex, "An error occurred while deleting the patient.");
				return RedirectToAction("Error");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unexpected error occurred.");
				return RedirectToAction("Error");
			}
		}
	}
}

