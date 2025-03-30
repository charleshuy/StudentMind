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
    public class IndexModel : PageModel
    {
        private readonly ISurveyResponseService _surveyResponseService;

        public IndexModel(ISurveyResponseService surveyResponseService)
        {
            _surveyResponseService = surveyResponseService;
        }

        public IList<SurveyResponse> SurveyResponse { get;set; } = default!;

        public async Task OnGetAsync()
        {
            SurveyResponse = await _surveyResponseService.GetSurveyResponses();
        }
    }
}
