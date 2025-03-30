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
    public class IndexModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<HealthScoreRule> HealthScoreRule { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HealthScoreRule = await _context.HealthScoreRules
                .Include(h => h.Survey).ToListAsync();
        }
    }
}
