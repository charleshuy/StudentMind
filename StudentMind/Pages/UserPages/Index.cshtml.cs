using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infracstructure.Context;

namespace StudentMind.Pages.UserPages
{
    public class IndexModel : PageModel
    {
        private readonly StudentMind.Infracstructure.Context.DatabaseContext _context;

        public IndexModel(StudentMind.Infracstructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _context.User
                .Include(u => u.Role).ToListAsync();
        }
    }
}
