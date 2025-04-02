using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.UserPages
{
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileModel(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
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

        public async Task<IActionResult> OnPostEditProfileAsync(string? fullName, string? username, string email, string? gender, bool status)
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
                Status = status,
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
    }
}
