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
    public class DetailsModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;

        public DetailsModel(IStudentHealthService studentHealthService)
        {
            _studentHealthService = studentHealthService;
        }

        public StudentHealth StudentHealth { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studenthealth = await _studentHealthService.GetStudentHealthById(id);
            if (studenthealth == null)
            {
                return NotFound();
            }
            else
            {
                StudentHealth = studenthealth;
            }
            return Page();
        }
    }
}
