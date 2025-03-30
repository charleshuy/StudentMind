using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.StudentHealthPages
{
    public class IndexModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;

        public IndexModel(IStudentHealthService studentHealthService)
        {
            _studentHealthService = studentHealthService;
        }

        public IList<StudentHealth> StudentHealth { get;set; } = default!;

        public async Task OnGetAsync()
        {
            StudentHealth = await _studentHealthService.GetStudentHealths();    
        }
    }
}
