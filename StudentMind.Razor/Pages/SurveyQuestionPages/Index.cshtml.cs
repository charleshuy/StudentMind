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

namespace StudentMind.Razor.Pages.SurveyQuestionPages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyQuestionService _surveyQuestionService;

        public IndexModel(ISurveyQuestionService surveyQuestionService)
        {
            _surveyQuestionService = surveyQuestionService;
        }

        public IList<SurveyQuestion> SurveyQuestion { get;set; } = default!;

        public async Task OnGetAsync()
        {
            SurveyQuestion = await _surveyQuestionService.GetSurveyQuestions();
        }
    }
}
