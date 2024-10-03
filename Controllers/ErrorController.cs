using Microsoft.AspNetCore.Mvc;

namespace MedManager.Controllers
{
    public class ErrorController : Controller   
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
