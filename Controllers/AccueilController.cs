using Microsoft.AspNetCore.Mvc;

namespace MedManager.Controllers
{
	public class AccueilController : Controller
	{

		public IActionResult Index()
		{
			return View();
		}
	}
}
