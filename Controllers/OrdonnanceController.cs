using MedManager.Data;
using MedManager.Models;
using MedManager.ViewModel.OrdonnanceVM;
using MedManager.ViewModel.PatientVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Data.Common;
using X.PagedList;

namespace MedManager.Controllers
{
    [Authorize]
    public class OrdonnanceController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly UserManager<Medecin> _userManager;
		private readonly ILogger<Patient> _logger;
		private readonly string _RepertoirePdf = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");

		public OrdonnanceController(ApplicationDbContext dbContext, UserManager<Medecin> userManager, ILogger<Patient> logger)
		{
			_logger = logger;
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index(int? page, string filtre)
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
				if (!string.IsNullOrEmpty(filtre))
				{
					ordonnances = ordonnances.Where(p => p.Patient.Nom.ToUpper().Contains(filtre.ToUpper()) || p.Patient.Prenom.ToUpper().Contains(filtre.ToUpper()));
				}

				int TaillePage = 9;
				int NombrePage = (page ?? 1);
				var ListePageeDesOrdonnances = ordonnances.ToPagedList(NombrePage, TaillePage);

				var viewModel = new IndexOrdonnanceViewModel
				{
					medecin = medecin,
					Ordonnances = ListePageeDesOrdonnances
                };


				ViewData["FiltreActuel"] = filtre;

				return View(viewModel);
			}
            catch (DbException ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération des données.");
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la récupération des données.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite.");
                TempData["ErrorMessage"] = "Une erreur inattendue s'est produite.";
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
					return RedirectToAction("Connexion", "Compte");
				}

				var medecin = await _dbContext.Users
					.Include(u => u.Patients)
					.FirstOrDefaultAsync(u => u.Id == MedecinId);
				if (medecin == null)
					return NotFound();

				var viewModel = new OrdonnanceViewModel
				{
					Medicaments = await _dbContext.Medicaments.ToListAsync(),
					patients = medecin.Patients,
                    MedicamentIdSelectionnes = new List<int>(),
                    PatientId = 0,
				};
				return View("Action", viewModel);
			}
            catch (DbException ex)
            {
                _logger.LogError(ex, "Une erreur s'est produite lors de la récupération des données.");
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la récupération des données.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite.");
                TempData["ErrorMessage"] = "Une erreur inattendue s'est produite.";
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter(OrdonnanceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var MedecinId = _userManager.GetUserId(User);
                    if (MedecinId == null)
                    {
                        return RedirectToAction("Connexion", "Compte");
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

                    if (model.MedicamentIdSelectionnes != null && model.MedicamentIdSelectionnes.Count > 0)
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
                    TempData["SuccessMessage"] = "L'ordonnance a été ajoutée avec succès.";
                    return RedirectToAction("Index", "Ordonnance");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Erreur lors de l'ajout de l'ordonnance.");
                    TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'ajout de l'ordonnance. Veuillez réessayer.";
                    return RedirectToAction("Index", "Ordonnance");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'ajout de l'ordonnance.");
                    TempData["ErrorMessage"] = "Une erreur inattendue s'est produite. Veuillez réessayer.";
                    return RedirectToAction("Index", "Ordonnance");
                }
            }
            model.Medicaments = await _dbContext.Medicaments.ToListAsync();
            model.patients = await _dbContext.Users
                                             .Include(u => u.Patients)
                                             .Where(u => u.Id == _userManager.GetUserId(User))
                                             .SelectMany(u => u.Patients)
                                             .ToListAsync();
            return View("Action", model);
        }


        [HttpGet]
        public async Task<IActionResult> Editer(int id)
        {
            try
            {
                var MedecinId = _userManager.GetUserId(User);
                if (MedecinId == null)
                {
                    return RedirectToAction("Connexion", "Compte");
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
                _logger.LogError(ex, "Erreur lors de la récupération de l'ordonnance avec ID {OrdonnanceId}.", id);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la récupération des informations de l'ordonnance. Veuillez réessayer.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'édition de l'ordonnance avec ID {OrdonnanceId}.", id);
                TempData["ErrorMessage"] = "Une erreur inattendue s'est produite lors de l'édition de l'ordonnance. Veuillez réessayer.";
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
                    return RedirectToAction("Connexion", "Compte");
                }

                if (ModelState.IsValid)
                {
                    var ordonnance = await _dbContext.Ordonnances
                        .Include(p => p.Medicaments)
                        .Include(p => p.Patient)
                        .FirstOrDefaultAsync(o => o.OrdonnanceId == model.OrdonnanceId);
                    var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.PatientId == model.PatientId);

                    if (ordonnance == null || patient == null)
                    {
                        return NotFound();
                    }

                    ordonnance.DateDebut = model.DateDebut;
                    ordonnance.DateFin = model.DateFin;
                    ordonnance.Patient = patient;
                    ordonnance.InfoSupplementaire = model.InfoSupplementaire;

                    ordonnance.Medicaments.Clear();
                    if (model.MedicamentIdSelectionnes != null)
                    {
                        var MedicamentSelectionne = await _dbContext.Medicaments
                            .Where(m => model.MedicamentIdSelectionnes.Contains(m.MedicamentId))
                            .ToListAsync();

                        foreach (var medicament in MedicamentSelectionne)
                        {
                            ordonnance.Medicaments.Add(medicament);
                        }
                    }
                    _dbContext.Entry(ordonnance).State = EntityState.Modified;
                    model.Medicaments = await _dbContext.Medicaments.ToListAsync();
                    await _dbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "L'ordonnance a été modifiée avec succès.";
                    return RedirectToAction("Index", "Ordonnance");
                }
                else
                {
                    TempData["ErrorMessage"] = "Les données soumises ne sont pas valides. Veuillez vérifier et réessayer.";
                    return RedirectToAction("Error");
                }
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de l'ordonnance avec ID {OrdonnanceId}.", model.OrdonnanceId);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la mise à jour de l'ordonnance. Veuillez réessayer.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de l'édition de l'ordonnance avec ID {OrdonnanceId}.", model.OrdonnanceId); 
                TempData["ErrorMessage"] = "Une erreur inattendue s'est produite lors de l'édition de l'ordonnance. Veuillez réessayer.";
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
                    TempData["SuccessMessage"] = "L'ordonnance a été supprimée avec succès.";
                    return RedirectToAction("Index", "Ordonnance");
				}
				return NotFound();
			}
            catch (DbException ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppresion de l'ordonnance avec ID {OrdonnanceId}.", id);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la mise à jour de l'ordonnance. Veuillez réessayer.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue s'est produite lors de la suppresion de l'ordonnance avec ID {OrdonnanceId}.", id);
                TempData["ErrorMessage"] = "Une erreur inattendue s'est produite lors de l'édition de l'ordonnance. Veuillez réessayer.";
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
					return RedirectToAction("Connexion", "Compte");
				}

				var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
				var medecin = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == MedecinId);
				var ordonnance = await _dbContext.Ordonnances
					.Include(o => o.Medicaments)
					.FirstOrDefaultAsync(o => o.OrdonnanceId == ordonnanceId);

				if (patient != null && medecin != null && ordonnance != null)
				{
					string fileName = $"Ordonnance_{patient.NomComplet}_{ordonnanceId}.pdf";
					string filePath = Path.Combine(_RepertoirePdf, fileName);

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
                _logger.LogError(ex, "Une erreur est survenue lors de la génération du PDF pour l'ordonnance ID {OrdonnanceId} et le patient ID {PatientId}.", ordonnanceId, patientId);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de la génération du PDF. Veuillez réessayer.";
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur inattendue est survenue lors de la génération du PDF pour l'ordonnance ID {OrdonnanceId}.", ordonnanceId);
                TempData["ErrorMessage"] = "Une erreur inattendue est survenue. Veuillez réessayer.";
                return RedirectToAction("Error");
            }
        }
	}
}

