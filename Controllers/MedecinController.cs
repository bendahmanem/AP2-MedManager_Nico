using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;
using MedManager.Data;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);


        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        else
        {
            string id = user.Id;

           
            Medecin? medecin = await _dbContext.Users
                                        .Include(u => u.Patients)
                                        .Include(u => u.Ordonnances)
                                        .FirstOrDefaultAsync(p => p.Id == id);

            if (medecin == null)
            {
                return RedirectToAction("Error");
            }

            return View(user);
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
