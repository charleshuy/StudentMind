using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.SurveyPages
{
    public class IndexModel : PageModel
    {
        private readonly ISurveyService _surveyService;

        public IndexModel(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public IList<Survey> Survey { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Survey = await _surveyService.GetSurveys();
        }
    }
}
