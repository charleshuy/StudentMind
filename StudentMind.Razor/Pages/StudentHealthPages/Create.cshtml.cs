using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentMind.Core.Entity;
using StudentMind.Infrastructure.Context;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.StudentHealthPages
{
    public class CreateModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;
        private readonly IUserService _userService;
        private readonly ISurveyService _surveyService;

        public CreateModel(IStudentHealthService studentHealthService, IUserService userService, ISurveyService surveyService)
        {
            _studentHealthService = studentHealthService;
            _userService = userService;
            _surveyService = surveyService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["StudentId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "FullName");
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public StudentHealth StudentHealth { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            StudentHealthDTO studentHealthDTO = new StudentHealthDTO
            {
                StudentId = StudentHealth.StudentId,
                SurveyId = StudentHealth.SurveyId,
                Score = StudentHealth.Score,
                Category = StudentHealth.Category
            };
            await _studentHealthService.CreateStudentHealth(studentHealthDTO);

            return RedirectToPage("./Index");
        }
    }
}
