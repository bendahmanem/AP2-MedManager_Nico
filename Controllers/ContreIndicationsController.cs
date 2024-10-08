using Microsoft.AspNetCore.Mvc;
using MedManager.Data;
using MedManager.Models;
using Microsoft.EntityFrameworkCore;

namespace MedManager.Controllers
{
	public class ContreIndicationsController : Controller
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly ILogger<Patient> _logger;

		public ContreIndicationsController(ApplicationDbContext dbContext, ILogger<Patient> logger)
		{
			_dbContext = dbContext;
			_logger = logger;
		}
		public async Task<IActionResult> Index()
		{
			List<Allergie> allergies = await _dbContext.Allergies.ToListAsync();
			List<Antecedent> antecedents = await _dbContext.Antecedents.ToListAsync();

			var model = (Allergies: allergies, Antecedents: antecedents);

			return View(model);
		}

		public async Task<IActionResult> SupprimerAllergie(int id)
		{
			try
			{
				Allergie? AllergieToDelete = await _dbContext.Allergies.FindAsync(id);

				if (AllergieToDelete != null)
				{
					_dbContext.Allergies.Remove(AllergieToDelete);
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", "ContreIndications");
				}
				return NotFound();
			}
			catch(DbUpdateException ex)
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

		public async Task<IActionResult> SupprimerAntecedent(int id)
		{
			try
			{
				Antecedent? AntecedentToDelete = await _dbContext.Antecedents.FindAsync(id);

				if (AntecedentToDelete != null)
				{
					_dbContext.Antecedents.Remove(AntecedentToDelete);
					await _dbContext.SaveChangesAsync();
					return RedirectToAction("Index", "ContreIndications");
				}
				return NotFound();
			}
			catch (DbUpdateException ex)
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
