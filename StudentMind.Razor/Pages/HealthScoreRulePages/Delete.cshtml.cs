using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.HealthScoreRulePages
{
    public class DeleteModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public DeleteModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HealthScoreRule HealthScoreRule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthscorerule = await _context.HealthScoreRules.FirstOrDefaultAsync(m => m.Id == id);

            if (healthscorerule == null)
            {
                return NotFound();
            }
            else
            {
                HealthScoreRule = healthscorerule;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthscorerule = await _context.HealthScoreRules.FindAsync(id);
            if (healthscorerule != null)
            {
                HealthScoreRule = healthscorerule;
                _context.HealthScoreRules.Remove(HealthScoreRule);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
