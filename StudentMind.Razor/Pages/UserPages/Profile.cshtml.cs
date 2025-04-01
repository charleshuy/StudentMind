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
            var role = await _unitOfWork.GetRepository<Role>().GetByIdAsync(CurrentUser?.RoleId);
            ViewData["RoleName"] = role?.RoleName;
            if (CurrentUser == null)
            {
                return RedirectToPage("/Login"); // Redirect if not logged in
            }

            return Page();
        }
    }
}
