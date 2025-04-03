using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.UserPages
{
    [Authorize(AuthenticationSchemes = "Jwt", Policy = "RequireAdmin")]
    public class CreateModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _roleService;

        public CreateModel(IUserService userService, IUnitOfWork roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var roles = _roleService.GetRepository<Role>().Entities;
            ViewData["RoleId"] = new SelectList(roles, "Id", "RoleName");
            return Page();
        }

        [BindProperty]
        public UserRequestDTO User { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _userService.CreateUserAsync(User);
            return RedirectToPage("./Index");
        }
    }
}
