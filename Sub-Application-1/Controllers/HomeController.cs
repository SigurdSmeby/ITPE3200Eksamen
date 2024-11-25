using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Sub_Application_1.Models;
namespace Sub_Application_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        // GET: /Home/Index or /
        [HttpGet]
         public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get the currently logged-in user
                var user = await _userManager.GetUserAsync(User);

                // Pass the username to the view via ViewData
                ViewData["Username"] = user?.UserName;
            }
            else
            {
                // User is not logged in, ensure ViewData["Username"] is null or empty
                ViewData["Username"] = null;
            }

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
