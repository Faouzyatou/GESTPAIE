using Microsoft.AspNetCore.Mvc;

namespace PayrollApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
