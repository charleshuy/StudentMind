using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages
{
    public class AdminLoginModel : PageModel
    {
        private readonly IFirebaseAuthService _adminAuthService;

        public AdminLoginModel(IFirebaseAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string? Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Message = "Invalid input.";
                return Page();
            }

            try
            {
                var jwtToken = await _adminAuthService.AdminLoginAsync(Email, Password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,   // Prevent access via JavaScript
                    Secure = true,     // Only send over HTTPS
                    Expires = DateTime.UtcNow.AddHours(2),
                    SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                };

                Response.Cookies.Append("JWT_Token", jwtToken, cookieOptions);

                return RedirectToPage("/UserPages/index"); // Redirect to admin dashboard after login
            }
            catch (Exception ex)
            {
                Message = $"Login failed: {ex.Message}";
                return Page();
            }
        }
    }
}
