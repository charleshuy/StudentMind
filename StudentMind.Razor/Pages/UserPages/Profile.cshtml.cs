using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;
using StudentMind.Services.Services;

namespace StudentMind.Razor.Pages.UserPages
{
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseAuthService _firebaseAuthService;

        public ProfileModel(IUserService userService, IUnitOfWork unitOfWork, IFirebaseAuthService firebaseAuthService)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
            _firebaseAuthService = firebaseAuthService;
        }

        public UserDTO? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CurrentUser = await _userService.GetProfileAsync();
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login"); // Redirect if not logged in
            }

            var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(CurrentUser?.RoleId);
            ViewData["RoleName"] = role?.RoleName;

            return Page();
        }

        public async Task<IActionResult> OnPostEditProfileAsync(string? fullName, string? username, string email, string? gender)
        {
            CurrentUser = await _userService.GetProfileAsync();
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login"); // Redirect if not logged in
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data, please check the fields.";
                return RedirectToPage();
            }

            var userRequestDto = new UserRequestDTO
            {
                FullName = fullName,
                Username = username,
                Email = email,
                Gender = gender,
                Status = true,
                RoleId = CurrentUser.RoleId // The role doesn't change here
            };

            var updated = await _userService.UpdateUserAsync(CurrentUser.Id, userRequestDto);

            if (!updated)
            {
                // Handle failure, provide a more detailed message if needed
                TempData["ErrorMessage"] = "Failed to update the profile. Please try again.";
                return RedirectToPage();
            }

            // Re-fetch updated user info
            CurrentUser = await _userService.GetProfileAsync();

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToPage(); // Redirect to reload the profile page with updated data
        }

        public async Task<IActionResult> OnPostRequestPasswordResetAsync(string resetEmail)
        {
            // Validate if the email is associated with a registered user
            if (string.IsNullOrEmpty(resetEmail))
            {
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToPage(); // Re-render the page
            }

            try
            {
                // Call the Firebase service to send the password reset email
                await _firebaseAuthService.SendPasswordResetEmailAsync(resetEmail);

                TempData["SuccessMessage"] = "Password reset email sent. Please check your inbox.";
                return RedirectToPage(); // Re-render the page
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to send reset email: {ex.Message}";
                return RedirectToPage();
            }
        }
    }
 }
