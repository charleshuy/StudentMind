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
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.StudentHealthPages
{
    public class EditModel : PageModel
    {
        private readonly IStudentHealthService _studentHealthService;
        private readonly IUserService _userService;
        private readonly ISurveyService _surveyService;

        public EditModel(IStudentHealthService studentHealthService, IUserService userService, ISurveyService surveyService)
        {
            _studentHealthService = studentHealthService;
            _userService = userService;
            _surveyService = surveyService;
        }

        [BindProperty]
        public StudentHealth StudentHealth { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studenthealth =  await _studentHealthService.GetStudentHealthById(id);
            if (studenthealth == null)
            {
                return NotFound();
            }
            StudentHealth = studenthealth;
            ViewData["StudentId"] = new SelectList(await _userService.GetAllUsersAsync(), "Id", "FullName");
            ViewData["SurveyId"] = new SelectList(await _surveyService.GetSurveys(), "Id", "Name");
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

            try
            {
                StudentHealthDTO studentHealthDTO = new StudentHealthDTO
                {
                    StudentId = StudentHealth.StudentId,
                    SurveyId = StudentHealth.SurveyId,
                    Score = StudentHealth.Score,
                    Category = StudentHealth.Category
                };
                await _studentHealthService.UpdateStudentHealth(StudentHealth.Id, studentHealthDTO);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await StudentHealthExists(StudentHealth.Id)))
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

        private async Task<bool> StudentHealthExists(string id)
        {
            return await _studentHealthService.GetStudentHealthById(id) != null;
        }
    }
}
