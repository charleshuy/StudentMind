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
    public class IndexModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<SurveyQuestion> SurveyQuestion { get;set; } = default!;

        public async Task OnGetAsync()
        {
            SurveyQuestion = await _context.SurveyQuestions
                .Include(s => s.Question)
                .Include(s => s.Survey).ToListAsync();
        }
    }
}
