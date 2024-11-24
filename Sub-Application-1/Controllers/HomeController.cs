using Microsoft.AspNetCore.Mvc;

namespace Sub_Application_1.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index or /
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Optional: About page
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
    }
}
