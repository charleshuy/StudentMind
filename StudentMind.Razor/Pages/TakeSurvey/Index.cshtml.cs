using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.TakeSurvey
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyService _surveyService;

        public IndexModel(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public List<Survey> Surveys { get; set; }

        public async Task OnGetAsync()
        {
            Surveys = await _surveyService.GetSurveys();
        }
    }
}
