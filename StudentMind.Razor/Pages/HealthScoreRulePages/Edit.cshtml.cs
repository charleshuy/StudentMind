using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.HealthScoreRulePages
{
    public class EditModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public EditModel(StudentMind.Infrastructure.Context.DatabaseContext context)
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

            var healthscorerule =  await _context.HealthScoreRules.FirstOrDefaultAsync(m => m.Id == id);
            if (healthscorerule == null)
            {
                return NotFound();
            }
            HealthScoreRule = healthscorerule;
           ViewData["SurveyId"] = new SelectList(_context.Surveys, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HealthScoreRule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HealthScoreRuleExists(HealthScoreRule.Id))
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

        private bool HealthScoreRuleExists(string id)
        {
            return _context.HealthScoreRules.Any(e => e.Id == id);
        }
    }
}
