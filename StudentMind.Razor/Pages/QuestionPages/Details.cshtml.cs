using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.QuestionPages
{
    public class DetailsModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public DetailsModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public Question Question { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }
            else
            {
                Question = question;
            }
            return Page();
        }
    }
}
