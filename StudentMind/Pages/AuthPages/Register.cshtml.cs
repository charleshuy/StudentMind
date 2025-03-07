using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace StudentMind.Pages.AuthPages
{
    public class RegisterModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if user already exists
            var existingUser = await _unitOfWork.GetRepository<User>().Entities
                .FirstOrDefaultAsync(u => u.Email == Input.Email || u.Username == Input.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "User with this email or username already exists.");
                return Page();
            }

            // Find User Role
            var userRole = await _unitOfWork.GetRepository<Role>().Entities
                .FirstOrDefaultAsync(r => r.RoleName == "User");

            if (userRole == null)
            {
                ModelState.AddModelError(string.Empty, "User role not found.");
                return Page();
            }

            // Create New User
            var newUser = new User
            {
                FullName = Input.FullName,
                Email = Input.Email,
                Username = Input.Username,
                Password =Input.Password,
                RoleId = userRole.Id.ToString()
            };

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            return RedirectToPage("Login");
        }


        public class RegisterInputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
            public string Password { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }
    }
}
