using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.Interfaces;

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
                HttpContext.Session.SetString("JWT_Token", jwtToken);
                return RedirectToPage("/Index");  // Redirect to the homepage or any authorized page
            }
            catch (Exception ex)
            {
                Message = $"Login failed: {ex.Message}";
                return Page();
            }
        }
    }
}
