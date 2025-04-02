using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public ForgotPasswordModel(IFirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
        }

        [BindProperty]
        public string Email { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return Page();
            }

            try
            {
                // Send password reset email using Firebase service
                await _firebaseAuthService.SendPasswordResetEmailAsync(Email);

                TempData["SuccessMessage"] = "Password reset email sent. Please check your inbox.";
                return RedirectToPage("/ForgotPassword"); // Keep user on the same page after success
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send reset email: {ex.Message}";
                return Page();
            }
        }
    }
}
