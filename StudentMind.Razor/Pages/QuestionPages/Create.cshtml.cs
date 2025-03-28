using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class CreateModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public CreateModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Question Question { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Questions.Add(Question);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
