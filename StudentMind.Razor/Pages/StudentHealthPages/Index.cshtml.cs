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
    public class IndexModel : PageModel
    {
        private readonly StudentMind.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(StudentMind.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<StudentHealth> StudentHealth { get;set; } = default!;

        public async Task OnGetAsync()
        {
            StudentHealth = await _context.StudentHealths
                .Include(s => s.Student)
                .Include(s => s.Survey).ToListAsync();
        }
    }
}
