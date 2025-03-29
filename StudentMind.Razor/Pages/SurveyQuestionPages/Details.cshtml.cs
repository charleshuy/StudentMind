using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class DetailsModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public DetailsModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public SurveyQuestion SurveyQuestion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyquestion = await _context.SurveyQuestions.FirstOrDefaultAsync(m => m.SurveyId == id);
            if (surveyquestion == null)
            {
                return NotFound();
            }
            else
            {
                SurveyQuestion = surveyquestion;
            }
            return Page();
        }
    }
}
