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

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class EditModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public EditModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public SurveyQuestion SurveyQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyquestion =  await _context.SurveyQuestions.FirstOrDefaultAsync(m => m.SurveyId == id);
            if (surveyquestion == null)
            {
                return NotFound();
            }
            SurveyQuestion = surveyquestion;
           ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id");
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

            _context.Attach(SurveyQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyQuestionExists(SurveyQuestion.SurveyId))
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

        private bool SurveyQuestionExists(string id)
        {
            return _context.SurveyQuestions.Any(e => e.SurveyId == id);
        }
    }
}
