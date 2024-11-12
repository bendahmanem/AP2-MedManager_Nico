using MedManager.Data;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

		public IActionResult Index()
		{
			var medecin = _userManager.GetUserAsync(User);
			return View(medecin);
		}
	}
}

