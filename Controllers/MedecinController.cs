using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;

namespace MedManager.Controllers;

public class MedecinController : Controller
{
    private readonly ILogger<Medecin> _logger;
    private readonly UserManager<Medecin> _userManager;

    public MedecinController(ILogger<Medecin> logger, UserManager<Medecin> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    //public IActionResult Index()
    //{
    //    return View();
    //}
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        return View(user);
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
