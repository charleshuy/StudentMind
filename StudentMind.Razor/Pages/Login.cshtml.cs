using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.Interfaces;
using System;

namespace StudentMind.Razor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public LoginModel(IFirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
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
                var jwtToken = await _firebaseAuthService.SignInWithEmailPasswordAsync(Email, Password);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,   // Prevent access via JavaScript
                    Secure = true,     // Only send over HTTPS
                    Expires = DateTime.UtcNow.AddHours(2),
                    SameSite = SameSiteMode.Strict // Prevent CSRF attacks
                };

                Response.Cookies.Append("JWT_Token", jwtToken, cookieOptions);

                return RedirectToPage("/Index"); // Redirect after login
            }
            catch (Exception ex)
            {
                Message = $"Login failed: {ex.Message}";
                return Page();
            }
        }
    }
}
