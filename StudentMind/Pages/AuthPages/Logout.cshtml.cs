using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentMind.Pages.AuthPages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            // Clear the authentication cookie
            Response.Cookies.Delete("AuthToken");

            // Redirect to home page after logout
            return RedirectToPage("/Index");
        }
    }
}
