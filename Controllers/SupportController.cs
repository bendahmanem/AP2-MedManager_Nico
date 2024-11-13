using Microsoft.AspNetCore.Mvc;

namespace MedManager.Controllers
{
	public class SupportController : Controller
	{
		public IActionResult Contact()
		{
			return View();
		}

		public IActionResult FAQ()
		{
			return View();
		}
	}
}
