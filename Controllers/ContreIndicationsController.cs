using Microsoft.AspNetCore.Mvc;
using MedManager.Data;
using MedManager.Models;
using MedManager.ViewModel;
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

        public async Task<IActionResult> Ajouter(string type)
        {
            var viewModel = new ContreIndicationViewModel { Type = type };
            return View("Action", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Ajouter(ContreIndicationViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                if (viewModel.Type == "Allergie")
                {
                    Allergie allergie = new Allergie
                    {
                        Nom = viewModel.Nom
                    };

                    await _dbContext.Allergies.AddAsync(allergie);
   
                }
                else if(viewModel.Type == "Antecedent")
                {
                    Antecedent antecedent = new Antecedent
                    {
                        Nom = viewModel.Nom
                    };
                    await _dbContext.Antecedents.AddAsync(antecedent);
                }
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
            }
            return NotFound();
        }

        public async Task<IActionResult> Modifier(int id, string type)
        {
            if (type == "Allergie")
            {
                var allergie = await _dbContext.Allergies.FindAsync(id);
                if (allergie == null) 
                    return NotFound();
                var viewModel = new ContreIndicationViewModel
                {
                    Id = allergie.AllergieId,
                    Nom = allergie.Nom,
                    Type = "Allergie"
                };
                return View("Action", viewModel);
            }
            else if (type == "Antecedent")
            {
                var antecedent = await _dbContext.Antecedents.FindAsync(id);
                if (antecedent == null) 
                    return NotFound();
                var viewModel = new ContreIndicationViewModel
                {
                    Id = antecedent.AntecedentId,
                    Nom = antecedent.Nom,
                    Type = "Antecedent"
                };
                return View("Action", viewModel);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Modifier(ContreIndicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Type == "Allergie")
                {
                    var allergie = await _dbContext.Allergies.FindAsync(model.Id);
                    if (allergie == null) 
                        return NotFound();
                    allergie.Nom = model.Nom;
                }
                else if (model.Type == "Antecedent")
                {
                    var antecedent = await _dbContext.Antecedents.FindAsync(model.Id);
                    if (antecedent == null) 
                        return NotFound();
                    antecedent.Nom = model.Nom;
                }
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View("Action", model);
        }

        public async Task<IActionResult> Supprimer(int id, string type)
        {
            if (type == "Allergie")
            {
                var allergie = await _dbContext.Allergies.FindAsync(id);
                if (allergie != null) 
                    _dbContext.Allergies.Remove(allergie);
            }
            else if (type == "Antecedent")
            {
                var antecedent = await _dbContext.Antecedents.FindAsync(id);
                if (antecedent != null) 
                    _dbContext.Antecedents.Remove(antecedent);
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
