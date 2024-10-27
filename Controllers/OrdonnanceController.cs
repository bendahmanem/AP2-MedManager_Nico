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

                OrdonnanceViewModel model = new OrdonnanceViewModel
                {

                };

                return RedirectToAction("Action", model);
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
    }
}
