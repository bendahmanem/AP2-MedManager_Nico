using MedManager.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MedManager.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult Index()
		{
			return View("~/Views/Shared/Error.cshtml",
				new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

