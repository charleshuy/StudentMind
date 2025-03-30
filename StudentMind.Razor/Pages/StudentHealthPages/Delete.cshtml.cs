using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.StudentHealthPages
{
    public class DeleteModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public DeleteModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentHealth StudentHealth { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studenthealth = await _context.StudentHealths.FirstOrDefaultAsync(m => m.Id == id);

            if (studenthealth == null)
            {
                return NotFound();
            }
            else
            {
                StudentHealth = studenthealth;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studenthealth = await _context.StudentHealths.FindAsync(id);
            if (studenthealth != null)
            {
                StudentHealth = studenthealth;
                _context.StudentHealths.Remove(StudentHealth);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
