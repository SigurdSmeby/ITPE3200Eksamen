using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Sub_Application_1.Views.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {   
            // Sign out the user
            await HttpContext.SignOutAsync();
            return RedirectToPage("/Account/Login");
        }
    }
}

