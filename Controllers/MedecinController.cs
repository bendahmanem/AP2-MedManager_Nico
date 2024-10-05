using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;
using MedManager.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using MedManager.ViewModel.MedecinVM;

namespace MedManager.Controllers;

public class MedecinController : Controller
{
    private readonly ILogger<Medecin> _logger;
    private readonly UserManager<Medecin> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public MedecinController(ILogger<Medecin> logger, UserManager<Medecin> userManager, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index(int? page)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string id = user.Id;

            // Récupération du médecin et des données associées
            Medecin? medecin = await _dbContext.Users
                                    .Include(u => u.Patients)
                                        .ThenInclude(p => p.Allergies)
                                    .Include(u => u.Ordonnances)
                                    .FirstOrDefaultAsync(p => p.Id == id);

            if (medecin == null)
            {
                return RedirectToAction("Error");
            }

            // Gestion de la pagination des patients
            int pageSize = 10; // Nombre de patients par page
            int pageNumber = (page ?? 1);
            var patientsPagedList = medecin.Patients.ToPagedList(pageNumber, pageSize);

            // Créer un ViewModel pour passer les données à la vue
            var viewModel = new MedecinViewModel
            {
                medecin = medecin,
                Patients = patientsPagedList // Liste paginée de patients
            };

            return View(viewModel);
        }
        catch (DbUpdateException ex)
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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
