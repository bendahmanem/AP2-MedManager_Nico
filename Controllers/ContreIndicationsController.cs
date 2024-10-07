using Microsoft.AspNetCore.Mvc;
using MedManager.Data;
using MedManager.Models;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Controllers
{
	public class ContreIndicationsController : Controller
	{
		private readonly ApplicationDbContext _dbContext;

		public ContreIndicationsController(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<IActionResult> Index()
		{
			List<Allergie> allergies = await _dbContext.Allergies.ToListAsync();
			List<Antecedent> antecedents = await _dbContext.Antecedents.ToListAsync();

			var model = (Allergies: allergies, Antecedents: antecedents);

			return View(model);
		}
	}
}
