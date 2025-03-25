using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public RegisterModel(IFirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Password != ConfirmPassword)
            {
                Message = "Passwords do not match!";
                return Page();
            }

            try
            {
                await _firebaseAuthService.RegisterUserAsync(Email, Password);
                Message = "Registration successful! Please check your email to verify your account.";
            }
            catch (Exception ex)
            {
                Message = $"Registration failed: {ex.Message}";
            }

            return Page();
        }

    }
}
