using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;

namespace StudentMind.Pages.UsersPages
{
    public class EditModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public EditModel(IUnitOfWork unitOfWork) // Fixed constructor name
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            User = user;

            ViewData["RoleId"] = new SelectList(
                _unitOfWork.GetRepository<Role>().Entities, "Id", "Id"
            );

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _unitOfWork.GetRepository<User>().Update(User); // Use UnitOfWork to update entity

            try
            {
                await _unitOfWork.SaveAsync(); // Save changes using UnitOfWork
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task<bool> UserExists(string id)
        {
            return await _unitOfWork.GetRepository<User>().Entities
                .AnyAsync(e => e.Id == id);
        }
    }
}
