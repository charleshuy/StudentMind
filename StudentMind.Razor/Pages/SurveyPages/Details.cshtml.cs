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

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class DetailsModel : PageModel
    {
        private readonly ISurveyService _surveyService;

        public DetailsModel(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public Survey Survey { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _surveyService.GetSurveyById(id);
            if (survey == null)
            {
                return NotFound();
            }
            else
            {
                Survey = survey;
            }
            return Page();
        }
    }
}
