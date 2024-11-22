using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MedManager.Models;
using Microsoft.AspNetCore.Identity;
using MedManager.Data;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using MedManager.ViewModel;
using MedManager.ViewModel.MedecinVM;
using Microsoft.AspNetCore.Authorization;

namespace MedManager.Controllers;

[Authorize]
public class Tableaudebord : Controller
{
	private readonly ILogger<Medecin> _logger;
	private readonly UserManager<Medecin> _userManager;
	private readonly ApplicationDbContext _dbContext;

	public Tableaudebord(ILogger<Medecin> logger, UserManager<Medecin> userManager, ApplicationDbContext dbContext)
	{
		_logger = logger;
		_userManager = userManager;
		_dbContext = dbContext;
	}


	public async Task<IActionResult> Index()
	{
		var medocs = await ObtenirMedicamentLesPlusUtilises();
		var frequenceAllergies = await ObtenirAllergiesLesPlusFrequentes();
		var frequenceAntecedents = await ObtenirAntecedentsLesPlusFrequentes();
		var repartitionAge = await ObtenirRepartitionAge();
		var CinqDerniersPatient = await ObtenirCinqDerniersPatients();
		var CinqDernieresOrdo = await ObtenirCinqDerniersOrdonnances();
		var TotalPatient = await ObtenirNombrePatient();
		var TotalOrdo = await ObtenirNombreOrdonnance();
		var OrdoEnCours = await ObtenirOrdonnanceEncours();

		var model = new TableauDeBordViewModel
		{
			FrequenceAllergies = frequenceAllergies,
			FrequenceAntecedents = frequenceAntecedents,
			RepartitionAges = repartitionAge,
			MedicamentPlusUtilises = medocs,
			CinqDerniersOrdo = CinqDernieresOrdo,
			CinqDerniersPatient = CinqDerniersPatient,
			TotalOrdonnance = TotalOrdo,
			TotalPatient = TotalPatient,
			OrdonnanceEnCours = OrdoEnCours
		};
		return View(model);
	}


	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}

	private async Task<string?> ObtenirIdMedecin()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user != null)
		{
			string id = user.Id;
			return id;
		}
		return null;
	}

	private async Task<int> ObtenirNombrePatient()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{
			var NombrePatients = await _dbContext.Patients
									.Where(p => p.MedecinID == id)
									.CountAsync();
			return NombrePatients;
		}
		return 0;
	}

	private async Task<int> ObtenirNombreOrdonnance()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{
			var NombreOrdo = await _dbContext.Ordonnances
									.Where(o => o.MedecinId == id)
									.CountAsync();
			return NombreOrdo;
		}
		return 0;
	}

	private async Task<List<Ordonnance>> ObtenirOrdonnanceEncours()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{

			var ordonnances = await _dbContext.Ordonnances
							.Where(o => o.DateFin >= DateTime.Now && o.DateDebut <= DateTime.Now && o.MedecinId == id).ToListAsync();
			return ordonnances;
		}

		return new List<Ordonnance>();
	}


	private async Task<List<MedicamentUtilisationViewModel>> ObtenirMedicamentLesPlusUtilises()
	{
		var id = await ObtenirIdMedecin();

		if (id != null)
		{
			var ordonnances = await _dbContext.Ordonnances
								.Include(o => o.Medicaments)
								.Where(o => o.MedecinId == id)
								.ToListAsync();

			var medicaments = ordonnances
								.SelectMany(o => o.Medicaments)
								.GroupBy(m => m.MedicamentId)
								.Select(g => new MedicamentUtilisationViewModel
								{
									Nom = g.First().Nom,
									UtilisationCount = g.Count()
								})
								.OrderByDescending(m => m.UtilisationCount)
								.Take(5)
								.ToList();

			return medicaments;
		}
		return new List<MedicamentUtilisationViewModel>();
	}

	private async Task<List<Frequence>> ObtenirAllergiesLesPlusFrequentes()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{
			var patients = await _dbContext.Patients
							.Include(p => p.Allergies)
							.Where(p => p.MedecinID == id)
							.ToListAsync();

			var allergies = patients
				.SelectMany(p => p.Allergies)
				.GroupBy(a => a.AllergieId)
				.Select(g => new Frequence
				{
					nom = g.First().Nom,
					compte = g.Count()
				})
				.OrderByDescending(a => a.compte)
				.Take(5)
				.ToList();
			return allergies;
		}
		return new List<Frequence>();
	}

	private async Task<List<Frequence>> ObtenirAntecedentsLesPlusFrequentes()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{
			var patients = await _dbContext.Patients
							.Include(p => p.Antecedents)
							.Where(p => p.MedecinID == id)
							.ToListAsync();

			var antecedents = patients
				.SelectMany(p => p.Antecedents)
				.GroupBy(a => a.AntecedentId)
				.Select(g => new Frequence
				{
					nom = g.First().Nom,
					compte = g.Count()
				})
				.OrderByDescending(a => a.compte)
				.Take(5)
				.ToList();
			return antecedents;
		}
		return new List<Frequence>();
	}

	private async Task<List<RepartitionAge>> ObtenirRepartitionAge()
	{
		var id = await ObtenirIdMedecin();
		if (id != null)
		{
			var patients = await _dbContext.Patients
							.Include(p => p.Antecedents)
							.Where(p => p.MedecinID == id)
							.ToListAsync();
			var ageCategoriesOrder = new Dictionary<string, int>
		{
			{ "0-20 ans", 1 },
			{ "20-40 ans", 2 },
			{ "40-60 ans", 3 },
			{ "60-80 ans", 4 },
			{ "80 ans et plus", 5 }
		};

			var repartition = patients
				.GroupBy(p =>
					p.Age < 20 ? "0-20 ans" :
					p.Age <= 40 ? "20-40 ans" :
					p.Age <= 60 ? "40-60 ans" :
					p.Age <= 80 ? "60-80 ans" :
					"80 ans et plus")
				.Select(g => new RepartitionAge
				{
					categorie = g.Key,

					compte = g.Count()
				})
				.OrderBy(r => ageCategoriesOrder[r.categorie])
				.ToList();

			return repartition;
		}
		return new List<RepartitionAge>();
	}


	private async Task<List<Patient>> ObtenirCinqDerniersPatients()
	{
		var id = await ObtenirIdMedecin();

		if (id != null)
		{
			var patients = await _dbContext.Patients
							.Where(p => p.MedecinID == id)
							.OrderByDescending(p => p.DateCreation)
							.Take(5)
							.ToListAsync();
			return patients;
		}
		return new List<Patient>();
	}

	private async Task<List<Ordonnance>> ObtenirCinqDerniersOrdonnances()
	{
		var id = await ObtenirIdMedecin();

		if (id != null)
		{
			var ordo = await _dbContext.Ordonnances
							.Where(o => o.MedecinId == id)
							.OrderByDescending(p => p.DateCreation)
							.Take(5)
							.ToListAsync();
			return ordo;
		}
		return new List<Ordonnance>();
	}
}
