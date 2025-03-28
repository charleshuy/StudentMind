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

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class CreateModel : PageModel
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyTypeService _surveyTypeService;

        public CreateModel(ISurveyService surveyService, ISurveyTypeService surveyTypeService)
        {
            _surveyService = surveyService;
            _surveyTypeService = surveyTypeService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            ViewData["TypeId"] = new SelectList(await _surveyTypeService.GetSurveyTypes(), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public SurveyDTO Survey { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _surveyService.CreateSurvey(Survey);

            return RedirectToPage("./Index");
        }
    }
}
