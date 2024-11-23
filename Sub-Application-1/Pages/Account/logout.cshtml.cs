using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace server.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}

