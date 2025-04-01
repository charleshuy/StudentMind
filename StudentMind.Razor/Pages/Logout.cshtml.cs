using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        Response.Cookies.Delete("JWT_Token");

        return RedirectToPage("/Login");
    }
}
