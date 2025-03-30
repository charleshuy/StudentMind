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

namespace StudentMind.Razor.Pages.SurveyResponsePages
{
    public class DetailsModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;

        public DetailsModel(ISurveyResponseService surveyResponseService)
        {
            _surveyResponseService = surveyResponseService;
        }

        public SurveyResponse SurveyResponse { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surveyresponse = await _surveyResponseService.GetSurveyResponseById(id);
            if (surveyresponse == null)
            {
                return NotFound();
            }
            else
            {
                SurveyResponse = surveyresponse;
            }
            return Page();
        }
    }
}
