using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;

namespace MedManager.Controllers;

public class MedecinController : Controller
{
    private readonly ILogger<Medecin> _logger;

    public MedecinController(ILogger<Medecin> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        
        return View();
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
