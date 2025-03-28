using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class IndexModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<Survey> Survey { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Survey = await _context.Surveys
                .Include(s => s.Type).ToListAsync();
        }
    }
}
